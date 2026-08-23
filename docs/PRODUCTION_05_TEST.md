# DEADREACH — Production 0.5 MEGA Runtime Acceptance

Production 0.5 is intentionally validated as **one large end-to-end runtime gate**, not a chain of tiny approval steps.

## Stable baseline that must not regress

Production 0.4 is merged and accepted. Keep:
- colored Quaternius Survivor/Infected art
- artist-rigged SingleWeapon firearm path; never mount a second external rifle
- muzzle/tracer derived from the embedded firearm
- Dead City streets, vehicles, containers and props
- extraction approach traversable
- Bunker -> Deploy -> combat/loot -> extraction -> Bunker loop

## Preflight

1. Pull `production/0.5-bunker-progression-boss-ui`.
2. Let Unity finish compilation/import.
3. Require **0 red C# compiler errors**.
4. Run **`DEADREACH > Build Production Slice 0.5` once**.
   - this build also auto-prepares missing Quaternius Lis/Matt SingleWeapon operator art
   - required 0.4 environment asset gate must pass
   - extraction traversal gate must pass
   - Bunker_Hub must reopen at the end
5. No blocking Console error may remain.

Do not stop for cosmetic warnings; record only blocking/red runtime failures.

# ONE MEGA RUNTIME GATE

Start Play once in Bunker and perform the following sequence as one acceptance run.

## 1. Bunker / menu / persistence

Confirm Play starts in **Bunker_Hub**, not Dead City.

Check all Command Center tabs:
- Overview
- Arsenal
- Operators
- Campaign
- Store

Desktop/editor visual direction must remain coherent after the accepted UI polish:
- DEADREACH header visible
- no overlapping Overview text
- Arsenal list and 3D inspector remain in separate columns
- Operator roster and 3D preview remain separate
- Campaign displays one 10-level sector at a time
- deploy bar stays accessible

This is still editor/desktop acceptance only. Mobile safe-area/responsive acceptance is a later mandatory release gate.

### Arsenal

Require:
- secured weapons listed
- rarity visible
- item power visible
- all affix rolls readable
- Equip works and persists
- **3D weapon preview is horizontal/canonical, not standing on its head / vertical**
- preview rotates cleanly without changing gameplay weapon transforms

### Operators — distinct visual gate

Select each operator and observe the 3D preview:
- **SAM / Ranger = Quaternius Sam**
- **RAVEN / Scout = Quaternius Lis**
- **BRIGGS / Warden = Quaternius Matt**

Require:
- the three operators are visibly different character models, not three recolored Sams
- preview faces the camera rather than permanently showing only the back
- selection persists when switching tabs

Leave one non-Sam operator selected for the first deployment.

### Campaign / Store

Campaign:
- 50 levels represented through five 10-level sectors
- locked levels cannot be selected
- every tenth level is marked as boss
- selected level persists

Store:
- cosmetic/operator/Bunker/weapon/season surfaces visible
- no fake purchase succeeds
- StoreKit / Google Play remains a later verified integration

## 2. Standard Level 1 full run

Select Level 1 and Deploy with the currently selected non-Sam operator.

Require:
- **the in-game character model matches the selected operator model**
- operator profile changes real health/mobility/damage as designed
- artist-authored SingleWeapon remains stable on the selected character
- muzzle follows the embedded firearm; no external rifle/socket hack returns
- HUD shows Level 01 / Dead City
- movement works
- aim/fire works
- damage works
- loot works

### Combat presentation

During repeated shots at infected and environment confirm:
- no old red square hit marker
- tracer is not a plain white prototype line
- bright tracer core + glow trail visible
- muzzle flash originates at the embedded firearm
- environment impacts create directional sparks
- infected impacts create directed gore/spark streaks rather than red billboard squares
- critical hit FX are stronger
- no obvious runaway VFX allocation/frame collapse

### Enemy variety

During the run, confirm the configured archetypes are meaningfully different in behavior/size/stats:
- Walker
- Runner
- Brute
- Stalker

They may share the validated Quaternius infected visual family, but their combat profiles must not feel identical.

### Extraction / progression

Carry loot into extraction:
- extraction corridor remains physically reachable
- extraction succeeds
- returns to Bunker
- secured Scrap/weapon loot persists
- Level 2 becomes unlocked
- selected operator remains selected
- equipped primary remains persisted

## 3. Cross-operator runtime swap

Back in Bunker:
1. switch to a different operator than the one used in Level 1
2. Deploy again briefly
3. confirm the newly selected **different model appears in gameplay**, with its own stats
4. confirm embedded weapon/muzzle still works
5. abandon/pause-return to Bunker

Abandonment must still lose unsecured run loot and return cleanly to Bunker.

## 4. Level 10 mutation boss

Stop Play.

Use:

`DEADREACH > Dev > 0.5 Select Boss Level 10`

Then Play -> Deploy.

Require:
- HUD shows Level 10
- mutation boss HP bar visible
- one infected promoted to a clearly larger/high-health boss
- boss is meaningfully challenging relative to normal infected
- aggression/scaling changes around ~66% and ~33% HP
- attempting extraction before boss death shows **EXTRACTION SEALED**
- extraction cannot complete while boss lives
- boss death unlocks extraction
- extraction after boss death succeeds
- boss clear persists
- Level 11 can unlock after successful Level 10 extraction

## 5. Final regression sweep

Before declaring 0.5 accepted, confirm the complete session contained no regression in:
- Bunker-first start flow
- menu interaction
- operator persistence
- distinct operator model selection
- Arsenal equip persistence
- horizontal weapon preview
- player movement
- aiming/firing
- embedded weapon stability
- muzzle alignment
- loot pickup
- vehicle/container collision
- extraction traversal
- successful extraction return
- failed/abandoned run behavior
- campaign unlock persistence
- boss extraction lock/unlock
- 0 red blocking Console errors

## Acceptance result

Production 0.5 remains Draft until this entire Mega Runtime Gate passes in real Unity.

After the gate:
- if everything passes: record one real-Unity acceptance and prepare PR #5 for merge
- if something fails: report the **first actual blocker plus screenshot/Console error**, fix it in branch, then rerun only the affected portion plus final regression sweep

## Separate mandatory mobile release gate

Do **not** treat this Mega Runtime Gate as mobile UI acceptance. Before App Store / Play release, the dedicated safe-area/responsive/touch/device gate in `docs/PROJECT_STATE.md` must still pass on real iPhone + Android landscape hardware.
