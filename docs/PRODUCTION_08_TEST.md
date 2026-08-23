# DEADREACH — Production 0.8 Test Gate

Production 0.8 starts from the fully real-Unity-validated Production 0.7 `main` baseline.

## Phase A — progression engine / migration gate

Status:
- initial Phase A Unity compile: **0 red compiler errors** ✅ — 2026-08-23
- Phase B fresh Unity compile after Workshop UI: **0 red compiler errors** ✅ — 2026-08-23
- `DEADREACH > Build Production Slice 0.8`: **passed with no blocking red error** ✅ — 2026-08-23

### Schema-v6 invariants
- existing secured Scrap remains intact
- existing stash weapons remain intact
- equipped weapon remains selected
- legacy weapons migrate with `upgradeLevel = 0`
- Bunker upgrade ranks migrate at `0` unless already present in a v6 profile

### Dev setup
Use `DEADREACH > Dev > 0.8 Set Workshop Test Profile` to prepare:
- at least 2500 secured Scrap
- Workbench rank 2
- Medbay rank 1
- Cargo Rig rank 1
- Scavenger Network rank 1
- four weapon families when the stash is empty

Other helpers:
- `0.8 Grant 1500 Workshop Scrap`
- `0.8 Seed Four Weapon Families`

## Phase B — Workshop UI / economy gate

Status: **PASSED functionally** on 2026-08-23.

Validated in real Unity:
- dedicated WORKSHOP navigation entry opens correctly inside the Bunker
- permanent-system cards for Workbench / Medbay / Cargo Rig / Scavenger Network render and refresh
- calibration spends the displayed Scrap cost
- calibration rank increases by one
- Item Power increases by 8
- displayed POWER DMG contribution refreshes
- non-equipped weapon salvage uses two-step confirmation
- salvage removes the weapon and adds the displayed Scrap value
- equipped weapon shows ACTIVE LOADOUT and cannot be salvaged
- Bunker-system purchases spend Scrap and update rank/effect/cost
- Workbench ceiling updates correctly
- profile-summary Scrap refreshes after transactions

## Remaining expedition/runtime bonus gate

With the currently tested profile shown in Workshop:
- Medbay rank 2 = **+12% field HP**
- Cargo Rig rank 2 = **loot capacity 8**
- Scavenger Network rank 2 = **+16% Scrap banked on successful extraction**

Use operator SAM for the clearest HP check.

1. Deploy a normal expedition.
2. Confirm HUD starts at **VITALS 112/112** for SAM.
3. Confirm HUD shows **WEAPON LOOT 0/8**.
4. Pick up Scrap and note the exact carried amount before extraction.
5. Extract successfully.
6. Confirm secured Scrap rises by approximately `round(carriedScrap × 1.16)` from the run reward, excluding any other deliberate Workshop transaction.
7. Re-enter Bunker/WORKSHOP and confirm calibrated weapon Item Power/calibration are still present.
8. Deploy with that calibrated weapon and confirm normal combat with no red runtime error.

## Final regression
- Arsenal orientation/framing remains accepted
- Bunker landscape layouts remain accepted
- compact Field Ops / boss / reward UI remains accepted
- sector FX remain accepted
- Bunker → expedition → combat → loot → extract → Bunker returns with **0 red runtime errors**
