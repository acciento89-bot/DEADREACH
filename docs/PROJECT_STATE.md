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
- compile/build/runtime gates passed
- Workshop / Calibration / Salvage / permanent Bunker upgrades validated
- expedition → extraction → Bunker lifecycle validated

### Production 0.9 — MERGED / REAL UNITY VALIDATED
- PR #9 squash merged to `main` at `2f15df3b5ca7b15eeacea39928b63118700e2432`
- compile/build/runtime/mobile/final-regression gates passed
- Walker / Runner / Brute / Stalker gameplay identities validated
- SAM / RAVEN / BRIGGS active abilities validated
- final fixed-zone mobile twin-stick controls validated
- mobile HUD readability validated
- final Bunker → Workshop → Deploy → combat/loot → extraction → Bunker regression passed
- **0 red runtime errors** at final acceptance

## 3. Current Git state

- stable branch: `main`
- stable production level: **0.9**
- stable merge: `2f15df3b5ca7b15eeacea39928b63118700e2432`
- no active production branch is authoritative after the 0.9 merge
- next production work must branch from current `main`

## 4. Stable Production 0.9 baseline

### Progression preserved
- save schema v6
- real Item Power combat scaling
- Calibration + two-step Salvage
- Workbench / Medbay / Cargo Rig / Scavenger Network progression
- Workshop survives expedition → Bunker scene reload

### Infected combat roles
- **WALKER** baseline chaser
- **RUNNER** timed medium-range burst
- **BRUTE** close-range slam
- **STALKER** lateral flank/reposition
- role telegraphs accepted
- normal role specials do not override mutation-boss phase logic

### Operator active abilities
- **SAM / FIELD PATCH**
- **RAVEN / VECTOR DASH**
- **BRIGGS / SHOCKWAVE**
- desktop/gamepad/mobile input accepted

### Mobile controls
- fixed lower-left MOVE control
- fixed lower-right AIM/FIRE control
- full 360-degree movement
- fast stop/change response
- camera-relative directional aiming only
- absolute touch coordinates never become weapon world targets
- right stick fires outside deadzone and stops on release
- upper-right Ability has an independent touch region
- Ability does not become move/aim/fire
- mobile FIELD OPS / Vitals / HP / loot / scrap / objective readability accepted

### Presentation baseline preserved
- Arsenal Rifle / SMG / Pistol / Shotgun orientation and framing
- Bunker layouts at 4:3 / 16:10 / 16:9 / ~19:9 landscape
- landscape-only mobile orientation
- slim boss health/identity strip and mutation-state chip
- lower-right Relic reward toast
- Bunker reward debrief → Arsenal transfer
- sector atmosphere FX

## 5. Production 0.10 entry point

Branch Production 0.10 from current `main`.

First confirmed 0.10 visual debt:
- add a dedicated visible BRIGGS Shockwave VFX: expanding ring / ground pulse / impact treatment

Potential next 0.10 work should preserve the accepted Production 0.9 combat and mobile-control stack.

## 6. Handoff protocol

When resuming:
1. read this file first
2. stable baseline is Production 0.9 on `main`
3. stable merge is `2f15df3b5ca7b15eeacea39928b63118700e2432`
4. branch the next production pass from current `main`
5. preserve schema-v6 Workshop progression
6. preserve fixed-zone mobile controls
7. never reintroduce external gameplay hand-mounted Rifle transforms
8. keep mobile landscape-only
9. first 0.10 known visual debt is BRIGGS Shockwave VFX
