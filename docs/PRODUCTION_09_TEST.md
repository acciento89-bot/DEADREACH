# DEADREACH — Production 0.9 Test Gate

Production 0.9 branches from the real-Unity-validated Production 0.8 `main` baseline.

## Goal

Turn existing statistical enemy/operator variants into real gameplay identities without destabilizing the accepted 0.8 progression/presentation stack, and finish a production-usable mobile twin-stick layer.

## Already accepted

### Infected roles
- WALKER baseline chaser ✅
- RUNNER timed forward burst ✅
- BRUTE high-damage close-range slam ✅
- STALKER lateral flank/reposition ✅

### Operator active abilities
- SAM / FIELD PATCH ✅
- RAVEN / VECTOR DASH ✅
- BRIGGS / SHOCKWAVE ✅

### Non-blocking 0.10 visual debt
- BRIGGS Shockwave has no dedicated visible shockwave FX yet.
- Add expanding ring / ground pulse / impact treatment in Production 0.10.

## First mobile attempt — REJECTED

Real phone/Device Simulator use exposed blocking problems:
- movement/aim did not feel production usable
- movement could behave one-sided/wrong
- right-side pointer aiming could pull weapon aim toward the Ability UI while firing
- Ability occupied the same practical control space as aim/fire
- Field Ops / health / status UI was too small for comfortable phone reading

0.9 must not be promoted on that implementation.

## Replacement mobile implementation

- left stick = relative 360-degree movement vector with deadzone + response shaping
- faster mobile acceleration/deceleration for immediate direction changes
- right stick = relative directional aim/fire vector with its own deadzone
- absolute finger screen position is no longer the weapon world target
- mobile/Device Simulator touch suppresses mirrored mouse-pointer aim
- right-stick fire begins only after a small threshold
- Ability is separately captured above the lower-right stick control band
- virtual stick radius/visuals scale with `Screen.safeArea.height`
- mobile Field Ops panel, Vitals/HP bar, weapon/loot/scrap/objective text, boss bar and extraction UI scale up for phone readability

## Fresh compile gate

1. Pull latest `production/0.9-combat-depth`.
2. Let Unity finish compiling.
3. Require **0 red compiler errors**.

## Replacement mobile runtime gate

Use the same landscape phone/Device Simulator setup where the first attempt failed.

### MOVE
1. Push left stick up / down / left / right / diagonals.
2. Character must move in all corresponding directions.
3. Movement must not remain one-sided.
4. Returning stick to center must stop quickly without floaty slide.
5. Stick knob direction must visually match thumb direction.

### AIM / FIRE
1. Use lower-right stick.
2. Rotate through all directions.
3. Character/weapon must aim by stick direction, not absolute finger location.
4. Push beyond threshold to fire.
5. Aim must never snap toward Ability/UI.
6. Releasing the stick stops firing.

### ABILITY
1. Ability appears clearly separated above lower-right control band.
2. Touching it triggers only the operator ability.
3. It must not also move, aim or fire.
4. READY/cooldown state remains readable.

### PHONE HUD
- FIELD OPS title readable
- LEVEL / zone readable
- VITALS value readable
- HP bar thick enough to understand immediately
- primary weapon + carried/secured/loot values readable
- objective readable
- boss/extraction UI readable when present

## Final regression

- Workshop survives expedition → extraction → Bunker.
- Calibration / Salvage / Bunker upgrades persist.
- Arsenal orientation remains accepted.
- enemy-role behavior remains accepted.
- SAM / RAVEN / BRIGGS ability gameplay remains accepted.
- sector FX remain accepted.
- final Bunker → expedition → combat → loot → extract → Bunker returns with **0 red runtime errors**.
