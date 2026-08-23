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

**Bunker → Deploy → Expedition → Mission / Combat → Loot → Risk decision → Extract / Die / Abandon → Bunker → Equip / Upgrade → Deploy stronger**

## 2. Validated / merged baselines

### Vertical Slice 0.1 — MERGED / VALIDATED
Merge `e4d5dbe2c52d3e9aeed52f421fdd99f7c6b01877`.

### Production 0.2 — MERGED / VALIDATED
Merge `fd0dca0ece7d18ca005f2f4b52d65039904fad27`.

### Production 0.3 — MERGED / REAL UNITY VALIDATED
PR #3 merge `924e8ff4ae250da13fd0d198b121802cf80131b0`.

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
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
PR #8 squash merge `876127fb9997951afcca738cd7251acd2f662014`.
- Workshop / Calibration / Salvage
- Item Power
- permanent Bunker upgrades

### Production 0.9 — MERGED / REAL UNITY VALIDATED
PR #9 squash merge `2f15df3b5ca7b15eeacea39928b63118700e2432`.
- Walker / Runner / Brute / Stalker combat roles
- SAM / RAVEN / BRIGGS active abilities
- fixed-zone mobile twin-stick controls
- full regression with 0 red runtime errors

### Production 0.10 — MERGED / REAL UNITY VALIDATED
PR #10 squash merge `f48368cd46799afa230c8bc52f475300d8f68761`.
- operator ability VFX
- infected special VFX
- world hit / crit feedback
- subtle camera-impact layer
- fixed-zone mobile regression green
- full Bunker → combat → extraction → Bunker regression green
- Unity Console: 0 red runtime errors

## 3. Current Git state

- stable branch: `main`
- stable production level before current merge: **0.10**
- stable merge: `f48368cd46799afa230c8bc52f475300d8f68761`
- active branch: **`production/0.11-expedition-director`**
- PR: **#11**
- Production 0.11 full real-Unity validation: **PASSED ✅**
- PR #11 is ready for promotion to `main`

## 4. Production 0.11 — Expedition Director — FULL REAL-UNITY VALIDATED ✅

### Mission system
- runtime `ExpeditionDirector`
- normal mission rotation plus boss-level BLACKSITE forcing
- **RECOVERY** — secure a world data core
- **PURGE** — eliminate a bounded infected target count
- **HOLDOUT** — activate uplink, remain in defense radius, survive reinforcement waves
- **BLACKSITE** — breach terminal → eliminate response / mutation boss → secure vault core

### Objective / HUD presentation
- FIELD OPS shows mission, threat, primary / optional objective and progress
- world objective markers use generated URP-safe ring / beam / light presentation
- mission start, reinforcement and signal alerts are visible
- mobile control zones remain unobstructed

### Extraction authority
- primary mission blocks extraction until complete
- existing boss and no-loot extraction gates remain intact
- primary completion unlocks extraction immediately
- `EXTRACTION SEALED` state and normal unlock flow passed real runtime testing

### Extraction egress hardening
0.11 exposed an older geometry edge because the accepted extraction is centered at `z=20` while the original base road/ground ended around `z=19`.

Accepted fix:
- extend `World_Ground` beyond the north extraction trigger
- extend `Road_Main` beyond the north extraction trigger
- force extraction-owned colliders to remain triggers
- do not alter mobile input or mission logic

Real runtime validation:
- enter sealed extraction ✅
- walk back out normally ✅
- re-enter successfully ✅
- extraction overlay/state clears on exit ✅

### Risk / reward
- Primary completion grants unsecured Scrap
- optional orange **BLACK CACHE** appears after Primary
- player can extract immediately or risk the optional cache
- approaching cache triggers hostile reinforcements
- optional cache grants a reserved bonus weapon with improved rarity / Item Power
- reward banks only after successful extraction
- death / abandon clears pending mission reward

### Reinforcement system
- HOLDOUT / BLACKSITE / optional cache can spawn runtime reinforcement waves
- live infected pressure is capped
- reinforcements use production infected visuals
- reinforcements use Walker / Runner / Brute / Stalker role brains and the Production 0.10 special-VFX stack

### Final accepted regression
- fresh compile: 0 red compiler errors ✅
- Build Production Slice 0.11 ✅
- mission HUD / objective marker / mission extraction seal ✅
- Primary / BLACK CACHE / reinforcements ✅
- extraction egress fix ✅
- fixed-zone MOVE / AIM-FIRE / Ability mobile regression ✅
- Bunker → Workshop / Arsenal → Deploy → mission/combat/loot → extraction → Bunker ✅
- Workshop / progression persist ✅
- optional cache weapon banks after successful extraction ✅
- Production 0.10 combat-impact / boss / reward / sector presentation intact ✅
- Unity Console: **0 red runtime errors** ✅

Test plan: `docs/PRODUCTION_11_TEST.md`

## 5. Stable systems that must be preserved after 0.11

### Progression
- save schema v6
- Item Power / Calibration / Salvage
- Workbench / Medbay / Cargo Rig / Scavenger Network

### Combat
- WALKER / RUNNER / BRUTE / STALKER
- SAM Field Patch
- RAVEN Vector Dash
- BRIGGS Shockwave
- Production 0.10 combat-impact VFX

### Mobile
- fixed lower-left MOVE
- fixed lower-right AIM/FIRE
- independent upper-right Ability
- full 360° movement
- direction-based aiming only
- landscape only

### Presentation
- accepted Arsenal orientation/framing
- responsive Bunker layouts
- boss/reward presentation
- sector atmosphere FX
- artist-rigged embedded firearms only

## 6. Next handoff

After PR #11 merges:
1. `main` becomes the stable Production 0.11 baseline
2. record the actual squash-merge SHA in this file
3. next production work must branch from current `main`
4. preserve the complete Expedition Director / mission / extraction / risk-reward stack
5. preserve schema-v6 Workshop progression
6. preserve fixed-zone mobile controls
7. preserve Production 0.10 combat presentation
8. never reintroduce external gameplay hand-mounted Rifle transforms
9. keep mobile landscape-only
