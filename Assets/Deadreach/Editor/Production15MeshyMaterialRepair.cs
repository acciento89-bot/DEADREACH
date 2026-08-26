using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Converts Meshy/glTF material instances used by the prepared 0.15 bunker prefabs to URP/Lit.
    /// Meshy GLBs can import with a glTF shader that renders magenta in the current URP project.
    /// This pass keeps the imported textures/colors but writes stable project-owned materials.
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
            var missing = new List<string>();
            var cache = new Dictionary<string, Material>(StringComparer.OrdinalIgnoreCase);

            foreach (var stem in Required)
            {
                var prefabPath = $"{PreparedRoot}/{stem}.prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
                {
                    missing.Add(stem);
                    continue;
                }

                var root = PrefabUtility.LoadPrefabContents(prefabPath);
                var changed = false;

                try
                {
                    foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                    {
                        var sourceMaterials = renderer.sharedMaterials;
                        var targetMaterials = new Material[sourceMaterials.Length];

                        for (var i = 0; i < sourceMaterials.Length; i++)
                        {
                            var source = sourceMaterials[i];
                            if (source == null)
                            {
                                targetMaterials[i] = null;
                                continue;
                            }

                            var key = BuildMaterialKey(stem, source, i);
                            if (!cache.TryGetValue(key, out var target))
                            {
                                target = CreateOrUpdateUrpMaterial(shader, stem, source, key);
                                cache[key] = target;
                            }

                            targetMaterials[i] = target;
                            if (source != target)
                            {
                                changed = true;
                                repairedSlots++;
                            }
                        }

                        renderer.sharedMaterials = targetMaterials;
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
                    $"DEADREACH 0.15 Meshy material repair completed for {repairedPrefabs} prefabs / {repairedSlots} material slots. " +
                    $"Prepared prefab missing for: {string.Join(", ", missing)}");
                return;
            }

            Debug.Log(
                $"DEADREACH 0.15 Meshy material repair PASSED: 18/18 prepared prefabs checked, " +
                $"{repairedPrefabs} prefabs updated, {repairedSlots} material slots normalized to URP/Lit.");
        }

        private static Material CreateOrUpdateUrpMaterial(Shader shader, string stem, Material source, string key)
        {
            var safeName = SanitizeFileName(key);
            var path = $"{MaterialRoot}/{safeName}.mat";
            var target = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (target == null)
            {
                target = new Material(shader) { name = safeName };
                AssetDatabase.CreateAsset(target, path);
            }
            else
            {
                target.shader = shader;
            }

            var baseTexture = FindTexture(source, BaseTextureNames) ?? source.mainTexture;
            var normalTexture = FindTexture(source, NormalTextureNames);
            var emissionTexture = FindTexture(source, EmissionTextureNames);
            var occlusionTexture = FindTexture(source, OcclusionTextureNames);
            var baseColor = FindColor(source, BaseColorNames, Color.white);

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
                target.DisableKeyword("_NORMALMAP");
            }

            if (occlusionTexture != null && target.HasProperty("_OcclusionMap"))
            {
                target.SetTexture("_OcclusionMap", occlusionTexture);
                target.SetFloat("_OcclusionStrength", 1f);
            }

            // glTF packs metallic/roughness differently than URP's metallic/smoothness map.
            // Until a texture repack pass is warranted, conservative scalar values look correct
            // and avoid feeding an incompatible packed texture into URP/Lit.
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
                target.DisableKeyword("_EMISSION");
            }

            target.enableInstancing = true;
            EditorUtility.SetDirty(target);
            return target;
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

        private static string BuildMaterialKey(string stem, Material source, int slot)
        {
            var sourcePath = AssetDatabase.GetAssetPath(source);
            var sourceName = string.IsNullOrWhiteSpace(source.name) ? $"Slot_{slot}" : source.name;
            if (!string.IsNullOrWhiteSpace(sourcePath))
                return $"{stem}_{sourceName}_{Math.Abs(sourcePath.GetHashCode())}";
            return $"{stem}_{sourceName}_{slot}";
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
