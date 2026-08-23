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
- Production 0.3 **real Unity compile + generator gate PASSED** on 2026-08-23
- Production 0.3 **empty-catalog fallback runtime gate PASSED** on 2026-08-23
- Quaternius Zombie Apocalypse Kit selected as first production-art source
- Google Drive automated retrieval failed because at least one public file currently rejects gdown access
- installer now uses a public mirror of the same pack for the selected glTF files
- Unity glTFast `6.17.0` added for Editor import of those `.gltf` assets
- actual production binaries still need the next local installer/import validation

Do not claim 0.3 production-art complete until real Survivor/Infected/Weapon assets are imported, assigned and validated.

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

## 8. Selected first production-art source

**Quaternius — Zombie Apocalypse Kit**

Official creator page states:

- 4 playable characters with animations
- 4 infected/enemy characters
- weapons + matching apocalypse props
- FBX / OBJ / glTF / Blend formats
- CC0 / commercial use

Initial DEADREACH subset:

- Survivor Sam
- Zombie Basic
- Zombie Chubby
- Zombie Arm
- Zombie Ribcage
- Rifle

License tracking:

`docs/THIRD_PARTY_ASSETS.md`

### Current import strategy

Original creator Google Drive remains documented as the original distribution, but automated `gdown` failed with a public-link/permission response for file id `1iBNVZtY_mYqHMe81_cGaF85rkGjhkouk`.

The installer now retrieves only the selected `.gltf` subset from:

`agentkaerf/FreeModels/Zombie Apocalypse Kit - March 2024`

The mirror contains a CC0 1.0 license marker; the official Quaternius page remains the license/source authority.

Unity package added:

`com.unity.cloud.gltfast` `6.17.0`

Git LFS now tracks `.gltf` and `.glb` as well.

## 9. Immediate next gate

On the local `production/0.3-art-presentation` branch:

1. `git pull`
2. let Unity resolve/install glTFast if the Editor is open
3. rerun `tools/install-quaternius-zombie-kit.ps1 -CommitAndPush`
4. confirm all six selected `.gltf` files download and push successfully
5. wait for Unity glTF import
6. run `DEADREACH > Production > Setup Quaternius Starter Art`
7. run `DEADREACH > Production > Validate Asset Catalog`
8. regenerate with `DEADREACH > Build Production Slice 0.3`
9. validate Survivor/Infected/Rifle visual replacement, scale/orientation, Animator hookup, weapon mount and muzzle origin
10. require no blocking Console errors

## 10. After real starter-art validation

1. fix scale/orientation/socket offsets as required
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
4. note Google Drive retrieval failed and mirror/glTFast fallback is now implemented
5. continue with actual six-file glTF download/import and wrapper validation
6. update this file after the next major pass

Do not rely on chat history alone.
