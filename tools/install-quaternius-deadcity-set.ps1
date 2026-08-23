param(
    [switch]$CommitAndPush
)

$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$kitRoot = Join-Path $repoRoot 'Assets\Deadreach\ThirdParty\Quaternius\ZombieApocalypseKit'
$environmentTarget = Join-Path $kitRoot 'Environment\glTF'
$vehiclesTarget = Join-Path $kitRoot 'Vehicles\glTF'
$branchRequired = 'production/0.4-environment-atmosphere'

$mirrorBase = 'https://raw.githubusercontent.com/agentkaerf/FreeModels/main/Zombie%20Apocalypse%20Kit%20-%20March%202024'
$officialPage = 'https://quaternius.com/packs/zombieapocalypsekit.html'
$mirrorRepo = 'https://github.com/agentkaerf/FreeModels/tree/main/Zombie%20Apocalypse%20Kit%20-%20March%202024'

Set-Location $repoRoot

$branch = (git branch --show-current).Trim()
if ($branch -ne $branchRequired) {
    throw "Expected branch $branchRequired, current branch is '$branch'."
}

New-Item -ItemType Directory -Force -Path $environmentTarget | Out-Null
New-Item -ItemType Directory -Force -Path $vehiclesTarget | Out-Null

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
    param([Parameter(Mandatory=$true)][string]$Path)

    $text = [System.IO.File]::ReadAllText($Path)
    $updated = [regex]::Replace(
        $text,
        '("uri"\s*:\s*")[^"]*Zombie_Atlas\.png(")',
        '${1}Zombie_Atlas.png${2}',
        [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
    )

    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($Path, $updated, $utf8NoBom)
}

function Download-Selection {
    param(
        [Parameter(Mandatory=$true)][array]$Selection,
        [Parameter(Mandatory=$true)][string]$TargetFolder
    )

    foreach ($entry in $Selection) {
        $relativeUrl = ($entry.Relative -replace ' ', '%20')
        $url = "$mirrorBase/$relativeUrl"
        $dest = Join-Path $TargetFolder $entry.Out
        Download-CheckedFile -Url $url -Destination $dest -MinimumBytes $entry.Minimum
        Normalize-GltfAtlasReference -Path $dest
    }
}

# Each flattened glTF subset gets the common Quaternius atlas beside it so glTFast can
# resolve material textures deterministically after DEADREACH normalizes the URI.
Download-CheckedFile -Url "$mirrorBase/Zombie_Atlas.png" -Destination (Join-Path $environmentTarget 'Zombie_Atlas.png') -MinimumBytes 1000
Download-CheckedFile -Url "$mirrorBase/Zombie_Atlas.png" -Destination (Join-Path $vehiclesTarget 'Zombie_Atlas.png') -MinimumBytes 1000

$environmentSelection = @(
    @{ Out='Street_Straight.gltf'; Relative='Environment/glTF/Street_Straight.gltf'; Minimum=10000 },
    @{ Out='Street_Crack1.gltf'; Relative='Environment/glTF/Street_Straight_Crack1.gltf'; Minimum=10000 },
    @{ Out='Street_Crack2.gltf'; Relative='Environment/glTF/Street_Straight_Crack2.gltf'; Minimum=10000 },
    @{ Out='Street_4Way.gltf'; Relative='Environment/glTF/Street_4Way.gltf'; Minimum=10000 },
    @{ Out='Street_Turn.gltf'; Relative='Environment/glTF/Street_Turn.gltf'; Minimum=10000 },
    @{ Out='TrafficBarrier_1.gltf'; Relative='Environment/glTF/TrafficBarrier_1.gltf'; Minimum=10000 },
    @{ Out='PlasticBarrier.gltf'; Relative='Environment/glTF/PlasticBarrier.gltf'; Minimum=10000 },
    @{ Out='StreetLights.gltf'; Relative='Environment/glTF/StreetLights.gltf'; Minimum=10000 },
    @{ Out='TrafficLight_1.gltf'; Relative='Environment/glTF/TrafficLight_1.gltf'; Minimum=10000 },
    @{ Out='Container_Green.gltf'; Relative='Environment/glTF/Container_Green.gltf'; Minimum=10000 },
    @{ Out='Container_Red.gltf'; Relative='Environment/glTF/Container_Red.gltf'; Minimum=10000 },
    @{ Out='Barrel.gltf'; Relative='Environment/glTF/Barrel.gltf'; Minimum=10000 },
    @{ Out='Pallet_Broken.gltf'; Relative='Environment/glTF/Pallet_Broken.gltf'; Minimum=10000 },
    @{ Out='Pipes.gltf'; Relative='Environment/glTF/Pipes.gltf'; Minimum=10000 },
    @{ Out='TrashBag_1.gltf'; Relative='Environment/glTF/TrashBag_1.gltf'; Minimum=10000 },
    @{ Out='TrashBag_2.gltf'; Relative='Environment/glTF/TrashBag_2.gltf'; Minimum=10000 },
    @{ Out='Blood_1.gltf'; Relative='Environment/glTF/Blood_1.gltf'; Minimum=5000 },
    @{ Out='Blood_2.gltf'; Relative='Environment/glTF/Blood_2.gltf'; Minimum=5000 },
    @{ Out='WaterTower.gltf'; Relative='Environment/glTF/WaterTower.gltf'; Minimum=10000 },
    @{ Out='Wheels_Stack.gltf'; Relative='Environment/glTF/Wheels_Stack.gltf'; Minimum=10000 }
)

