# DEADREACH — Production 0.6 BIG Runtime Acceptance

Production 0.6 is validated as one consolidated pass. Production 0.5 is already merged and must not regress.

## Preflight

1. `git fetch`
2. `git switch production/0.6-content-rewards-mobile`
3. `git pull`
4. let Unity finish compile/import
5. require **0 red compiler errors**
6. run **`DEADREACH > Build Production Slice 0.6`** once
7. require Console message that 0.6 weapon family art is READY
8. no blocking red import/build error

## A. Bunker / weapon families

Open Play in Bunker.

Arsenal:
- existing 0.5 weapons still load
- old weapons remain valid Rifles
- new extracted weapons may be Rifle / SMG / Pistol / Shotgun
- equipped family shows the correct standalone 3D model in the Arsenal inspector
- Rifle preview remains upright
- SMG / Pistol / Shotgun previews are horizontal/readable and not clipped
- finish color still applies
- Equip persists

Gameplay family stats:
- Rifle feels baseline
- SMG fires clearly faster with lower per-shot damage/range
- Pistol has lower sustained output but stronger crit identity
- Shotgun currently behaves as a heavy slow short-range slug profile; pellet spread is not claimed in 0.6

Important: gameplay character still uses the validated artist-rigged operator firearm. No external hand-mounted visual may reappear.

## B. Sector identities

Use unlocked campaign levels / dev level selection as needed and inspect one run from each sector:

- Level 1 / Dead City: cold blue/red emergency identity
- Level 11 / Flooded Industrial: teal flood patches + cyan light + wet/mist atmosphere
- Level 21 / Ash District: scorch zones + orange fire light + ash particles
- Level 31 / Blackout Sector: clearly darker environment + flickering purple/blue lights
- Level 41 / Ground Zero: red mutation pools + aggressive contamination light/particles

Required:
- streets / vehicles / extraction layout remain the validated 0.5 geometry
- world boundaries still stop void falling
- sector presentation cannot block traversal

## C. Boss identity + reward visibility

Use a boss level, ideally Level 10 first.

During fight:
- dedicated boss identity overlay appears
- Tier 1 name reads **THE BREAKER**
- boss has visible mutation tint/aura/particles
- phase text changes around existing ~66% and ~33% thresholds
- extraction remains sealed while boss lives

On boss death:
- boss itself drops **no ordinary Scrap**
- visible **MUTATION RELIC SECURED** popup appears immediately
- popup includes reward name, rarity, weapon family, item power, finish and affixes
- reward is still unsecured until extraction

After successful extraction and Bunker return:
- a second **RECOVERED MUTATION RELIC / RELIC SECURED** debrief appears
- reward details match the granted reward
- `TRANSFER TO ARSENAL` dismisses the debrief
- the reward exists in Arsenal afterwards
- the debrief does not reappear after acknowledgment

Failure check:
- if player dies/abandons after boss kill but before extraction, no Bunker secured-reward debrief should be persisted

Optional higher tiers when convenient:
- T2 FLOOD MAW
- T3 ASH TITAN
- T4 BLACKOUT WRAITH
- T5 GROUND ZERO PRIME

## D. Mobile-landscape responsive editor gate

This is still editor simulation, not real-device release acceptance.

In Unity Game view inspect at least:
- 16:9 landscape
- ~19.5:9 / ultrawide phone landscape
- compact/tablet landscape around 4:3–16:10 if available

Bunker requirements:
- DEADREACH header remains visible
- navigation remains tappable/readable
- content does not enter notch/safe-area margins
- deploy bar remains accessible
- Arsenal preview remains inside its column
- Operator preview remains usable
- Campaign sector buttons remain accessible
- Store cards do not clip off-screen

Gameplay HUD already uses `Screen.safeArea`; ensure pause/HUD remain visible at the simulated ratios.

Real notched iPhone + Android hardware remains a later mandatory release gate.

## E. Final regression

Confirm no regression in:
- Bunker-first start
- Sam / Raven / Briggs selection
- movement / aim / fire
- muzzle/tracer
- combat impacts
- loot pickup
- automatic next-level selection
- vehicle/container collision
- extraction traversal
- world fall safety
- boss extraction seal/unseal
- Save/load persistence
- 0 blocking red Console errors

## Acceptance

If the entire pass is clean, Production 0.6 can move from Draft to Ready and merge to `main`.
