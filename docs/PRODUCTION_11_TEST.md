# DEADREACH — Production 0.11 Test Gate

Production 0.11 branches from the fully real-Unity-validated Production 0.10 `main` baseline.

## Goal

Turn the expedition from a simple loot → extraction run into a mission-driven run with primary objectives, reinforcement pressure and a real risk/reward choice after the primary objective.

## Compile / build gate

1. Pull `production/0.11-expedition-director`.
2. Let Unity finish compiling.
3. Initial 0.11 compile reached **0 red compiler errors** ✅ 2026-08-23.
4. Extraction-support geometry fix was pulled and the rebuilt 0.11 scene was used for the successful extraction-egress retest ✅.

## Expedition Director gate

The first real runtime pass confirmed the new mission flow is active: mission HUD, objective marker, mission-gated extraction, primary completion, BLACK CACHE and reinforcement behavior all functioned in the tested run ✅.

The mission rotates by level/run state; boss levels force BLACKSITE.

### RECOVERY
- FIELD OPS shows `MISSION // RECOVERY`.
- cyan objective marker is visible in the world.
- standing inside the core radius progresses `SECURE DATA CORE`.
- leaving the radius causes partial progress decay.
- primary completion grants carried Scrap and unlocks extraction.

### PURGE
- FIELD OPS shows `MISSION // PURGE`.
- objective counts infected eliminations.
- kill target never exceeds the available ordinary infected population.
- primary completes after the displayed kill target is reached.

### HOLDOUT
- FIELD OPS shows `MISSION // HOLDOUT`.
- activate the yellow uplink marker first.
- after activation the objective changes to `DEFEND UPLINK` with a countdown.
- countdown advances only while the player stays inside the large hold radius.
- leaving the radius pauses the hold and raises a visible signal-loss alert.
- reinforcement waves arrive while the hold is active.
- reinforcement infected use production visuals and the existing Walker / Runner / Brute / Stalker combat-role stack.

### BLACKSITE
- boss levels use BLACKSITE; non-boss BLACKSITE runs are also allowed by mission rotation.
- breach the purple terminal first.
- non-boss run: a response wave spawns and must be cleared.
- boss run: the mutation target remains the elimination stage authority.
- after elimination the objective marker relocates to the vault/core point.
- secure the core to complete the primary objective.

## Extraction gate

- entering extraction before the primary objective is complete shows `EXTRACTION SEALED` and the current primary objective ✅ observed in real runtime.
- no extraction progress occurs while mission-gated ✅.
- after primary completion, mission gate opens immediately ✅.
- existing boss gate and no-loot gate remain intact.

### Extraction egress — FIXED / REAL RUNTIME ACCEPTED ✅

The sealed-extraction state exposed an older world-geometry edge case: `ExtractionZone_Alpha` is centered at `z=20`, while the original base ground and main road ended around `z=19`. The fix extends the supported road/ground beyond the extraction trigger and forces extraction-owned colliders to remain triggers.

Real runtime retest passed:
- enter sealed extraction before primary completion ✅
- walk back out normally ✅
- re-enter successfully ✅
- extraction state/overlay clears correctly on exit ✅

No mobile-input or mission-logic workaround was required.

## Risk / reward gate

After every primary objective:
- primary completion grants unsecured carried Scrap.
- `EXTRACTION AVAILABLE` is visible.
- an orange optional BLACK CACHE marker appears away from the primary objective.
- player may extract immediately or travel to the optional cache.
- approaching the cache triggers a hostile response wave.
- securing the cache grants a reserved bonus weapon.
- cache reward has a minimum Uncommon rarity, minimum Rare from level 25+, and bonus Item Power.
- if run inventory is full, the weapon remains reserved and is still banked only on successful extraction.
- dying/abandoning clears the reserved mission reward.

## HUD / presentation gate

- existing mobile-readable FIELD OPS layout remains readable.
- HUD shows mission name, threat state, objective text and progress.
- short center alerts show mission start, reinforcement warnings, signal loss/restored and objective completion.
- objective world markers pulse and remain readable without hiding combat.
- boss bar, ability HUD and existing 0.10 hit/ability/special VFX remain readable.

## Mobile regression

Use the accepted landscape phone / Device Simulator setup:
- MOVE remains fixed lower-left and full 360°.
- AIM/FIRE remains fixed lower-right.
- Ability remains independent upper-right.
- mission HUD does not cover the control zones.
- objective markers / reinforcement alerts do not steal touch input.

## Final full regression — REMAINING GATE

1. Bunker → Workshop present.
2. Arsenal orientation/framing intact.
3. Deploy.
4. Complete a primary objective.
5. Confirm extraction unlocks.
6. Either extract immediately or complete the optional cache first.
7. Extract.
8. Return to Bunker.
9. Workshop and progression persist.
10. Optional cache weapon is banked only after successful extraction.
11. Sector atmosphere / reward / boss / 0.10 combat-impact presentation remain intact.
12. Fixed-zone mobile MOVE / AIM-FIRE / Ability remain green.
13. Unity Console ends with **0 red runtime errors**.

Production 0.11 remains unmerged until the final full regression passes.
