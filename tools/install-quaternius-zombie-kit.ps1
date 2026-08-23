param(
    [switch]$CommitAndPush
)

$ErrorActionPreference = 'Stop'

$driveFolder = 'https://drive.google.com/drive/folders/1mWP6sCHun7OUMHQeDNZLrXTteXlzWg_t?usp=sharing'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$targetRoot = Join-Path $repoRoot 'Assets\Deadreach\ThirdParty\Quaternius\ZombieApocalypseKit'
$fbxTarget = Join-Path $targetRoot 'FBX'
$textureTarget = Join-Path $targetRoot 'Textures'
$tempRoot = Join-Path $env:TEMP 'deadreach_quaternius_zombie_apocalypse'

Set-Location $repoRoot

$branch = (git branch --show-current).Trim()
if ($branch -ne 'production/0.3-art-presentation') {
    throw "Expected branch production/0.3-art-presentation, current branch is '$branch'."
}

$python = $null
if (Get-Command py -ErrorAction SilentlyContinue) {
    $python = 'py'
} elseif (Get-Command python -ErrorAction SilentlyContinue) {
    $python = 'python'
} else {
    throw 'Python was not found. Install Python or add it to PATH, then run this script again.'
}

Write-Host 'Installing/updating gdown for the original Quaternius Google Drive download...'
& $python -m pip install --user --disable-pip-version-check -q gdown
if ($LASTEXITCODE -ne 0) {
    throw 'Failed to install gdown.'
}

if (Test-Path $tempRoot) {
    Remove-Item $tempRoot -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $tempRoot | Out-Null

Write-Host 'Downloading Quaternius Zombie Apocalypse Kit from the original creator folder...'
& $python -m gdown --folder $driveFolder --remaining-ok -O $tempRoot
if ($LASTEXITCODE -ne 0) {
    throw 'Quaternius download failed.'
}

New-Item -ItemType Directory -Force -Path $fbxTarget | Out-Null
New-Item -ItemType Directory -Force -Path $textureTarget | Out-Null

function Find-AssetFile {
    param(
        [Parameter(Mandatory=$true)][string[]]$Patterns,
        [string[]]$ExcludePatterns = @()
    )

    $files = Get-ChildItem $tempRoot -Recurse -File
    foreach ($pattern in $Patterns) {
        $match = $files | Where-Object {
            $_.Name -like $pattern -and -not ($ExcludePatterns | Where-Object { $_ -and $_.Length -gt 0 -and $_.Name -like $_ })
        } | Select-Object -First 1
        if ($match) { return $match }
    }
    return $null
}

# Avoid PowerShell scope confusion in the generic exclusion helper by selecting explicitly below.
$allFiles = Get-ChildItem $tempRoot -Recurse -File

function Find-PreferredFbx {
    param(
        [Parameter(Mandatory=$true)][string[]]$NamePatterns,
        [string[]]$RejectPatterns = @()
    )

    foreach ($pattern in $NamePatterns) {
        $candidate = $allFiles | Where-Object {
            $_.Extension -ieq '.fbx' -and $_.Name -like $pattern
        } | Where-Object {
            $name = $_.Name
            -not ($RejectPatterns | Where-Object { $name -like $_ })
        } | Select-Object -First 1

        if ($candidate) { return $candidate }
    }

    return $null
}

$selection = @(
    @{ Out='Survivor_Sam.fbx'; Patterns=@('Characters_Sam.fbx','*Sam*.fbx'); Reject=@('*SingleWeapon*') },
    @{ Out='Infected_Basic.fbx'; Patterns=@('Zombie_Basic.fbx','*Zombie*Basic*.fbx'); Reject=@() },
    @{ Out='Infected_Chubby.fbx'; Patterns=@('Zombie_Chubby.fbx','*Zombie*Chubby*.fbx'); Reject=@() },
    @{ Out='Infected_Arm.fbx'; Patterns=@('Zombie_Arm.fbx','*Zombie*Arm*.fbx'); Reject=@() },
    @{ Out='Infected_Ribcage.fbx'; Patterns=@('Zombie_Ribcage.fbx','*Zombie*Ribcage*.fbx'); Reject=@() },
    @{ Out='Weapon_Rifle.fbx'; Patterns=@('Rifle.fbx','Weapons_Rifle.fbx','*Rifle*.fbx'); Reject=@() }
)

$missing = @()
foreach ($entry in $selection) {
    $source = Find-PreferredFbx -NamePatterns $entry.Patterns -RejectPatterns $entry.Reject
    if (-not $source) {
        $missing += $entry.Out
        continue
    }

    $dest = Join-Path $fbxTarget $entry.Out
    Copy-Item $source.FullName $dest -Force
    Write-Host "Selected $($source.Name) -> $($entry.Out)"
}

if ($missing.Count -gt 0) {
    Write-Warning ("Could not locate these FBX files in the downloaded pack: " + ($missing -join ', '))
    Write-Warning 'The download remains in the temp folder so the file names can be inspected.'
}

$pngs = $allFiles | Where-Object { $_.Extension -ieq '.png' }
foreach ($png in $pngs) {
    Copy-Item $png.FullName (Join-Path $textureTarget $png.Name) -Force
}

$licenseText = @"
Quaternius — Zombie Apocalypse Kit
Source: https://quaternius.com/packs/zombieapocalypsekit.html
Original creator download: $driveFolder
License: Creative Commons CC0 1.0 / public domain dedication.
Commercial use and modification are permitted by the original creator.

This DEADREACH import intentionally uses only a selected subset of the original pack.
"@
Set-Content -Path (Join-Path $targetRoot 'LICENSE_AND_SOURCE.txt') -Value $licenseText -Encoding UTF8

Write-Host ''
Write-Host "Imported starter art into: $targetRoot"
Write-Host 'Unity will import the FBX/texture files automatically.'
Write-Host 'Then use: DEADREACH > Production > Setup Quaternius Starter Art'

if ($CommitAndPush) {
    Write-Host ''
    Write-Host 'Committing selected art through Git LFS...'
    git lfs install
    if ($LASTEXITCODE -ne 0) { throw 'git lfs install failed.' }

    git add -- 'Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit'
    git status --short

    git diff --cached --quiet
    if ($LASTEXITCODE -ne 0) {
        git commit -m 'art: import Quaternius CC0 zombie starter set'
        if ($LASTEXITCODE -ne 0) { throw 'git commit failed.' }
        git push origin $branch
        if ($LASTEXITCODE -ne 0) { throw 'git push failed.' }
        Write-Host 'Starter art committed and pushed.'
    } else {
        Write-Host 'No new binary asset changes to commit.'
    }
}
