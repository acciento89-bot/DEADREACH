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

## 3. Vertical Slice 0.1 — MERGED / VALIDATED

PR #1 merged on 2026-08-23.

Merge commit:

`e4d5dbe2c52d3e9aeed52f421fdd99f7c6b01877`

Validated in real Unity:

- clean compile
- Bunker / Dead City generation
- movement / aim / fire
- infected combat / death
- Scrap / extraction / persistence
- Pause / Resume / Abandon
- quality presets

## 4. Production Pass 0.2 — MERGED / VALIDATED

PR #2 `production: game feel, weapon loot and equipment loop 0.2 — validated` merged on 2026-08-23.

Squash merge commit:

`fd0dca0ece7d18ca005f2f4b52d65039904fad27`

Real Unity validation passed with:

- **0 red compiler errors**
- **0 yellow compiler warnings**
- Production Slice 0.2 generator working
- tracer / impact / critical feedback working
- weapon case pickup
- run weapon inventory
- unsecured weapon loss on death/abandon
- successful weapon extraction into persistent stash
- rarity / item power / affixes
- equipped primary persistence
- equipped affixes modifying next-run combat stats
- stash/equipment persistence across Play Mode restart
- foundation death / extraction / pause / abandon flows still working
- no blocking Console errors

Established progression loop:

**Find weapon → survive → extract → stash → equip → next run becomes stronger/different**

Physical-device touch/haptics and iOS/Android builds are still separate validation tasks.

## 5. Current Git state

- `main` — contains validated 0.1 + validated 0.2
- active branch: **`production/0.3-art-presentation`**
- PR #3 — `production: art asset binding and presentation pipeline 0.3` — Draft
- Production 0.3 real Unity compile + generator gate **PASSED**
- Production 0.3 empty-catalog fallback runtime gate **PASSED**
- Quaternius Zombie Apocalypse Kit selected as first production-art source
- real Survivor/Infected assets load and replace prototypes in Play Mode
- old external Rifle/hand-socket transform path has now been removed from Survivor presentation
- Survivor uses the weapon mesh already authored and rigged inside `Characters_Sam_SingleWeapon.gltf`
- **artist-rigged Survivor weapon visual placement is now REAL UNITY VALIDATED**: user confirmed it sits perfectly on the character
- current embedded weapon is on the **left hand**; this is accepted for the current 0.3 starter-art pass and must not be “fixed” with another transform hack
- muzzle is derived directly from the embedded weapon mesh instead of from a separately mounted external Rifle

Do not claim the complete 0.3 art pass finished until muzzle/tracer origin, animation alignment, materials and all remaining gameplay regression checks are validated.

## 6. Production Art / Presentation 0.3 — current implementation

### Production Asset Catalog

`ProductionAssetCatalog` slots:

- Survivor prefab
- one or more Infected prefabs
- Primary Weapon prefab (kept for catalog/progression architecture, but no longer instantiated as a second visible Survivor weapon in the current Sam starter-art path)
- local transform offsets/scales for survivor/infected production visuals

Expected asset path:

`Assets/Deadreach/Resources/Deadreach/ProductionAssetCatalog.asset`

Editor menu:

`DEADREACH > Production > Create or Select Asset Catalog`

### Production Visual Binder

`ProductionVisualBinder` separates gameplay roots from production art.

- production prefab assigned → prototype renderer hidden
- CharacterController / Damageable / AI / weapon gameplay stay on validated root
- Animator is rebound into existing animation drivers
- Survivor no longer instantiates the external `PrimaryWeaponPrefab` as a second visible gun
- existing weapon mesh already authored inside the Survivor rig is located by weapon-name tokens and re-enabled
- the artist-authored weapon transform is left untouched
- a runtime `MuzzleSocket` is generated from the embedded weapon renderer bounds and forwarded into `HitscanWeapon`
- missing production assets safely fall back to prototype visuals

### Production asset validator

`DEADREACH > Production > Validate Asset Catalog`

Checks Survivor/Infected/Weapon assignment and animation/presentation contracts.

### Generated-scene integration

`ProductionSliceEnhancer` attaches visual binders to Survivor + generated Infected roots.

Main generator:

**`DEADREACH > Build Production Slice 0.3`**

### Combat VFX mobile hardening

`CombatFeedbackPresenter` uses a preallocated tracer pool (default 24) instead of per-shot GameObject creation/destruction.

### Quaternius starter art / weapon correction

Earlier iterations proved that trying to mount `Weapon_Rifle.gltf` onto a discovered/fallback hand socket created unstable visual orientation and repeated transform fixes.

The corrected 0.3 strategy is now:

