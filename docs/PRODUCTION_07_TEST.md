# DEADREACH — Production 0.7 Presentation / Layout / Arsenal Polish

Production 0.7 is a focused polish pass stacked on the functionally validated Production 0.6 branch. It must preserve 0.6 gameplay/reward behavior while fixing the visual debt exposed by the real Unity gate.

## Scope

- Bunker navigation/content/header/deploy layout must not overlap across landscape aspect ratios.
- Boss reward popup and Bunker relic debrief must use safe-area-aware presentation and must not collide with persistent HUD/header regions.
- Arsenal 3D weapon inspector must present Rifle / SMG / Pistol / Shotgun in readable production orientation; DR-7 remains the visual baseline.
- Sector atmosphere FX must no longer render as magenta/purple missing-material streaks.
- Flooded Industrial rain, Ash District ash, Blackout dust and Ground Zero contamination must read as different atmosphere families.
- No regression to 0.6 boss/reward persistence, extraction, weapon stats or sector traversal.

## Validation progress

- real Unity compile gate: ✅ 0 compiler errors on 2026-08-23
- real Unity build gate: ✅ `DEADREACH > Build Production Slice 0.7` completed on 2026-08-23 with no blocking red build/import error reported
- runtime presentation gates: pending

## Preflight

1. `git fetch`
2. `git switch production/0.7-presentation-polish`
3. `git pull`
4. let Unity finish compile/import
5. require **0 red compiler errors** ✅
6. run **`DEADREACH > Build Production Slice 0.7`** once ✅
7. require no blocking red build/import errors ✅

## A. Bunker / responsive layout

Inspect Game view at:
- 16:9 landscape
- ~19.5:9 ultrawide phone landscape
- compact/tablet landscape around 4:3–16:10

Required:
- header, navigation, content and deploy bar never overlap
- navigation remains fully tappable
- Arsenal list and 3D inspector remain inside their own columns
- Store / Operators / Campaign content remains readable
- no content enters safe-area margins

## B. Arsenal weapon orientation

Open Arsenal and equip at least one weapon from every family.

Required:
- DR-7 / Rifle remains correct
- SMG magazine / grip reads downward, not upside down
- Pistol grip reads downward and silhouette is horizontal/readable
- Shotgun is horizontal/readable and not inverted
- model remains centered while rotating
- finish tint remains visible

## C. Boss / reward / debrief presentation

Use Level 10.

Required:
- boss identity remains readable and does not collide with the reward card
- `MUTATION RELIC SECURED` appears inside safe area and below persistent top HUD regions
- all reward fields remain readable
- after extraction, Bunker debrief blocks underlying interaction and remains fully inside safe area
- `TRANSFER TO ARSENAL` remains visible/tappable
- transferred reward exists in Arsenal and debrief does not return after acknowledgement

## D. Sector FX polish

Inspect Levels 11 / 21 / 31 / 41.

Required:
- no magenta/purple missing-material particle streaks
- Level 11 reads as cool rain / wet atmosphere
- Level 21 reads as warm drifting ash
- Level 31 reads as sparse dark dust / blackout atmosphere
- Level 41 reads as red contamination motes / mutation atmosphere
- FX do not block traversal or obscure gameplay excessively

## E. Final regression

Confirm:
- 0 compiler errors
- no blocking red Console errors
- Bunker-first start
- movement / aim / fire
- loot pickup
- boss seal / unseal
- boss reward popup
- extraction
- relic debrief persistence
- Arsenal equip persistence
- Levels 1 / 11 / 21 / 31 / 41 remain traversable

## Acceptance

Production 0.7 can be promoted only when the presentation gates above are clean in real Unity. Until then its PR remains Draft.
