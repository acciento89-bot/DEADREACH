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
    /// <summary>
    /// Atomic 0.5 operator-art bootstrap.
    ///
    /// The first implementation mixed filesystem mutation with AssetDatabase.DeleteAsset / Refresh
    /// calls. Unity could therefore observe a half-deleted graph (legacy Lis/Matt nested prefab still
    /// alive while its glTF source GUID was already gone) and glTFast was also asked to import a new
    /// source before all of its external dependencies existed.
    ///
    /// V2 prepares the complete filesystem first, performs one synchronous refresh/import pass, then
    /// builds fully unpacked wrapper prefabs. Unity never sees the broken intermediate state.
    /// </summary>
    internal static class Production05OperatorArtSetupV2
    {
        private const string SourceRoot = "Assets/Deadreach/ThirdParty/Quaternius/ZombieApocalypseKit/glTF";
        private const string ProductionRoot = "Assets/Deadreach/Art/Production";
        private const string PrefabRoot = ProductionRoot + "/Prefabs";
        private const string ControllerRoot = ProductionRoot + "/Controllers";
        private const string MaterialRoot = ProductionRoot + "/Materials";

        private const string AtlasMaterialPath = MaterialRoot + "/Quaternius_ZombieAtlas.mat";
        private const string SamPrefabPath = PrefabRoot + "/Survivor_Quaternius_Sam.prefab";

        // Production 0.5 mapping:
        // SAM    -> existing validated Sam SingleWeapon / Pistol
        // RAVEN  -> Shaun SingleWeapon / SMG
        // BRIGGS -> Matt full export / Rifle only
        private const string ScoutSourcePath = SourceRoot + "/Survivor_Shaun.gltf";
        private const string WardenSourcePath = SourceRoot + "/Survivor_Matt_Full.gltf";
        private const string ScoutPrefabPath = PrefabRoot + "/Survivor_Quaternius_Shaun.prefab";
        private const string WardenPrefabPath = PrefabRoot + "/Survivor_Quaternius_Matt.prefab";
        private const string ScoutControllerPath = ControllerRoot + "/Survivor_Quaternius_Shaun.controller";
        private const string WardenControllerPath = ControllerRoot + "/Survivor_Quaternius_Matt.controller";

        private const string LegacyLisSourcePath = SourceRoot + "/Survivor_Lis.gltf";
        private const string LegacyMattSingleSourcePath = SourceRoot + "/Survivor_Matt.gltf";
        private const string LegacyLisPrefabPath = PrefabRoot + "/Survivor_Quaternius_Lis.prefab";
        private const string LegacyLisControllerPath = ControllerRoot + "/Survivor_Quaternius_Lis.controller";

        private const string ScoutUrl = "https://raw.githubusercontent.com/agentkaerf/FreeModels/main/Zombie%20Apocalypse%20Kit%20-%20March%202024/Characters/glTF/Characters_Shaun_SingleWeapon.gltf";
        private const string WardenUrl = "https://raw.githubusercontent.com/agentkaerf/FreeModels/main/Zombie%20Apocalypse%20Kit%20-%20March%202024/Characters/glTF/Characters_Matt.gltf";

        private static readonly Regex ExternalUriRegex = new(
            "\\\"uri\\\"\\s*:\\s*\\\"(?<uri>(?:\\\\.|[^\\\"])*)\\\"",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly string[] WeaponTokens =
        {
            "axe", "guitar", "knife", "pistol", "rifle", "shotgun", "smg", "submachine",
            "spear", "woodenbat", "baseballbat", "gun", "firearm"
        };

        public static bool EnsureOperatorAssetsReady()
        {
            try
            {
                EnsureDirectoriesOnDisk();

                // Query failed importer state BEFORE touching its meta file. LoadAssetAtPath does not
                // force a new import here; it only tells us whether the last glTFast import produced
                // a usable GameObject.
                var resetScoutImporter = NeedsImporterReset(ScoutSourcePath);
                var resetWardenImporter = NeedsImporterReset(WardenSourcePath);

                // IMPORTANT: no AssetDatabase mutation/refresh occurs until the entire filesystem is
                // coherent. This is what prevents the Missing Nested Prefab errors seen in real Unity.
                CleanupGeneratedAndLegacyFiles(resetScoutImporter, resetWardenImporter);

                PrepareSourceFile(ScoutSourcePath, ScoutUrl, 100000);
                PrepareSourceFile(WardenSourcePath, WardenUrl, 100000);

                // Both glTFs and every external dependency now exist. Let glTFast see them once.
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

                var scoutSource = AssetDatabase.LoadAssetAtPath<GameObject>(ScoutSourcePath);
                var wardenSource = AssetDatabase.LoadAssetAtPath<GameObject>(WardenSourcePath);
                if (scoutSource == null || wardenSource == null)
                {
                    var failed = string.Join(
                        ", ",
                        new[]
                        {
                            scoutSource == null ? Path.GetFileName(ScoutSourcePath) : null,
                            wardenSource == null ? Path.GetFileName(WardenSourcePath) : null
                        }.Where(value => !string.IsNullOrEmpty(value)));

                    Debug.LogError(
                        $"DEADREACH 0.5 atomic operator import failed for: {failed}. " +
                        "The sources/dependencies were prepared before the single glTFast pass; inspect the FIRST glTFast importer detail above this message.");
                    return false;
                }

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
                var scoutController = BuildAnimatorController(
                    ScoutSourcePath,
                    ScoutControllerPath,
                    "Survivor_Quaternius_Shaun",
                    samFallbackController);
                var wardenController = BuildAnimatorController(
                    WardenSourcePath,
                    WardenControllerPath,
                    "Survivor_Quaternius_Matt",
                    samFallbackController);

                var scoutPrefab = BuildOperatorWrapper(
                    scoutSource,
                    ScoutPrefabPath,
                    "Survivor_Quaternius_Shaun",
                    atlasMaterial,
                    scoutController,
                    "smg");

                var wardenPrefab = BuildOperatorWrapper(
                    wardenSource,
                    WardenPrefabPath,
                    "Survivor_Quaternius_Matt",
                    atlasMaterial,
                    wardenController,
                    "rifle");

                if (scoutPrefab == null || wardenPrefab == null)
                    return false;

                var catalog = ProductionArtBootstrap.EnsureCatalog();
                catalog.ConfigureSurvivorOperators(samPrefab, scoutPrefab, wardenPrefab);
                EditorUtility.SetDirty(catalog);
                AssetDatabase.SaveAssets();

                Debug.Log(
                    "DEADREACH 0.5 operator art READY (atomic V2): SAM=Sam/Pistol, RAVEN=Shaun/SMG, " +
                    "BRIGGS=Matt/Rifle. Wrapper prefabs are fully unpacked and cannot retain a nested glTF prefab parent.");
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"DEADREACH 0.5 atomic operator-art setup failed: {exception}");
                return false;
            }
        }

        private static void EnsureDirectoriesOnDisk()
        {
            Directory.CreateDirectory(AssetPathToAbsolute(SourceRoot));
            Directory.CreateDirectory(AssetPathToAbsolute(PrefabRoot));
            Directory.CreateDirectory(AssetPathToAbsolute(ControllerRoot));
            Directory.CreateDirectory(AssetPathToAbsolute(MaterialRoot));
        }

        private static bool NeedsImporterReset(string assetPath)
        {
            var absolute = AssetPathToAbsolute(assetPath);
            if (!File.Exists(absolute) || !File.Exists(absolute + ".meta"))
                return false;

            return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) == null;
        }

        private static void CleanupGeneratedAndLegacyFiles(bool resetScoutImporter, bool resetWardenImporter)
        {
            // Generated wrappers/controllers are cheap and deterministic. Remove them on disk before
            // touching any source GUID, so Unity can never import a wrapper whose nested parent vanished.
            DeleteFileAndMetaSilently(LegacyLisPrefabPath);
            DeleteFileAndMetaSilently(LegacyLisControllerPath);
            DeleteFileAndMetaSilently(ScoutPrefabPath);
            DeleteFileAndMetaSilently(ScoutControllerPath);
            DeleteFileAndMetaSilently(WardenPrefabPath);
            DeleteFileAndMetaSilently(WardenControllerPath);

            // Obsolete sources from the failed Lis/Matt-SingleWeapon experiment.
            DeleteFileAndMetaSilently(LegacyLisSourcePath);
            DeleteFileAndMetaSilently(LegacyMattSingleSourcePath);

            // A failed ScriptedImporter can leave a bad .meta/importer cache. Reset only failed new
            // sources; successful imports keep a stable GUID on subsequent builds.
            if (resetScoutImporter)
                DeleteMetaSilently(ScoutSourcePath);
            if (resetWardenImporter)
                DeleteMetaSilently(WardenSourcePath);

            Debug.Log(
                $"DEADREACH 0.5 filesystem batch prepared. " +
                $"reset Shaun importer={resetScoutImporter}, reset Matt importer={resetWardenImporter}. " +
                "No AssetDatabase refresh has occurred yet.");
        }

        private static void DeleteFileAndMetaSilently(string assetPath)
        {
            var absolute = AssetPathToAbsolute(assetPath);
            if (File.Exists(absolute))
                File.Delete(absolute);
            if (File.Exists(absolute + ".meta"))
                File.Delete(absolute + ".meta");
        }

        private static void DeleteMetaSilently(string assetPath)
        {
            var meta = AssetPathToAbsolute(assetPath) + ".meta";
            if (File.Exists(meta))
                File.Delete(meta);
        }

        private static void PrepareSourceFile(string assetPath, string sourceUrl, long minimumBytes)
        {
            var absolutePath = AssetPathToAbsolute(assetPath);
            var needsDownload = !File.Exists(absolutePath) || new FileInfo(absolutePath).Length < minimumBytes;
            if (needsDownload)
            {
                Debug.Log($"DEADREACH downloading Quaternius operator source: {Path.GetFileName(assetPath)}");
                DownloadFile(sourceUrl, absolutePath);
            }

            var sourceFile = new FileInfo(absolutePath);
            if (!sourceFile.Exists || sourceFile.Length < minimumBytes)
                throw new IOException($"Operator glTF '{assetPath}' is unexpectedly small ({(sourceFile.Exists ? sourceFile.Length : 0)} bytes).");

            RepairExternalDependenciesBeforeImport(absolutePath, sourceUrl);
        }

        private static void RepairExternalDependenciesBeforeImport(string absoluteGltfPath, string sourceUrl)
        {
            var json = File.ReadAllText(absoluteGltfPath);
            if (string.IsNullOrWhiteSpace(json) || json.TrimStart()[0] != '{')
                throw new InvalidDataException($"'{Path.GetFileName(absoluteGltfPath)}' is not valid glTF JSON.");

            var sourceBaseUri = new Uri(sourceUrl, UriKind.Absolute);
            var localDirectory = Path.GetDirectoryName(absoluteGltfPath)
                ?? throw new InvalidOperationException("Could not resolve local glTF directory.");

            var replacements = new Dictionary<string, string>(StringComparer.Ordinal);
            var dependencyOrigins = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
                    : new Uri(sourceBaseUri, dependencyUri);

                var fileName = Path.GetFileName(Uri.UnescapeDataString(remoteUri.AbsolutePath));
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new InvalidDataException($"glTF dependency URI has no filename: '{dependencyUri}'.");

                if (dependencyOrigins.TryGetValue(fileName, out var existingOrigin) &&
                    !string.Equals(existingOrigin, remoteUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException(
                        $"Two glTF dependencies collapse to local '{fileName}': '{existingOrigin}' and '{remoteUri.AbsoluteUri}'.");
                }

                dependencyOrigins[fileName] = remoteUri.AbsoluteUri;
                var localDependency = Path.Combine(localDirectory, fileName);
                if (!File.Exists(localDependency) || new FileInfo(localDependency).Length <= 16)
                {
                    Debug.Log($"DEADREACH downloading glTF dependency '{fileName}' before Unity import.");
                    DownloadFile(remoteUri.AbsoluteUri, localDependency);
                }

                if (!File.Exists(localDependency) || new FileInfo(localDependency).Length <= 16)
                    throw new IOException($"glTF dependency '{fileName}' is missing/empty after download.");

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

            Debug.Log(
                dependencyNames.Count > 0
                    ? $"DEADREACH prepared {Path.GetFileName(absoluteGltfPath)} dependencies BEFORE import: {string.Join(", ", dependencyNames)}"
                    : $"DEADREACH {Path.GetFileName(absoluteGltfPath)} contains no non-data external dependency URIs.");
        }

        private static RuntimeAnimatorController BuildAnimatorController(
            string sourcePath,
            string controllerPath,
            string assetName,
            RuntimeAnimatorController fallback)
        {
            var clips = AssetDatabase.LoadAllAssetsAtPath(sourcePath)
                .OfType<AnimationClip>()
                .Where(clip => !clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (clips.Length == 0)
            {
                Debug.LogWarning($"DEADREACH found no clips in '{sourcePath}'. Using validated Sam controller for {assetName}.");
                return fallback;
            }

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
            GameObject source,
            string prefabPath,
            string prefabName,
            Material atlasMaterial,
            RuntimeAnimatorController controller,
            string preferredWeaponToken)
        {
            var root = new GameObject(prefabName);
            try
            {
                GameObject model;
                var prefabInstance = PrefabUtility.InstantiatePrefab(source) as GameObject;
                if (prefabInstance != null)
                {
                    model = prefabInstance;
                    if (PrefabUtility.IsPartOfPrefabInstance(model))
                    {
                        PrefabUtility.UnpackPrefabInstance(
                            model,
                            PrefabUnpackMode.Completely,
                            InteractionMode.AutomatedAction);
                    }
                }
                else
                {
                    model = UnityEngine.Object.Instantiate(source);
                }

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

                KeepOnlyPreferredEmbeddedWeapon(model, preferredWeaponToken);

                var animator = model.GetComponentInChildren<Animator>(true) ?? model.AddComponent<Animator>();
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;

                var prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                if (prefab == null)
                    Debug.LogError($"DEADREACH failed to create unpacked operator prefab at {prefabPath}.");
                return prefab;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void KeepOnlyPreferredEmbeddedWeapon(GameObject model, string preferredWeaponToken)
        {
            var preferred = NormalizeName(preferredWeaponToken);
            var kept = 0;
            var hidden = 0;

            foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
            {
                if (!TryGetWeaponIdentity(renderer.transform, model.transform, out var identity))
                    continue;

                var keep = !string.IsNullOrEmpty(preferred) && identity.Contains(preferred);
                renderer.enabled = keep;
                if (keep) kept++;
                else hidden++;
            }

            if (kept == 0)
                throw new InvalidOperationException($"Operator '{model.name}' contains no embedded '{preferredWeaponToken}' renderer.");

            Debug.Log($"DEADREACH '{model.name}': kept {kept} {preferredWeaponToken} renderer(s), hid {hidden} other weapon renderer(s).");
        }

        private static bool TryGetWeaponIdentity(Transform rendererTransform, Transform modelRoot, out string identity)
        {
            var current = rendererTransform;
            while (current != null)
            {
                var normalized = NormalizeName(current.name);
                foreach (var token in WeaponTokens)
                {
                    if (normalized.Contains(NormalizeName(token)))
                    {
                        identity = normalized;
                        return true;
                    }
                }

                if (current == modelRoot)
                    break;
                current = current.parent;
            }

            identity = string.Empty;
            return false;
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
                client.Headers[HttpRequestHeader.UserAgent] = "DEADREACH-Unity-Operator-Art-Bootstrap-V2";
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
            return value
                .Replace("\\/", "/")
                .Replace("\\\\", "\\")
                .Replace("\\u0020", " ");
        }

        private static string EscapeJson(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string NormalizeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return new string(value
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray());
        }

        private static string AssetPathToAbsolute(string assetPath)
        {
            var relative = assetPath.Replace("Assets/", string.Empty).Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(Application.dataPath, relative);
        }
    }
}
