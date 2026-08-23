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

### Production 0.10 — MERGED / REAL UNITY VALIDATED
- PR #10 squash merged to `main` at `f48368cd46799afa230c8bc52f475300d8f68761`
- compile and `Build Production Slice 0.10` gates passed
- operator and infected special VFX accepted
- hit / critical / camera-impact presentation accepted
- fixed-zone mobile controls revalidated
- full Bunker → Workshop → Deploy → combat/loot → extraction → Bunker regression passed
- final Unity Console: **0 red runtime errors**

## 3. Current Git state

- stable branch: `main`
- stable production level: **0.10**
- stable merge: `f48368cd46799afa230c8bc52f475300d8f68761`
- active branch: **`production/0.11-expedition-director`**
- initial Production 0.11 fresh real-Unity compile passed with **0 red compiler errors**
- first real runtime pass confirmed mission HUD → objective marker → mission extraction seal → primary completion → BLACK CACHE → reinforcements ✅
- extraction-egress geometry defect was fixed and the rebuilt scene passed real runtime enter/exit/re-entry validation ✅
- only the final mobile + full Bunker→mission→extraction→Bunker regression remains before merge

## 4. Stable Production 0.10 baseline that must remain green

### Progression
- save schema v6
- Item Power / Calibration / Salvage
- Workbench / Medbay / Cargo Rig / Scavenger Network
- Workshop survives expedition → Bunker reload

### Combat identities / controls
- WALKER / RUNNER / BRUTE / STALKER roles
- SAM / RAVEN / BRIGGS active abilities
- fixed lower-left MOVE
- fixed lower-right AIM/FIRE
- independent upper-right Ability
- full 360-degree movement and direction-based aiming

### Combat impact / presentation
- operator ability VFX
- infected special VFX
- hit / critical markers
- camera-lens impact
- tracer / muzzle / sparks / gore
- responsive mobile-readable FIELD OPS HUD
- accepted Arsenal / Bunker / boss / reward / sector presentation

## 5. Production 0.11 — Expedition Director — REAL RUNTIME ACCEPTANCE IN PROGRESS

### Mission system
- runtime `ExpeditionDirector` attaches only in expedition scenes with a real `RunSession` + player
- mission rotates across normal runs using level + run history
- boss levels force **BLACKSITE**
- four primary mission types:
  - **RECOVERY** — secure a world data core
  - **PURGE** — eliminate a bounded infected target count
  - **HOLDOUT** — activate uplink, remain in defense radius and survive timed reinforcement pressure
  - **BLACKSITE** — breach terminal → eliminate response / mutation boss → secure vault core
- tested runtime confirms mission HUD / marker / mission gate / primary completion path is live ✅

### Objective world presentation
- runtime mission markers use URP-safe generated line / light / core presentation
- primary marker color identifies mission role
- Holdout uses a large visible defense radius
- objective markers pulse and switch to green when completed
- objective marker presentation observed successfully in real runtime ✅

### Extraction authority
- `RunSession` tracks `ExtractionBlockedByMission`
- `ExtractionZone` blocks extraction while the Production 0.11 primary objective is incomplete
- existing boss and no-loot extraction gates remain intact
- primary completion unlocks extraction and grants unsecured carried Scrap
- mission-sealed extraction messaging observed successfully in real runtime ✅

### Extraction egress hardening — FIXED / ACCEPTED ✅
The sealed-state test exposed an older geometry edge because the accepted extraction sits at `z=20` while the original road/base support ended around `z=19`.

Committed and real-runtime validated fix:
- `World_Ground` support extended beyond the north extraction trigger
- `Road_Main` support extended beyond the north extraction trigger
- every collider under the extraction-zone hierarchy is forced to `isTrigger = true`
- extraction transform, mobile input and mission logic remain unchanged
- player can enter sealed extraction, walk back out normally and re-enter successfully ✅
- extraction overlay/state clears on exit ✅

### Risk / reward decision
- after primary completion the player may extract immediately
- an optional orange **BLACK CACHE** appears away from the primary objective
- approaching the cache triggers a reinforcement response
- securing the cache grants a reserved bonus weapon
- cache rarity is minimum Uncommon, minimum Rare at level 25+, plus bonus Item Power
- if run inventory is full, mission reward remains pending and is banked only after successful extraction
- death / abandon clears pending mission rewards
- BLACK CACHE + reinforcement path observed successfully in real runtime ✅

### Reinforcement system
- Holdout / Blacksite / optional-cache pressure can spawn runtime reinforcement waves
- max live infected pressure is capped
- reinforcements use the same production infected visual catalog
- reinforcements are configured as Walker / Runner / Brute / Stalker and receive the existing 0.9 role brain + 0.10 special VFX path
- reinforcements observed successfully in real runtime ✅

### FIELD OPS mission HUD
- existing mobile-readable FIELD OPS panel shows mission name + threat
- primary / optional objective text and progress
- extraction-sealed messaging names the current primary objective
- short visible alerts cover mission start, reinforcements, uplink signal and objective completion
- accepted mobile control zones are not modified
- mission HUD observed successfully in real runtime ✅

### Build / validation
- menu: `DEADREACH > Build Production Slice 0.11`
- test plan: `docs/PRODUCTION_11_TEST.md`
- initial compile: **PASSED — 0 red compiler errors** ✅
- mission HUD / marker / extraction seal / primary / BLACK CACHE / reinforcements: **PASSED** ✅
- extraction egress after geometry/trigger hardening: **PASSED** ✅
- remaining gate: fixed-zone mobile regression + full Bunker → Workshop → Deploy → mission/combat/loot → extract → Bunker regression + **0 red runtime errors**

## 6. Handoff protocol

When resuming:
1. read this file first
2. stable baseline remains Production 0.10 on `main`
3. active work is Production 0.11 on `production/0.11-expedition-director`
4. mission flow and extraction egress are real-runtime accepted
5. run final fixed-zone mobile regression
6. run full Bunker → Workshop → Deploy → mission/combat/loot → extraction → Bunker regression
7. require Unity Console with **0 red runtime errors**
8. preserve schema-v6 Workshop progression
9. preserve fixed-zone mobile controls
10. preserve Production 0.10 combat-impact presentation
11. never reintroduce external gameplay hand-mounted Rifle transforms
12. keep mobile landscape-only
