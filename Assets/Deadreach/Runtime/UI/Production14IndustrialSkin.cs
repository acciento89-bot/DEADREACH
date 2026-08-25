using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// Production 0.14 UI skin. Genuine external pack sprites are preferred.
    /// The fallback exists only so the project stays usable when the optional
    /// editor-side asset bootstrap could not download the pack.
    /// </summary>
    public static class Production14IndustrialSkin
    {
        public enum PlateKind
        {
            Header,
            Tab,
            TabActive,
            Counter,
            Mission,
            Glass,
            Footer,
            Deploy,
            Tag
        }

        private static readonly Dictionary<PlateKind, Sprite> Cache = new();

        public static Sprite Get(PlateKind kind)
        {
            if (Cache.TryGetValue(kind, out var cached) && cached != null)
                return cached;

            var external = Resources.Load<Sprite>(ExternalResourcePath(kind));
            if (external != null)
            {
                Cache[kind] = external;
                return external;
            }

            var fallback = BuildFallback(kind);
            Cache[kind] = fallback;
            return fallback;
        }

        private static string ExternalResourcePath(PlateKind kind)
        {
            return kind switch
            {
                PlateKind.Header => "Production14/UI/External/HeaderFrame",
                PlateKind.Tab => "Production14/UI/External/TabFrame",
                PlateKind.TabActive => "Production14/UI/External/TabActiveFrame",
                PlateKind.Counter => "Production14/UI/External/CounterFrame",
                PlateKind.Mission => "Production14/UI/External/MissionFrame",
                PlateKind.Glass => "Production14/UI/External/GlassFrame",
                PlateKind.Footer => "Production14/UI/External/FooterFrame",
                PlateKind.Deploy => "Production14/UI/External/DeployFrame",
                _ => "Production14/UI/External/TagFrame"
            };
        }

        private static Sprite BuildFallback(PlateKind kind)
        {
            var color = kind switch
            {
                PlateKind.Mission => new Color32(52, 40, 32, 245),
                PlateKind.Glass => new Color32(15, 39, 44, 228),
                PlateKind.Deploy => new Color32(44, 34, 27, 250),
                _ => new Color32(30, 34, 35, 245)
            };

            const int size = 64;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = $"P14_Fallback_{kind}",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[size * size];
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var edge = x < 3 || y < 3 || x >= size - 3 || y >= size - 3;
                    pixels[y * size + x] = edge ? new Color32(72, 96, 100, 255) : color;
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);

            var sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(12f, 12f, 12f, 12f));
            sprite.name = $"P14_Fallback_{kind}";
            return sprite;
        }
    }
}
