using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Production 0.15 Meshy material hardening. Every renderer slot in every prepared bunker
    /// prefab receives a project-owned URP/Lit material. This also repairs glTF imports where the
    /// renderer slot is null by recovering embedded textures directly from the source GLB.
    /// </summary>
    public static class Production15MeshyMaterialRepair
    {
        private const string AssetRoot = "Assets/Deadreach/Art/Production15/Meshy/Bunker";
        private const string PreparedRoot = AssetRoot + "/Prepared";
        private const string MaterialRoot = AssetRoot + "/PreparedMaterials";

        private static readonly string[] Required =
        {
            "Wall_Standard", "Wall_Technical", "Wall_Utility", "Wall_Corner", "Door_Blast",
            "Floor_A", "Pillar_A", "CommandConsole", "HologramTable", "ArsenalRack",
            "WorkshopBench", "SupplyStation", "OperatorBay", "Generator", "Crates",
            "VentUnit", "PowerBox", "LightFixture"
        };

        private static readonly string[] BaseTextureNames =
        {
            "_BaseMap", "_BaseColorTexture", "_MainTex", "baseColorTexture", "_BaseColorTex"
        };

        private static readonly string[] NormalTextureNames =
        {
            "_BumpMap", "_NormalTexture", "_NormalMap", "normalTexture"
        };

        private static readonly string[] EmissionTextureNames =
        {
            "_EmissionMap", "_EmissiveTexture", "emissiveTexture"
        };

        private static readonly string[] OcclusionTextureNames =
        {
            "_OcclusionMap", "_OcclusionTexture", "occlusionTexture"
        };

        private static readonly string[] BaseColorNames =
        {
            "_BaseColor", "_Color", "baseColorFactor"
        };

        [MenuItem("DEADREACH/Production 0.15/Repair Meshy Materials (URP)", priority = 3)]
        public static void Repair()
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                Debug.LogError("DEADREACH 0.15 material repair aborted: URP/Lit shader not found.");
                return;
            }

            EnsureFolder(MaterialRoot);

            var repairedPrefabs = 0;
            var repairedSlots = 0;
            var generatedFallbackSlots = 0;
            var missing = new List<string>();

            foreach (var stem in Required)
            {
                var prefabPath = $"{PreparedRoot}/{stem}.prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
                {
                    missing.Add(stem);
                    continue;
                }

                var sourcePath = ResolveSourcePath(stem);
                var embeddedTextures = LoadEmbeddedTextures(sourcePath);
                var root = PrefabUtility.LoadPrefabContents(prefabPath);
                var changed = false;

                try
                {
                    var renderers = root.GetComponentsInChildren<Renderer>(true);
                    foreach (var renderer in renderers)
                    {
                        if (renderer == null)
                            continue;

                        var sourceMaterials = renderer.sharedMaterials ?? Array.Empty<Material>();
                        var slotCount = Mathf.Max(sourceMaterials.Length, GetSubMeshCount(renderer));
                        if (slotCount <= 0)
                            slotCount = 1;

                        var targetMaterials = new Material[slotCount];
                        for (var i = 0; i < slotCount; i++)
                        {
                            var source = i < sourceMaterials.Length ? sourceMaterials[i] : null;
                            var target = CreateOrUpdateUrpMaterial(
                                shader,
                                stem,
                                renderer.name,
                                i,
                                source,
                                embeddedTextures,
                                source == null);

                            targetMaterials[i] = target;
                            repairedSlots++;
                            if (source == null)
                                generatedFallbackSlots++;
                        }

                        renderer.sharedMaterials = targetMaterials;
                        EditorUtility.SetDirty(renderer);
                        changed = true;
                    }

                    if (changed)
                    {
                        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                        repairedPrefabs++;
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            if (missing.Count > 0)
            {
                Debug.LogWarning(
                    $"DEADREACH 0.15 Meshy material hard repair completed for {repairedPrefabs} prefabs / " +
                    $"{repairedSlots} renderer slots ({generatedFallbackSlots} null slots rebuilt). " +
                    $"Prepared prefab missing for: {string.Join(", ", missing)}");
                return;
            }

            Debug.Log(
                $"DEADREACH 0.15 Meshy material hard repair PASSED: 18/18 prepared prefabs rewritten, " +
                $"{repairedSlots} renderer slots forced to URP/Lit, {generatedFallbackSlots} null slots rebuilt from GLB textures/fallbacks.");
        }

        private static Material CreateOrUpdateUrpMaterial(
            Shader shader,
            string stem,
            string rendererName,
            int slot,
            Material source,
            List<Texture> embeddedTextures,
            bool sourceWasNull)
        {
            var safeRenderer = SanitizeFileName(string.IsNullOrWhiteSpace(rendererName) ? "Renderer" : rendererName);
            var safeName = SanitizeFileName($"{stem}_{safeRenderer}_Slot{slot}_URP");
            var path = $"{MaterialRoot}/{safeName}.mat";
            var target = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (target == null)
            {
                target = new Material(shader) { name = safeName };
                AssetDatabase.CreateAsset(target, path);
            }

            target.shader = shader;

            var baseTexture = source != null ? FindTexture(source, BaseTextureNames) ?? source.mainTexture : null;
            var normalTexture = source != null ? FindTexture(source, NormalTextureNames) : null;
            var emissionTexture = source != null ? FindTexture(source, EmissionTextureNames) : null;
            var occlusionTexture = source != null ? FindTexture(source, OcclusionTextureNames) : null;

            baseTexture ??= PickTexture(embeddedTextures, "basecolor", "base_color", "albedo", "diffuse", "color");
            normalTexture ??= PickTexture(embeddedTextures, "normal");
            emissionTexture ??= PickTexture(embeddedTextures, "emiss", "emission");
            occlusionTexture ??= PickTexture(embeddedTextures, "occlusion", "ao");

            // Meshy GLBs commonly contain a single atlas texture with a generic name. If no semantic
            // match exists, use the first texture that is not clearly a normal/metallic utility map.
            if (baseTexture == null)
                baseTexture = PickBestGenericBaseTexture(embeddedTextures);

            var baseColor = source != null
                ? FindColor(source, BaseColorNames, Color.white)
                : Color.white;

            if (target.HasProperty("_BaseMap"))
                target.SetTexture("_BaseMap", baseTexture);
            if (target.HasProperty("_BaseColor"))
                target.SetColor("_BaseColor", baseColor);

            if (normalTexture != null && target.HasProperty("_BumpMap"))
            {
                target.SetTexture("_BumpMap", normalTexture);
                target.EnableKeyword("_NORMALMAP");
            }
            else
            {
                if (target.HasProperty("_BumpMap")) target.SetTexture("_BumpMap", null);
                target.DisableKeyword("_NORMALMAP");
            }

            if (occlusionTexture != null && target.HasProperty("_OcclusionMap"))
            {
                target.SetTexture("_OcclusionMap", occlusionTexture);
                target.SetFloat("_OcclusionStrength", 1f);
            }
            else if (target.HasProperty("_OcclusionMap"))
            {
                target.SetTexture("_OcclusionMap", null);
            }

            if (target.HasProperty("_Metallic"))
                target.SetFloat("_Metallic", GuessMetallic(stem));
            if (target.HasProperty("_Smoothness"))
                target.SetFloat("_Smoothness", GuessSmoothness(stem));

            if (emissionTexture != null && target.HasProperty("_EmissionMap"))
            {
                target.SetTexture("_EmissionMap", emissionTexture);
                target.SetColor("_EmissionColor", Color.white);
                target.EnableKeyword("_EMISSION");
                target.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            else
            {
                if (target.HasProperty("_EmissionMap")) target.SetTexture("_EmissionMap", null);
                target.DisableKeyword("_EMISSION");
            }

            // Never leave a Meshy renderer with an error/missing shader appearance. If the GLB did
            // not expose a recoverable base texture, use a neutral bunker-metal base instead of magenta.
            if (sourceWasNull && baseTexture == null && target.HasProperty("_BaseColor"))
                target.SetColor("_BaseColor", GuessFallbackColor(stem));

            target.enableInstancing = true;
            EditorUtility.SetDirty(target);
            return target;
        }

        private static int GetSubMeshCount(Renderer renderer)
        {
            if (renderer is SkinnedMeshRenderer skinned && skinned.sharedMesh != null)
                return skinned.sharedMesh.subMeshCount;

            var filter = renderer.GetComponent<MeshFilter>();
            return filter != null && filter.sharedMesh != null ? filter.sharedMesh.subMeshCount : 0;
        }

        private static string ResolveSourcePath(string stem)
        {
            var extensions = new[] { ".glb", ".gltf", ".fbx" };
            foreach (var extension in extensions)
            {
                var path = $"{AssetRoot}/{stem}{extension}";
                var absolute = ToAbsoluteProjectPath(path);
                if (File.Exists(absolute))
                    return path;
            }
            return null;
        }

        private static List<Texture> LoadEmbeddedTextures(string sourcePath)
        {
            var textures = new List<Texture>();
            if (string.IsNullOrWhiteSpace(sourcePath))
                return textures;

            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(sourcePath))
            {
                if (asset is Texture texture && !textures.Contains(texture))
                    textures.Add(texture);
            }
            return textures;
        }

        private static Texture PickTexture(IEnumerable<Texture> textures, params string[] keywords)
        {
            foreach (var texture in textures)
            {
                if (texture == null)
                    continue;
                var name = texture.name ?? string.Empty;
                foreach (var keyword in keywords)
                {
                    if (name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        return texture;
                }
            }
            return null;
        }

        private static Texture PickBestGenericBaseTexture(IEnumerable<Texture> textures)
        {
            Texture first = null;
            foreach (var texture in textures)
            {
                if (texture == null)
                    continue;
                first ??= texture;
                var name = texture.name ?? string.Empty;
                if (name.IndexOf("normal", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    name.IndexOf("metal", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    name.IndexOf("rough", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    name.IndexOf("occlusion", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    name.IndexOf("emiss", StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;
                return texture;
            }
            return first;
        }

        private static Texture FindTexture(Material source, IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                if (!source.HasProperty(name))
                    continue;
                var texture = source.GetTexture(name);
                if (texture != null)
                    return texture;
            }
            return null;
        }

        private static Color FindColor(Material source, IEnumerable<string> names, Color fallback)
        {
            foreach (var name in names)
            {
                if (!source.HasProperty(name))
                    continue;
                var color = source.GetColor(name);
                if (color.maxColorComponent > 0.001f)
                    return color;
            }
            return fallback;
        }

        private static Color GuessFallbackColor(string stem)
        {
            if (stem.Contains("Floor", StringComparison.OrdinalIgnoreCase)) return new Color(0.17f, 0.18f, 0.18f);
            if (stem.Contains("Wall", StringComparison.OrdinalIgnoreCase)) return new Color(0.20f, 0.21f, 0.21f);
            if (stem.Contains("Door", StringComparison.OrdinalIgnoreCase)) return new Color(0.13f, 0.14f, 0.14f);
            return new Color(0.18f, 0.19f, 0.19f);
        }

        private static string ToAbsoluteProjectPath(string assetPath)
        {
            var projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty;
            return Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
        }

        private static string SanitizeFileName(string value)
        {
            foreach (var invalid in Path.GetInvalidFileNameChars())
                value = value.Replace(invalid, '_');
            return value.Replace('/', '_').Replace('\\', '_').Replace(':', '_');
        }

        private static float GuessMetallic(string stem)
        {
            if (stem.Contains("Floor", StringComparison.OrdinalIgnoreCase)) return 0.28f;
            if (stem.Contains("Wall", StringComparison.OrdinalIgnoreCase)) return 0.42f;
            if (stem.Contains("Crate", StringComparison.OrdinalIgnoreCase)) return 0.48f;
            return 0.55f;
        }

        private static float GuessSmoothness(string stem)
        {
            if (stem.Contains("Floor", StringComparison.OrdinalIgnoreCase)) return 0.22f;
            if (stem.Contains("Wall", StringComparison.OrdinalIgnoreCase)) return 0.2f;
            return 0.27f;
        }

        private static void EnsureFolder(string fullPath)
        {
            if (AssetDatabase.IsValidFolder(fullPath))
                return;

            var parts = fullPath.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
