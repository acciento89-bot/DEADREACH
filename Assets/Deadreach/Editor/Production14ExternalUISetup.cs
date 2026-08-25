using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Downloads the selected original MIT-licensed Devdog Sci-Fi UI sprites into Resources.
    /// The upstream repository stores binary art in Git LFS, therefore media.githubusercontent.com
    /// is used rather than raw.githubusercontent.com (which would return only LFS pointer text).
    /// </summary>
    public static class Production14ExternalUISetup
    {
        private const string TargetFolder = "Assets/Deadreach/Resources/Production14/UI/External";
        private const string MediaRoot = "https://media.githubusercontent.com/media/devdogio/sci-fi-ui/master/Assets/Devdog/SciFiDesign/FirstVersion/Images/Hud/";

        private readonly struct AssetSpec
        {
            public readonly string LocalName;
            public readonly string RemotePath;
            public readonly Vector4 Border;

            public AssetSpec(string localName, string remotePath, Vector4 border)
            {
                LocalName = localName;
                RemotePath = remotePath;
                Border = border;
            }
        }

        private static readonly AssetSpec[] Assets =
        {
            new("HeaderFrame.png", "TopCenterRound.png", new Vector4(24, 18, 24, 18)),
            new("TabFrame.png", "Notification/Notification_BG_1.png", new Vector4(18, 14, 18, 14)),
            new("TabActiveFrame.png", "Notification/Notification_BG_Filled_1.png", new Vector4(18, 14, 18, 14)),
            new("CounterFrame.png", "Radial/HexagonBorder1Thick.png", new Vector4(18, 18, 18, 18)),
            new("MissionFrame.png", "Notification/Notification_BG_Filled_1.png", new Vector4(18, 18, 18, 18)),
            new("GlassFrame.png", "Notification/Notification_BG_1.png", new Vector4(18, 18, 18, 18)),
            new("FooterFrame.png", "LowerBarLeftHalfThick.png", new Vector4(22, 12, 22, 12)),
            new("DeployFrame.png", "Notification/Notification_BG_Filled_1.png", new Vector4(18, 14, 18, 14)),
            new("TagFrame.png", "LowerBarLeftHalf.png", new Vector4(18, 10, 18, 10))
        };

        [InitializeOnLoadMethod]
        private static void QueueAutomaticSetup()
        {
            EditorApplication.delayCall += () =>
            {
                if (!AllAssetsReady())
                    EnsureReady(false);
            };
        }

        [MenuItem("DEADREACH/Setup Production 0.14 Real UI Pack", priority = 8)]
        public static void SetupFromMenu()
        {
            EnsureReady(true);
        }

        public static bool EnsureReady(bool verbose = true)
        {
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
                        client.DownloadFile(MediaRoot + spec.RemotePath, fullPath);
                        if (!IsValidPng(fullPath))
                        {
                            File.Delete(fullPath);
                            throw new InvalidDataException($"Downloaded file is not a PNG: {spec.LocalName}");
                        }
                        changed = true;
                    }

                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
                    ConfigureSprite(assetPath, spec.Border);
                }

                WriteLicenseNotice();
                AssetDatabase.Refresh();

                if (verbose || changed)
                    Debug.Log($"DEADREACH 0.14 real UI pack READY: {Assets.Length} MIT Devdog Sci-Fi UI sprites available.");
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"DEADREACH 0.14 real UI pack could not be prepared; fallback skin remains active. {exception.Message}");
                return false;
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
            if (!File.Exists(path))
                return false;

            var info = new FileInfo(path);
            if (info.Length < 32)
                return false;

            using var stream = File.OpenRead(path);
            var signature = new byte[8];
            if (stream.Read(signature, 0, signature.Length) != signature.Length)
                return false;

            return signature[0] == 0x89 && signature[1] == 0x50 && signature[2] == 0x4E && signature[3] == 0x47 &&
                   signature[4] == 0x0D && signature[5] == 0x0A && signature[6] == 0x1A && signature[7] == 0x0A;
        }

        private static void ConfigureSprite(string assetPath, Vector4 border)
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
            if (importer.alphaIsTransparency == false)
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
            if (importer.spriteBorder != border)
            {
                importer.spriteBorder = border;
                changed = true;
            }

            if (changed)
                importer.SaveAndReimport();
        }

        private static void WriteLicenseNotice()
        {
            var path = $"{TargetFolder}/DEVDOG_SCI_FI_UI_LICENSE.txt";
            if (File.Exists(path))
                return;

            File.WriteAllText(path,
                "Devdog Sci-Fi UI\n" +
                "Source: https://github.com/devdogio/sci-fi-ui\n" +
                "Copyright (c) 2019 Devdog\n\n" +
                "MIT License\n\n" +
                "Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\n" +
                "The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\n" +
                "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.\n");
            AssetDatabase.ImportAsset(path);
        }
    }
}
