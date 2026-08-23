using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Kamilunavo.Deadreach.Presentation;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production05OperatorArtSetup
    {
        private const string SourceRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/glTF";
        private const string ProductionRoot = "Assets/Deadreach/Art/Production";
        private const string PrefabRoot = ProductionRoot + "/Prefabs";
        private const string ControllerRoot = ProductionRoot + "/Controllers";
        private const string AtlasMaterialPath = ProductionRoot + "/Materials/Quaternius_ZombieAtlas.mat";
        private const string SamPrefabPath = PrefabRoot + "/Survivor_Quaternius_Sam.prefab";
        private const string ScoutSourcePath = SourceRoot + "/Survivor_Lis.gltf";
        private const string WardenSourcePath = SourceRoot + "/Survivor_Matt.gltf";
        private const string ScoutPrefabPath = PrefabRoot + "/Survivor_Quaternius_Lis.prefab";
        private const string WardenPrefabPath = PrefabRoot + "/Survivor_Quaternius_Matt.prefab";

        private const string ScoutUrl = "https://raw.githubusercontent.com/agentkaerf/FreeModels/main/Zombie%20Apocalypse%20Kit%20-%20March%202024/Characters/glTF/Characters_Lis_SingleWeapon.gltf";
        private const string WardenUrl = "https://raw.githubusercontent.com/agentkaerf/FreeModels/main/Zombie%20Apocalypse%20Kit%20-%20March%202024/Characters/glTF/Characters_Matt_SingleWeapon.gltf";

        private static readonly Regex ExternalUriRegex = new(
            "\\\"uri\\\"\\s*:\\s*\\\"(?<uri>(?:\\\\.|[^\\\"])*)\\\"",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [MenuItem("DEADREACH/Production/Repair 0.5 Operator Art", priority = 24)]
        public static void RepairMenu()
        {
            EnsureOperatorAssetsReady();
        }

        public static bool EnsureOperatorAssetsReady()
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/glTF"));
            EnsureFolder(ProductionRoot, "Prefabs");
            EnsureFolder(ProductionRoot, "Controllers");

            if (!EnsureSource(ScoutSourcePath, ScoutUrl, 100000))
                return false;
            if (!EnsureSource(WardenSourcePath, WardenUrl, 100000))
                return false;

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            var samPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SamPrefabPath);
            var atlasMaterial = AssetDatabase.LoadAssetAtPath<Material>(AtlasMaterialPath);
            if (samPrefab == null || atlasMaterial == null)
            {
                Debug.LogError(
                    "DEADREACH 0.5 operator setup requires the validated Sam production prefab and Quaternius atlas material. " +
                    "Run DEADREACH > Production > Setup Quaternius Starter Art once if either baseline asset is missing.");
                return false;
            }

            var samFallbackController = samPrefab.GetComponentInChildren<Animator>(true)?.runtimeAnimatorController;
            var scoutController = BuildAnimatorController(ScoutSourcePath, "Survivor_Quaternius_Lis", samFallbackController);
            var wardenController = BuildAnimatorController(WardenSourcePath, "Survivor_Quaternius_Matt", samFallbackController);

            var scoutPrefab = BuildOperatorWrapper(ScoutSourcePath, ScoutPrefabPath, "Survivor_Quaternius_Lis", atlasMaterial, scoutController);
            var wardenPrefab = BuildOperatorWrapper(WardenSourcePath, WardenPrefabPath, "Survivor_Quaternius_Matt", atlasMaterial, wardenController);
            if (scoutPrefab == null || wardenPrefab == null)
                return false;

            var catalog = ProductionArtBootstrap.EnsureCatalog();
            catalog.ConfigureSurvivorOperators(samPrefab, scoutPrefab, wardenPrefab);
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("DEADREACH 0.5 operator art READY: SAM=Sam, RAVEN=Lis, BRIGGS=Matt. All use artist-authored SingleWeapon rigs and operator-specific animation controllers when source clips are available.");
            return true;
        }

        private static bool EnsureSource(string assetPath, string sourceUrl, long minimumBytes)
        {
            var absolutePath = AssetPathToAbsolute(assetPath);

            try
            {
                var needsDownload = !File.Exists(absolutePath) || new FileInfo(absolutePath).Length < minimumBytes;
                if (needsDownload)
                {
                    Debug.Log($"DEADREACH downloading missing Quaternius operator art: {Path.GetFileName(assetPath)}");
                    DownloadFile(sourceUrl, absolutePath);
                }

                var sourceFile = new FileInfo(absolutePath);
                if (!sourceFile.Exists || sourceFile.Length < minimumBytes)
                    throw new IOException($"Operator glTF is unexpectedly small ({(sourceFile.Exists ? sourceFile.Length : 0)} bytes).");

                // IMPORTANT: repair is deliberately run for BOTH newly downloaded and already existing
                // files. A previous failed glTFast import may have left Lis/Matt on disk with unresolved
                // relative image/buffer URIs; simply seeing a large file is therefore not a valid gate.
                RepairExternalDependencies(absolutePath, sourceUrl);

                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

                var imported = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (imported == null)
                {
                    // One explicit second pass after dependencies have entered the AssetDatabase makes
                    // glTFast deterministic when a dependency was created during the same Editor call.
                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                    imported = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                }

                if (imported == null)
                    throw new InvalidOperationException("glTFast still exposes no GameObject after dependency repair and forced reimport.");

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"DEADREACH could not prepare operator asset '{assetPath}': {exception.Message}");
                return false;
            }
        }

        private static void RepairExternalDependencies(string absoluteGltfPath, string sourceUrl)
        {
            var json = File.ReadAllText(absoluteGltfPath);
            if (string.IsNullOrWhiteSpace(json) || json.TrimStart()[0] != '{')
                throw new InvalidDataException("Downloaded operator file is not valid glTF JSON.");

            var baseUri = new Uri(sourceUrl, UriKind.Absolute);
            var localDirectory = Path.GetDirectoryName(absoluteGltfPath)
                ?? throw new InvalidOperationException("Could not resolve local glTF directory.");

            var replacements = new Dictionary<string, string>(StringComparer.Ordinal);
            var localSources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var dependencyNames = new List<string>();

            foreach (Match match in ExternalUriRegex.Matches(json))
            {
                var encodedUri = match.Groups["uri"].Value;
                if (string.IsNullOrWhiteSpace(encodedUri) || encodedUri.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                    continue;

                var dependencyUri = DecodeJsonUri(encodedUri);
                if (string.IsNullOrWhiteSpace(dependencyUri) || dependencyUri.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                    continue;

                var remoteUri = Uri.TryCreate(dependencyUri, UriKind.Absolute, out var absoluteDependency)
                    ? absoluteDependency
                    : new Uri(baseUri, dependencyUri);

                var remotePath = Uri.UnescapeDataString(remoteUri.AbsolutePath);
                var fileName = Path.GetFileName(remotePath);
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new InvalidDataException($"glTF dependency URI has no file name: '{dependencyUri}'.");

                if (localSources.TryGetValue(fileName, out var existingRemote) &&
                    !string.Equals(existingRemote, remoteUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException(
                        $"Two glTF dependencies collapse to the same local file '{fileName}'. " +
                        $"Sources: '{existingRemote}' and '{remoteUri.AbsoluteUri}'.");
                }

                localSources[fileName] = remoteUri.AbsoluteUri;
                var localDependency = Path.Combine(localDirectory, fileName);
                if (!File.Exists(localDependency) || new FileInfo(localDependency).Length <= 16)
                {
                    Debug.Log($"DEADREACH downloading glTF dependency '{fileName}' for {Path.GetFileName(absoluteGltfPath)}");
                    DownloadFile(remoteUri.AbsoluteUri, localDependency);
                }

                if (!File.Exists(localDependency) || new FileInfo(localDependency).Length <= 16)
                    throw new IOException($"glTF dependency '{fileName}' is missing or empty after download.");

                replacements[encodedUri] = fileName;
                if (!dependencyNames.Contains(fileName))
                    dependencyNames.Add(fileName);
            }

            var repaired = ExternalUriRegex.Replace(json, match =>
            {
                var original = match.Groups["uri"].Value;
                return replacements.TryGetValue(original, out var localName)
                    ? $"\"uri\" : \"{EscapeJson(localName)}\""
                    : match.Value;
            });

            if (!string.Equals(repaired, json, StringComparison.Ordinal))
                File.WriteAllText(absoluteGltfPath, repaired, new UTF8Encoding(false));

            if (dependencyNames.Count > 0)
            {
                Debug.Log(
                    $"DEADREACH repaired {Path.GetFileName(absoluteGltfPath)} dependency URIs -> " +
                    string.Join(", ", dependencyNames));
            }
            else
            {
                Debug.Log($"DEADREACH {Path.GetFileName(absoluteGltfPath)} uses only embedded glTF payloads; no external dependency download was required.");
            }
        }

        private static void DownloadFile(string url, string destination)
        {
            var directory = Path.GetDirectoryName(destination);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            var temporary = destination + ".deadreach-download";
            if (File.Exists(temporary))
                File.Delete(temporary);

            try
            {
                using var client = new WebClient();
                client.Headers[HttpRequestHeader.UserAgent] = "DEADREACH-Unity-Operator-Art-Bootstrap";
                client.DownloadFile(url, temporary);

                if (!File.Exists(temporary) || new FileInfo(temporary).Length == 0)
                    throw new IOException($"Download returned an empty file from '{url}'.");

                File.Copy(temporary, destination, true);
            }
            finally
            {
                if (File.Exists(temporary))
                    File.Delete(temporary);
            }
        }

        private static string DecodeJsonUri(string value)
        {
            // glTF URIs produced by Blender are ordinary JSON strings. We only need the escapes that
            // are legal/useful in paths here; data: URIs are filtered before this method is called.
            return value
                .Replace("\\/", "/")
                .Replace("\\\\", "\\")
                .Replace("\\u0020", " ");
        }

        private static string EscapeJson(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static RuntimeAnimatorController BuildAnimatorController(string sourcePath, string assetName, RuntimeAnimatorController fallback)
        {
            var clips = AssetDatabase.LoadAllAssetsAtPath(sourcePath)
                .OfType<AnimationClip>()
                .Where(clip => !clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (clips.Length == 0)
            {
                Debug.LogWarning($"DEADREACH found no animation clips in '{sourcePath}'. Using the validated Sam controller as fallback for {assetName}.");
                return fallback;
            }

            var controllerPath = $"{ControllerRoot}/{assetName}.controller";
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) != null)
                AssetDatabase.DeleteAsset(controllerPath);

            var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
            controller.AddParameter("IsAiming", AnimatorControllerParameterType.Bool);
            controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);

            var stateMachine = controller.layers[0].stateMachine;
            var idleClip = FindClip(clips, "idle") ?? clips[0];
            var moveClip = FindClip(clips, "run", "walk", "move") ?? idleClip;
            var hitClip = FindClip(clips, "hit", "damage", "hurt") ?? idleClip;
            var deathClip = FindClip(clips, "death", "die", "dead") ?? idleClip;

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

        private static GameObject BuildOperatorWrapper(
            string sourcePath,
            string prefabPath,
            string prefabName,
            Material atlasMaterial,
            RuntimeAnimatorController controller)
        {
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            if (source == null)
            {
                Debug.LogError($"DEADREACH operator source is not imported: {sourcePath}");
                return null;
            }

            var root = new GameObject(prefabName);
            try
            {
                var model = PrefabUtility.InstantiatePrefab(source) as GameObject ?? UnityEngine.Object.Instantiate(source);
                model.name = "Model";
                model.transform.SetParent(root.transform, false);

                foreach (var collider in model.GetComponentsInChildren<Collider>(true))
                    UnityEngine.Object.DestroyImmediate(collider);

                foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
                {
                    var slotCount = Mathf.Max(1, renderer.sharedMaterials?.Length ?? 0);
                    var materials = new Material[slotCount];
                    for (var i = 0; i < materials.Length; i++)
                        materials[i] = atlasMaterial;
                    renderer.sharedMaterials = materials;
                }

                var animator = model.GetComponentInChildren<Animator>(true);
                if (animator == null)
                    animator = model.AddComponent<Animator>();
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;

                var prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                if (prefab == null)
                    Debug.LogError($"DEADREACH failed to create operator prefab at {prefabPath}.");
                return prefab;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static string AssetPathToAbsolute(string assetPath)
        {
            var relative = assetPath.Replace("Assets/", string.Empty).Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(Application.dataPath, relative);
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
