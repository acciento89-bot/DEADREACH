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
- PR #4 remains **Draft**
- Production 0.4 compile gate: **PASSED in real Unity**
- Production 0.4 environment visual gate: **PASSED in real Unity**
- final gameplay/start-flow regression gate is still pending before merge

## 6. Production 0.4 — implemented on branch

Goal: make Dead City read as a real environment while preserving the validated 0.3 character/combat baseline.

### Quaternius Dead City asset installer

`tools/install-quaternius-deadcity-set.ps1`

Curated CC0 subset from the same Quaternius Zombie Apocalypse Kit visual family:
- modular street / cracked street / intersection pieces
- traffic + plastic barriers
- real streetlight + traffic-light geometry
- containers
- barrel / broken pallet / pipes / trash / wheel stack
- blood ground props
- WaterTower landmark
- pickup / sports car / truck
- shared `Zombie_Atlas.png`

### Dead City Environment Pass

`Assets/Deadreach/Editor/DeadCityEnvironmentPass.cs`

Current implementation:
- deterministic environment root `Production_DeadCity_Environment_0_4`
- modular real street surfaces over retained prototype collision underlay
- containers / barriers / landmark / wrecked vehicles / street clutter
- DEADREACH-owned `CollisionBounds` child colliders on major blockers/vehicles
- explicit Quaternius environment atlas material
- cold moon / warm street-light contrast
- gameplay-targeted exponential fog
- global URP post-processing profile
  - ACES tonemapping
  - modest Bloom
  - contrast/saturation/color filter
  - modest vignette
- stronger extraction beacon column + green local light

### Import / collider hardening

First real-Unity 0.4 validation exposed:
- missing `Unity.RenderPipelines.Core.Runtime` reference in `Deadreach.Editor.asmdef`
- glTF reimport state not retrying after the compile fix
- imported glTF prefab roots rejecting/invalidating direct `BoxCollider` authoring

Fixes now on branch:
- Core Runtime assembly reference added
- required environment gate + forced synchronous glTF reimport added
- Production Slice 0.4 refuses to build if required streets/containers/vehicles are unavailable
- Play Mode start locked to `Bunker_Hub`
- bounds colliders moved onto plain DEADREACH-owned `CollisionBounds` child objects

## 7. Real Unity 0.4 validation status

### Compile gate — PASSED
User confirmed **0 C# compiler errors** after Core Runtime/import-repair fixes.

### Generator / visual environment gate — PASSED
After the required-asset and collider fixes, the real Unity screenshot/user acceptance confirmed:
- Production Slice 0.4 generates and runs
- real street surfaces are visible
- green/red containers are visible
- multiple vehicles/wrecks are visible, including colored car/truck silhouettes
- traffic barriers / road furniture / barrels / street props are visible
- extraction beacon is visible
- environment scale and overall dressing are acceptable for this pass
- user response: **“sehr gut”**

This is a real visual acceptance, not an inferred pass.

### Final merge gate — PENDING
Before merging PR #4, confirm in one short regression run:
1. pressing Play starts at **Bunker_Hub / main menu**
2. Deploy loads Dead City
3. movement / aim / firing work
4. embedded left-hand weapon remains stable and tracer/muzzle remain aligned
5. loot pickup works
6. extraction returns to Bunker
7. no blocking Console errors

If all seven pass, mark PR #4 ready and squash-merge to `main` immediately.

## 8. After 0.4 merge

Next priorities:
1. production muzzle flash + impact VFX
2. first real combat audio-content pass
3. replace prototype IMGUI with production HUD/loadout UI
4. production NavMesh navigation
5. Addressables/content organization
6. physical-device mobile validation + iOS/Android profiling
7. proper authored rifle-hold character/animation path later
8. backend/accounts/leaderboards/events
9. IAP cosmetics / season structure

## 9. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1 / 0.2 / 0.3 as validated merged baselines
3. never reintroduce external Rifle transform/socket hacks onto Sam
4. current left-hand embedded weapon is accepted
5. active work is `production/0.4-environment-atmosphere`
6. 0.4 compile + environment visual gates have passed in real Unity
7. only the short gameplay/start-flow regression gate remains before PR #4 can be marked ready and merged
8. update this file after final validation/merge

Do not rely on chat history alone.
