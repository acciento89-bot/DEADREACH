# DEADREACH — Production 0.9 Test Gate

Production 0.9 branches from the real-Unity-validated Production 0.8 `main` baseline.

## Goal

Turn existing statistical enemy/operator variants into real gameplay identities without destabilizing the accepted 0.8 progression and 0.7 presentation stack, and finish a production-usable mobile twin-stick layer.

## Implemented combat depth

### Infected roles
- WALKER remains the readable baseline chaser.
- RUNNER gains a timed forward burst from medium range and can deal burst contact damage.
- BRUTE gains a high-damage close-range slam on its own cooldown.
- STALKER gains a timed lateral flank/reposition move rather than only running directly at the player.
- Runner / Brute / Stalker special actions flash a short role-colored point-light telegraph.
- mutation bosses remain on the accepted boss system.

### Operator active abilities
- SAM / RANGER — `FIELD PATCH`: restore 32% max health; cannot be wasted at full health; 18s cooldown.
- RAVEN / SCOUT — `VECTOR DASH`: 4.6m collision-aware CharacterController dash; 7.5s cooldown.
- BRIGGS / WARDEN — `SHOCKWAVE`: damage every infected inside 4.6m; 12s cooldown.
- desktop: `SPACE`
- gamepad: right shoulder

## Earlier gates already passed

- original 0.9 compile gate: **0 red compiler errors** ✅ 2026-08-23
- original `DEADREACH > Build Production Slice 0.9`: completed ✅ 2026-08-23
- Walker / Runner / Brute / Stalker runtime behavior accepted ✅
- SAM / RAVEN / BRIGGS ability gameplay accepted ✅

### Non-blocking visual debt for Production 0.10
- BRIGGS Shockwave currently has no dedicated visible shockwave FX.
- Gameplay behavior is accepted in 0.9; add expanding ring / ground pulse / impact treatment in 0.10.

## Mobile gate — FIRST ATTEMPT REJECTED

Real mobile/Device Simulator use exposed blocking problems:
- movement/aim did not feel production usable
- movement could behave one-sided/wrong
- right-side touch/pointer aiming could pull weapon aim toward the Ability UI while firing
- Ability occupied the same practical control space as right-side aim/fire
- Field Ops / health / status UI was too small for comfortable phone reading

0.9 must not be promoted on the rejected input implementation.

## Replacement mobile implementation now awaiting validation

- left stick = relative 360-degree movement vector with deadzone + response shaping
- faster mobile acceleration/deceleration for immediate direction changes
- right stick = relative directional aim/fire vector with its own deadzone
- touch screen position is no longer treated as the weapon's world target
- mobile/Device Simulator touch suppresses mirrored mouse-pointer aim
- fire begins only once the right stick leaves a small threshold
- Ability is a separately captured touch control above the lower-right aim control band
- virtual-stick size scales with Screen.safeArea height
- mobile Field Ops panel, Vitals/HP bar, weapon/loot/scrap/objective text, boss bar and extraction UI scale up for phone readability

## Fresh compile gate after replacement mobile pass

1. Pull latest `production/0.9-combat-depth`.
2. Let Unity finish compiling.
3. Require **0 red compiler errors**.
4. No need to claim the previous build validates this new runtime code; retest mobile behavior after compile.

## Replacement mobile runtime gate

Use the same phone/Device Simulator landscape setup where the first attempt failed.

### MOVE
1. Touch left control area.
2. Push up / down / left / right / diagonals.
3. Character must respond in all directions with no one-sided movement.
4. Returning stick to center must stop quickly without floaty slide.
5. Visual stick knob must follow the same thumb direction.

### AIM / FIRE
1. Touch lower-right control area.
2. Drag right stick through all directions.
3. Character/weapon aim must rotate by stick direction, not toward the finger's absolute screen location.
4. Push beyond the small threshold to fire.
5. Aim must never snap toward the Ability button/UI.
6. Releasing the right stick stops firing.

### ABILITY
1. Ability button must appear clearly separated above the lower-right stick area.
2. Touching Ability triggers only the operator ability.
3. It must not also claim move, aim or fire.
4. READY/cooldown state must remain readable.

### PHONE HUD
- FIELD OPS title readable without zooming
- LEVEL/zone readable
- VITALS value clearly readable
- HP bar thick enough to understand at a glance
- primary weapon + carried/secured/loot values readable
- objective readable
- boss/extraction UI remains readable when shown

## Final regression — REMAINING

- Workshop remains present after expedition → extraction → Bunker.
- Calibration / Salvage / Bunker upgrades persist.
- Arsenal orientation remains accepted.
- enemy-role behavior remains accepted.
- SAM / RAVEN / BRIGGS ability gameplay remains accepted.
- sector FX remain accepted.
- final Bunker → expedition → combat → loot → extract → Bunker returns with **0 red runtime errors**.
