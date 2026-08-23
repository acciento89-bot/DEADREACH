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
- compile gates passed with **0 red compiler errors**
- `DEADREACH > Build Production Slice 0.8` completed successfully
- Workshop layout / Calibration / Item Power / Scrap spending validated
- two-step Salvage and active-loadout protection validated
- Workbench / Medbay / Cargo Rig / Scavenger purchases and persistence validated
- expedition → extraction → Bunker return validated after lifecycle fix; WORKSHOP remains present
- final runtime/regression check passed

## 3. Current Git state

- stable branch: `main`
- stable production level: **0.8**
- stable merge: `876127fb9997951afcca738cd7251acd2f662014`
- no active production branch is authoritative after the 0.8 merge
- next production work must branch from current `main`

## 4. Production 0.8 shipped baseline

Production 0.8 closes the missing **Equip / Upgrade** half of the core loop with real persistent progression.

### Progression engine
- save schema **v6** with non-destructive migration from v5
- per-weapon persistent `upgradeLevel`
- Item Power contributes to real combat damage
- calibration adds Item Power plus small range/crit handling gains
- secured Scrap is the Workshop currency
- weapon calibration spends Scrap, raises calibration and adds +8 Item Power
- non-equipped weapons can be salvaged for secured Scrap
- equipped weapon cannot be salvaged

### Permanent Bunker systems
- **Workbench** — raises weapon calibration ceiling
- **Medbay** — +6% operator max HP per rank
- **Cargo Rig** — +1 expedition weapon capacity per rank
- **Scavenger Network** — +8% Scrap banked on extraction per rank
- each track has five ranks with escalating Scrap costs

### Workshop UI
- dedicated **WORKSHOP** navigation entry in the Bunker
- live rank / effect / cost / affordability cards
- weapon cards show family / rarity / Item Power / calibration / affixes / real Item-Power damage contribution
- Calibration and Salvage refresh immediately
- Salvage uses two-step confirmation
- active weapon shows `ACTIVE LOADOUT`
- Scrap profile summary refreshes after transactions
- `Production08WorkshopBootstrap` reinstalls Workshop after each scene load so expedition → Bunker return remains correct

## 5. Production 0.7 presentation baseline that remains accepted

- Arsenal Rifle / SMG / Pistol / Shotgun orientation and framing
- Bunker layout at 4:3 / 16:10 / 16:9 / ~19:9 landscape
- landscape-only mobile orientation
- compact Field Ops HUD
- slim boss health/identity strip and mutation-state chip
- lower-right Relic reward toast
- Bunker reward debrief → Arsenal transfer
- Flooded Industrial rain / Ash District ash / Blackout dust / Ground Zero contamination FX
- Bunker → expedition → combat → loot → return flow with no red runtime errors

## 6. Next development entry point

1. `git switch main`
2. `git pull`
3. branch the next production pass from current `main`
4. preserve schema-v6 Workshop progression and the accepted 0.7 presentation baseline
5. never reintroduce external gameplay hand-mounted Rifle transforms
6. keep mobile landscape-only
