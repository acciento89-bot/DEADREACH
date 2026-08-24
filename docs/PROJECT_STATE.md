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
- stable branch: `main`
- stable production level: **0.12**
- Production 0.12 / PR #12 squash merge: `2d328868a6510cb744cff65c7c547cd8148c448e`
- active production branch: **none**
- next production work must branch from current `main`

Permanent firearm rule:
- use artist-authored firearm geometry already parented to the Quaternius survivor rig
- derive muzzle from that embedded firearm
- never reintroduce the failed external hand-mounted Rifle transform path

## Production 0.12 — Sector Expansion — STABLE ✅

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

### Stable world / sector architecture
- expanded east/west cross-street network
- four authored layouts: QUARANTINE WARD / TRANSIT COLLAPSE / INDUSTRIAL SPILL / BLACKOUT PLAZA
- sector-specific player, extraction, enemy, loot, objective and reinforcement anchors
- sector-specific fog / key-light identity
- automatic sector selection with editor-only deterministic test override
- ordinary Runner enemies excluded from `_R##` reinforcement relocation

### Stable gameplay hazards
- Contamination / Electrical Arc / Fireline
- trigger-only gameplay zones
- periodic player damage while inside
- HUD danger warning clears after exit
- accepted CharacterController traversal / no control takeover

### Stable sector risk/reward
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

### Accepted 0.12b layout polish
- Q-WARD unchanged
- TRANSIT large wrecks / barrier moved outward and objective/hazard spacing improved
- INDUSTRIAL props pushed out of core lanes and objective/hazard spacing improved
- BLACKOUT strongly decluttered with an open central HOLDOUT arena
- moved/rotated prop `CollisionBounds` refreshed after final pose to prevent stale invisible blocking

### Earlier stable systems that remain authoritative
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

## Handoff

Production 0.12 is the current stable `main` baseline. No active production branch exists. Any next production pass must branch from current `main` and preserve the complete validated 0.12 world/mission/mobile/progression baseline.

Test plan: `docs/PRODUCTION_12_TEST.md`
