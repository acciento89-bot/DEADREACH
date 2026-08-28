# DEADREACH — Production 0.16 Field Ops Test Gate

Production 0.16 branches from merged Production 0.15 and leaves the accepted Bunker presentation frozen.

## Scope

Replace the expedition-only legacy `PrototypeHud` with the production uGUI Field Ops presentation while preserving the validated 0.12 mission / sector / hazard / combat / extraction loop.

## First Unity gate

1. Pull `production/0.16-dead-city-field-ops` and wait for a fresh Unity compile.
2. Run `DEADREACH > Production 0.16 > Apply Dead City Field Ops`.
3. Run `DEADREACH > Production 0.16 > Validate Dead City Field Ops`.
4. Enter Play Mode in `DeadCity_VerticalSlice`.

Expected static result:
- compile: 0 red compiler errors
- Field Ops validator: PASS
- legacy PrototypeHud is disabled
- one `Production16FieldOpsUI` owns expedition HUD presentation

## Runtime smoke

- three top HUD zones are readable and do not overlap
- safe-area layout remains inside landscape screen bounds
- mobile MOVE / AIM-FIRE lower zones remain unobstructed
- VITALS changes with player health
- carried Scrap / weapon loot update live
- operator ability READY / cooldown updates live
- current sector + hazard identity is visible
- mission name / threat / primary objective update live
- optional BLACK CACHE state remains visible
- NAV reports objective distance + LEFT / AHEAD / RIGHT
- after primary completion NAV switches to EXTRACT
- extraction correctly reports SEALED / LOCKED / EXTRACTING states
- extraction progress updates to completion
- boss level still exposes mutation health gate
- success/failure result state remains readable
- Bunker return and persistence remain unchanged

## Merge gate

Do not merge until fresh compile, Field Ops validator, one complete non-boss expedition, mobile control clearance and return-to-Bunker regression are green. Boss-level presentation may be validated in the same pass or a deterministic follow-up before merge.
