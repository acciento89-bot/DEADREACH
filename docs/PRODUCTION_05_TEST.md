# DEADREACH — Production 0.5 MEGA Runtime Acceptance

Production 0.5 was intentionally validated as **one large end-to-end runtime gate**, not a chain of tiny approval steps.

## Stable baseline that must not regress

Production 0.4 is merged and accepted. Keep:
- colored Quaternius Survivor/Infected art
- artist-rigged embedded firearm path; never mount a second external rifle
- muzzle/tracer derived from the embedded firearm
- Dead City streets, vehicles, containers and props
- extraction approach traversable
- Bunker -> Deploy -> combat/loot -> extraction -> Bunker loop

## Preflight — PASSED IN REAL UNITY

Real Unity has already confirmed:
- 0 red C# compile errors before the Mega Gate
- Atomic operator importer V2 build/import completes without blocking error
- Bunker_Hub starts correctly

## MEGA RUNTIME GATE — MAJOR PORTION PASSED

Real Unity screenshots/user validation confirm:
- Bunker Command Center layout and navigation
- distinct SAM / RAVEN / BRIGGS previews
- character selection persists and changes the actual gameplay operator
- Level 1 gameplay/movement/aim/fire
- upgraded tracer/combat presentation visible
- loot/extraction loop works
- Level 10 mutation boss appears with boss HUD
- boss can be defeated and campaign progresses through Level 10 into Level 11 / Sector 02

The full Mega Gate exposed five finalization defects instead of requiring a complete restart:
1. next level unlocked but was not automatically selected after successful extraction
2. boss still used ordinary Scrap as its configured reward
3. Arsenal weapon preview was horizontal but upside-down
4. stash weapons had no meaningful visible finish variation
5. player could leave the authored road at the map end, fall into the void and continue running below the level

All five are fixed in branch and require only the targeted retest below.

# TARGETED FINALIZATION RETEST

## 1. Preflight / rebuild

1. `git pull`
2. let Unity compile; require **0 red compiler errors**
3. run **`DEADREACH > Build Production Slice 0.5` once**
   - required because the finalization pass authors new Dead City world-bound colliders and `PlayerFallSafety`
4. no manual asset deletion/import is required

## 2. Arsenal orientation + finishes

Open Arsenal and equip at least two or three existing stash weapons.

Require:
- 3D rifle is horizontal **and upright** (grip/magazine below receiver, not on its head)
- inspector shows `FINISH // ...`
- old stash weapons derive different deterministic finishes where their IDs differ
- newly generated weapons carry a persisted finish ID
- changing equipped weapon changes the preview finish when appropriate
- deploy once and verify the selected finish also colors the artist-rigged embedded firearm
- hand/muzzle alignment remains unchanged

## 3. Automatic campaign advance

Run any currently selected standard level and extract successfully.

Require on return to Bunker:
- next level is unlocked
- **next level is automatically selected** without manually clicking Campaign
- deploy bar immediately targets that next level

Replaying an old level should advance to its next already-unlocked level; Level 50 stays at 50.

## 4. Boss reward replaces boss Scrap

Use:

`DEADREACH > Dev > 0.5 Select Boss Level 10`

Play -> Deploy -> kill boss.

Require:
- boss itself drops **0 ordinary Scrap**
- normal enemies may still drop normal Scrap
- boss grants guaranteed `MUTATION T1 // DR-7 RELIC`
- reward has Mutation Core finish and strong affixes
- if weapon inventory has room, Weapon Loot count increases immediately
- if inventory is full, reward remains reserved rather than disappearing
- extraction is allowed after boss death even if the reserved boss reward is the only loot
- after successful extraction the mutation weapon appears in Bunker stash
- boss clear still advances Level 10 -> Level 11 automatically

## 5. Dead City world/fall safety

Deliberately run toward:
- West edge
- East edge
- South/start edge
- North/end beyond extraction

Require:
- invisible boundaries stop the player before walking into the void
- no visible wall geometry is introduced
- player cannot continue running beneath the map
- if any physics/spawn edge case gets outside/below the authored rectangle, `PlayerFallSafety` immediately restores the last valid in-bounds position
- extraction approach remains traversable and is not blocked by the new North boundary

## 6. Final regression sweep

After the targeted retest confirm:
- operator switching still works
- embedded firearm/muzzle still works
- combat/loot/extraction still works
- Level 10 boss gate still works
- no red blocking Console errors

## Acceptance result

If the targeted finalization retest passes, Production 0.5 is accepted in real Unity and PR #5 can be marked Ready and merged to `main`.

## Separate mandatory mobile release gate

This is **not** mobile UI acceptance. Before App Store / Play release, safe-area/responsive/touch/device validation in `docs/PROJECT_STATE.md` still must pass on real iPhone + Android landscape hardware.
