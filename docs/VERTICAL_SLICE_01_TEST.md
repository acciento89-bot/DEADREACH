# DEADREACH — Vertical Slice 0.1 Test Runbook

This runbook exists so the first playable slice can be reproduced after a fresh clone and after chat handoffs.

## Required editor

- Unity 6000.3.22f1
- Open the repository root as the Unity project.
- Allow Package Manager imports and script compilation to finish.

## First open

The editor tooling should automatically:

1. Apply product identity and mobile project settings.
2. Create/assign `Assets/Deadreach/Settings/Deadreach_URP.asset` if no DEADREACH URP asset exists yet.
3. Keep the project on the Universal Render Pipeline.

If needed, both operations can be re-run manually from:

- `DEADREACH > Project > Apply Production Settings`
- `DEADREACH > Project > Ensure URP Pipeline`

## Generate the playable slice

Use:

`DEADREACH > Build Vertical Slice 0.1`

This creates and saves:

`Assets/Deadreach/Scenes/DeadCity_VerticalSlice.unity`

The generated scene is automatically added to Build Settings.

## Expected playable loop

1. Player starts at the south end of the Dead City test street.
2. Move with WASD in Editor or the left half of the touchscreen on device.
3. Aim/fire with the mouse in Editor or the right half of the touchscreen on device.
4. Infected enemies chase and attack the player when in range.
5. Killing infected drops Scrap.
6. Additional Scrap caches exist in the level.
7. Picked-up Scrap is `carried`, not yet permanently secured.
8. Reach the green extraction zone at the north end of the level.
9. Remain inside the zone long enough to complete extraction.
10. Carried Scrap is transferred to the persistent profile and the extraction streak increases.
11. If the player dies before extraction, carried Scrap is lost and the current extraction streak resets.

## Persistence

Development profile file:

`Application.persistentDataPath/deadreach-profile.json`

Currently persisted:

- secured Scrap
- successful extractions
- failed runs
- current extraction streak
- best extraction streak

## Graphics presets

Runtime presets exist for:

- Performance
- Balanced
- Ultra

A recommended preset is selected from available device memory/GPU information on first run. The player-facing graphics settings screen is not built yet.

## Current controls

### Desktop / Editor

- WASD: move
- mouse position: aim
- left mouse button: automatic fire

### Touch

- left half drag: virtual movement stick
- right half touch/drag: aim and automatic fire

The current controls are functional gameplay scaffolding. Production joystick visuals, aim feedback, haptics and accessibility options are still pending.

## Acceptance checks before merging the foundation branch

- Project opens without C# compile errors.
- URP pipeline is active; generated URP materials are not pink.
- Scene generator completes without exceptions.
- Player movement is camera-relative and collision-safe.
- Camera follows without jitter.
- Mouse and touch aiming rotate the survivor toward the aim point.
- Weapon damages infected but not the survivor.
- Infected can damage and kill the survivor.
- Enemy death creates collectible Scrap.
- Extraction only completes after the hold duration.
- Successful extraction persists Scrap after stopping and restarting Play Mode.
- Death discards carried Scrap and resets the streak.
- No blocking errors appear in Console during a complete run.

## Important quality note

The generated environment uses a deliberate development palette and procedural primitive geometry to validate systems and composition. It is **not** the final visual target. The production art pass must replace environment/player/enemy meshes and materials while keeping the gameplay architecture intact.
