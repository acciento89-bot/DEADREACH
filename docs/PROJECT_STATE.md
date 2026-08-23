# DEADREACH — Project State

_Last updated: 2026-08-23_

This is the canonical handoff file for DEADREACH. Update it after every major development, validation, build, architecture, backend, monetization, store, or release step.

## 1. Product identity

- **Game:** DEADREACH
- **Studio:** Kamilunavo
- **Repository:** `acciento89-bot/DEADREACH`
- **Platforms:** iOS + Android
- **Unity:** `6000.3.22f1`
- **Render pipeline:** URP 17.3
- **iOS Bundle ID:** `de.kamilunavo.deadzone`
- **App Store SKU:** `deadzone-001`
- **Monetization:** IAP only
- **Advertising:** none

## 2. Locked direction

Premium-feeling mobile 3D survival/extraction roguelite with persistent progression.

Core loop:

**Bunker → Deploy → Expedition → Combat → Loot → Risk decision → Extract / Die / Abandon → Bunker → Equip / Upgrade → Deploy stronger**

No cheap generic mobile finish. Primitive geometry is temporary scaffolding. No advertising SDKs.

## 3. Validated / merged baselines

### Vertical Slice 0.1 — MERGED / VALIDATED
Merge: `e4d5dbe2c52d3e9aeed52f421fdd99f7c6b01877`

Validated: Bunker/Dead City generation, movement, aim/fire, infected combat/death, Scrap/extraction/persistence, Pause/Resume/Abandon, quality presets.

### Production 0.2 — MERGED / VALIDATED
Merge: `fd0dca0ece7d18ca005f2f4b52d65039904fad27`

Validated: combat feedback, weapon loot/run inventory, extraction stash, rarity/item power/affixes, equipped primary persistence, affix-driven next-run combat stats, no regression to foundation flows.

### Production 0.3 — MERGED / REAL UNITY VALIDATED
PR #3 merge: `924e8ff4ae250da13fd0d198b121802cf80131b0`

Validated in Unity `6000.3.22f1`:
- 0 compiler errors
- Production Slice 0.3 generator passes
- real Quaternius Survivor/Infected replace prototypes
- atlas colors/materials render correctly
- embedded artist-rigged Survivor weapon is stable
- current embedded weapon is on the **left hand**; this is accepted and locked
- weapon remains aligned while moving / aiming / firing
- muzzle/tracer originate from embedded weapon
- movement/combat/loot/extraction/Bunker return remain functional
- no blocking Console errors reported

## 4. Locked 0.3 presentation decision

Do **not** reintroduce the old external Rifle hand-socket transform approach.

Current accepted Sam strategy:
1. `Characters_Sam_SingleWeapon.gltf` is the visible Survivor source
2. use its artist-authored embedded weapon
3. do not instantiate a second external Rifle on Sam
4. do not rotate/reposition the embedded weapon to move it from the left hand
5. derive gameplay muzzle from the embedded weapon mesh
6. keep external Rifle asset for progression/future authored rifle-hold work

## 5. Current Git state

- `main` contains validated 0.1 + 0.2 + 0.3
- active branch: **`production/0.4-environment-atmosphere`**
- Production 0.4 is **IMPLEMENTED IN CODE, NOT YET UNITY COMPILED/RUNTIME VALIDATED**
- merge is forbidden until `docs/PRODUCTION_04_TEST.md` passes

## 6. Production 0.4 — implemented on branch

Goal: make Dead City read as a real environment while preserving the validated 0.3 character/combat baseline.

### Quaternius Dead City asset installer

New:

`tools/install-quaternius-deadcity-set.ps1`

Downloads a curated CC0 subset from the same Quaternius Zombie Apocalypse Kit visual family:
- modular street / cracked street / intersection pieces
- traffic + plastic barriers
- real streetlight + traffic-light geometry
- containers
- barrel / broken pallet / pipes / trash / wheel stack
- blood ground props
- WaterTower landmark
- pickup / sports car / truck
- shared `Zombie_Atlas.png`

The script requires branch `production/0.4-environment-atmosphere`, supports `-CommitAndPush`, normalizes glTF atlas URIs and verifies the CC0 marker before accepting the import.

### Dead City Environment Pass

New:

`Assets/Deadreach/Editor/DeadCityEnvironmentPass.cs`

Current implementation:
- deterministic environment root `Production_DeadCity_Environment_0_4`
- modular real street surfaces layered over the retained prototype road collider/underlay
- containers / barriers / landmark / wrecked vehicles / street clutter
- bounds colliders on major blockers/vehicles
- explicit Quaternius environment atlas material
- stronger cold moon / warmer street-light contrast
- denser but still gameplay-targeted exponential fog
- global URP post-processing profile
  - ACES tonemapping
  - modest Bloom
  - contrast/saturation/color filter pass
  - modest vignette
- camera post-processing enabled through `UniversalAdditionalCameraData`
- stronger extraction beacon column + green local light
- pass can run without the optional assets and will warn instead of destroying gameplay

### Main generator

Main command on this branch:

**`DEADREACH > Build Production Slice 0.4`**

Order:
1. build validated Dead City base
2. apply Production 0.3 gameplay/art binders
3. apply Production 0.4 environment/atmosphere
4. build Bunker
5. repair complete Build Settings
6. reopen Bunker

### Acceptance runbook

`docs/PRODUCTION_04_TEST.md`

Critical regression lock:
- 0.3 colored Survivor/Infected must remain
- current embedded left-hand weapon mount must remain untouched
- weapon/muzzle/tracer alignment must remain correct
- movement/combat/loot/extraction/Bunker return must remain functional

## 7. Immediate next local gate

On the local repo:

```powershell
git fetch
git switch production/0.4-environment-atmosphere
git pull
powershell -ExecutionPolicy Bypass -File .\tools\install-quaternius-deadcity-set.ps1 -CommitAndPush
```

Then wait for Unity/glTFast import.

Required next:
1. **0 red compiler errors**
2. run `DEADREACH > Build Production Slice 0.4`
3. Play → Deploy
4. visually validate environment scale/orientation/colors/lighting/fog/post-processing
5. validate environment blockers do not trap spawn or block extraction
6. revalidate the locked 0.3 character/weapon/muzzle path
7. run the full gameplay regression gate from `docs/PRODUCTION_04_TEST.md`

Do not claim 0.4 works until this real Unity gate passes.

## 8. After 0.4 environment validation

Next likely priorities:
1. correct any scale/orientation/dressing problems found in screenshots
2. production muzzle flash + impact VFX
3. first real combat audio-content pass
4. replace prototype IMGUI with production HUD/loadout UI
5. production NavMesh navigation
6. Addressables/content organization
7. physical-device mobile validation + iOS/Android profiling
8. proper authored rifle-hold character/animation path later
9. backend/accounts/leaderboards/events
10. IAP cosmetics / season structure

## 9. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1 / 0.2 / 0.3 as validated merged baselines
3. never reintroduce external Rifle transform/socket hacks onto Sam
4. current left-hand embedded weapon is accepted
5. active work is `production/0.4-environment-atmosphere`
6. 0.4 code exists but is **not yet real-Unity validated**
7. next action is the environment installer + compile + Production Slice 0.4 acceptance run
8. update this file after each major validation/fix

Do not rely on chat history alone.
