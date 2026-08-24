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
- Q-WARD runtime gate: **PASSED / still accepted**; 0.12b does not modify Q-WARD
- TRANSIT runtime gate: **must be repeated** after 0.12b geometry changes
- INDUSTRIAL runtime gate: **must be repeated** after 0.12b geometry changes
- BLACKOUT first layout: **rejected as too crowded**; 0.12b fix implemented and awaiting test
- fresh compile: required again after code changes
- Build Production Slice 0.12: required again after generator changes
- sector reward gate: pending
- fixed-zone mobile regression: pending
- full Bunker → mission/risk-reward → extraction → Bunker regression: pending
- final Unity Console 0 red runtime errors: pending

### 0.12b layout-polish implementation

New editor pass: `Assets/Deadreach/Editor/Production12LayoutPolishPass.cs`.

`Build Production Slice 0.12` now runs:
1. accepted base scene generation
2. normal Production 0.12 sector authoring
3. Production 0.12b layout-polish pass

Q-WARD is deliberately untouched.

TRANSIT:
- large wreck bodies / barrier moved toward lane edges
- central HOLDOUT area opened
- mission anchors redistributed
- arc hazard moved away from objective center

INDUSTRIAL:
- barrels moved out of the central channel
- service truck moved farther east
- north barrier moved outward
- mission anchors redistributed
- chemical/fire hazard lanes separated from mission circles

BLACKOUT:
- both large vehicles moved to opposite lane edges
- barriers moved outward
- central HOLDOUT plaza cleared
- mission anchors redistributed
- nearby loot/enemy positions removed from the holdout circle
- arc/fire hazards separated from the central objective arena

Important collision hardening:
- every moved-and-rotated production prop refreshes its generated `CollisionBounds` after the final pose so stale collider orientation cannot continue blocking a route after the visual object moved.

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

1. `git pull` on `production/0.12-sector-expansion`
2. fresh Unity compile → **0 red compiler errors**
3. `DEADREACH > Build Production Slice 0.12`
4. recheck TRANSIT
5. recheck INDUSTRIAL
6. recheck BLACKOUT
7. return override to AUTO
8. sector reward + mobile + full regression

Test plan: `docs/PRODUCTION_12_TEST.md`
