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

### Validation state
- fresh real-Unity compile: **PASSED — 0 red compiler errors** ✅ 2026-08-24
- `DEADREACH > Build Production Slice 0.12`: **PASSED** ✅ 2026-08-24
- QUARANTINE WARD runtime gate: **PASSED** ✅ 2026-08-24
- TRANSIT COLLAPSE runtime gate: **PASSED** ✅ 2026-08-24
- INDUSTRIAL SPILL runtime gate: **PASSED** ✅ 2026-08-24
- next: BLACKOUT PLAZA
- sector reward gate: pending
- fixed-zone mobile regression: pending
- full Bunker → mission/risk-reward → extraction → Bunker regression: pending
- final Unity Console 0 red runtime errors: pending

### Accepted real-runtime sector behavior

QUARANTINE WARD:
- Q-WARD / BIOHAZARD identity and green/teal atmosphere accepted
- west/east spur out-and-back accepted
- east extraction reachable
- pre-Primary extraction sealed
- mission marker works in sector geography
- contamination damage/warning clears on exit
- no tested CharacterController trapping

TRANSIT COLLAPSE:
- cold/blue identity accepted
- wreck cluster changes route choice
- alternate route around wrecks traversable
- west extraction reachable and reversible
- electrical hazard damage/warning clears on exit
- mission marker stays on supported geometry
- ordinary infected remain on normal enemy geography
- reinforcement geography accepted

INDUSTRIAL SPILL:
- amber industrial identity accepted
- container / pipe / barrel channel layout accepted
- north extraction reachable and reversible
- chemical and fire hazards visually distinct
- both hazards damage only while inside and clear on exit
- no hazard CharacterController trapping
- mission marker / loot / enemy placement remain reachable

### 0.12 implementation
- expanded east/west cross-street network
- four authored sector layouts: QUARANTINE WARD / TRANSIT COLLAPSE / INDUSTRIAL SPILL / BLACKOUT PLAZA
- sector-specific player, extraction, enemy, loot, objective and reinforcement anchors
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

## Next exact test

Select:
`DEADREACH > Dev > Sector 0.12 > BLACKOUT PLAZA`

Validate:
- FIELD OPS BLACKOUT PLAZA
- violet/red blackout emergency identity
- route blockers clearly differ from previous sectors
- east extraction reachable and reversible
- electrical arc hazard damages only while inside and clears on exit
- fire hazard damages only while inside and clears on exit
- both hazards remain readable during combat and do not trap the CharacterController
- mission marker / loot / enemy positions remain reachable
- runtime reinforcements use valid sector geography

Test plan: `docs/PRODUCTION_12_TEST.md`
