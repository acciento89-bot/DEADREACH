# Production 0.8 — Workshop / Meta Progression

## Phase A implemented

Production 0.8 closes the gap between the documented core loop and the actual runtime: secured gear and Scrap can now become persistent power rather than only collection/progression metadata.

### Persistence
- schema v6
- per-weapon `upgradeLevel`
- Workbench / Medbay / Cargo Rig / Scavenger Network ranks
- migration clamps and preserves existing v5 profiles

### Weapon progression
- Item Power now affects real damage
- calibration adds +8 Item Power per level
- calibration adds small range and crit gains
- Workbench controls the calibration ceiling
- upgrade cost scales with rarity, Item Power and calibration level
- non-equipped weapons can be salvaged to secured Scrap
- active equipped weapon cannot be salvaged

### Bunker progression
- Workbench: +2 calibration ceiling levels per rank after the baseline field-bench rank
- Medbay: +6% operator max health per rank
- Cargo Rig: +1 extracted-weapon carrying slot per rank
- Scavenger Network: +8% secured Scrap on successful extraction per rank
- all tracks max at rank 5

### Validation strategy
Phase A must first pass real Unity compile + `Build Production Slice 0.8`. The dedicated responsive Workshop UI is Phase B on the same branch after this engine gate is green.
