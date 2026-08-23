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
- PR #7 merge `9633aabd903251a09c8475b5b8672a03988a92bc`
- PR #6 promotion to `main` `b69f5270a6e3e26780ccaa0445e4e6764808f753`

### Production 0.8 — MERGED / REAL UNITY VALIDATED
PR #8 squash merge `876127fb9997951afcca738cd7251acd2f662014`.

### Production 0.9 — MERGED / REAL UNITY VALIDATED
PR #9 squash merge `2f15df3b5ca7b15eeacea39928b63118700e2432`.

### Production 0.10 — MERGED / REAL UNITY VALIDATED
PR #10 squash merge `f48368cd46799afa230c8bc52f475300d8f68761`.

### Production 0.11 — MERGED / REAL UNITY VALIDATED
PR #11 squash merge `5b1b40322e305b1546a9ca5a37c1f6b89eabea72`.
- RECOVERY / PURGE / HOLDOUT / BLACKSITE mission system
- objective-gated extraction
- optional BLACK CACHE risk/reward path
- runtime reinforcement waves
- mission HUD / objective markers / alerts
- extraction north-edge geometry hardening
- fixed-zone mobile regression passed
- full Bunker → mission → extraction → Bunker regression passed
- Unity Console: **0 red runtime errors**

## 3. Current Git state

- stable branch: **`main`**
- stable production level: **0.11**
- stable merge: **`5b1b40322e305b1546a9ca5a37c1f6b89eabea72`**
- active branch: **`production/0.12-sector-expansion`**
- Production 0.12 is implemented in code but **not yet real-Unity validated**
- next gate: fresh Unity compile → `Build Production Slice 0.12` → sector/world/runtime/mobile validation

## 4. Stable Production 0.11 baseline that must remain green

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
- BRIGGS Shockwave

### Mobile
- fixed lower-left MOVE
- fixed lower-right AIM/FIRE
- independent upper-right Ability
- full 360° movement
- direction-based aiming only
- landscape only

### Combat impact / presentation
- operator ability VFX
- infected special VFX
- world hit marker / critical marker
- subtle camera-lens impact
- tracer / muzzle / sparks / gore preserved
- responsive mobile-readable FIELD OPS HUD

### Expedition Director
- RECOVERY / PURGE / HOLDOUT / BLACKSITE
- boss levels force BLACKSITE
- Primary gates extraction
- BLACK CACHE creates extract-now vs risk-more decision
- optional mission reward banks only after successful extraction
- death / abandon clears pending mission rewards

### Extraction hardening
- trigger-only extraction hierarchy
- north extraction support extended
- sealed-zone enter → exit → re-entry real-runtime validated

## 5. Production 0.12 — Sector Expansion — CODE IMPLEMENTED / NOT YET UNITY VALIDATED

### World network expansion
- new `Production_SectorNetwork_0_12` authoring pass adds real Quaternius east/west road spurs to the existing north/south route
- world-safety rectangle expands from the old narrow corridor to the full cross-street playspace
- PlayerFallSafety receives widened x bounds while preserving north/south safety
- existing 0.11 north extraction support remains intact

### Four authored sector layouts
`Production_SectorLayouts_0_12` contains four inactive authored roots; runtime activates exactly one per expedition:

1. **QUARANTINE WARD**
   - checkpoint/slalom barriers
   - quarantine containers / response pickup
   - east-side extraction
   - contamination hazard
   - teal/green atmosphere

2. **TRANSIT COLLAPSE**
   - wrecked truck + car route blockers
   - alternate path around transport wrecks
   - west-side extraction
   - electrical arc hazard
   - cold blue atmosphere

3. **INDUSTRIAL SPILL**
   - container channels / pipes / barrels
   - north extraction
   - contamination + fire hazards
   - amber industrial atmosphere

4. **BLACKOUT PLAZA**
   - blackout wreck/barrier layout
   - east-side extraction
   - electrical + fire hazards
   - dark violet/red emergency atmosphere

All sector route blockers use existing Quaternius production assets; no dev-cube blocker layer was introduced.

### Sector runtime director
- `SectorDirector` selects a sector deterministically from campaign/run state
- non-selected authored sector roots remain inactive
- baseline 0.4 route blockers are disabled so they cannot collide with sector-specific layouts
- active sector applies its own fog/key-light identity
- player moves to sector spawn anchor
- ExtractionZone + both extraction beacon presentations move to sector extraction anchor
- ordinary infected move to sector enemy anchors
- existing Scrap and weapon loot move to sector loot anchors

### Sector-aware Production 0.11 integration
The validated 0.11 `ExpeditionDirector` code itself remains intact.

0.12 layers geography over it:
- Primary objective marker is moved to sector-authored objective anchors
- BLACKSITE vault/core stage moves to the sector vault anchor
- optional BLACK CACHE moves to a distant sector anchor
- newly spawned Holdout / Blacksite / cache reinforcements are detected and moved to sector reinforcement anchors
- objective and reinforcement behavior remains owned by the 0.11 director

### Gameplay hazards
- `SectorHazardZone` is trigger-only
- hazard types: Contamination / Electrical Arc / Fireline
- pulsing world ring + local light presentation
- periodic neutral-faction damage while player remains inside
- damage stops when player leaves
- hazard state is reported to FIELD OPS
- hazards never modify or steal mobile input

### Sector risk/reward
Primary completion grants additional unsecured sector-risk Scrap:
- Quarantine +4
- Transit +6
- Industrial +8
- Blackout +10

BLACK CACHE receives sector Item Power bonus:
- Quarantine +2
- Transit +3
- Industrial +5
- Blackout +6

`RunInventory` clones inserted weapons, so `SectorRewardSynchronizer` mirrors the sector power bonus onto the carried inventory clone while the existing event object keeps the correct bonus for the full-inventory pending-reward path.

### FIELD OPS 0.12 HUD
- panel enlarged without moving accepted lower control zones
- dedicated `SECTOR // <name> // <hazard profile>` line
- live sector line changes to danger state while inside a hazard
- centered `HAZARD // <type> // MOVE CLEAR` warning
- Mission / Vitals / weapon / loot / objective information preserved

### Build / validation
- menu: `DEADREACH > Build Production Slice 0.12`
- build first regenerates the validated base slice, then applies the 0.12 sector scene pass
- test plan: `docs/PRODUCTION_12_TEST.md`
- **no real Unity compile/build/runtime claim yet**

## 6. Handoff protocol

When resuming:
1. read this file first
2. stable baseline remains Production 0.11 on `main`
3. active work is Production 0.12 on `production/0.12-sector-expansion`
4. first gate is fresh local Unity compile with **0 red compiler errors**
5. then run `DEADREACH > Build Production Slice 0.12`
6. validate wider cross-street traversal and outer world safety
7. validate sector identity / extraction / enemy / loot / objective / reinforcement anchors
8. validate hazard damage + warning clear-on-exit
9. validate sector Scrap / Item Power reward bonuses
10. preserve complete 0.11 mission / extraction / BLACK CACHE behavior
11. preserve schema-v6 Workshop progression
12. preserve fixed-zone mobile controls
13. preserve Production 0.10 combat-impact presentation
14. never reintroduce external gameplay hand-mounted Rifle transforms
15. keep mobile landscape-only
