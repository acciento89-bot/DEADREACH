# DEADREACH — Production 0.9 Test Gate

Production 0.9 branches from the real-Unity-validated Production 0.8 `main` baseline.

## Goal

Turn existing statistical enemy/operator variants into real gameplay identities without destabilizing the accepted 0.8 progression/presentation stack, and finish a production-usable mobile twin-stick layer.

## Accepted combat depth

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

## Fixed-zone mobile implementation — PASSED 2026-08-23

Real landscape phone/Device Simulator validation accepted:
- fixed lower-left MOVE control ✅
- up/down/left/right/diagonal movement works ✅
- movement stop/change response accepted ✅
- fixed lower-right AIM/FIRE control ✅
- directional aiming works without snapping toward Ability/UI ✅
- shooting works reliably from the right stick ✅
- releasing the right stick stops firing ✅
- upper-right Ability control works independently ✅
- Ability touch does not become move/aim/fire ✅
- Ability feedback makes registration visible ✅
- mobile FIELD OPS / Vitals / HP / loot / scrap / objective readability accepted ✅

## Final regression — REMAINING

Run one complete accepted-baseline loop:
1. Bunker → Workshop: Workshop present and progression values intact.
2. Arsenal: weapon orientation/framing still accepted.
3. Deploy.
4. Combat and collect loot/scrap.
5. Extract successfully.
6. Return to Bunker.
7. Workshop remains present after scene reload.
8. Calibration / Salvage / Bunker upgrade persistence remains intact.
9. Enemy role behavior remains accepted.
10. SAM / RAVEN / BRIGGS ability gameplay remains accepted.
11. Sector FX / reward / boss presentation remain accepted where encountered.
12. Unity Console ends with **0 red runtime errors**.

When this regression passes, Production 0.9 is ready to leave Draft and merge to `main`.
