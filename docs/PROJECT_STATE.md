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

## 3. Current Git state

- stable branch: `main`
- stable production level before promotion: **0.8**
- stable merge before promotion: `876127fb9997951afcca738cd7251acd2f662014`
- active branch: **`production/0.9-combat-depth`**
- PR #9 targets `main`
- Production 0.9 compile/build/runtime/mobile/final-regression gates are all green
- PR #9 is ready for promotion to `main`

## 4. Production 0.8 baseline preserved in 0.9

### Workshop / progression
- save schema v6 and existing profiles preserved
- real Item Power combat scaling
- weapon Calibration and two-step Salvage
- Workbench / Medbay / Cargo Rig / Scavenger Network progression
- Workshop remains available after every expedition → Bunker return

### Presentation / content baseline
- Arsenal Rifle / SMG / Pistol / Shotgun orientation and framing
- Bunker layouts at 4:3 / 16:10 / 16:9 / ~19:9 landscape
- landscape-only mobile orientation
- accepted desktop/tablet combat presentation
- slim boss health/identity strip and mutation-state chip
- lower-right Relic reward toast
- Bunker reward debrief → Arsenal transfer
- sector atmosphere FX

## 5. Production 0.9 — Combat Depth — FULLY REAL-UNITY VALIDATED

### Infected combat roles
- **WALKER** remains the readable baseline chaser
- **RUNNER** has a timed medium-range burst
- **BRUTE** has a separate close-range slam
- **STALKER** has lateral flank/reposition movement
- role-colored special telegraphs accepted
- normal role specials do not override mutation-boss phase logic

### Operator active abilities
- **SAM / FIELD PATCH** — heal ability with cooldown protection at full HP
- **RAVEN / VECTOR DASH** — collision-aware tactical dash
- **BRIGGS / SHOCKWAVE** — close-range infected damage ability
- desktop / gamepad inputs accepted
- mobile Ability input accepted

### Fixed-zone mobile controls — ACCEPTED
- fixed lower-left MOVE center from `Screen.safeArea`
- fixed lower-right AIM/FIRE center from `Screen.safeArea`
- full 360-degree movement with deadzone/response shaping
- faster mobile acceleration/deceleration for immediate twin-stick response
- camera-relative directional aiming only
- absolute touch coordinates never become weapon world targets
- right stick fires outside deadzone and stops on release
- simulated mobile touch suppresses mirrored mouse-pointer aim
- upper-right Ability has an enlarged independent touch region
- Ability queues on touch-begin and does not become move/aim/fire
- Ability feedback includes `FIRED`, `NO TARGET`, `FULL HP`, `BLOCKED` or `COOLDOWN`
- phone FIELD OPS / Vitals / HP / loot / scrap / objective readability accepted

### Final 0.9 regression — PASSED 2026-08-23
- Bunker → Workshop → Deploy → combat/loot → extraction → Bunker passed
- Workshop remains present after scene reload
- Calibration / Salvage / Bunker upgrade persistence remains intact
- Arsenal / accepted presentation remain intact
- enemy roles remain accepted
- SAM / RAVEN / BRIGGS gameplay remains accepted
- **0 red runtime errors**

### Known 0.10 follow-up
- BRIGGS Shockwave needs a dedicated visible expanding ring / ground pulse / impact VFX

## 6. Promotion status

Production 0.9 is fully validated and ready to merge to `main`.

After merge:
1. update this file on `main` with the actual PR #9 merge commit
2. mark Production 0.9 as the stable baseline
3. branch Production 0.10 from current `main`
4. first 0.10 visual debt: dedicated BRIGGS Shockwave FX

## 7. Handoff protocol

When resuming after the merge:
1. read this file first
2. Production 0.9 is the intended stable baseline
3. preserve schema-v6 Workshop progression and accepted presentation
4. preserve fixed-zone mobile controls
5. never reintroduce external gameplay hand-mounted Rifle transforms
6. keep mobile landscape-only
7. start 0.10 from current `main`
