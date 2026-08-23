# DEADREACH — Production 0.9 Test Gate

Production 0.9 branches from the real-Unity-validated Production 0.8 `main` baseline.

## Goal

Turn existing statistical enemy/operator variants into real gameplay identities without destabilizing the accepted 0.8 progression and 0.7 presentation stack.

## Implemented combat depth

### Infected roles
- WALKER remains the readable baseline chaser.
- RUNNER gains a timed forward burst from medium range and can deal burst contact damage.
- BRUTE gains a high-damage close-range slam on its own cooldown.
- STALKER gains a timed lateral flank/reposition move rather than only running directly at the player.
- Runner / Brute / Stalker special actions flash a short role-colored point-light telegraph.
- mutation bosses remain on the accepted boss system and are not given the normal infected-role specials in this pass.

### Operator active abilities
- SAM / RANGER — `FIELD PATCH`: restore 32% max health; cannot be wasted at full health; 18s cooldown.
- RAVEN / SCOUT — `VECTOR DASH`: 4.6m collision-aware CharacterController dash in movement/facing direction; 7.5s cooldown.
- BRIGGS / WARDEN — `SHOCKWAVE`: damage every infected inside 4.6m; 12s cooldown.
- desktop: `SPACE`
- gamepad: right shoulder
- mobile: dedicated reserved Ability touch region so pressing the skill does not also become an aim/fire touch.
- in-expedition ability HUD shows ability name and READY/cooldown state.

## Compile / build gate

1. Pull `production/0.9-combat-depth`.
2. Let Unity finish compiling.
3. Require **0 red compiler errors**. ✅ 2026-08-23
4. Run `DEADREACH > Build Production Slice 0.9`. ✅ 2026-08-23
5. Require no blocking red build/setup error. ✅ 2026-08-23

## Enemy-role runtime gate — PASSED 2026-08-23

Runtime accepted:
- existing encounter still spawns and moves normally ✅
- RUNNER forward burst + cyan/blue telegraph ✅
- BRUTE close-range slam + red/orange telegraph ✅
- STALKER lateral flank/reposition + purple telegraph ✅
- WALKER remains the predictable baseline ✅
- no blocking role-runtime regression reported ✅

## Operator ability gate — PASSED 2026-08-23

Runtime accepted:
- SAM / FIELD PATCH heals and respects cooldown rules ✅
- RAVEN / VECTOR DASH performs the intended tactical dash ✅
- BRIGGS / SHOCKWAVE damages nearby infected and respects cooldown rules ✅

### Non-blocking visual debt for Production 0.10
- BRIGGS Shockwave currently has no dedicated visible shockwave FX.
- Gameplay behavior is accepted in 0.9; add a dedicated expanding ring / ground pulse / impact treatment in the next production patch.

## Mobile input gate — REMAINING

On mobile / touchscreen simulation:
- dedicated Ability control appears separately from move and aim/fire
- touching/releasing Ability fires only the operator ability request
- it must not claim the touch as move or aim/fire
- READY/cooldown state remains readable inside Screen.safeArea

## Regression — REMAINING

- Workshop remains present after expedition → extraction → Bunker.
- Calibration / Salvage / Bunker upgrades persist.
- Arsenal orientation remains accepted.
- compact Field Ops / boss / reward UI remains accepted.
- sector FX remain accepted.
- final Bunker → expedition → combat → loot → extract → Bunker returns with **0 red runtime errors**.
