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

- stable branch before promotion: `main`
- stable production level before promotion: **0.9**
- stable merge before promotion: `2f15df3b5ca7b15eeacea39928b63118700e2432`
- active branch: **`production/0.10-combat-impact`**
- PR #10 targets `main`
- Production 0.10 compile, build, combat presentation, mobile and final regression gates are all green
- Production 0.10 is fully real-Unity validated and ready for promotion

## 4. Stable Production 0.9 baseline preserved in 0.10

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

## 5. Production 0.10 — Combat Impact / Presentation — FULLY REAL-UNITY VALIDATED

### Ability impact presentation
- **SAM / Field Patch**: dual green/cyan expanding rings + healing motes + light lens impulse ✅
- **RAVEN / Vector Dash**: blue/white directional dash trails + endpoint pulse + trail particles + lens impulse ✅
- **BRIGGS / Shockwave**: large orange/hot expanding ground rings + radial debris/energy particles + strong lens impulse ✅
- explicit 0.9 Shockwave visual debt closed ✅

### Infected special impact presentation
- **Runner Burst**: cyan movement streak + endpoint pulse ✅
- **Brute Slam**: red/orange expanding slam rings + radial particles + stronger lens impulse ✅
- **Stalker Flank**: violet start/end pulses + flank trail ✅
- mutation-boss authority remains unchanged

### Gunfight feedback
- existing tracer / muzzle / sparks / gore presenter preserved ✅
- successful-hit world marker accepted ✅
- critical hit marker + critical pulse accepted ✅
- player damage / death / heavy-ability lens impacts accepted ✅

### Runtime architecture
- `CombatFeedback` exposes typed operator-ability and infected-special impact events
- `OperatorAbilityController` emits presentation events only after successful ability activation
- `InfectedCombatRoleBrain` emits presentation events after accepted special movement/damage actions
- `CombatImpactPresentation` is a persistent runtime presenter
- `RuntimeImpactRing` and `RuntimeImpactLine` are short-lived runtime renderers
- runtime URP-safe materials; no new external art dependency

### Validation
- Unity compile: **PASSED — 0 red compiler errors** ✅
- `DEADREACH > Build Production Slice 0.10`: **PASSED** ✅
- operator / infected / hit / crit / camera impact runtime: **PASSED** ✅
- fixed-zone mobile MOVE / AIM-FIRE / ABILITY regression: **PASSED** ✅
- VFX do not obstruct mobile controls or HUD: **PASSED** ✅
- Bunker → Workshop → Deploy → combat/loot → extraction → Bunker: **PASSED** ✅
- Workshop/progression/Arsenal/accepted presentation remain intact ✅
- final Unity Console: **0 red runtime errors** ✅

## 6. Promotion status

Production 0.10 is fully validated and ready to merge to `main`.

After merge:
1. update this file on `main` with the actual PR #10 merge commit
2. mark Production 0.10 as the stable baseline
3. branch the next production pass from current `main`
4. preserve the accepted 0.10 combat-impact layer and 0.9 mobile-control baseline

## 7. Handoff protocol

When resuming after merge:
1. read this file first
2. Production 0.10 is the intended stable baseline
3. preserve schema-v6 Workshop progression
4. preserve fixed-zone mobile controls
5. preserve the accepted combat-impact presentation layer
6. never reintroduce external gameplay hand-mounted Rifle transforms
7. keep mobile landscape-only
8. start the next production pass from current `main`
