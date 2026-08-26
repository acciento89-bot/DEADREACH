# DEADREACH Production 0.15 — Meshy Bunker Assets

Export the accepted Meshy models as **GLB** and copy them into this folder. The editor layout builder also accepts `.gltf`, `.fbx` or `.prefab`.

Use these exact file stems (extension does not matter):

- `Wall_Standard`
- `Wall_Technical`
- `Wall_Utility`
- `Wall_Corner`
- `Door_Blast`
- `Floor_A`
- `Pillar_A`
- `CommandConsole`
- `HologramTable`
- `ArsenalRack`
- `WorkshopBench`
- `SupplyStation`
- `OperatorBay`
- `Generator`
- `Crates`
- `VentUnit`
- `PowerBox`
- `LightFixture`

The rejected first Arsenal rack with permanent weapons is intentionally not part of the set. The ceiling also intentionally remains native Unity geometry.

After Unity imports the files, run:

`DEADREACH > Production 0.15 > Build Meshy Bunker Layout`

The builder opens the existing `Bunker_Hub` scene, keeps the accepted Production 0.14 UI/camera/gameplay, creates only `P15_Meshy_Bunker`, scales/places the custom assets, and hides matching prototype visuals only when replacements exist. Missing models therefore do not destroy the working Bunker.

Optional preflight:

`DEADREACH > Production 0.15 > Validate Meshy Bunker Assets`

Expected result when all exports are present: `PASS (18/18 found)`.
