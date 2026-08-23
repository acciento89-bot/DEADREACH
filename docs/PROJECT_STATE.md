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
- Production 0.7 remains the canonical stable `main` baseline

## 3. Current Git state

- stable branch: `main`
- stable production level: **0.7**
- active branch: **`production/0.8-workshop-progression`**
- PR #8: Draft, targets `main`
- Production 0.8 Phase A compile gate passed with **0 red compiler errors** on 2026-08-23
- Production 0.8 Phase B fresh compile gate passed with **0 red compiler errors** on 2026-08-23
- `DEADREACH > Build Production Slice 0.8` completed successfully with no blocking red error on 2026-08-23
- Workshop/economy UI runtime gate passed on 2026-08-23: calibration, Scrap spending, two-step salvage, active-loadout protection and all four permanent Bunker-system purchases behaved as designed
- all new work must preserve the accepted 0.7 presentation/regression baseline

## 4. Production 0.8 goal

Close the missing **Equip / Upgrade** half of the core loop with real persistent progression instead of presentation-only Item Power.

### Phase A — progression engine implemented
- save schema advanced from v5 to **v6**
- existing profiles migrate without losing Scrap, stash, campaign or equipped weapon
- each weapon now persists `upgradeLevel`
- Item Power now contributes to real combat damage
- calibration levels add small range/crit handling gains
- secured Scrap becomes the in-game Workshop currency
- weapon upgrade/calibration API spends Scrap, raises calibration level and adds Item Power
- non-equipped weapon salvage API returns secured Scrap
- equipped weapon is protected from salvage
- four permanent Bunker upgrade tracks:
  - **Workbench** — raises weapon calibration ceiling
  - **Medbay** — +6% operator max HP per rank
  - **Cargo Rig** — +1 expedition weapon capacity per rank
  - **Scavenger Network** — +8% Scrap banked on extraction per rank
- Bunker upgrades have five ranks and escalating in-game Scrap costs
- Cargo Rig is wired into `RunInventory`
- Medbay is wired into `OperatorRuntimeApplier`
- Scavenger Network is wired into `SaveService.RegisterExtraction`
- new `DEADREACH > Build Production Slice 0.8` gate
- new 0.8 dev helpers for Scrap / weapon-family seeding / test profile setup

### Phase B — Workshop UI implemented / runtime accepted
- new **WORKSHOP** entry is injected into the existing Bunker navigation without rewriting the validated 0.7 Bunker command-center implementation
- Store navigation is shifted down and Bunker status is compacted to make room for the sixth navigation item
- Workshop renders inside the validated Bunker content viewport so existing landscape/safe-area behavior is inherited
- permanent-system cards show Workbench / Medbay / Cargo Rig / Scavenger Network rank, live effect, next purchase cost and affordability
- weapon list prioritizes the active loadout, then sorts by Item Power
- every weapon card shows family / rarity / Item Power / calibration / affixes and real Item-Power damage contribution
- Calibration spends secured Scrap, adds one calibration level and +8 Item Power, then refreshes immediately
- Workshop exposes the actual Workbench calibration ceiling; weapons at the cap show `WORKBENCH REQUIRED`
- Salvage is two-step confirmed to prevent accidental destruction
- active equipped weapon shows `ACTIVE LOADOUT` and remains unsalvageable
- Scrap summary refreshes after calibration, salvage and Bunker upgrades
- Item-Power damage multiplier is exposed by `WeaponStatResolver` so Workshop presentation uses the same source of truth as combat
- real Unity runtime check confirmed the Workshop actions and displayed economy state behave as designed

## 5. Production 0.7 regression baseline that must stay green

- Arsenal Rifle / SMG / Pistol / Shotgun orientation and framing
- Bunker layout at 4:3 / 16:10 / 16:9 / ~19:9 landscape
- landscape-only mobile orientation
- compact Field Ops HUD
- slim boss health/identity strip and mutation-state chip
- lower-right Relic reward toast
- Bunker reward debrief → Arsenal transfer
- Flooded Industrial rain / Ash District ash / Blackout dust / Ground Zero contamination FX
- final Bunker → expedition → combat → loot → return flow with no red runtime errors

## 6. Current 0.8 gate

Run `docs/PRODUCTION_08_TEST.md`.

Completed:
1. Phase A compile → **0 red compiler errors** ✅
2. Phase B compile → **0 red compiler errors** ✅
3. `DEADREACH > Build Production Slice 0.8` ✅
4. Workshop calibration / salvage / active-loadout protection / four permanent purchases ✅

Remaining before merge:
1. validate actual Medbay / Cargo Rig / Scavenger Network effects in an expedition
2. confirm calibrated Item Power remains active after leaving/re-entering the Bunker and during combat
3. final 0.7 regression + **0 red runtime errors**

## 7. Handoff protocol

When resuming:
1. read this file first
2. treat Production 0.7 on `main` as the stable real-Unity-validated baseline
3. active work is Production 0.8 on `production/0.8-workshop-progression`
4. Compile, Build 0.8 and Workshop/economy UI runtime gates are green
5. next required gate is expedition runtime bonuses + final 0.7 regression
6. never reintroduce external gameplay hand-mounted Rifle transforms
7. keep mobile landscape-only
