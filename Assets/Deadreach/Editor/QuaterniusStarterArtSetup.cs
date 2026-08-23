using System;
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
        private const string MaterialRoot = ProductionRoot + "/Materials";
        private const string AtlasPath = SourceRoot + "/Zombie_Atlas.png";
        private const string AtlasMaterialPath = MaterialRoot + "/Quaternius_ZombieAtlas.mat";

        private static readonly string SurvivorSource = SourceRoot + "/Survivor_Sam.gltf";
        private static readonly string RifleSource = SourceRoot + "/Weapon_Rifle.gltf";

        private static readonly string[] InfectedSources =
        {
            SourceRoot + "/Infected_Basic.gltf",
            SourceRoot + "/Infected_Chubby.gltf",
            SourceRoot + "/Infected_Arm.gltf",
            SourceRoot + "/Infected_Ribcage.gltf"
        };

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
            EnsureFolder(ProductionRoot, "Materials");

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

            var atlasMaterial = EnsureAtlasMaterial();
            if (atlasMaterial == null)
                return;

            var survivor = BuildCharacterWrapper(SurvivorSource, "Survivor_Quaternius_Sam", true, atlasMaterial);
            var infected = new GameObject[InfectedSources.Length];
            for (var i = 0; i < InfectedSources.Length; i++)
                infected[i] = BuildCharacterWrapper(InfectedSources[i], $"Infected_Quaternius_{i + 1:00}", false, atlasMaterial);

            var rifle = BuildWeaponWrapper(RifleSource, "Weapon_Quaternius_Rifle", atlasMaterial);

            var catalog = ProductionArtBootstrap.EnsureCatalog();
            catalog.ConfigureAssets(survivor, infected, rifle);
            catalog.ConfigureVisualOffsets(Vector3.zero, Vector3.zero, 1f, Vector3.zero, Vector3.zero, 1f);
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ProductionAssetValidator.Validate(catalog);
            Selection.activeObject = catalog;
            EditorGUIUtility.PingObject(catalog);

            Debug.Log("DEADREACH Quaternius starter art setup complete: explicit URP atlas material assigned, Survivor hand socket resolved and Rifle grip/muzzle wrapper rebuilt. Regenerate Production Slice 0.3 and validate visually.");
        }

        private static Material EnsureAtlasMaterial()
        {
            var atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(AtlasPath);
            if (atlas == null)
            {
                Debug.LogError($"DEADREACH cannot build Quaternius materials because '{AtlasPath}' is missing or not imported as a Texture2D. Rerun the Quaternius installer, wait for Unity import, then retry setup.");
                return null;
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null)
            {
                Debug.LogError("DEADREACH could not find URP/Lit or Standard shader for the Quaternius atlas material.");
                return null;
            }

            var material = AssetDatabase.LoadAssetAtPath<Material>(AtlasMaterialPath);
            if (material == null)
            {
                material = new Material(shader) { name = "Quaternius_ZombieAtlas" };
                AssetDatabase.CreateAsset(material, AtlasMaterialPath);
            }
            else if (material.shader != shader)
            {
                material.shader = shader;
            }

            if (material.HasProperty("_BaseMap"))
                material.SetTexture("_BaseMap", atlas);
            if (material.HasProperty("_MainTex"))
                material.SetTexture("_MainTex", atlas);
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", Color.white);
            if (material.HasProperty("_Color"))
                material.SetColor("_Color", Color.white);
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", 0.12f);
            if (material.HasProperty("_Metallic"))
                material.SetFloat("_Metallic", 0f);

            EditorUtility.SetDirty(material);
            AssetDatabase.SaveAssets();
            return material;
        }

        private static GameObject BuildCharacterWrapper(string sourcePath, string prefabName, bool survivor, Material atlasMaterial)
        {
            ConfigureModelImporter(sourcePath);

            var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            var root = new GameObject(prefabName);
            var model = PrefabUtility.InstantiatePrefab(source) as GameObject ?? UnityEngine.Object.Instantiate(source);
            model.name = "Model";
            model.transform.SetParent(root.transform, false);

            RemoveVisualColliders(model);
            ApplyAtlasMaterial(model, atlasMaterial);

            if (survivor)
                DisableEmbeddedSurvivorWeaponVisuals(model);

            var animator = model.GetComponentInChildren<Animator>(true);
            if (animator == null)
                animator = model.AddComponent<Animator>();

            animator.runtimeAnimatorController = BuildAnimatorController(sourcePath, prefabName, survivor);
            animator.applyRootMotion = false;

            if (survivor)
                CreateSurvivorWeaponSocket(root.transform, model, animator);

            var path = $"{PrefabRoot}/{prefabName}.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject BuildWeaponWrapper(string sourcePath, string prefabName, Material atlasMaterial)
        {
            ConfigureModelImporter(sourcePath);

            var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            var root = new GameObject(prefabName);
            var model = PrefabUtility.InstantiatePrefab(source) as GameObject ?? UnityEngine.Object.Instantiate(source);
            model.name = "Model";
            model.transform.SetParent(root.transform, false);
            RemoveVisualColliders(model);
            ApplyAtlasMaterial(model, atlasMaterial);

            // Quaternius rifle geometry is authored primarily along X. DEADREACH weapons use
            // local +Z as forward, with wrapper origin at the grip/trigger instead of the mesh pivot.
            if (TryGetCombinedRendererBounds(model, out var initialBounds) && initialBounds.size.x > initialBounds.size.z * 1.15f)
                model.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);

            if (TryGetCombinedRendererBounds(model, out var alignedBounds))
            {
                var gripWorld = new Vector3(
                    alignedBounds.center.x,
                    alignedBounds.min.y + alignedBounds.size.y * 0.42f,
                    alignedBounds.min.z + alignedBounds.size.z * 0.38f);

                model.transform.position -= gripWorld - root.transform.position;
            }

            var muzzle = new GameObject("MuzzleSocket").transform;
            muzzle.SetParent(root.transform, false);

            if (TryGetCombinedRendererBounds(model, out var finalBounds))
            {
                var worldMuzzle = new Vector3(finalBounds.center.x, finalBounds.center.y, finalBounds.max.z);
                muzzle.position = worldMuzzle;
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

        private static void ApplyAtlasMaterial(GameObject root, Material atlasMaterial)
        {
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                var slotCount = Mathf.Max(1, renderer.sharedMaterials?.Length ?? 0);
                var materials = new Material[slotCount];
                for (var i = 0; i < materials.Length; i++)
                    materials[i] = atlasMaterial;
                renderer.sharedMaterials = materials;
            }
        }

        private static void ConfigureModelImporter(string sourcePath)
        {
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
                Debug.LogWarning($"DEADREACH found no AnimationClip subassets in '{sourcePath}'. The model will render, but animation setup may still need a dedicated Quaternius animation import/retarget pass.");

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
                var match = clips.FirstOrDefault(clip => clip.name.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0);
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
                Debug.Log($"DEADREACH suppressed {disabled} embedded Quaternius weapon renderer(s) on Survivor Sam.");
            else
                Debug.Log("DEADREACH found no separately named embedded weapon renderers on Survivor Sam. The SingleWeapon source variant remains active.");
        }

        private static bool IsEmbeddedWeaponRenderer(Transform rendererTransform, Transform modelRoot)
        {
            var current = rendererTransform;
            while (current != null)
            {
                var normalized = NormalizeName(current.name);
                if (EmbeddedWeaponTokens.Any(token => normalized.Contains(NormalizeName(token))))
                    return true;

                if (current == modelRoot)
                    break;
                current = current.parent;
            }
            return false;
        }

        private static void CreateSurvivorWeaponSocket(Transform root, GameObject model, Animator animator)
        {
            var hand = ResolveRightHand(model, animator);
            var socket = new GameObject("WeaponSocket").transform;

            if (hand != null)
            {
                socket.SetParent(hand, false);
                socket.localPosition = Vector3.zero;
                socket.localRotation = Quaternion.identity;
                Debug.Log($"DEADREACH WeaponSocket bound to Survivor hand transform '{hand.name}'.");
                return;
            }

            socket.SetParent(root, false);
            if (TryGetCombinedRendererBounds(model, out var bounds))
            {
                var fallbackWorld = new Vector3(
                    bounds.center.x + bounds.extents.x * 0.72f,
                    bounds.min.y + bounds.size.y * 0.56f,
                    bounds.center.z);
                socket.position = fallbackWorld;
            }
            else
            {
                socket.localPosition = new Vector3(0.32f, 0.86f, 0.16f);
            }

            socket.localRotation = Quaternion.identity;
            Debug.LogWarning("DEADREACH could not identify the Quaternius right-hand bone. WeaponSocket uses a geometry-derived hand-height fallback instead of the old root/head placement.");
        }

        private static Transform ResolveRightHand(GameObject model, Animator animator)
        {
            if (animator != null && animator.avatar != null && animator.avatar.isHuman)
            {
                var humanoidHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
                if (humanoidHand != null)
                    return humanoidHand;
            }

            var transforms = model.GetComponentsInChildren<Transform>(true);
            var named = transforms
                .Select(t => new { Transform = t, Score = ScoreRightHandName(t.name) })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Transform)
                .FirstOrDefault();

            if (named != null)
                return named;

            var bones = model.GetComponentsInChildren<SkinnedMeshRenderer>(true)
                .SelectMany(renderer => renderer.bones ?? Array.Empty<Transform>())
                .Where(t => t != null)
                .Distinct()
                .ToArray();

            var candidates = bones.Length > 0 ? bones : transforms;
            if (!TryGetCombinedRendererBounds(model, out var bounds) || candidates.Length == 0)
                return null;

            var expectedHand = new Vector3(
                bounds.center.x + bounds.extents.x * 0.78f,
                bounds.min.y + bounds.size.y * 0.56f,
                bounds.center.z);

            var minimumY = bounds.min.y + bounds.size.y * 0.32f;
            var maximumY = bounds.min.y + bounds.size.y * 0.78f;

            return candidates
                .Where(t => t.position.y >= minimumY && t.position.y <= maximumY)
                .OrderBy(t => Vector3.SqrMagnitude(t.position - expectedHand))
                .FirstOrDefault();
        }

        private static int ScoreRightHandName(string name)
        {
            var normalized = NormalizeName(name);
            if (normalized.Contains("lefthand") || normalized.Contains("handleft") || normalized.Contains("handl") || normalized.Contains("lhand"))
                return -1000;

            if (normalized.Contains("righthand")) return 120;
            if (normalized.Contains("handright")) return 115;
            if (normalized.Contains("handr")) return 105;
            if (normalized.Contains("rhand")) return 100;
            if (normalized.Contains("rightwrist")) return 90;
            if (normalized.Contains("wristright")) return 85;
            if (normalized.Contains("wristr")) return 80;
            return 0;
        }

        private static string NormalizeName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return new string(value
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray());
        }

        private static void RemoveVisualColliders(GameObject root)
        {
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
                UnityEngine.Object.DestroyImmediate(collider);
        }

        private static bool TryGetCombinedRendererBounds(GameObject root, out Bounds bounds)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true)
                .Where(renderer => renderer.enabled)
                .ToArray();

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
