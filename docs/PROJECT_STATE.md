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
- six selected Quaternius glTF model files + license evidence are now committed on the 0.3 branch
- first real-art Play Mode test proves Survivor/Infected visual replacement works
- first real-art test exposed two content-prep defects: gray/untextured materials and multiple embedded Survivor weapon meshes
- fixes are implemented in tooling but require one local asset refresh + Unity revalidation

Do not claim 0.3 production-art complete until the corrected textured Survivor/Infected/Rifle presentation is visually/runtime validated.

## 6. Production Art / Presentation 0.3 — current implementation

### Production Asset Catalog

`ProductionAssetCatalog` slots:

- Survivor prefab
- one or more Infected prefabs
- Primary Weapon prefab
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
- Survivor `WeaponSocket` / `RightHandWeaponSocket` detected
- Primary Weapon mounted on Survivor socket
- weapon `MuzzleSocket` / `Muzzle` forwarded into `HitscanWeapon`
- missing production assets safely fall back to prototype visuals

### Production asset validator

`DEADREACH > Production > Validate Asset Catalog`

Checks Survivor/Infected/Weapon assignment, Animators, weapon socket and muzzle socket.

### Generated-scene integration

`ProductionSliceEnhancer` attaches visual binders to Survivor + generated Infected roots.

Main generator:

**`DEADREACH > Build Production Slice 0.3`**

### Combat VFX mobile hardening

`CombatFeedbackPresenter` uses a preallocated tracer pool (default 24) instead of per-shot GameObject creation/destruction.

### Quaternius cleanup added after first real-art test

The first imported models rendered and replaced the prototype visuals, but the screenshot/runtime test exposed:

1. models were gray because the selected flattened glTF subset did not include a locally resolvable shared `Zombie_Atlas.png`
2. full Survivor Sam export contained multiple built-in weapon presentation meshes while DEADREACH also mounted its own equipped Rifle

Implemented correction:

- installer now downloads `Zombie_Atlas.png`
- every selected glTF `Zombie_Atlas.png` URI is normalized to the atlas beside the flattened DEADREACH glTF subset
- Survivor source switched to Quaternius `Characters_Sam_SingleWeapon.gltf`
- wrapper setup additionally disables separately named embedded weapon renderers
- DEADREACH remains the sole owner of equipped-weapon presentation

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

### First real-art visual gate — PARTIAL PASS / CLEANUP REQUIRED

Confirmed by user screenshots in Play Mode:

- real Survivor model appears instead of Capsule
- real Infected models appear instead of prototype enemies
- production visual binder therefore works with actual imported glTF assets
- models currently render gray/untextured
- Survivor currently shows excessive/duplicate weapon visuals

The gray/duplicate-weapon issues are now addressed in branch tooling but are **not yet revalidated in Unity**.

## 8. Selected first production-art source

**Quaternius — Zombie Apocalypse Kit**

Initial DEADREACH subset:

- Survivor Sam
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

On local `production/0.3-art-presentation`:

1. `git pull`
2. rerun `tools/install-quaternius-zombie-kit.ps1 -CommitAndPush`
3. confirm `Zombie_Atlas.png` is downloaded and updated art is pushed
4. wait for Unity/glTFast reimport
5. run `DEADREACH > Production > Setup Quaternius Starter Art` again to overwrite wrappers/controllers
6. run `DEADREACH > Production > Validate Asset Catalog`
7. regenerate with `DEADREACH > Build Production Slice 0.3`
8. Play → Deploy
9. require textured/colored Survivor + Infected
10. require only one DEADREACH-equipped Rifle presentation on Survivor
11. validate scale/orientation/animations/weapon mount/muzzle origin
12. require movement/combat/loot/extraction regression-free and no blocking Console errors

## 10. After corrected real starter-art validation

1. tune weapon socket/orientation if needed
2. proper muzzle flash / impact production VFX
3. first real combat audio-content pass
4. extend Quaternius kit into Dead City environment props
5. URP post-processing / color grading / atmosphere
6. replace prototype IMGUI with production HUD/loadout UI
7. production NavMesh navigation
8. physical-device mobile validation + iOS/Android build profiling

## 11. Handoff protocol

When resuming:

1. read this file first
2. inspect active branch / PR #3
3. note compile/generator + empty-catalog fallback gates passed
4. note first real-art rendering works but first pass was gray and had duplicate Survivor weapons
5. note atlas + SingleWeapon/suppression fixes are implemented but require revalidation
6. update this file after the corrected art test

Do not rely on chat history alone.
