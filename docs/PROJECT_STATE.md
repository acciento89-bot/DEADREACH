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
- Production 0.8 is the stable branch baseline for 0.9

## 3. Current Git state

- stable branch: `main`
- stable production level: **0.8**
- stable merge: `876127fb9997951afcca738cd7251acd2f662014`
- active branch: **`production/0.9-combat-depth`**
- PR #9: Draft, targets `main`
- enemy-role runtime gate passed: Walker baseline, Runner burst, Brute slam and Stalker flank behavior accepted
- operator active-ability gameplay passed on desktop: SAM Field Patch, RAVEN Vector Dash and BRIGGS Shockwave accepted
- BRIGGS Shockwave dedicated FX remains non-blocking 0.10 visual debt
- **mobile gate remains RED** after two rejected control implementations
- current third implementation uses fixed safe-area controls, touch-begin Ability capture and visible Ability feedback
- this third implementation is committed but **not yet Unity-validated**

## 4. Production 0.8 shipped baseline that must remain green

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

## 5. Production 0.9 — Combat Depth

### Runtime accepted
- **WALKER** baseline chaser
- **RUNNER** timed medium-range burst
- **BRUTE** close-range slam
- **STALKER** lateral flank/reposition
- **SAM / FIELD PATCH**
- **RAVEN / VECTOR DASH**
- **BRIGGS / SHOCKWAVE** gameplay

### Mobile history
- attempt 1 rejected: movement/aim not production usable; aim could pull toward Ability UI; HUD too small
- attempt 2 rejected: firing improved but MOVE still used a floating first-touch origin and real mobile use remained unreliable; Ability lacked trustworthy visible confirmation
- neither rejected implementation may be treated as accepted

### Current fixed-zone mobile pass — AWAITING REAL VALIDATION
- fixed lower-left MOVE center derived from `Screen.safeArea`
- fixed lower-right AIM/FIRE center derived from `Screen.safeArea`
- generous circular capture zones larger than the visible sticks
- MOVE uses full 360-degree X/Y input with deadzone + shaped response
- mobile movement acceleration/deceleration is fast for immediate direction changes
- AIM/FIRE uses camera-relative directional input only
- absolute touch coordinates are never converted to a weapon world target
- right stick fires immediately once directional input leaves its deadzone
- simulated mobile touch suppresses mirrored mouse-pointer aim
- Ability is upper-right with an enlarged independent touch region
- Ability queues on touch-begin
- Ability shows `FIRED`, `NO TARGET`, `FULL HP`, `BLOCKED` or `COOLDOWN` feedback so input registration is visible
- fixed stick visuals and fixed input capture share the exact same centers
- phone HUD remains scaled up for readability

### Known 0.10 follow-up
- BRIGGS Shockwave needs a dedicated expanding ring / ground pulse / impact VFX

## 6. Current 0.9 gate

Already green:
1. enemy roles ✅
2. SAM / RAVEN / BRIGGS gameplay on desktop ✅

Fresh gate required now:
3. `git pull` latest branch
4. Unity compile → **0 red compiler errors**
5. MOVE remains fixed lower-left and supports up/down/left/right/diagonal
6. movement stops/changes direction quickly
7. AIM/FIRE remains fixed lower-right and both aiming + shooting work in every direction
8. aim never snaps toward Ability/UI
9. Ability upper-right gives immediate visible feedback and never becomes move/aim/fire
10. mobile HUD remains comfortably readable
11. final 0.8 regression + **0 red runtime errors**

## 7. Handoff protocol

When resuming:
1. read this file first
2. stable baseline remains Production 0.8 on `main`
3. active work is Production 0.9 on `production/0.9-combat-depth`
4. enemy roles and operator gameplay are green
5. both earlier mobile implementations were rejected and must never be treated as accepted
6. current fixed-zone mobile pass must pass fresh compile + runtime validation
7. BRIGGS Shockwave FX is tracked for 0.10 and does not block 0.9
8. preserve schema-v6 Workshop progression and accepted presentation
9. never reintroduce external gameplay hand-mounted Rifle transforms
10. keep mobile landscape-only
11. run `docs/PRODUCTION_09_TEST.md` before promoting 0.9
