# DEADREACH — Project State

_Last updated: 2026-08-24_

Canonical handoff for DEADREACH. Update after every major implementation, validation and merge.

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

- Vertical Slice 0.1: `e4d5dbe2c52d3e9aeed52f421fdd99f7c6b01877`
- Production 0.2: `fd0dca0ece7d18ca005f2f4b52d65039904fad27`
- Production 0.3 / PR #3: `924e8ff4ae250da13fd0d198b121802cf80131b0`
- Production 0.4 / PR #4: `e86c067720f8f6badc6c8a29e41bcd856c29ffe6`
- Production 0.5 / PR #5: `a066386f05c6593f1840ef6902f62c808cbdf319`
- Production 0.6 + 0.7: `b69f5270a6e3e26780ccaa0445e4e6764808f753`
- Production 0.8 / PR #8: `876127fb9997951afcca738cd7251acd2f662014`
- Production 0.9 / PR #9: `2f15df3b5ca7b15eeacea39928b63118700e2432`
- Production 0.10 / PR #10: `f48368cd46799afa230c8bc52f475300d8f68761`
- Production 0.11 / PR #11: `5b1b40322e305b1546a9ca5a37c1f6b89eabea72`

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
- derive muzzle from that embedded firearm
- **never reintroduce the failed external hand-mounted Rifle transform path**

## 3. Current Git state

- stable branch: **`main`**
- stable production level: **0.11**
- stable merge: **`5b1b40322e305b1546a9ca5a37c1f6b89eabea72`**
- active branch: **`production/0.12-sector-expansion`**
- PR #12: **Draft**
- fresh real-Unity compile: **PASSED — 0 red compiler errors** ✅ 2026-08-24
- `DEADREACH > Build Production Slice 0.12`: **PASSED** ✅ 2026-08-24
- QUARANTINE WARD runtime gate: **PASSED** ✅ 2026-08-24
- next gate: **TRANSIT COLLAPSE**
- INDUSTRIAL SPILL / BLACKOUT PLAZA / reward / mobile / full regression remain pending

## 4. Stable Production 0.11 baseline that must remain green

### Progression
- save schema v6
- Item Power / Calibration / Salvage
- Workbench / Medbay / Cargo Rig / Scavenger Network
- Workshop survives expedition → Bunker reload

### Combat / operators / mobile
- WALKER / RUNNER / BRUTE / STALKER
- SAM Field Patch / RAVEN Vector Dash / BRIGGS Shockwave
- fixed lower-left MOVE
- fixed lower-right AIM/FIRE
- independent upper-right Ability
- full 360° movement
- landscape only

### Combat presentation
- operator ability VFX
- infected special VFX
- hit / critical markers
- camera-lens impact
- tracer / muzzle / sparks / gore
- accepted Arsenal / Bunker / boss / reward presentation

### Expedition Director
- RECOVERY / PURGE / HOLDOUT / BLACKSITE
- boss levels force BLACKSITE
- Primary gates extraction
- BLACK CACHE risk/reward path
- reinforcement waves
- optional mission reward banks only after successful extraction
- death/abandon clears pending rewards
- extraction trigger/egress hardening remains accepted

## 5. Production 0.12 — Sector Expansion

### Compile / build — PASSED
- static integration sanity pass complete
- fresh real Unity compile: **0 red compiler errors** ✅
- `Build Production Slice 0.12`: **PASSED** ✅

### QUARANTINE WARD — REAL UNITY VALIDATED ✅
Accepted in runtime:
- Q-WARD / BIOHAZARD identity and green/teal atmosphere
- west spur out-and-back
- east spur out-and-back
- no world-safety snap-back on tested side routes
- east-side extraction reachable
- pre-Primary extraction remains sealed
- mission marker uses the expanded sector geography
- contamination hazard warning appears on entry
- contamination damage applies while inside
- warning/damage stop after leaving
- hazard / containers do not trap the CharacterController

This is the first real-runtime confirmation that the 0.12 expanded cross-street world, sector extraction movement, mission geography and hazard loop work together.

### Remaining sector validation
1. **TRANSIT COLLAPSE** — next
2. **INDUSTRIAL SPILL** — pending
3. **BLACKOUT PLAZA** — pending

### Sector runtime architecture
- deterministic automatic sector selection from run/campaign state
- editor-only override: AUTO / QUARANTINE / TRANSIT / INDUSTRIAL / BLACKOUT
- only one authored layout active per expedition
- active layout controls player spawn, extraction, beacon, enemies, loot and atmosphere
- ordinary Runner enemies are explicitly excluded from `_R##` reinforcement relocation

### Production 0.11 geography integration
- Primary objective moves to sector anchor
- BLACKSITE vault/core moves to sector anchor
- optional BLACK CACHE moves to a distant sector anchor
- Holdout / Blacksite / cache reinforcements move to sector reinforcement anchors
- mission ownership remains in the accepted 0.11 Expedition Director

### Gameplay hazards
- Contamination / Electrical Arc / Fireline
- trigger-only
- pulsing ring/light presentation
- periodic neutral-faction player damage while inside
- damage and HUD warning stop on exit
- no mobile-input takeover

### Sector risk/reward — NOT YET ACCEPTED
Primary completion extra unsecured Scrap:
- Quarantine +4
- Transit +6
- Industrial +8
- Blackout +10

BLACK CACHE Item Power bonus:
- Quarantine +2
- Transit +3
- Industrial +5
- Blackout +6

Both carried-inventory clone and full-inventory pending-reward paths must still be validated in real runtime.

### FIELD OPS 0.12 HUD
- enlarged mobile-readable panel
- dedicated Sector + hazard-profile line
- live danger state while in hazard
- centered `HAZARD // <type> // MOVE CLEAR` warning
- accepted mission/vitals/weapon/loot information preserved

## 6. Handoff protocol

1. stay on `production/0.12-sector-expansion`
2. compile + build + Q-WARD are green
3. next select `DEADREACH > Dev > Sector 0.12 > TRANSIT COLLAPSE`
4. validate west extraction, wreck detour, electrical hazard and sector mission/reinforcement geography
5. then validate INDUSTRIAL SPILL
6. then validate BLACKOUT PLAZA
7. validate sector Scrap / Item Power bonuses
8. return override to AUTO
9. run fixed-zone mobile regression
10. run full Bunker → mission/risk-reward → extraction → Bunker regression
11. require 0 red runtime errors
12. preserve complete 0.11 mission / BLACK CACHE / extraction behavior
13. preserve schema-v6 Workshop progression
14. preserve fixed-zone mobile controls
15. preserve Production 0.10 combat-impact presentation
16. never reintroduce external gameplay hand-mounted Rifle transforms
17. keep mobile landscape-only
