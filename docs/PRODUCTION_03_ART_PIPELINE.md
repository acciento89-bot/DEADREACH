# DEADREACH — Production Art / Presentation 0.3

## Goal

Replace prototype survivor/infected/weapon visuals without touching validated gameplay, hitboxes, movement, combat stats or extraction logic.

## Production Asset Catalog

Unity menu:

`DEADREACH > Production > Create or Select Asset Catalog`

Asset path:

`Assets/Deadreach/Resources/Deadreach/ProductionAssetCatalog.asset`

Assign:

- Survivor Prefab
- one or more Infected Prefabs
- Primary Weapon Prefab
- local position / rotation / scale offsets when required

## Required hierarchy contracts

### Survivor prefab

Must contain:

- an `Animator`
- `WeaponSocket` or `RightHandWeaponSocket`

Animator parameters driven by DEADREACH:

- `Speed` float
- `IsMoving` bool
- `IsAiming` bool
- `IsDead` bool
- `Hit` trigger

### Infected prefab

Must contain:

- an `Animator`

Animator parameters:

- `Speed` float
- `Attack` trigger
- `Hit` trigger
- `IsDead` bool

### Primary weapon prefab

Must contain:

- `MuzzleSocket` or `Muzzle`

The production binder attaches this prefab to the survivor weapon socket and forwards its muzzle transform to `HitscanWeapon`, so tracers originate from the real weapon rather than the prototype fallback offset.

## Runtime binding behavior

`ProductionVisualBinder` is attached to the generated survivor and infected gameplay roots.

At runtime it:

1. loads the production catalog
2. selects the correct production prefab
3. disables only the old prototype renderers
4. keeps gameplay root / CharacterController / Damageable / AI / weapon logic unchanged
5. instantiates the production visual under the gameplay root
6. reconnects `PlayerAnimationDriver` or `InfectedAnimationDriver`
7. mounts the production primary weapon on the survivor weapon socket
8. forwards the real muzzle transform to `HitscanWeapon`

This separation is intentional: art can change without destabilizing the validated gameplay layer.

## Current validation state

### Compile / generator gate — PASSED

Confirmed in real Unity `6000.3.22f1`:

- 0 compiler errors
- `DEADREACH > Build Production Slice 0.3` completes without errors

### Empty-catalog fallback runtime gate — PASSED

Confirmed in real Unity:

- Bunker / Deploy works
- prototype Survivor/Infected remain visible when catalog is empty
- movement / aim / fire work
- combat / weapon loot / extraction / Bunker return work
- catalog creation works
- validator runs without exceptions
- only expected yellow warnings occur for missing production assets

## Selected first production-art source

### Quaternius — Zombie Apocalypse Kit

Selected because it covers the first DEADREACH slice from one coherent art source instead of mixing unrelated styles.

Official pack information:

- 4 playable survivor characters
- 20 animations per playable character
- 4 infected/enemy characters
- rifles / shotgun / SMG / pistol and melee weapons
- city / street / debris / survival props
- FBX / OBJ / glTF / Blend distributions
- original creator license: CC0, including commercial use

Initial DEADREACH subset target:

- one survivor character
- Zombie Basic
- Zombie Chubby
- Zombie Arm
- Zombie Ribcage
- Rifle

License/source tracking is recorded in `docs/THIRD_PARTY_ASSETS.md`.

## Next production integration sequence

1. import only the selected CC0 character + infected + rifle assets
2. create DEADREACH wrapper prefabs around imported models
3. configure humanoid/generic rigs as appropriate
4. create production Animator controllers from supplied animation clips
5. add/fix `WeaponSocket` on survivor wrapper
6. add/fix `MuzzleSocket` on rifle wrapper
7. assign wrappers to `ProductionAssetCatalog`
8. run `Validate Asset Catalog` until missing-asset warnings are gone
9. regenerate Production Slice 0.3
10. validate visual replacement, animation, weapon mounting and tracer origin in Play Mode
11. only then extend the same kit into Dead City environment props

## Mobile performance rule

Production models should target a mobile midcore budget rather than console budgets.

Initial guidance:

- one skinned mesh where practical
- limited material slots
- texture atlases where practical
- LODs for environment assets where scene density justifies them
- avoid per-instance material cloning
- avoid unnecessary real-time lights on characters
- baked/static lighting for environment where possible
- gameplay colliders remain on DEADREACH roots; imported visual meshes should not define gameplay collision

Actual budgets must be finalized after physical-device profiling.

## Combat VFX improvement in 0.3

Tracer GameObjects are preallocated in a runtime pool instead of instantiated/destroyed every shot. This removes a known allocation/GC source from automatic fire before final VFX assets are introduced.

## Current generator

`DEADREACH > Build Production Slice 0.3`

The generator creates/updates the production catalog, attaches visual binders and keeps the validated 0.2 weapon/extraction systems.

## Validation gate before PR merge

1. actual licensed/owned production assets imported
2. asset catalog validates without missing-asset warnings
3. survivor replaces prototype visual and Animator binds
4. production weapon mounts correctly and muzzle/tracer origin follows it
5. infected production prefabs replace prototype visuals while AI/hitboxes remain functional
6. death/extraction/stash/equipment flows have no regression
7. no blocking Console errors
8. physical-device profiling follows before final art lock

Do not claim real production art is complete until actual character, infected, weapon and environment assets have been integrated and device-profiled.
