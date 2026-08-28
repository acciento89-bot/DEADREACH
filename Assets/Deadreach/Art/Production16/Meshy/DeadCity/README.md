# DEADREACH Production 0.16 — Meshy Dead City

Export the accepted Meshy models as GLB into this folder using these exact names:

Required:
- `RuinedBuilding_A.glb`
- `RuinedBuilding_B.glb`
- `CornerBuilding.glb`
- `CollapsedStorefront.glb`
- `IndustrialBuilding.glb`
- `RubbleLarge.glb`
- `MilitaryCheckpoint.glb`

Optional:
- `VehicleVan.glb`

The destroyed bus is intentionally excluded.

Workflow after Unity imports the files:
1. `DEADREACH > Production 0.16 > Repair Meshy Dead City GLB Imports`
2. `DEADREACH > Production 0.16 > Repair Meshy Dead City Materials (URP)`
3. `DEADREACH > Production 0.16 > Validate Meshy Dead City Assets`
4. `DEADREACH > Production 0.16 > Build Meshy Dead City Layout`

The six legacy `Building_Block_XX` objects remain as gameplay/collision fallback only. Their renderers are disabled when the matching Meshy layout is built. Existing mission, sector, hazard, combat and extraction logic must remain unchanged.
