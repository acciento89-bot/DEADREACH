# DEADREACH — Project State

_Last updated: 2026-08-23_

Canonical handoff for DEADREACH. Update after every major implementation, validation and merge. Do not rely on chat history alone.

## 1. Product identity

- **Game:** DEADREACH
- **Studio:** Kamilunavo
- **Repository:** `acciento89-bot/DEADREACH`
- **Platforms:** iOS + Android
- **Unity:** `6000.3.22f1`
- **Render pipeline:** URP 17.3
- **iOS Bundle ID:** `de.kamilunavo.deadzone`
- **App Store SKU:** `deadzone-001`
- **Monetization:** IAP only, no ads
- **Mobile orientation:** landscape only

Core loop:

**Bunker → Deploy → Expedition → Combat → Loot → Risk decision → Extract / Die / Abandon → Bunker → Equip / Upgrade → Deploy stronger**

## 2. Validated / merged baselines

### Vertical Slice 0.1 — MERGED / VALIDATED
Merge `e4d5dbe2c52d3e9aeed52f421fdd99f7c6b01877`.

### Production 0.2 — MERGED / VALIDATED
Merge `fd0dca0ece7d18ca005f2f4b52d65039904fad27`.

### Production 0.3 — MERGED / REAL UNITY VALIDATED
PR #3 merge `924e8ff4ae250da13fd0d198b121802cf80131b0`.

Permanent weapon rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig for gameplay
- derive muzzle from that embedded firearm
- **never reintroduce the failed external hand-mounted Rifle transform path**

### Production 0.4 — MERGED / REAL UNITY VALIDATED
PR #4 squash merge `e86c067720f8f6badc6c8a29e41bcd856c29ffe6`.

### Production 0.5 — MERGED / REAL UNITY VALIDATED
PR #5 squash merge `a066386f05c6593f1840ef6902f62c808cbdf319`.

### Production 0.6 + 0.7 — MERGED / REAL UNITY VALIDATED
- PR #7 merged Production 0.7 presentation polish into the 0.6 integration branch at `9633aabd903251a09c8475b5b8672a03988a92bc`
- PR #6 promoted the full 0.6 + 0.7 stack to `main` at `b69f5270a6e3e26780ccaa0445e4e6764808f753`

### Production 0.8 — MERGED / REAL UNITY VALIDATED
- PR #8 squash merged to `main` at `876127fb9997951afcca738cd7251acd2f662014`
- Workshop / Calibration / Salvage / permanent Bunker upgrades validated

### Production 0.9 — MERGED / REAL UNITY VALIDATED
- PR #9 squash merged to `main` at `2f15df3b5ca7b15eeacea39928b63118700e2432`
- combat roles + operator abilities validated
- final fixed-zone mobile twin-stick controls validated
- mobile HUD readability validated
- final full regression passed with **0 red runtime errors**

## 3. Current Git state

- stable branch: `main`
- stable production level: **0.9**
- stable merge: `2f15df3b5ca7b15eeacea39928b63118700e2432`
- active branch: **`production/0.10-combat-impact`**
- PR #10: Draft, targets `main`
- Production 0.10 compile gate passed in real Unity with **0 red compiler errors**
- `DEADREACH > Build Production Slice 0.10` completed successfully in real Unity
- next gate: visual/runtime acceptance for operator VFX, infected-special VFX, hit/crit feedback and camera impact

## 4. Stable Production 0.9 baseline that must remain green

### Progression
- save schema v6
- Item Power / Calibration / Salvage
- Workbench / Medbay / Cargo Rig / Scavenger Network
- Workshop survives expedition → Bunker reload

### Combat identities
- WALKER baseline
- RUNNER burst
- BRUTE slam
- STALKER flank
- SAM Field Patch
- RAVEN Vector Dash
- BRIGGS Shockwave gameplay

### Mobile
- fixed lower-left MOVE
- fixed lower-right AIM/FIRE
- independent upper-right Ability
- full 360-degree movement
- direction-based aiming only
- phone HUD readable

### Presentation
- accepted Arsenal orientation/framing
- responsive Bunker layouts
- boss/reward presentation
- sector atmosphere FX

## 5. Production 0.10 — Combat Impact / Presentation

Goal: make combat feel materially less prototype-like without destabilizing the accepted 0.9 gameplay and mobile control layer.

### Ability impact presentation implemented
- **SAM / Field Patch**: dual green/cyan expanding rings + healing motes + light lens impulse
- **RAVEN / Vector Dash**: blue/white directional dash trails + endpoint pulse + trail particles + lens impulse
- **BRIGGS / Shockwave**: large orange/hot expanding ground rings + radial debris/energy particles + strong lens impulse
- this closes the explicit 0.10 Shockwave visual debt from 0.9

### Infected special impact presentation implemented
- **Runner Burst**: cyan movement streak + endpoint pulse
- **Brute Slam**: red/orange expanding slam rings + radial particles + stronger lens impulse
- **Stalker Flank**: violet start/end pulses + flank trail
- mutation-boss authority remains unchanged

### Gunfight feedback added
- existing tracer / muzzle / sparks / gore presenter remains authoritative
- new short world-space hit marker appears on successful damage hits
- critical hit marker uses stronger magenta presentation
- critical hits add a small expanding impact ring
- player damage / death / heavy abilities add subtle camera-lens impact rather than moving the camera transform

### Runtime architecture
- `CombatFeedback` exposes typed operator-ability and infected-special impact events
- `OperatorAbilityController` emits presentation events only after successful ability activation
- `InfectedCombatRoleBrain` emits presentation events after its accepted special movement/damage action
- `CombatImpactPresentation` is a persistent runtime presenter
- `RuntimeImpactRing` and `RuntimeImpactLine` are short-lived runtime renderers
- effects use runtime URP-safe materials; no new external art dependency

### Build gate
- menu: `DEADREACH > Build Production Slice 0.10`
- test plan: `docs/PRODUCTION_10_TEST.md`
- compile: **PASSED 2026-08-23 — 0 red compiler errors**
- build: **PASSED 2026-08-23 — Production Slice 0.10 completed**

## 6. Current 0.10 gate

Green:
1. Unity compile → **0 red compiler errors** ✅
2. `DEADREACH > Build Production Slice 0.10` ✅

Required next:
3. SAM / RAVEN / BRIGGS VFX visible and readable
4. Runner / Brute / Stalker impact VFX visible without obscuring gameplay
5. hit marker visible on successful hits; critical presentation distinct
6. camera-lens impulse noticeable but not uncomfortable
7. fixed-zone mobile controls remain fully correct
8. final Bunker → Workshop → Deploy → combat/loot → extract → Bunker regression
9. **0 red runtime errors**

## 7. Handoff protocol

When resuming:
1. read this file first
2. stable baseline is Production 0.9 on `main`
3. active work is Production 0.10 on `production/0.10-combat-impact`
4. 0.10 compile + build gates are green; visual/runtime/mobile gates remain
5. preserve schema-v6 Workshop progression
6. preserve fixed-zone mobile controls
7. never reintroduce external gameplay hand-mounted Rifle transforms
8. keep mobile landscape-only
