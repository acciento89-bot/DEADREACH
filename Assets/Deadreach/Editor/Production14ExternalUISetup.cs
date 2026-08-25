using System;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Prepares the CC0 Wenrexa "Free UI Minimalism SciFi" art used by Production 0.14.
    ///
    /// Unlike the rejected Devdog experiment, every source here is selected by its authored
    /// role: MainPanel -> main content card, SelectPanel -> compact card, TitlePanel -> strip,
    /// Button -> button. No radial HUD pieces or partial HUD bars are stretched into panels.
    /// </summary>
    public static class Production14ExternalUISetup
    {
        private const string RejectedFolder = "Assets/Deadreach/Resources/Production14/UI/External";
        private const string TargetFolder = "Assets/Deadreach/Resources/Production14/UI/Wenrexa";
        private const string SourceRoot = "https://raw.githubusercontent.com/Bamjr/Delivery-Espacio-space-shooter-game/main/WenrexaAssetsUI_SciFI/PNG/";

        private readonly struct AssetSpec
        {
            public readonly string LocalName;
            public readonly string SourceName;
            public readonly float BorderX;
            public readonly float BorderY;

            public AssetSpec(string localName, string sourceName, float borderX, float borderY)
            {
                LocalName = localName;
                SourceName = sourceName;
                BorderX = borderX;
                BorderY = borderY;
            }
        }

        private static readonly AssetSpec[] Assets =
        {
            // Authored title/panel/button families only. These are not arbitrary HUD fragments.
            new("HeaderFrame.png", "TitlePanel01.png", 0.10f, 0.22f),
            new("TabFrame.png", "Button11.png", 0.16f, 0.30f),
            new("TabActiveFrame.png", "Button12.png", 0.16f, 0.30f),
            new("CounterFrame.png", "SelectPanel01.png", 0.14f, 0.16f),
            new("MissionFrame.png", "MainPanel01.png", 0.10f, 0.10f),
            new("GlassFrame.png", "MainPanel02.png", 0.10f, 0.10f),
            new("FooterFrame.png", "TitlePanel02.png", 0.10f, 0.22f),
            new("DeployFrame.png", "Button07.png", 0.16f, 0.30f),
            new("TagFrame.png", "Button13.png", 0.16f, 0.30f)
        };

        [InitializeOnLoadMethod]
        private static void QueueAutomaticSetup()
        {
            EditorApplication.delayCall += () =>
            {
                CleanupRejectedPack();
                if (!AllAssetsReady())
                    EnsureReady(false);
                else
                    ConfigureAllExisting();
            };
        }

        [MenuItem("DEADREACH/Setup Production 0.14 Wenrexa UI Pack", priority = 8)]
        public static void SetupFromMenu()
        {
            EnsureReady(true);
        }

        public static bool EnsureReady(bool verbose = true)
        {
            CleanupRejectedPack();
            Directory.CreateDirectory(TargetFolder);
            var changed = false;

            try
            {
                using var client = new WebClient();
                client.Headers[HttpRequestHeader.UserAgent] = "DEADREACH-Unity-Editor";

                foreach (var spec in Assets)
                {
                    var assetPath = $"{TargetFolder}/{spec.LocalName}";
                    var fullPath = Path.GetFullPath(assetPath);

                    if (!IsValidPng(fullPath))
                    {
                        client.DownloadFile(SourceRoot + spec.SourceName, fullPath);
                        if (!IsValidPng(fullPath))
                        {
                            File.Delete(fullPath);
                            throw new InvalidDataException($"Downloaded file is not a PNG: {spec.LocalName}");
                        }
                        changed = true;
                    }

                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
                    ConfigureSprite(assetPath, spec.BorderX, spec.BorderY);
                }

                WriteLicenseNotice();
                AssetDatabase.Refresh();

                if (verbose || changed)
                    Debug.Log($"DEADREACH 0.14 Wenrexa UI pack READY: {Assets.Length} CC0 panel/button sprites available.");
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"DEADREACH 0.14 Wenrexa UI pack could not be prepared; safe fallback skin remains active. {exception.Message}");
                return false;
            }
        }

        private static void ConfigureAllExisting()
        {
            foreach (var spec in Assets)
            {
                var path = $"{TargetFolder}/{spec.LocalName}";
                if (File.Exists(Path.GetFullPath(path)))
                    ConfigureSprite(path, spec.BorderX, spec.BorderY);
            }
        }

        private static bool AllAssetsReady()
        {
            foreach (var spec in Assets)
            {
                if (!IsValidPng(Path.GetFullPath($"{TargetFolder}/{spec.LocalName}")))
                    return false;
            }
            return true;
        }

        private static bool IsValidPng(string path)
        {
            if (!File.Exists(path) || new FileInfo(path).Length < 64)
                return false;

            using var stream = File.OpenRead(path);
            var signature = new byte[8];
            if (stream.Read(signature, 0, signature.Length) != signature.Length)
                return false;

            return signature[0] == 0x89 && signature[1] == 0x50 && signature[2] == 0x4E && signature[3] == 0x47 &&
                   signature[4] == 0x0D && signature[5] == 0x0A && signature[6] == 0x1A && signature[7] == 0x0A;
        }

        private static void ConfigureSprite(string assetPath, float borderXFactor, float borderYFactor)
        {
            if (AssetImporter.GetAtPath(assetPath) is not TextureImporter importer)
                return;

            var changed = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }
            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }
            if (!importer.alphaIsTransparency)
            {
                importer.alphaIsTransparency = true;
                changed = true;
            }
            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }
            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear;
                changed = true;
            }

            if (changed)
                importer.SaveAndReimport();

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
                return;

            var maxX = Mathf.Max(1, texture.width / 3);
            var maxY = Mathf.Max(1, texture.height / 3);
            var bx = Mathf.Clamp(Mathf.RoundToInt(texture.width * borderXFactor), 4, maxX);
            var by = Mathf.Clamp(Mathf.RoundToInt(texture.height * borderYFactor), 4, maxY);
            var border = new Vector4(bx, by, bx, by);

            if (importer.spriteBorder != border)
            {
                importer.spriteBorder = border;
                importer.SaveAndReimport();
            }
        }

        private static void CleanupRejectedPack()
        {
            if (AssetDatabase.IsValidFolder(RejectedFolder))
                AssetDatabase.DeleteAsset(RejectedFolder);
        }

        private static void WriteLicenseNotice()
        {
            var path = $"{TargetFolder}/WENREXA_UI_MINIMALISM_SCIFI_CC0.txt";
            if (File.Exists(path))
                return;

            File.WriteAllText(path,
                "Wenrexa — Assets: UI Minimalism SciFi\n" +
                "Original source: https://opengameart.org/content/assets-ui-minimalism-scifi\n" +
                "Original author: Wenrexa\n" +
                "License: Creative Commons Zero v1.0 Universal (CC0)\n" +
                "Mirror used for deterministic editor download of individual PNG files:\n" +
                "https://github.com/Bamjr/Delivery-Espacio-space-shooter-game/tree/main/WenrexaAssetsUI_SciFI/PNG\n\n" +
                "The original OpenGameArt listing explicitly permits commercial and free projects.\n");
            AssetDatabase.ImportAsset(path);
        }
    }
}
