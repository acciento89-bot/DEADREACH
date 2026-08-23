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

Validated Dead City streets / vehicles / containers / props / atmosphere / collision / Bunker-first flow / extraction traversal.

### Production 0.5 — MERGED / REAL UNITY VALIDATED
PR #5 squash merge **`a066386f05c6593f1840ef6902f62c808cbdf319`**.

Real Unity acceptance included:
- post-apocalyptic Bunker Command Center
- Overview / Arsenal / Operators / Campaign / Store
- three distinct operator models and runtime swapping
- persistent 50-level campaign / five sectors
- Walker / Runner / Brute / Stalker combat profiles
- Level 10 mutation boss, boss HUD and extraction seal
- upgraded tracer / muzzle / impact feedback
- weapon finishes
- automatic next-level selection after extraction
- boss no longer drops ordinary Scrap; dedicated Mutation Relic is actually granted
- Arsenal weapon preview is upright
- Dead City world boundaries / fall safety prevent void-running

`main` stable baseline remains Production 0.5.

## 3. Current Git state

- active stacked branch: **`production/0.7-presentation-polish`**
- parent branch: **`production/0.6-content-rewards-mobile`**
- PR #7 is Draft and targets the 0.6 branch
- Production 0.7 compile gate passed in real Unity on 2026-08-23
- Production 0.7 build gate passed in real Unity on 2026-08-23
- Arsenal weapon orientation gate passed in real Unity on 2026-08-23
- Bunker layout gate passed for supported landscape ratios: 4:3 / 16:10 / 16:9 / ~19:9
- mobile production orientation is intentionally landscape-only; portrait 9:16 is unsupported
- reward/sector runtime presentation gates remain before promotion

## 4. Production 0.6 functional scope inherited by 0.7

- visible boss reward acquisition
- persistent post-extraction Bunker reward debrief
- save schema v5 for secured boss reward state
- true persistent weapon families Rifle / SMG / Pistol / Shotgun
- family-specific combat profiles
- family-aware loot and boss relics
- standalone Quaternius Arsenal models
- five sector identities
- five named mutation boss identities
- safe-area-aware Bunker implementation
- `DEADREACH > Build Production Slice 0.6`

## 5. Production 0.7 presentation polish implemented

### Bunker / responsive layout
- separate header / navigation / content / deploy anchor zones
- dedicated ultrawide / 16:9 / 16:10 / 4:3 landscape behavior
- compact landscape navigation rail tightened to return space to content
- Operator 3D preview binds to the real `OperatorInspector` panel instead of fixed screen percentages
- Operator preview preserves square render framing and avoids compact-layout drift
- portrait is intentionally excluded because the project bootstrap disables portrait autorotation

### Arsenal 3D inspector
- DR-7 baseline orientation preserved
- SMG / Pistol / Shotgun no longer inherit the incorrect Rifle inversion
- family-aware yaw
- combined-bounds recentering
- automatic camera framing
- real Unity Arsenal gate accepted

### Sector atmosphere
- runtime particle materials use URP / Standard / Sprite fallback instead of materialless magenta rendering
- Flooded Industrial: cool rain
- Ash District: drifting ash
- Blackout: sparse dust with reduced purple intensity
- Ground Zero: rising contamination motes

### Reward / debrief presentation
- Bunker debrief uses safe-area-aware modal sizing and blocks underlying interaction
- human-readable affix names
- combat reward was further redesigned after real Unity screenshots showed the centered card still obscured gameplay
- combat reward is now a compact lower-right **RELIC SECURED** toast with weapon / rarity / family / power / finish / affixes
- detailed post-extraction reward review remains in the Bunker

### Gameplay HUD polish
Real Unity screenshots showed that the old runtime field HUD still read like a development overlay even after the first 0.7 reward pass. Follow-up implementation now includes:
- Field Ops panel reduced from a 255px debug-style block to a denser production HUD
- carried Scrap / weapon loot compressed onto one row
- objective line shortened to a dedicated compact status row
- desktop control hint shortened and omitted on mobile builds
- mutation boss bar reduced to a compact top-center strip
- boss name integrated into the boss health bar (`TIER // NAME`)
- the separate large boss identity box is removed
- boss mutation phase survives as a small status chip only
- extraction feedback panel reduced in size
- run-result panel reduced in size

## 6. Current 0.7 real Unity gate state

Passed:
1. 0 red compiler errors
2. `DEADREACH > Build Production Slice 0.7`
3. Arsenal Rifle / SMG / Pistol / Shotgun orientation and framing
4. Bunker layout at supported landscape ratios

Needs re-test after latest runtime-only HUD changes:
5. Level 10 gameplay HUD / boss bar / phase chip
6. boss kill → compact Relic toast
7. extraction → Bunker relic debrief regression
8. Levels 11 / 21 / 31 / 41 sector FX presentation
9. final no-red-error regression

Latest gameplay-HUD polish commits:
- `1a4f1fb50a23c7ac53c0460dd0044c1583beb432`
- `169cd95320857d438f28cc89635f8b546d677a8b`
- `9e85c0d564f9d827f4069ce6b1bace1699fb152c`

These are runtime-only changes; no scene rebuild is required after pulling them, but Unity must compile with 0 red errors before re-test.

## 7. Handoff protocol

When resuming:
1. read this file first
2. treat 0.1–0.5 as merged / real-Unity validated baseline
3. never reintroduce external gameplay hand-mounted Rifle transforms
4. active branch is `production/0.7-presentation-polish`
5. PR #7 remains Draft until remaining runtime presentation gates pass
6. use landscape-only mobile assumptions
7. latest required re-test starts with Level 10 gameplay HUD / compact reward toast
