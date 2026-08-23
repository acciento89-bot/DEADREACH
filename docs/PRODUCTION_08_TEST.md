# DEADREACH — Production 0.8 Test Gate

Production 0.8 starts from the fully real-Unity-validated Production 0.7 `main` baseline.

## Phase A — progression engine / migration gate

1. Pull `production/0.8-workshop-progression`.
2. Let Unity finish compiling.
3. Require **0 red compiler errors**.
4. Run `DEADREACH > Build Production Slice 0.8`.
5. Require no blocking red build/setup error.

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

Added after Phase A compile/build is green.

Required acceptance:
- dedicated Bunker Workshop surface
- equipped weapon can be calibrated while below the Workbench limit
- calibration spends secured Scrap, increments calibration level and Item Power, and persists
- Item Power changes real combat damage; calibration also gives small handling/crit gains
- non-equipped weapons can be salvaged for displayed Scrap value
- equipped weapon cannot be salvaged
- Workbench / Medbay / Cargo Rig / Scavenger Network purchases persist and spend Scrap
- Workbench raises calibration ceiling
- Medbay raises actual operator max HP
- Cargo Rig raises actual expedition weapon capacity
- Scavenger Network raises actual Scrap banked on extraction
- Bunker HUD/profile Scrap display refreshes after purchases

## Final regression
- Arsenal orientation/framing remains accepted
- Bunker landscape layouts remain accepted
- compact Field Ops / boss / reward UI remains accepted
- sector FX remain accepted
- Bunker → expedition → combat → loot → extract → Bunker returns with **0 red runtime errors**
