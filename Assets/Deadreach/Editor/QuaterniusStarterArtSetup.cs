using System;
using System.IO;
using System.Linq;
using Kamilunavo.Deadreach.Presentation;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class QuaterniusStarterArtSetup
    {
        private const string SourceRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/glTF";
        private const string ProductionRoot = "Assets/Deadreach/Art/Production";
        private const string PrefabRoot = ProductionRoot + "/Prefabs";
        private const string ControllerRoot = ProductionRoot + "/Controllers";

        private static readonly string SurvivorSource = SourceRoot + "/Survivor_Sam.gltf";
        private static readonly string RifleSource = SourceRoot + "/Weapon_Rifle.gltf";

        private static readonly string[] InfectedSources =
        {
            SourceRoot + "/Infected_Basic.gltf",
            SourceRoot + "/Infected_Chubby.gltf",
            SourceRoot + "/Infected_Arm.gltf",
            SourceRoot + "/Infected_Ribcage.gltf"
        };

        // Quaternius character files can contain weapon presentation meshes. DEADREACH owns
        // equipment presentation itself, so those embedded visuals must stay hidden and only
        // the weapon mounted through ProductionVisualBinder may be visible.
        private static readonly string[] EmbeddedWeaponTokens =
        {
            "rifle",
            "shotgun",
            "pistol",
            "smg",
            "submachine",
            "axe",
            "knife",
            "spear",
            "woodenbat",
            "baseballbat",
            "guitar"
        };

        [MenuItem("DEADREACH/Production/Setup Quaternius Starter Art", priority = 20)]
        public static void Setup()
        {
            EnsureFolder("Assets/Deadreach/Art", "Production");
            EnsureFolder(ProductionRoot, "Prefabs");
            EnsureFolder(ProductionRoot, "Controllers");

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            var missing = new[] { SurvivorSource, RifleSource }
                .Concat(InfectedSources)
                .Where(path => AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
                .ToArray();

            if (missing.Length > 0)
            {
                Debug.LogError(
                    "DEADREACH Quaternius starter glTF files are missing or have not finished importing. " +
                    "Run tools/install-quaternius-zombie-kit.ps1, wait for Unity glTFast import to finish, then retry. Missing:\n" +
                    string.Join("\n", missing));
                return;
            }

            var survivor = BuildCharacterWrapper(SurvivorSource, "Survivor_Quaternius_Sam", true, 0);
            var infected = new GameObject[InfectedSources.Length];
            for (var i = 0; i < InfectedSources.Length; i++)
                infected[i] = BuildCharacterWrapper(InfectedSources[i], $"Infected_Quaternius_{i + 1:00}", false, i);

            var rifle = BuildWeaponWrapper(RifleSource, "Weapon_Quaternius_Rifle");

            var catalog = ProductionArtBootstrap.EnsureCatalog();
            catalog.ConfigureAssets(survivor, infected, rifle);
            catalog.ConfigureVisualOffsets(Vector3.zero, Vector3.zero, 1f, Vector3.zero, Vector3.zero, 1f);
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ProductionAssetValidator.Validate(catalog);
            Selection.activeObject = catalog;
            EditorGUIUtility.PingObject(catalog);

            Debug.Log("DEADREACH Quaternius glTF starter art setup complete. Regenerate with DEADREACH > Build Production Slice 0.3 and test visual scale/orientation in Play Mode.");
        }

        private static GameObject BuildCharacterWrapper(string sourcePath, string prefabName, bool survivor, int variantIndex)
        {
            ConfigureModelImporter(sourcePath);

            var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            var root = new GameObject(prefabName);
            var model = PrefabUtility.InstantiatePrefab(source) as GameObject ?? UnityEngine.Object.Instantiate(source);
            model.name = "Model";
            model.transform.SetParent(root.transform, false);

            RemoveVisualColliders(model);

            if (survivor)
                DisableEmbeddedSurvivorWeaponVisuals(model);

            var animator = model.GetComponentInChildren<Animator>(true);
            if (animator == null)
                animator = model.AddComponent<Animator>();

            animator.runtimeAnimatorController = BuildAnimatorController(sourcePath, prefabName, survivor);
            animator.applyRootMotion = false;

            if (survivor)
                CreateSurvivorWeaponSocket(root.transform, model.transform);

            var path = $"{PrefabRoot}/{prefabName}.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject BuildWeaponWrapper(string sourcePath, string prefabName)
        {
            ConfigureModelImporter(sourcePath);

            var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            var root = new GameObject(prefabName);
            var model = PrefabUtility.InstantiatePrefab(source) as GameObject ?? UnityEngine.Object.Instantiate(source);
            model.name = "Model";
            model.transform.SetParent(root.transform, false);
            RemoveVisualColliders(model);

            var muzzle = new GameObject("MuzzleSocket").transform;
            muzzle.SetParent(root.transform, false);

            if (TryGetCombinedRendererBounds(model, out var bounds))
            {
                var useX = bounds.size.x >= bounds.size.z;
                muzzle.position = useX
                    ? new Vector3(bounds.max.x, bounds.center.y, bounds.center.z)
                    : new Vector3(bounds.center.x, bounds.center.y, bounds.max.z);
            }
            else
            {
                muzzle.localPosition = new Vector3(0f, 0f, 0.65f);
            }

            var path = $"{PrefabRoot}/{prefabName}.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static void ConfigureModelImporter(string sourcePath)
        {
            // FBX uses ModelImporter; glTFast uses its own scripted importer.
            // Keep the old ModelImporter tuning path so the setup remains format-tolerant.
            if (AssetImporter.GetAtPath(sourcePath) is not ModelImporter importer)
                return;

            var changed = false;
            if (!importer.importAnimation)
            {
                importer.importAnimation = true;
                changed = true;
            }

            if (changed)
                importer.SaveAndReimport();
        }

        private static RuntimeAnimatorController BuildAnimatorController(string sourcePath, string assetName, bool survivor)
        {
            var controllerPath = $"{ControllerRoot}/{assetName}.controller";
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) != null)
                AssetDatabase.DeleteAsset(controllerPath);

            var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
            controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
            if (survivor)
                controller.AddParameter("IsAiming", AnimatorControllerParameterType.Bool);
            else
                controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

            var clips = AssetDatabase.LoadAllAssetsAtPath(sourcePath)
                .OfType<AnimationClip>()
                .Where(clip => !clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (clips.Length == 0)
            {
                Debug.LogWarning($"DEADREACH found no AnimationClip subassets in '{sourcePath}'. The model will still render, but animation setup may need a dedicated Quaternius animation import/retarget pass.");
            }

            var stateMachine = controller.layers[0].stateMachine;
            var idleClip = FindClip(clips, "idle") ?? clips.FirstOrDefault();
            var moveClip = FindClip(clips, "run", "walk", "move") ?? idleClip;
            var hitClip = FindClip(clips, "hit", "damage", "hurt") ?? idleClip;
            var deathClip = FindClip(clips, "death", "die", "dead") ?? idleClip;
            var attackClip = FindClip(clips, "attack", "bite", "punch", "melee") ?? moveClip;

            var idle = stateMachine.AddState("Idle");
            idle.motion = idleClip;
            stateMachine.defaultState = idle;

            var move = stateMachine.AddState("Move");
            move.motion = moveClip;

            var toMove = idle.AddTransition(move);
            toMove.hasExitTime = false;
            toMove.duration = 0.08f;
            toMove.AddCondition(AnimatorConditionMode.If, 0f, "IsMoving");

            var toIdle = move.AddTransition(idle);
            toIdle.hasExitTime = false;
            toIdle.duration = 0.08f;
            toIdle.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsMoving");

            var hit = stateMachine.AddState("Hit");
            hit.motion = hitClip;
            var hitTransition = stateMachine.AddAnyStateTransition(hit);
            hitTransition.hasExitTime = false;
            hitTransition.duration = 0.04f;
            hitTransition.AddCondition(AnimatorConditionMode.If, 0f, "Hit");
            var hitReturn = hit.AddTransition(idle);
            hitReturn.hasExitTime = true;
            hitReturn.exitTime = 0.88f;
            hitReturn.duration = 0.06f;

            if (!survivor)
            {
                var attack = stateMachine.AddState("Attack");
                attack.motion = attackClip;
                var attackTransition = stateMachine.AddAnyStateTransition(attack);
                attackTransition.hasExitTime = false;
                attackTransition.duration = 0.04f;
                attackTransition.AddCondition(AnimatorConditionMode.If, 0f, "Attack");
                var attackReturn = attack.AddTransition(idle);
                attackReturn.hasExitTime = true;
                attackReturn.exitTime = 0.9f;
                attackReturn.duration = 0.06f;
            }

            var death = stateMachine.AddState("Death");
            death.motion = deathClip;
            var deathTransition = stateMachine.AddAnyStateTransition(death);
            deathTransition.hasExitTime = false;
            deathTransition.duration = 0.08f;
            deathTransition.AddCondition(AnimatorConditionMode.If, 0f, "IsDead");

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            return controller;
        }

        private static AnimationClip FindClip(AnimationClip[] clips, params string[] tokens)
        {
            foreach (var token in tokens)
            {
                var match = clips.FirstOrDefault(clip =>
                    clip.name.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0);
                if (match != null)
                    return match;
            }

            return null;
        }

        private static void DisableEmbeddedSurvivorWeaponVisuals(GameObject model)
        {
            var disabled = 0;
            foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
            {
                if (!IsEmbeddedWeaponRenderer(renderer.transform, model.transform))
                    continue;

                renderer.enabled = false;
                disabled++;
            }

            if (disabled > 0)
            {
                Debug.Log($"DEADREACH suppressed {disabled} embedded Quaternius weapon renderer(s) on Survivor Sam. Equipped weapon presentation remains owned by DEADREACH.");
            }
            else
            {
                Debug.Log("DEADREACH found no separately named embedded weapon renderers on Survivor Sam. The SingleWeapon source variant is still used to avoid the multi-weapon character export.");
            }
        }

        private static bool IsEmbeddedWeaponRenderer(Transform rendererTransform, Transform modelRoot)
        {
            // Inspect the renderer and a few meaningful ancestors, but do not use the generic
            // word 'weapon': the source asset itself is named SingleWeapon and that would hide
            // the entire character hierarchy.
            var current = rendererTransform;
            while (current != null)
            {
                var normalized = current.name
                    .Replace(" ", string.Empty)
                    .Replace("_", string.Empty)
                    .Replace("-", string.Empty)
                    .ToLowerInvariant();

                if (EmbeddedWeaponTokens.Any(token => normalized.Contains(token)))
                    return true;

                if (current == modelRoot)
                    break;

                current = current.parent;
            }

            return false;
        }

        private static void CreateSurvivorWeaponSocket(Transform root, Transform model)
        {
            var hand = FindTransform(model, "righthand", "right_hand", "hand_r", "hand.r", "r_hand", "handright");
            var socket = new GameObject("WeaponSocket").transform;

            if (hand != null)
            {
                socket.SetParent(hand, false);
                socket.localPosition = Vector3.zero;
                socket.localRotation = Quaternion.identity;
            }
            else
            {
                socket.SetParent(root, false);
                socket.localPosition = new Vector3(0.28f, 1.15f, 0.38f);
                socket.localRotation = Quaternion.identity;
                Debug.LogWarning("DEADREACH could not identify the Quaternius right-hand bone automatically. WeaponSocket received a temporary root-space fallback and may need visual adjustment.");
            }
        }

        private static Transform FindTransform(Transform root, params string[] tokens)
        {
            var normalized = root.name.Replace(" ", string.Empty).ToLowerInvariant();
            if (tokens.Any(token => normalized.Contains(token.Replace(" ", string.Empty).ToLowerInvariant())))
                return root;

            for (var i = 0; i < root.childCount; i++)
            {
                var found = FindTransform(root.GetChild(i), tokens);
                if (found != null)
                    return found;
            }

            return null;
        }

        private static void RemoveVisualColliders(GameObject root)
        {
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
                UnityEngine.Object.DestroyImmediate(collider);
        }

        private static bool TryGetCombinedRendererBounds(GameObject root, out Bounds bounds)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                bounds = default;
                return false;
            }

            bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);
            return true;
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
