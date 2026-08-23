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

## 3. Validated / merged milestones

### Vertical Slice 0.1 — MERGED / VALIDATED

PR #1 merged on 2026-08-23.

Merge commit:

`e4d5dbe2c52d3e9aeed52f421fdd99f7c6b01877`

Validated:
- clean compile
- Bunker / Dead City generation
- movement / aim / fire
- infected combat / death
- Scrap / extraction / persistence
- Pause / Resume / Abandon
- quality presets

### Production 0.2 — MERGED / VALIDATED

PR #2 merged on 2026-08-23.

Merge commit:

`fd0dca0ece7d18ca005f2f4b52d65039904fad27`

Validated:
- 0 compiler errors / 0 compiler warnings at acceptance
- tracer / impact / critical feedback
- weapon pickups + run inventory
- unsecured weapon loss on death/abandon
- extraction into persistent stash
- rarity / item power / affixes
- equipped primary persistence
- affixes modifying next-run combat stats
- foundation death / extraction / pause / abandon flows remain functional

Established progression loop:

**Find weapon → survive → extract → stash → equip → next run becomes stronger/different**

### Production 0.3 — MERGED / REAL UNITY VALIDATED

PR #3 `production: art asset binding and presentation pipeline 0.3 — validated` merged on 2026-08-23.

Squash merge commit:

`924e8ff4ae250da13fd0d198b121802cf80131b0`

Real Unity validation passed:
- 0 compiler errors
- `DEADREACH > Build Production Slice 0.3` completes with 0 errors
- empty-catalog fallback remains functional
- real Quaternius Survivor replaces Capsule
- real Infected variants replace prototype enemies
- atlas/material colors render correctly in gameplay
- embedded artist-rigged Survivor weapon is visible and stable
- current embedded weapon is on the **left hand**; accepted and locked for this starter-art pass
- weapon remains aligned while moving / aiming / firing
- muzzle/tracer originate from the embedded weapon
- movement / combat / loot / extraction / Bunker return remain functional
- no blocking Console errors reported

## 4. Production 0.3 architecture / locked decisions

### Production Asset Catalog

`ProductionAssetCatalog` supports:
- Survivor prefab
- Infected variants
- Primary Weapon prefab for progression/future presentation
- survivor/infected visual offsets/scales

Catalog path:

`Assets/Deadreach/Resources/Deadreach/ProductionAssetCatalog.asset`

### Production Visual Binder

`ProductionVisualBinder` keeps gameplay roots separate from production art:
- gameplay CharacterController / Damageable / AI / weapon logic remain on validated roots
- production prefab replaces prototype rendering
- Animator is rebound into existing animation drivers
- missing art safely falls back to prototype visuals

### Current Sam weapon strategy — LOCKED

Earlier attempts to mount `Weapon_Rifle.gltf` on discovered/fallback hand sockets produced unstable orientation and repeated transform conflicts.

The accepted strategy is:
1. use Quaternius `Characters_Sam_SingleWeapon.gltf`
2. use its artist-authored embedded weapon as the visible Survivor weapon
3. do **not** instantiate a second external Rifle on Sam
4. do **not** rotate/reposition the embedded weapon to move it from the left hand
5. derive gameplay muzzle directly from the embedded weapon mesh
6. keep the external Rifle asset for progression/future weapon presentation until a proper rifle-hold rig/animation path exists

Do not reintroduce the old transform/socket hacks.

### Combat VFX hardening

`CombatFeedbackPresenter` uses a preallocated tracer pool instead of per-shot GameObject creation/destruction.

## 5. Production art source

**Quaternius — Zombie Apocalypse Kit**

Current subset:
- Survivor Sam / Sam SingleWeapon
- Zombie Basic
- Zombie Chubby
- Zombie Arm
- Zombie Ribcage
- Rifle asset for future/progression use
- shared `Zombie_Atlas.png`

License/source tracking:

`docs/THIRD_PARTY_ASSETS.md`

Unity package:

`com.unity.cloud.gltfast` `6.17.0`

Git LFS tracks glTF/GLB and large art assets.

## 6. Current Git state

- `main` contains validated 0.1 + 0.2 + 0.3
- latest validated merge: `924e8ff4ae250da13fd0d198b121802cf80131b0`
- next production branch: **`production/0.4-environment-atmosphere`**

## 7. Production 0.4 target

Goal: make Dead City stop looking like prototype geometry and start reading as a real game environment without breaking the validated 0.3 character/combat path.

Priority order:
1. real Dead City environment props/building pieces from compatible licensed assets
2. environment dressing / streets / barriers / debris / wreckage / vertical landmarks
3. URP lighting pass
4. fog / atmosphere / dust / sparks / local VFX
5. post-processing / tonemapping / bloom / color grading
6. stronger extraction-beacon presentation
7. production muzzle flash / impact VFX
8. first real combat audio-content pass
9. preserve 0.3 character/weapon mount exactly
10. retain mobile performance presets

## 8. Deferred / later

- proper authored rifle-hold character/animation path
- production HUD/loadout UI replacing prototype IMGUI
- production NavMesh navigation
- Addressables/content organization
- physical-device touch/haptics validation
- iOS / Android builds and profiling
- backend/accounts/leaderboards/events
- IAP cosmetics / season structure

## 9. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1 / 0.2 / 0.3 as validated merged baselines
3. never reintroduce external Rifle transform/socket hacks onto Sam
4. current left-hand embedded weapon is accepted
5. active work should continue on `production/0.4-environment-atmosphere`
6. update this file after each major 0.4 pass / Unity validation

Do not rely on chat history alone.