$vehicleSelection = @(
    @{ Out='Vehicle_Pickup.gltf'; Relative='Vehicles/glTF/Vehicle_Pickup.gltf'; Minimum=100000 },
    @{ Out='Vehicle_Sports.gltf'; Relative='Vehicles/glTF/Vehicle_Sports.gltf'; Minimum=100000 },
    @{ Out='Vehicle_Truck.gltf'; Relative='Vehicles/glTF/Vehicle_Truck.gltf'; Minimum=100000 }
)

Download-Selection -Selection $environmentSelection -TargetFolder $environmentTarget
Download-Selection -Selection $vehicleSelection -TargetFolder $vehiclesTarget

# Preserve license evidence close to the expanded production-art subset.
$licensePath = Join-Path $kitRoot 'MIRROR_LICENSE.txt'
Download-CheckedFile -Url "$mirrorBase/License.txt" -Destination $licensePath -MinimumBytes 100
$license = Get-Content $licensePath -Raw
if ($license -notmatch 'CC0\s+1\.0') {
    throw 'Mirror license file did not contain the expected CC0 1.0 marker. Import aborted.'
}

$environmentLicense = @"
DEADREACH Production 0.4 — Quaternius Dead City environment subset
Official Quaternius source: $officialPage
Mirror used for automated retrieval: $mirrorRepo
License: Creative Commons CC0 1.0 / public domain dedication.

Selected content:
- modular street pieces / cracked street pieces
- traffic and plastic barriers
- street lights and traffic light
- green/red containers
- barrels, broken pallet, pipes, trash bags, wheel stack
- blood ground props
- water tower landmark
- pickup, sports car and truck
- shared Zombie_Atlas.png

The environment subset intentionally stays in the same Quaternius visual family as the validated 0.3 Survivor/Infected starter art.
"@
Set-Content -Path (Join-Path $kitRoot 'ENVIRONMENT_LICENSE_AND_SOURCE.txt') -Value $environmentLicense -Encoding UTF8

Write-Host ''
Write-Host 'Quaternius Dead City subset downloaded.'
Write-Host "Environment: $environmentTarget"
Write-Host "Vehicles:    $vehiclesTarget"
Write-Host 'Wait for Unity glTFast import, then use:'
Write-Host '  DEADREACH > Build Production Slice 0.4'

if ($CommitAndPush) {
    Write-Host ''
    Write-Host 'Committing Dead City environment assets through Git LFS...'
    git lfs install
    if ($LASTEXITCODE -ne 0) { throw 'git lfs install failed.' }

    git add -- 'Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/Environment' 'Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/Vehicles' 'Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/MIRROR_LICENSE.txt' 'Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/ENVIRONMENT_LICENSE_AND_SOURCE.txt'
    git status --short

    git diff --cached --quiet
    if ($LASTEXITCODE -ne 0) {
        git commit -m 'art: import Quaternius Dead City environment set'
        if ($LASTEXITCODE -ne 0) { throw 'git commit failed.' }
        git push origin $branch
        if ($LASTEXITCODE -ne 0) { throw 'git push failed.' }
        Write-Host 'Dead City environment set committed and pushed.'
    } else {
        Write-Host 'No new environment asset changes to commit.'
    }
}
