using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Hardens every prepared Meshy Dead City renderer to project-owned URP/Lit materials.
    /// Also recovers an embedded GLB base texture when the imported renderer material is null.
    /// </summary>
    public static class Production16MeshyDeadCityMaterialRepair
    {
        private const string MaterialRoot = Production16MeshyDeadCityGlbBridge.AssetRoot + "/PreparedMaterials";

        private static readonly string[] BaseTextureNames =
        {
            "_BaseMap", "_BaseColorTexture", "_MainTex", "baseColorTexture", "_BaseColorTex"
        };

        private static readonly string[] NormalTextureNames =
        {
            "_BumpMap", "_NormalTexture", "_NormalMap", "normalTexture"
        };

        private static readonly string[] BaseColorNames =
        {
            "_BaseColor", "_Color", "baseColorFactor"
        };

        [MenuItem("DEADREACH/Production 0.16/Repair Meshy Dead City Materials (URP)", priority = 11)]
        public static void Repair()
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                Debug.LogError("DEADREACH 0.16 Dead City material repair aborted: URP/Lit shader not found.");
                return;
            }

            EnsureFolder(MaterialRoot);

            var stems = new List<string>(Production16MeshyDeadCityGlbBridge.Required);
            var optionalPrefabPath = $"{Production16MeshyDeadCityGlbBridge.PreparedRoot}/{Production16MeshyDeadCityGlbBridge.OptionalVehicle}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(optionalPrefabPath) != null)
                stems.Add(Production16MeshyDeadCityGlbBridge.OptionalVehicle);

            var repairedPrefabs = 0;
            var repairedSlots = 0;
            var missing = new List<string>();

            foreach (var stem in stems)
            {
                var prefabPath = $"{Production16MeshyDeadCityGlbBridge.PreparedRoot}/{stem}.prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
                {
                    if (stem != Production16MeshyDeadCityGlbBridge.OptionalVehicle)
                        missing.Add(stem);
                    continue;
                }

                var embeddedTextures = LoadEmbeddedTextures(Production16MeshyDeadCityGlbBridge.ResolveSourcePath(stem));
                var root = PrefabUtility.LoadPrefabContents(prefabPath);
                var changed = false;

                try
                {
                    foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                    {
                        if (renderer == null)
                            continue;

                        var sourceMaterials = renderer.sharedMaterials ?? Array.Empty<Material>();
                        var slotCount = Mathf.Max(sourceMaterials.Length, GetSubMeshCount(renderer));
                        if (slotCount <= 0)
                            slotCount = 1;

                        var targetMaterials = new Material[slotCount];
                        for (var slot = 0; slot < slotCount; slot++)
                        {
                            var source = slot < sourceMaterials.Length ? sourceMaterials[slot] : null;
                            targetMaterials[slot] = CreateOrUpdateUrpMaterial(
                                shader, stem, renderer.name, slot, source, embeddedTextures);
                            repairedSlots++;
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
                    $"DEADREACH 0.16 Meshy Dead City material repair completed for {repairedPrefabs} prefabs / {repairedSlots} renderer slots. " +
                    $"Prepared prefab missing for: {string.Join(", ", missing)}");
                return;
            }

            Debug.Log(
                $"DEADREACH 0.16 Meshy Dead City material repair PASSED: {repairedPrefabs} prepared prefab(s), " +
                $"{repairedSlots} renderer slot(s) forced to URP/Lit.");
        }

        private static Material CreateOrUpdateUrpMaterial(
            Shader shader,
            string stem,
            string rendererName,
            int slot,
            Material source,
            List<Texture> embeddedTextures)
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
            baseTexture ??= PickTexture(embeddedTextures, "basecolor", "base_color", "albedo", "diffuse", "color");
            baseTexture ??= PickBestGenericBaseTexture(embeddedTextures);

            var normalTexture = source != null ? FindTexture(source, NormalTextureNames) : null;
            normalTexture ??= PickTexture(embeddedTextures, "normal");

            var baseColor = source != null ? FindColor(source, BaseColorNames, Color.white) : Color.white;
            if (target.HasProperty("_BaseMap")) target.SetTexture("_BaseMap", baseTexture);
            if (target.HasProperty("_BaseColor")) target.SetColor("_BaseColor", baseTexture != null ? baseColor : GuessFallbackColor(stem));

            if (target.HasProperty("_Metallic")) target.SetFloat("_Metallic", GuessMetallic(stem));
            if (target.HasProperty("_Smoothness")) target.SetFloat("_Smoothness", GuessSmoothness(stem));

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
            if (stem.Contains("Rubble", StringComparison.OrdinalIgnoreCase)) return new Color(0.22f, 0.20f, 0.18f);
            if (stem.Contains("Checkpoint", StringComparison.OrdinalIgnoreCase)) return new Color(0.19f, 0.20f, 0.19f);
            if (stem.Contains("Vehicle", StringComparison.OrdinalIgnoreCase)) return new Color(0.16f, 0.17f, 0.17f);
            return new Color(0.20f, 0.19f, 0.18f);
        }

        private static float GuessMetallic(string stem)
        {
            if (stem.Contains("Vehicle", StringComparison.OrdinalIgnoreCase)) return 0.48f;
            if (stem.Contains("Checkpoint", StringComparison.OrdinalIgnoreCase)) return 0.22f;
            return 0.06f;
        }

        private static float GuessSmoothness(string stem)
        {
            if (stem.Contains("Vehicle", StringComparison.OrdinalIgnoreCase)) return 0.24f;
            return 0.16f;
        }

        private static string SanitizeFileName(string value)
        {
            foreach (var invalid in Path.GetInvalidFileNameChars())
                value = value.Replace(invalid, '_');
            return value.Replace('/', '_').Replace('\\', '_').Replace(':', '_');
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
