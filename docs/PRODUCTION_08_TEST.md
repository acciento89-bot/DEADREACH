# DEADREACH — Production 0.8 Test Gate

Production 0.8 starts from the fully real-Unity-validated Production 0.7 `main` baseline.

## Phase A — progression engine / migration gate

Status:
- initial Phase A Unity compile: **0 red compiler errors** ✅ — 2026-08-23
- Phase B Workshop UI was implemented after that compile gate, so a fresh compile is required before build/runtime acceptance

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

Implemented:
- dedicated WORKSHOP navigation entry injected into the validated Bunker Command Center
- permanent-system cards for Workbench / Medbay / Cargo Rig / Scavenger Network
- live ranks, effects, escalating Scrap costs and purchase buttons
- weapon calibration list with family / rarity / Item Power / calibration / real Item-Power damage contribution
- calibration spends Scrap and refreshes the Workshop immediately
- two-step salvage confirmation for non-equipped weapons
- active loadout remains unsalvageable
- Workshop profile-summary Scrap refresh after every transaction
- responsive layout uses the validated Bunker content viewport rather than a second independent screen-space canvas

### Required real Unity acceptance
1. pull latest `production/0.8-workshop-progression`
2. Unity compile → **0 red compiler errors**
3. run `DEADREACH > Build Production Slice 0.8`
4. require no blocking red build/setup error
5. run `DEADREACH > Dev > 0.8 Set Workshop Test Profile`
6. Play → Bunker → WORKSHOP is visible and opens correctly
7. supported landscape layouts remain usable at 4:3 / 16:10 / 16:9 / ~19:9
8. calibrate a weapon below the Workbench ceiling:
   - Scrap decreases by displayed amount
   - calibration rank increases by one
   - Item Power increases by 8
   - displayed POWER DMG contribution rises
   - values persist after leaving/reopening Workshop
9. try calibration at the current Workbench ceiling → button must require Workbench rather than silently exceed the cap
10. salvage a non-equipped weapon:
   - first press changes to CONFIRM SALVAGE
   - second press removes the weapon and adds the displayed Scrap value
11. equipped weapon must show ACTIVE LOADOUT and cannot be salvaged
12. buy one Bunker system rank and verify cost/rank/effect refresh
13. Workbench raises calibration ceiling
14. Medbay raises actual operator max HP on deployment
15. Cargo Rig raises actual expedition weapon capacity
16. Scavenger Network raises actual Scrap banked on successful extraction

## Final regression
- Arsenal orientation/framing remains accepted
- Bunker landscape layouts remain accepted
- compact Field Ops / boss / reward UI remains accepted
- sector FX remain accepted
- Bunker → expedition → combat → loot → extract → Bunker returns with **0 red runtime errors**
