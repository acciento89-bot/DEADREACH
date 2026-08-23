# DEADREACH — Production 0.5 Acceptance

Production 0.5 is the large Bunker / progression / boss / combat-presentation pass.

## Baseline locks

Production 0.4 is merged and accepted. Do not regress:
- colored Quaternius Survivor/Infected
- accepted artist-rigged embedded left-hand Survivor weapon
- muzzle/tracer originates from the embedded weapon
- Dead City streets, vehicles, containers and props
- extraction approach remains traversable
- Bunker -> Deploy -> loot -> extraction -> Bunker flow

## Gate A — Compile

1. Switch/pull `production/0.5-bunker-progression-boss-ui`.
2. Let Unity finish script/package import.
3. Require **0 red C# compiler errors**.

Do not run the generator until Gate A passes.

## Gate B — Generator

Run:

`DEADREACH > Build Production Slice 0.5`

Require:
- required 0.4 art gate passes
- extraction traversal gate passes
- Bunker_Hub is reopened after generation
- no blocking Console error

## Gate C — Bunker Command Center

Press Play. It must start in Bunker, not Dead City.

Validate the new post-apocalyptic Bunker Command Center:
- Overview tab opens
- Arsenal tab opens
- Operators tab opens
- Campaign tab opens
- Store tab opens
- Deploy button works
- blast-door / industrial bunker dressing is visible behind the command UI

### Arsenal
- secured stash weapons are listed
- rarity is visible
- item power is visible
- all affix rolls are readable
- Equip changes the equipped primary and persists after returning to Bunker

### Operators
Validate all three base operators can be selected:
- SAM / Ranger
- RAVEN / Scout
- BRIGGS / Warden

Selection must persist. In run, the operator profile must affect health/mobility/damage and use the selected visual tint profile without disturbing the artist-rigged weapon.

### Campaign
- 50 levels are represented in five sectors
- only unlocked levels are selectable
- every tenth level is visibly marked BOSS
- selected level persists

### Store
- cosmetic / Bunker theme / weapon finish / season cards are visible
- buttons are non-purchasing placeholders in 0.5
- no purchase may be claimed completed; StoreKit / Google Play verification is a later integration gate

## Gate D — Standard level progression

Select Level 1 and Deploy.

Validate:
- HUD shows Level 01 / Dead City
- Walker / Runner / Brute / Stalker stat profiles create noticeable enemy variety
- movement / aim / fire / damage / loot work
- extraction works after carrying loot
- successful extraction returns to Bunker
- Level 2 becomes unlocked

## Gate E — Boss Level 10

Use the Editor shortcut so this test does not require nine manual clears:

`DEADREACH > Dev > 0.5 Select Boss Level 10`

Then Play -> Deploy.

Require:
- HUD shows Level 10 and mutation-class boss bar
- one infected is promoted to a large high-health boss
- boss becomes more aggressive around 66% and 33% HP
- entering extraction before boss death shows `EXTRACTION SEALED`
- extraction cannot complete while boss is alive
- after boss death the extraction gate unlocks
- successful extraction records the boss clear and can unlock Level 11

## Gate F — Combat VFX

Validate repeated firing at enemies and environment:
- no old red square hit marker remains
- tracer is no longer a plain white line
- shot has bright core + colored glow trail
- muzzle flash appears at the accepted weapon muzzle
- world impacts produce directional sparks
- infected hits produce small gore/spark FX
- critical hits are visually stronger
- no runaway VFX object creation / obvious frame collapse

## Gate G — Regression

Final run must still pass:
- Bunker starts first
- Deploy
- player moves
- player aims/fires
- accepted embedded weapon remains stable
- muzzle alignment remains correct
- loot pickups work
- vehicle/container collision works
- extraction corridor remains reachable
- successful extraction returns to Bunker
- failed/abandoned run returns to Bunker and loses unsecured run loot
- 0 red blocking Console errors

Production 0.5 stays Draft until these real-Unity gates pass.
