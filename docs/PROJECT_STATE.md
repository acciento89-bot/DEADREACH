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
- stable `main`: Production 0.11 until PR #12 promotion completes
- Production 0.11 merge: `5b1b40322e305b1546a9ca5a37c1f6b89eabea72`
- active branch: `production/0.12-sector-expansion`
- PR #12: fully validated, ready for promotion

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
- derive muzzle from that embedded firearm
- never reintroduce the failed external hand-mounted Rifle transform path

## Production 0.12 — Sector Expansion — FULLY VALIDATED ✅

### Final real-Unity validation
- fresh compile after 0.12b: **PASSED — 0 red compiler errors**
- `DEADREACH > Build Production Slice 0.12`: **PASSED**
- QUARANTINE WARD: **PASSED**
- TRANSIT COLLAPSE after declutter: **PASSED**
- INDUSTRIAL SPILL after declutter: **PASSED**
- BLACKOUT PLAZA after declutter: **PASSED**
- sector reward / BLACK CACHE Item Power behavior: **PASSED**
- fixed-zone mobile controls: **PASSED**
- full Bunker → Workshop/Arsenal → Deploy → mission/risk-reward → extraction → Bunker regression: **PASSED**
- Workshop/progression persistence: **PASSED**
- 0.10 combat-impact / boss / reward presentation regression: **PASSED**
- final Unity Console: **0 red runtime errors**

### World / sector architecture
- expanded east/west cross-street network
- four authored layouts: QUARANTINE WARD / TRANSIT COLLAPSE / INDUSTRIAL SPILL / BLACKOUT PLAZA
- sector-specific player, extraction, enemy, loot, objective and reinforcement anchors
- sector-specific fog / key-light identity
- dynamic sector selection plus editor-only deterministic override
- ordinary Runner enemies remain excluded from `_R##` reinforcement relocation

### Gameplay hazards
- Contamination / Electrical Arc / Fireline
- trigger-only gameplay zones
- periodic player damage while inside
- HUD danger warning clears after exit
- no control takeover / accepted CharacterController traversal

### Sector risk/reward
Primary completion extra unsecured Scrap:
- Quarantine +4
- Transit +6
- Industrial +8
- Blackout +10

BLACK CACHE Item Power bonus:
- Quarantine +2
- Transit +3
- Industrial +5
- Blackout +6

Successful-extraction banking behavior is accepted; unsecured run state remains risk-bearing.

### 0.12b layout polish
- Q-WARD intentionally unchanged
- TRANSIT large wrecks and barrier moved outward; objective/hazard spacing improved
- INDUSTRIAL props pushed out of core lanes; objective and hazard spacing improved
- BLACKOUT strongly decluttered with an open central HOLDOUT arena
- moved/rotated prop `CollisionBounds` are refreshed after final pose to prevent stale invisible blocking

### Stable systems preserved
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

## Promotion gate

Production 0.12 has passed all requested real-Unity gates. Next action is PR #12 Ready → verify mergeability → squash merge exact validated head → update this file on `main` to record the stable 0.12 merge SHA and clear the active production branch.

Test plan: `docs/PRODUCTION_12_TEST.md`
