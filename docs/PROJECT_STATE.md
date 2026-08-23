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
- six selected Quaternius glTF model files + license evidence are committed on the 0.3 branch
- real Survivor/Infected/Rifle assets load and replace prototypes in Play Mode
- duplicate built-in Survivor weapon rack was reduced, but current mounted Rifle still uses an incorrect fallback socket and appears at head height
- Survivor, Infected and Rifle prefabs still render white/gray, proving the problem exists at prefab/material setup level rather than only in the generated gameplay scene
- explicit URP atlas-material assignment + robust right-hand/grip pivot correction are implemented on branch and require local Unity revalidation

Do not claim 0.3 production-art complete until corrected colored materials and weapon mounting are visually/runtime validated.

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

### Quaternius asset cleanup / material hardening

First real-art tests established:

1. actual glTF models load and replace prototype visuals correctly
2. all imported production prefabs still render white/gray, including the standalone Rifle prefab
3. current generated Survivor prefab shows `WeaponSocket` as a root sibling of `Model`, proving automatic right-hand lookup fell back instead of attaching to the hand bone
4. current Rifle can therefore appear around head/upper-body height even though the equipment binder itself is working

Implemented correction now on branch:

- installer keeps `Zombie_Atlas.png` beside flattened glTF subset and normalizes atlas URIs
- setup now additionally creates `Assets/Deadreach/Art/Production/Materials/Quaternius_ZombieAtlas.mat`
- material uses URP/Lit (Standard fallback), explicit atlas BaseMap/MainTex, white base color, low smoothness and zero metallic
- setup explicitly assigns that material to every active Survivor/Infected/Rifle renderer instead of trusting glTF material resolution
- Survivor source remains Quaternius `Characters_Sam_SingleWeapon.gltf`
- separately named embedded weapon renderers remain suppressed
- right-hand resolution now tries Humanoid `HumanBodyBones.RightHand`, normalized transform/bone-name scoring, then a geometry-derived bone fallback
- old root/head weapon fallback is replaced with hand-height geometry fallback if no bone is resolved
- Rifle wrapper now rotates long-X geometry onto DEADREACH +Z forward, relocates wrapper origin to a calculated grip/trigger point and places `MuzzleSocket` at the barrel-forward bound
- DEADREACH remains sole owner of equipped-weapon presentation

Latest implementation commit for these fixes:

`8852cf428847249c86e8da0908286554b62ed20d`

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

### Real-art visual gate — PARTIAL PASS / MATERIAL + SOCKET REVALIDATION REQUIRED

Confirmed by user screenshots in Play Mode and prefab view:

- real Survivor model appears instead of Capsule
- real Infected models appear instead of prototype enemies
- real Rifle prefab loads
- production visual binder therefore works with actual imported glTF assets
- standalone Survivor, Infected and Rifle prefabs are still white/gray
- current Survivor `WeaponSocket` is at wrapper root instead of right-hand bone
- mounted Rifle appears at head/upper-body height
- duplicate/multi-weapon problem is substantially reduced compared with first import

Explicit material and socket/grip fixes are implemented but **not yet revalidated in Unity**.

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
2. wait for Unity compile
3. require **0 red compile errors**
4. run `DEADREACH > Production > Setup Quaternius Starter Art` again to rebuild wrappers/material/controllers
5. inspect `Survivor_Quaternius_Sam.prefab`: `WeaponSocket` should ideally be nested under a hand/bone hierarchy; root fallback is acceptable only if positioned at actual hand height
6. inspect Survivor/Rifle prefab color: atlas texture should now be visible rather than plain white/gray
7. run `DEADREACH > Production > Validate Asset Catalog`
8. regenerate with `DEADREACH > Build Production Slice 0.3`
9. Play → Deploy
10. require colored Survivor + Infected + Rifle
11. require exactly one DEADREACH-equipped Rifle at/near the right hand rather than the head
12. validate muzzle/tracer origin, movement/combat/loot/extraction and no blocking Console errors

## 10. After corrected real starter-art validation

1. final weapon socket/orientation polish if visually required
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
4. note real model binding works
5. note latest screenshots proved white/gray prefab materials and root-level weapon socket/head placement
6. note explicit URP atlas assignment + right-hand/grip wrapper fixes are implemented in commit `8852cf428847249c86e8da0908286554b62ed20d` but need Unity revalidation
7. update this file after corrected art test

Do not rely on chat history alone.
