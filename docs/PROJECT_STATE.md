# DEADREACH — Project State

_Last updated: 2026-08-24_

Canonical handoff for DEADREACH. Update after every major implementation, validation and merge.

## Product / stable baseline

- Game: DEADREACH
- Studio: Kamilunavo
- Repository: `acciento89-bot/DEADREACH`
- Platforms: iOS + Android
- Unity: `6000.3.22f1`
- URP: 17.3
- Bundle ID: `de.kamilunavo.deadzone`
- Mobile: landscape only
- stable `main`: Production 0.11
- Production 0.11 merge: `5b1b40322e305b1546a9ca5a37c1f6b89eabea72`
- active branch: `production/0.12-sector-expansion`
- PR #12: Draft

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
- derive muzzle from that embedded firearm
- never reintroduce the failed external hand-mounted Rifle transform path

## Production 0.12 — Sector Expansion

### Current validation state
- Q-WARD runtime gate: **PASSED / still accepted** ✅ 2026-08-24
- fresh compile after 0.12b: **PASSED — 0 red compiler errors** ✅ 2026-08-24
- `Build Production Slice 0.12` after 0.12b: **PASSED** ✅ 2026-08-24
- TRANSIT runtime revalidation after declutter: **PASSED** ✅ 2026-08-24
- INDUSTRIAL runtime revalidation after declutter: **PASSED** ✅ 2026-08-24
- BLACKOUT runtime revalidation after declutter: **PASSED** ✅ 2026-08-24
- all four sector layouts now have accepted real-Unity runtime coverage
- sector reward gate: pending
- fixed-zone mobile regression: pending
- full Bunker → mission/risk-reward → extraction → Bunker regression: pending
- final Unity Console 0 red runtime errors: pending

### 0.12b layout-polish implementation — ACCEPTED

New editor pass: `Assets/Deadreach/Editor/Production12LayoutPolishPass.cs`.

`Build Production Slice 0.12` runs:
1. accepted base scene generation
2. normal Production 0.12 sector authoring
3. Production 0.12b layout-polish pass

Q-WARD is deliberately untouched.

TRANSIT:
- large wreck bodies / barrier moved toward lane edges
- central HOLDOUT area opened
- mission anchors redistributed
- arc hazard moved away from objective center
- moved/rotated collision bounds refreshed

INDUSTRIAL:
- barrels moved out of the central channel
- service truck moved farther east
- north barrier moved outward
- mission anchors redistributed
- chemical/fire hazard lanes separated from mission circles
- moved/rotated collision bounds refreshed

BLACKOUT:
- both large vehicles moved to opposite lane edges
- barriers moved outward
- central HOLDOUT plaza cleared
- mission anchors redistributed
- nearby loot/enemy positions removed from the holdout circle
- arc/fire hazards separated from the central objective arena
- moved/rotated collision bounds refreshed

### Existing 0.12 systems preserved
- expanded east/west cross-street network
- sector-specific player / extraction / enemy / loot / objective / reinforcement anchors
- sector-specific fog / key-light identity
- Contamination / Electrical Arc / Fireline hazards
- FIELD OPS Sector + hazard status
- Primary sector Scrap risk bonus
- BLACK CACHE sector Item Power bonus
- editor-only sector override AUTO / QUARANTINE / TRANSIT / INDUSTRIAL / BLACKOUT
- ordinary Runner enemies excluded from `_R##` reinforcement relocation

### Stable systems that must remain green
- 0.11 RECOVERY / PURGE / HOLDOUT / BLACKSITE
- objective-gated extraction
- BLACK CACHE risk/reward path
- reinforcement waves
- schema-v6 Workshop progression
- fixed lower-left MOVE
- fixed lower-right AIM/FIRE
- independent upper-right Ability
- 0.10 combat-impact VFX
- accepted Arsenal / Bunker / boss / reward presentation

## Next exact gate

1. return Sector 0.12 override to `AUTO`
2. validate sector Primary Scrap bonus
3. validate BLACK CACHE Item Power bonus
4. validate free-slot and full-inventory pending-reward paths
5. validate fixed-zone mobile controls + HUD
6. run full Bunker → Workshop/Arsenal → Deploy → mission/risk-reward → extraction → Bunker regression
7. require final Unity Console **0 red runtime errors**

Test plan: `docs/PRODUCTION_12_TEST.md`
