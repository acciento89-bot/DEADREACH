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

## Enemy-role runtime gate

Use a normal non-boss expedition, ideally Level 1 or another early level.

1. Confirm existing encounter still spawns and moves normally.
2. RUNNER:
   - should occasionally burst forward from medium range
   - short cyan/blue telegraph flash should accompany the burst
3. BRUTE:
   - should trigger a distinct close-range slam in addition to normal melee
   - red/orange telegraph flash
4. STALKER:
   - should periodically side-step/flank instead of only taking the direct line
   - purple telegraph flash
5. WALKER remains the predictable baseline.
6. No infected special should throw red runtime errors or move enemies through sealed world bounds in normal play.

## Operator ability gate

Test all three operators from Bunker → Operators → Deploy.

### SAM
1. Take damage first.
2. Press SPACE / ability control.
3. Health must restore by roughly 32% of max HP, clamped at max.
4. Ability enters cooldown.
5. At full health, pressing ability must not consume cooldown.

### RAVEN
1. Move in a clear direction.
2. Press ability.
3. Raven should perform an immediate ~4.6m dash while respecting CharacterController collision.
4. Ability enters cooldown.

### BRIGGS
1. Let multiple infected enter close range.
2. Press ability.
3. Infected within 4.6m should take immediate damage.
4. If no infected is in range, Shockwave should not consume cooldown.

## Mobile input gate

On mobile / touchscreen simulation:
- dedicated Ability control appears separately from move and aim/fire
- touching/releasing Ability fires only the operator ability request
- it must not claim the touch as move or aim/fire
- READY/cooldown state remains readable inside Screen.safeArea

## Regression

- Workshop remains present after expedition → extraction → Bunker.
- Calibration / Salvage / Bunker upgrades persist.
- Arsenal orientation remains accepted.
- compact Field Ops / boss / reward UI remains accepted.
- sector FX remain accepted.
- final Bunker → expedition → combat → loot → extract → Bunker returns with **0 red runtime errors**.
