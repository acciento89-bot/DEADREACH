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
- BRIGGS / SHOCKWAVE gameplay ✅

### Non-blocking 0.10 visual debt
- BRIGGS Shockwave has no dedicated visible shockwave FX yet.
- Add expanding ring / ground pulse / impact treatment in Production 0.10.

## Mobile attempts rejected

### First attempt — REJECTED
- movement/aim not production usable
- pointer aiming could pull weapon toward Ability UI
- Ability overlapped practical aim/fire space
- phone HUD too small

### Second attempt — REJECTED
- firing/aim improved but still failed during real Device Simulator use
- floating MOVE origin appeared where the first touch landed instead of remaining at a predictable control position
- movement was not reliably usable in all directions
- mobile Ability did not provide trustworthy visible confirmation

0.9 must not be promoted on either rejected implementation.

## Current fixed-zone mobile implementation — AWAITING VALIDATION

- MOVE has a fixed lower-left safe-area center
- AIM/FIRE has a fixed lower-right safe-area center
- both controls have generous circular capture zones larger than the visible stick
- MOVE uses full 360-degree X/Y vector with deadzone + response shaping
- mobile movement acceleration/deceleration is faster for immediate changes
- AIM/FIRE is camera-relative directional input only
- absolute finger screen position can never become the weapon world target
- right stick fires whenever directional input leaves its deadzone
- releasing right stick stops firing
- mobile/Device Simulator touch suppresses mirrored mouse-pointer aim
- Ability uses a separate upper-right region with enlarged invisible hit target
- Ability queues on touch-begin rather than touch-release
- Ability displays immediate `FIRED`, `NO TARGET`, `FULL HP`, `BLOCKED` or `COOLDOWN` feedback so input registration is visible even before dedicated Shockwave FX exists
- fixed stick visuals use the exact same centers as input capture
- mobile Field Ops / Vitals / HP / loot / scrap / objective / boss / extraction UI remain scaled for phone readability

## Fresh compile gate

1. Pull latest `production/0.9-combat-depth`.
2. Let Unity finish compiling.
3. Require **0 red compiler errors**.

## Current mobile runtime gate

Use the same landscape phone/Device Simulator setup where the earlier attempts failed.

### MOVE
1. The MOVE stick must stay fixed lower-left before and during use.
2. Touch inside/near its visible circle.
3. Push up / down / left / right / diagonals.
4. Character must move in all corresponding directions.
5. Returning to center/releasing must stop quickly.

### AIM / FIRE
1. The AIM/FIRE stick must stay fixed lower-right.
2. Push it in any direction.
3. Character/weapon must aim by stick direction only.
4. Once outside deadzone, firing must start reliably.
5. Aim must never snap toward Ability/UI.
6. Releasing the stick must stop firing.

### ABILITY
1. Ability remains upper-right, away from both sticks.
2. Pressing it must immediately change the button feedback text.
3. RAVEN should show `FIRED` and dash when ready.
4. SAM at full HP may show `FULL HP`; after taking damage it must show `FIRED` and heal.
5. BRIGGS with no infected in range may show `NO TARGET`; with infected in range it must show `FIRED` and deal Shockwave damage.
6. Ability touch must not also move, aim or fire.

### PHONE HUD
- FIELD OPS title readable
- LEVEL / zone readable
- VITALS value readable
- HP bar readable at a glance
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
