# DEADREACH — Production 0.10 Test Gate

Production 0.10 branches from the fully real-Unity-validated Production 0.9 `main` baseline.

## Goal

Increase combat readability and impact without changing the accepted 0.9 core loop, progression, operator input or fixed-zone mobile controls.

## Compile / build gate

1. Pull `production/0.10-combat-impact`.
2. Let Unity finish compiling.
3. Require **0 red compiler errors**.
4. Run `DEADREACH > Build Production Slice 0.10`.
5. Require no blocking red build/setup error.

## Operator ability presentation gate

### SAM / FIELD PATCH
- take damage first
- trigger Field Patch
- heal still works
- green/cyan expanding rings appear around SAM
- healing motes rise around the operator
- lens impulse is subtle and does not disturb aiming

### RAVEN / VECTOR DASH
- trigger dash while moving
- dash gameplay distance/direction remains accepted
- blue/white streak remains between start/end briefly
- endpoint pulse appears
- trail particles are visible but do not obscure the scene

### BRIGGS / SHOCKWAVE
- place at least one infected inside valid range
- trigger Shockwave
- damage behavior remains correct
- large orange/hot expanding ground pulse is clearly visible
- radial particles reinforce impact
- camera/lens punch is stronger than normal gunfire but comfortable

## Infected special presentation gate

- RUNNER burst: cyan movement streak + endpoint pulse
- BRUTE slam: red/orange expanding impact rings + radial particles
- STALKER flank: violet movement trail + start/end pulses
- Walker remains visually quiet baseline
- no special FX should remain permanently stuck in the scene
- normal role VFX must not override mutation-boss phase presentation

## Gunfight impact gate

- existing tracer / muzzle / sparks / gore remain functional
- successful damage hit shows a short world-space hit marker at the impact location
- normal hit marker is orange/bright
- critical hit marker is visibly magenta/stronger
- critical hit also produces a small pulse ring
- no hit marker on pure environment impacts
- player damage adds a subtle lens punch

## Mobile regression

Use the accepted landscape phone / Device Simulator setup:
- MOVE remains fixed lower-left
- up/down/left/right/diagonal movement still works
- AIM/FIRE remains fixed lower-right
- aiming and shooting remain reliable
- Ability remains independent upper-right
- new hit markers / ability VFX do not block touch controls
- mobile HUD readability remains accepted

## Full regression

1. Bunker → Workshop present.
2. Arsenal orientation/framing intact.
3. Deploy.
4. Combat / loot / ability / infected-special presentation test.
5. Extract.
6. Return to Bunker.
7. Workshop and progression persist.
8. Sector atmosphere / reward / boss presentation remain intact where encountered.
9. Unity Console ends with **0 red runtime errors**.

Production 0.10 remains unmerged until this full gate passes.
