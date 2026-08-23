param(
    [switch]$CommitAndPush
)

$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$targetRoot = Join-Path $repoRoot 'Assets\Deadreach\ThirdParty\Quaternius\ZombieApocalypseKit'
$gltfTarget = Join-Path $targetRoot 'glTF'
$branchRequired = 'production/0.3-art-presentation'

# The creator's original Google Drive folder intermittently blocks automated tools.
# Use a public mirror of the SAME Quaternius pack for the selected glTF files.
# The official creator page remains the license/source authority and explicitly states CC0/commercial use.
$mirrorBase = 'https://raw.githubusercontent.com/agentkaerf/FreeModels/main/Zombie%20Apocalypse%20Kit%20-%20March%202024'
$officialPage = 'https://quaternius.com/packs/zombieapocalypsekit.html'
$officialDrive = 'https://drive.google.com/drive/folders/1mWP6sCHun7OUMHQeDNZLrXTteXlzWg_t?usp=sharing'
$mirrorRepo = 'https://github.com/agentkaerf/FreeModels/tree/main/Zombie%20Apocalypse%20Kit%20-%20March%202024'

Set-Location $repoRoot

$branch = (git branch --show-current).Trim()
if ($branch -ne $branchRequired) {
    throw "Expected branch $branchRequired, current branch is '$branch'."
}

New-Item -ItemType Directory -Force -Path $gltfTarget | Out-Null

function Download-CheckedFile {
    param(
        [Parameter(Mandatory=$true)][string]$Url,
        [Parameter(Mandatory=$true)][string]$Destination,
        [int]$MinimumBytes = 1024
    )

    Write-Host "Downloading $(Split-Path $Destination -Leaf)..."
    Invoke-WebRequest -Uri $Url -OutFile $Destination -UseBasicParsing -Headers @{ 'User-Agent' = 'DEADREACH-Asset-Importer' }

    if (-not (Test-Path $Destination)) {
        throw "Download did not create '$Destination'."
    }

    $size = (Get-Item $Destination).Length
    if ($size -lt $MinimumBytes) {
        Remove-Item $Destination -Force -ErrorAction SilentlyContinue
        throw "Downloaded file '$Destination' is unexpectedly small ($size bytes)."
    }
}

function Normalize-GltfAtlasReference {
    param(
        [Parameter(Mandatory=$true)][string]$Path
    )

    $text = [System.IO.File]::ReadAllText($Path)
    $updated = [regex]::Replace(
        $text,
        '("uri"\s*:\s*")[^"]*Zombie_Atlas\.png(")',
        '${1}Zombie_Atlas.png${2}',
        [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
    )

    if ($updated -eq $text -and $text -notmatch 'Zombie_Atlas\.png') {
        Write-Warning "$(Split-Path $Path -Leaf) contains no Zombie_Atlas.png URI. The model may use embedded material data instead."
        return
    }

    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($Path, $updated, $utf8NoBom)
}

# The selected Quaternius glTF files reference the shared atlas through their original
# folder layout. Because DEADREACH flattens the selected subset into one folder, keep
# the atlas beside the glTFs and normalize every URI to that local file.
$atlasPath = Join-Path $gltfTarget 'Zombie_Atlas.png'
Download-CheckedFile -Url "$mirrorBase/Zombie_Atlas.png" -Destination $atlasPath -MinimumBytes 1000

$selection = @(
    # Use Quaternius' SingleWeapon variant to avoid importing the full built-in weapon rack.
    # DEADREACH still suppresses any remaining embedded weapon renderer and mounts its own equipped rifle.
    @{ Out='Survivor_Sam.gltf'; Relative='Characters/glTF/Characters_Sam_SingleWeapon.gltf'; Minimum=100000 },
    @{ Out='Infected_Basic.gltf'; Relative='Characters/glTF/Zombie_Basic.gltf'; Minimum=100000 },
    @{ Out='Infected_Chubby.gltf'; Relative='Characters/glTF/Zombie_Chubby.gltf'; Minimum=100000 },
    @{ Out='Infected_Arm.gltf'; Relative='Characters/glTF/Zombie_Arm.gltf'; Minimum=100000 },
    @{ Out='Infected_Ribcage.gltf'; Relative='Characters/glTF/Zombie_Ribcage.gltf'; Minimum=100000 },
    @{ Out='Weapon_Rifle.gltf'; Relative='Weapons/glTF/Rifle.gltf'; Minimum=10000 }
)

foreach ($entry in $selection) {
    $relativeUrl = ($entry.Relative -replace ' ', '%20')
    $url = "$mirrorBase/$relativeUrl"
    $dest = Join-Path $gltfTarget $entry.Out
    Download-CheckedFile -Url $url -Destination $dest -MinimumBytes $entry.Minimum
    Normalize-GltfAtlasReference -Path $dest
}

# Keep a copy of the mirror's CC0 license evidence as well.
$mirrorLicensePath = Join-Path $targetRoot 'MIRROR_LICENSE.txt'
Download-CheckedFile -Url "$mirrorBase/License.txt" -Destination $mirrorLicensePath -MinimumBytes 100
$mirrorLicense = Get-Content $mirrorLicensePath -Raw
if ($mirrorLicense -notmatch 'CC0\s+1\.0') {
    throw 'Mirror license file did not contain the expected CC0 1.0 marker. Import aborted.'
}

$licenseText = @"
Quaternius — Zombie Apocalypse Kit
Official source: $officialPage
Original creator download: $officialDrive
Public mirror used for automated selected-file retrieval: $mirrorRepo
License: Creative Commons CC0 1.0 / public domain dedication.
Commercial use and modification are permitted by the original creator.

Reason for mirror fallback:
The original Google Drive folder currently rejects automated gdown access for at least one public file. The selected files are therefore retrieved from the public mirror above while the official Quaternius page remains the license/source authority.

Selected DEADREACH subset:
- Survivor Sam (SingleWeapon source variant; embedded weapon visual suppressed by DEADREACH wrapper)
- Zombie Basic
- Zombie Chubby
- Zombie Arm
- Zombie Ribcage
- Rifle
- Zombie_Atlas.png shared material atlas

Unity import format: glTF via Unity glTFast.
The original glTF atlas paths are normalized to a local Zombie_Atlas.png beside the selected models.
"@
Set-Content -Path (Join-Path $targetRoot 'LICENSE_AND_SOURCE.txt') -Value $licenseText -Encoding UTF8

Write-Host ''
Write-Host "Imported Quaternius starter art into: $gltfTarget"
Write-Host 'Unity will import the .gltf files and Zombie_Atlas.png through Unity glTFast.'
Write-Host 'After Unity finishes package/import work, use:'
Write-Host '  DEADREACH > Production > Setup Quaternius Starter Art'

if ($CommitAndPush) {
    Write-Host ''
    Write-Host 'Committing selected art through Git LFS...'
    git lfs install
    if ($LASTEXITCODE -ne 0) { throw 'git lfs install failed.' }

    git add -- 'Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit'
    git status --short

    git diff --cached --quiet
    if ($LASTEXITCODE -ne 0) {
        git commit -m 'art: refresh Quaternius starter set with atlas'
        if ($LASTEXITCODE -ne 0) { throw 'git commit failed.' }
        git push origin $branch
        if ($LASTEXITCODE -ne 0) { throw 'git push failed.' }
        Write-Host 'Starter art committed and pushed.'
    } else {
        Write-Host 'No new binary asset changes to commit.'
    }
}