1. use Quaternius `Characters_Sam_SingleWeapon.gltf` as the Survivor source
2. keep its artist-authored embedded weapon in the rig
3. do **not** instantiate a second external Rifle for the visible Survivor weapon
4. do **not** modify the embedded weapon local position/rotation/scale
5. derive the gameplay muzzle directly from the embedded weapon mesh bounds
6. keep the external Rifle asset only for catalog/progression/future weapon-presentation work until a proper matching rifle-hold rig/animation path exists

Original-source inspection confirmed the SingleWeapon character exports contain weapon meshes parented directly into the hand/finger hierarchy. For Sam, the embedded weapon currently used by the export is a pistol-type weapon.

Latest implementation commit for the strategy switch:

`3cd7f4af2a1394869c84f0d1c0ab54e6fdcf0dcc`

## 7. Production 0.3 real Unity validation

### Compile / generator gate — PASSED

Confirmed by the user in real Unity `6000.3.22f1` on 2026-08-23:

- **0 compiler errors after pulling 0.3**
- `DEADREACH > Build Production Slice 0.3` completes with **0 errors**

### Empty-catalog fallback runtime gate — PASSED

Confirmed by the user:

- Play from Bunker works
- Deploy to Dead City works
- prototype Survivor/Infected remain visible when catalog empty
- movement / aiming / shooting remain functional
- combat / weapon loot / extraction / Bunker return remain functional
- catalog create/select runs
- validator runs without exception/red error
- only expected yellow missing-asset warnings occur

### Real-art visual gate — PARTIAL PASS

Confirmed in Play Mode:

- real Survivor model appears instead of Capsule
- real Infected models appear instead of prototype enemies
- production visual binder works with actual imported glTF assets
- external duplicate Rifle mount path has been removed
- embedded artist-rigged weapon is visible
- **weapon placement visually passes**: user confirmed it “sits perfectly”
- weapon is currently attached on the **left hand**, which is accepted for this pass

Still to validate before 0.3 completion:

- muzzle/tracer visibly originate from the embedded weapon
- weapon remains aligned through movement/aim/fire animations
- final material/atlas appearance is acceptable in gameplay
- all infected variants remain aligned/animated
- movement/combat/loot/extraction/Bunker return still have no regression with the corrected art path
- no blocking Console errors

## 8. Selected first production-art source

**Quaternius — Zombie Apocalypse Kit**

Initial DEADREACH subset:

- Survivor Sam / Sam SingleWeapon
- Zombie Basic
- Zombie Chubby
- Zombie Arm
- Zombie Ribcage
- Rifle
- shared `Zombie_Atlas.png`

License tracking:

`docs/THIRD_PARTY_ASSETS.md`

Original creator Google Drive remains documented as original distribution, but automated `gdown` access failed. Selected files are retrieved from:

`agentkaerf/FreeModels/Zombie Apocalypse Kit - March 2024`

The mirror contains a CC0 1.0 marker; the official Quaternius page remains the source/license authority.

Unity package:

`com.unity.cloud.gltfast` `6.17.0`

Git LFS tracks `.gltf`, `.glb` and image/binary art assets.

## 9. Immediate next gate

On local `production/0.3-art-presentation` with commit `3cd7f4af2a1394869c84f0d1c0ab54e6fdcf0dcc` or newer:

1. keep the currently correct embedded weapon mount untouched
2. Play → Deploy
3. fire in several directions and confirm muzzle/tracer starts at the embedded weapon rather than hand/head/body
4. move + aim + fire and confirm the weapon stays aligned during animation
5. verify Survivor/Infected atlas/material appearance
6. fight all visible infected variants
7. collect weapon loot and extract back to Bunker
8. require no blocking Console errors

## 10. After corrected real starter-art validation

1. lock the validated Sam starter-art mount path; no further transform hacks
2. proper muzzle flash / impact production VFX
3. first real combat audio-content pass
4. extend Quaternius kit into Dead City environment props
5. URP post-processing / color grading / atmosphere
6. replace prototype IMGUI with production HUD/loadout UI
7. production NavMesh navigation
8. later introduce a proper rifle-hold character/animation path for visible rifle equipment instead of forcing a foreign Rifle onto the current Sam rig
9. physical-device mobile validation + iOS/Android build profiling

## 11. Handoff protocol

When resuming:

1. read this file first
2. inspect active branch / PR #3
3. note compile/generator + empty-catalog fallback gates passed
4. note real model binding works
5. note the external Rifle hand-socket transform approach was abandoned
6. note commit `3cd7f4af2a1394869c84f0d1c0ab54e6fdcf0dcc` switched Survivor presentation to the embedded artist-rigged weapon
7. note user visually validated the weapon placement as perfect, though it is on the left hand
8. do not “correct” the left-hand mount by rotating/repositioning the embedded weapon
9. continue with muzzle/tracer + animation/material/gameplay validation

Do not rely on chat history alone.
