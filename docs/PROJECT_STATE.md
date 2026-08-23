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
- Production 0.9 compile gate passed with **0 red compiler errors** on 2026-08-23
- `DEADREACH > Build Production Slice 0.9` completed successfully on 2026-08-23
- enemy-role runtime gate passed: Walker baseline, Runner burst, Brute slam and Stalker flank behavior accepted
- operator active-ability runtime gate passed: SAM Field Patch, RAVEN Vector Dash and BRIGGS Shockwave all function as intended
- open non-blocking presentation debt: BRIGGS Shockwave currently has no dedicated visible FX; add dedicated shockwave VFX in Production 0.10
- current remaining gate: mobile ability-touch isolation → final Production 0.8 regression / 0 red runtime errors

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
- compact Field Ops HUD
- slim boss health/identity strip and mutation-state chip
- lower-right Relic reward toast
- Bunker reward debrief → Arsenal transfer
- Flooded Industrial rain / Ash District ash / Blackout dust / Ground Zero contamination FX
- final Bunker → expedition → combat → loot → return flow with no red runtime errors

## 5. Production 0.9 goal — Combat Depth

Turn existing statistical variants into real gameplay identities.

### Infected combat roles implemented / runtime accepted
- **WALKER** remains the readable baseline chaser
- **RUNNER** gains a timed medium-range forward burst and possible burst contact damage
- **BRUTE** gains a separate high-damage close-range slam cooldown
- **STALKER** gains periodic lateral flank/reposition movement instead of only direct pursuit
- Runner / Brute / Stalker special moves have short role-colored point-light telegraphs
- normal role abilities are not applied to mutation bosses; existing boss phase logic remains authoritative
- role binding happens after `RunDifficultyDirector` has named/configured the encounter, preserving the validated scene-generation path

### Operator active abilities implemented / runtime accepted
- **SAM / FIELD PATCH** — restore 32% max HP, 18s cooldown, no cooldown waste at full health
- **RAVEN / VECTOR DASH** — collision-aware 4.6m dash, 7.5s cooldown
- **BRIGGS / SHOCKWAVE** — damage all infected within 4.6m, 12s cooldown, no cooldown waste with no valid target
- Operator definitions expose ability name / description / cooldown
- desktop input: `SPACE`
- gamepad input: right shoulder
- mobile input: reserved Ability touch region excluded from move/aim touch ownership
- in-expedition ability HUD shows ability name and READY/cooldown state

### Known 0.10 visual-polish follow-up
- BRIGGS Shockwave needs a dedicated visible expanding ring / impact FX so the gameplay effect has strong visual feedback
- this is presentation debt only; Shockwave damage/range/cooldown behavior is already runtime accepted in 0.9

### Build / bootstrap
- `Production09CombatDepthBootstrap` binds role brains and operator ability controller at runtime after validated systems finish `Start`
- menu gate: `DEADREACH > Build Production Slice 0.9`
- test plan: `docs/PRODUCTION_09_TEST.md`

## 6. Current 0.9 gate

Completed:
1. Unity compile → **0 red compiler errors** ✅
2. `DEADREACH > Build Production Slice 0.9` → completed successfully ✅
3. Runner / Brute / Stalker / Walker role behavior → runtime accepted ✅
4. SAM / RAVEN / BRIGGS active abilities → runtime accepted ✅

Remaining:
5. validate mobile Ability touch does not also become move / aim / fire
6. run full 0.8 regression and require **0 red runtime errors**

## 7. Handoff protocol

When resuming:
1. read this file first
2. stable baseline is Production 0.8 on `main`
3. active work is Production 0.9 on `production/0.9-combat-depth`
4. compile + Build Production Slice 0.9 + enemy roles + operator abilities are green
5. BRIGGS Shockwave dedicated FX is tracked for Production 0.10 and does not block 0.9
6. next gate is mobile ability-touch isolation, then full 0.8 regression
7. preserve schema-v6 Workshop progression and accepted 0.7 presentation
8. never reintroduce external gameplay hand-mounted Rifle transforms
9. keep mobile landscape-only
10. run `docs/PRODUCTION_09_TEST.md` before promoting 0.9
