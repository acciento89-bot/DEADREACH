# DEADREACH — Production 0.10 Test Gate

Production 0.10 branches from the fully real-Unity-validated Production 0.9 `main` baseline.

## Goal

Increase combat readability and impact without changing the accepted 0.9 core loop, progression, operator input or fixed-zone mobile controls.

## Compile / build gate

1. Pull `production/0.10-combat-impact`.
2. Let Unity finish compiling.
3. Require **0 red compiler errors**. ✅ PASSED 2026-08-23
4. Run `DEADREACH > Build Production Slice 0.10`. ✅ PASSED 2026-08-23
5. No blocking red build/setup error. ✅

## Combat presentation gate — PASSED 2026-08-23

Real Unity runtime acceptance confirmed for the complete 0.10 combat-impact layer:

### Operator abilities
- SAM / FIELD PATCH heal presentation accepted ✅
- RAVEN / VECTOR DASH trail / endpoint presentation accepted ✅
- BRIGGS / SHOCKWAVE expanding ground pulse / radial impact presentation accepted ✅
- ability gameplay behavior remains correct ✅

### Infected specials
- RUNNER burst presentation accepted ✅
- BRUTE slam presentation accepted ✅
- STALKER flank presentation accepted ✅
- Walker remains readable baseline ✅
- effects do not obscure gameplay or remain stuck ✅

### Gunfight / camera impact
- existing tracer / muzzle / sparks / gore remain functional ✅
- successful-hit marker accepted ✅
- critical-hit presentation distinct and accepted ✅
- critical pulse accepted ✅
- camera/lens impact accepted as noticeable but comfortable ✅

## Mobile regression — REMAINING

Use the accepted landscape phone / Device Simulator setup:
- MOVE remains fixed lower-left
- up/down/left/right/diagonal movement still works
- AIM/FIRE remains fixed lower-right
- aiming and shooting remain reliable
- Ability remains independent upper-right
- new hit markers / ability VFX do not block touch controls
- mobile HUD readability remains accepted

## Full regression — REMAINING

1. Bunker → Workshop present.
2. Arsenal orientation/framing intact.
3. Deploy.
4. Combat / loot / ability / infected-special presentation remains correct.
5. Extract.
6. Return to Bunker.
7. Workshop and progression persist.
8. Sector atmosphere / reward / boss presentation remain intact where encountered.
9. Unity Console ends with **0 red runtime errors**.

Production 0.10 remains unmerged until the mobile and full regression gates pass.
