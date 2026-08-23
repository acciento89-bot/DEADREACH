# DEADREACH — Production 0.8 Test Gate

Production 0.8 starts from the fully real-Unity-validated Production 0.7 `main` baseline.

## Phase A — progression engine / migration gate

Status:
- initial Phase A Unity compile: **0 red compiler errors** ✅ — 2026-08-23
- Phase B Workshop UI fresh compile: **0 red compiler errors** ✅ — 2026-08-23
- `DEADREACH > Build Production Slice 0.8`: **passed** ✅ — 2026-08-23

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

Validated before deployment:
- dedicated WORKSHOP navigation entry is visible and opens correctly ✅
- permanent-system cards show Workbench / Medbay / Cargo Rig / Scavenger Network ✅
- live ranks, effects, Scrap costs and purchases refresh correctly ✅
- weapon calibration spends Scrap, increments calibration and adds +8 Item Power ✅
- two-step salvage removes only non-equipped weapons and returns displayed Scrap ✅
- active loadout remains unsalvageable ✅
- profile Scrap refresh works after transactions ✅

## Scene lifecycle regression found / fixed

Observed on 2026-08-23:
- initial Bunker WORKSHOP worked
- after expedition → successful extraction → Bunker, WORKSHOP navigation disappeared

Root cause:
- the first 0.8 installer relied on `RuntimeInitializeOnLoadMethod(AfterSceneLoad)`, which only covered the initial player scene setup
- the Bunker Command Center UI is recreated after returning from an expedition

Fix:
- `Production08WorkshopBootstrap` registers a single `SceneManager.sceneLoaded` hook
- every newly loaded Bunker scene is checked and `Production08WorkshopUI` is reattached when needed
- duplicate attachment is prevented by checking for an existing component

### Required lifecycle retest
1. pull latest `production/0.8-workshop-progression`
2. Unity compile → **0 red compiler errors**
3. Play → Bunker → confirm WORKSHOP exists and opens
4. Deploy → complete a successful extraction → return to Bunker
5. confirm WORKSHOP is still present in navigation
6. reopen WORKSHOP and confirm Calibration / Item Power / Bunker ranks / Scrap persisted
7. repeat one more Deploy → return cycle if practical; WORKSHOP must still remain present
8. Console → **0 red runtime errors**

## Runtime bonus acceptance
After lifecycle retest is green:
- Medbay raises actual operator max HP on deployment
- Cargo Rig raises actual expedition weapon capacity
- Scavenger Network raises actual Scrap banked on successful extraction
- Workbench raises calibration ceiling
- Item Power changes actual combat damage and Calibration adds small range/crit gains

## Final regression
- Arsenal orientation/framing remains accepted
- Bunker landscape layouts remain accepted
- compact Field Ops / boss / reward UI remains accepted
- sector FX remain accepted
- Bunker → expedition → combat → loot → extract → Bunker returns with **0 red runtime errors**
