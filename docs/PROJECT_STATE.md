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
- Production Art / Presentation 0.3 is implemented in part but **NOT YET Unity-compiled on this branch**

Do not claim 0.3 is stable until the next real Unity compile/runtime gate passes.

## 6. Production Art / Presentation 0.3 — CURRENT IMPLEMENTATION

### Production Asset Catalog

New runtime asset:

`ProductionAssetCatalog`

Catalog slots:

- Survivor prefab
- one or more Infected prefabs
- Primary Weapon prefab
- local transform offsets/scales for survivor/infected production visuals

Expected asset path created by editor tooling:

`Assets/Deadreach/Resources/Deadreach/ProductionAssetCatalog.asset`

Editor menu:

`DEADREACH > Production > Create or Select Asset Catalog`

### Production Visual Binder

`ProductionVisualBinder` now separates gameplay roots from production art.

Behavior:

- if a production prefab is assigned, prototype renderers are hidden
- CharacterController / Damageable / AI / weapon gameplay stay on the validated root
- Survivor production prefab is instantiated as visual child
- Infected production prefabs are selected by variant index
- Animator is rebound into existing animation drivers
- Survivor `WeaponSocket` / `RightHandWeaponSocket` is detected
- Primary Weapon prefab is mounted on that socket
- weapon `MuzzleSocket` / `Muzzle` is forwarded into `HitscanWeapon`
- if no production prefab is assigned, prototype visual remains as safe fallback

### Production asset validation

Editor menu:

`DEADREACH > Production > Validate Asset Catalog`

Validator checks:

- Survivor prefab assigned
- Survivor Animator
- Survivor weapon socket
- Infected variants assigned
- Infected Animators
- Primary Weapon prefab assigned
- Weapon muzzle socket

Missing production assets do not intentionally break gameplay; fallback visuals remain available.

### Generated-scene integration

`ProductionSliceEnhancer` now automatically attaches production visual binders to:

- Player survivor gameplay root
- all generated infected gameplay roots

The production asset catalog is automatically ensured during slice generation.

Main generator is now:

**`DEADREACH > Build Production Slice 0.3`**

### Combat VFX mobile hardening

Tracer presentation no longer creates/destroys a GameObject every shot.

`CombatFeedbackPresenter` now uses a preallocated tracer pool (default 24) and recycles LineRenderers based on unscaled lifetime.

This removes a known automatic-fire allocation/GC source before final muzzle/tracer assets are integrated.

### Weapon integration hook

`HitscanWeapon` now exposes and accepts a runtime muzzle transform so production weapon prefabs can drive the true tracer origin.

## 7. Production 0.3 art contract

Survivor prefab should contain:

- Animator
- `WeaponSocket` or `RightHandWeaponSocket`

Existing animation parameters:

- `Speed` float
- `IsMoving` bool
- `IsAiming` bool
- `IsDead` bool
- `Hit` trigger

Infected prefab should contain Animator with:

- `Speed` float
- `Attack` trigger
- `Hit` trigger
- `IsDead` bool

Primary weapon prefab should contain:

- `MuzzleSocket` or `Muzzle`

Detailed art integration document:

`docs/PRODUCTION_03_ART_PIPELINE.md`

## 8. Production 0.3 validation gate — NEXT

1. switch local repo to `production/0.3-art-presentation`
2. pull latest
3. require 0 red compiler errors
4. ideally 0 yellow compiler warnings
5. run `DEADREACH > Build Production Slice 0.3`
6. verify fallback slice still works with an empty production catalog
7. run `DEADREACH > Production > Validate Asset Catalog`
8. verify no gameplay regression in movement/combat/loot/extraction/stash/equipment
9. after real production prefabs are supplied, assign them to the catalog and validate real visual replacement, Animator rebinding, weapon mounting and muzzle origin

## 9. Next priorities inside 0.3

1. integrate actual licensed/owned survivor 3D asset + production Animator controller
2. integrate actual infected 3D assets + animations
3. integrate actual weapon models
4. proper muzzle flash / impact production VFX
5. first real combat audio-content pass
6. Dead City production environment-art replacement framework
7. URP post-processing / color grading / atmosphere
8. replace prototype IMGUI with production HUD/loadout UI
9. production NavMesh navigation
10. physical-device mobile validation + iOS/Android build profiling

## 10. Handoff protocol

When resuming:

1. read this file first
2. inspect active branch / open PR
3. distinguish implemented vs Unity-validated vs device-validated vs actual production-art-complete
4. continue from the 0.3 validation gate and then actual asset integration
5. update this file after the next major pass

Do not rely on chat history alone.
