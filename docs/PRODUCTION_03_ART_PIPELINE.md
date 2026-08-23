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

Recommended Animator parameters already driven by DEADREACH:

- `Speed` float
- `IsMoving` bool
- `IsAiming` bool
- `IsDead` bool
- `Hit` trigger

### Infected prefab

Must contain:

- an `Animator`

Recommended Animator parameters:

- `Speed` float
- `Attack` trigger
- `Hit` trigger
- `IsDead` bool

### Primary weapon prefab

Must contain:

- `MuzzleSocket` or `Muzzle`

The production binder attaches this prefab to the survivor weapon socket and forwards its muzzle transform to `HitscanWeapon`, so tracers originate from the real weapon rather than the prototype fallback offset.

## Validation

Unity menu:

`DEADREACH > Production > Validate Asset Catalog`

The validator checks:

- survivor assigned
- survivor Animator
- survivor weapon socket
- infected variants assigned
- infected Animators
- primary weapon assigned
- weapon muzzle socket

Missing assets do not break gameplay. The generated slice keeps prototype renderers as fallback until a valid production prefab exists.

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

## Mobile performance rule

Production models should target a mobile midcore budget rather than console budgets.

Initial target guidance per visible character:

- one skinned mesh where practical
- limited material slots
- texture atlases where practical
- LODs for environment assets
- avoid per-instance material cloning
- avoid unnecessary real-time lights on characters
- baked/static lighting for environment where possible

Actual budgets must be finalized after physical-device profiling.

## Combat VFX improvement in 0.3

Tracer GameObjects are now preallocated in a runtime pool instead of instantiated/destroyed every shot. This removes a known allocation/GC source from automatic fire before final VFX assets are introduced.

## Current generator

`DEADREACH > Build Production Slice 0.3`

The generator creates/updates the production catalog, attaches visual binders and keeps the validated 0.2 weapon/extraction systems.

## Validation gate before PR merge

1. clean Unity compile: 0 errors, ideally 0 warnings
2. `Build Production Slice 0.3` succeeds
3. gameplay still works with an empty catalog using fallback visuals
4. asset catalog menu opens/selects the catalog
5. catalog validator runs without exceptions
6. once real prefabs are assigned: survivor replaces capsule, Animator binds, weapon mounts and muzzle/tracer origin follows the production weapon
7. infected prefabs replace prototype infected while AI/hitboxes remain functional
8. death/extraction/stash/equipment flows have no regression
9. no blocking Console errors

Do not claim real production art is complete until actual licensed/owned character, infected, weapon and environment assets have been integrated and device-profiled.
