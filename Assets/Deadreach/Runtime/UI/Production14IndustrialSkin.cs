using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.Deadreach.UI
{
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
            if (Cache.TryGetValue(kind, out var sprite) && sprite != null)
                return sprite;

            var spec = kind switch
            {
                PlateKind.Header => new PlateSpec(640, 128, new Color32(38, 43, 44, 250), new Color32(118, 126, 126, 255), new Color32(42, 198, 216, 255), 18, 32, true, false),
                PlateKind.Tab => new PlateSpec(384, 96, new Color32(45, 47, 46, 252), new Color32(116, 115, 109, 255), new Color32(78, 91, 93, 255), 16, 26, true, false),
                PlateKind.TabActive => new PlateSpec(384, 96, new Color32(38, 49, 51, 252), new Color32(150, 162, 161, 255), new Color32(64, 230, 244, 255), 16, 26, true, true),
                PlateKind.Counter => new PlateSpec(256, 112, new Color32(52, 54, 51, 250), new Color32(130, 125, 113, 255), new Color32(64, 214, 229, 255), 14, 24, true, false),
                PlateKind.Mission => new PlateSpec(512, 512, new Color32(54, 43, 36, 246), new Color32(128, 112, 94, 255), new Color32(240, 98, 24, 255), 24, 36, true, false),
                PlateKind.Glass => new PlateSpec(384, 512, new Color32(18, 45, 49, 224), new Color32(81, 123, 128, 255), new Color32(44, 223, 235, 255), 22, 34, true, false),
                PlateKind.Footer => new PlateSpec(768, 96, new Color32(31, 34, 34, 252), new Color32(117, 120, 116, 255), new Color32(53, 204, 221, 255), 18, 30, true, false),
                PlateKind.Deploy => new PlateSpec(384, 96, new Color32(46, 37, 30, 255), new Color32(137, 111, 88, 255), new Color32(241, 91, 24, 255), 16, 28, true, true),
                _ => new PlateSpec(320, 96, new Color32(21, 35, 38, 238), new Color32(88, 105, 108, 255), new Color32(45, 208, 226, 255), 12, 20, false, false)
            };

            sprite = Build(kind.ToString(), spec);
            Cache[kind] = sprite;
            return sprite;
        }

        private static Sprite Build(string name, PlateSpec spec)
        {
            var texture = new Texture2D(spec.Width, spec.Height, TextureFormat.RGBA32, false)
            {
                name = $"P14_{name}_Texture",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[spec.Width * spec.Height];
            for (var y = 0; y < spec.Height; y++)
            {
                for (var x = 0; x < spec.Width; x++)
                {
                    var idx = y * spec.Width + x;
                    var transparent = IsCutCorner(x, y, spec.Width, spec.Height, spec.CornerCut);
                    if (transparent)
                    {
                        pixels[idx] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    var edge = EdgeDistance(x, y, spec.Width, spec.Height);
                    var brushed = ((x * 17 + y * 29 + (x ^ y) * 3) & 31) / 31f;
                    var scan = ((y / 2) & 1) == 0 ? 0.012f : -0.006f;
                    var vertical = Mathf.Lerp(0.86f, 1.06f, y / (float)Mathf.Max(1, spec.Height - 1));
                    var noise = Mathf.Lerp(0.94f, 1.035f, brushed) + scan;

                    var baseColor = Scale(spec.Fill, vertical * noise);
                    if (edge < 3)
                        baseColor = spec.Accent;
                    else if (edge < 8)
                        baseColor = Lerp(spec.Edge, spec.Accent, spec.Glow ? 0.28f : 0.08f);
                    else if (edge < 15)
                        baseColor = Lerp(baseColor, spec.Edge, 0.58f);
                    else if (edge > Mathf.Min(spec.Width, spec.Height) * 0.43f)
                        baseColor = Scale(baseColor, 0.94f);

                    if (spec.Brushed && (x % 37 == 0 || x % 53 == 0))
                        baseColor = Lerp(baseColor, spec.Edge, 0.12f);

                    pixels[idx] = baseColor;
                }
            }

            PaintRivet(pixels, spec.Width, spec.Height, 18, 18, 5, spec.Edge);
            PaintRivet(pixels, spec.Width, spec.Height, spec.Width - 19, 18, 5, spec.Edge);
            PaintRivet(pixels, spec.Width, spec.Height, 18, spec.Height - 19, 5, spec.Edge);
            PaintRivet(pixels, spec.Width, spec.Height, spec.Width - 19, spec.Height - 19, 5, spec.Edge);

            if (spec.Glow)
            {
                var y0 = 7;
                for (var x = spec.CornerCut + 12; x < spec.Width - spec.CornerCut - 12; x++)
                {
                    for (var y = y0; y < y0 + 3; y++)
                        pixels[y * spec.Width + x] = spec.Accent;
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);

            var border = Mathf.Clamp(spec.Border, 10, Mathf.Min(spec.Width, spec.Height) / 2 - 1);
            var sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, spec.Width, spec.Height),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(border, border, border, border));
            sprite.name = $"P14_{name}";
            return sprite;
        }

        private static bool IsCutCorner(int x, int y, int width, int height, int cut)
        {
            if (cut <= 0)
                return false;

            if (x < cut && y < cut && x + y < cut)
                return true;
            if (x >= width - cut && y < cut && (width - 1 - x) + y < cut)
                return true;
            if (x < cut && y >= height - cut && x + (height - 1 - y) < cut)
                return true;
            if (x >= width - cut && y >= height - cut && (width - 1 - x) + (height - 1 - y) < cut)
                return true;
            return false;
        }

        private static int EdgeDistance(int x, int y, int width, int height)
        {
            return Mathf.Min(Mathf.Min(x, width - 1 - x), Mathf.Min(y, height - 1 - y));
        }

        private static Color32 Scale(Color32 c, float f)
        {
            return new Color32(
                (byte)Mathf.Clamp(Mathf.RoundToInt(c.r * f), 0, 255),
                (byte)Mathf.Clamp(Mathf.RoundToInt(c.g * f), 0, 255),
                (byte)Mathf.Clamp(Mathf.RoundToInt(c.b * f), 0, 255),
                c.a);
        }

        private static Color32 Lerp(Color32 a, Color32 b, float t)
        {
            return new Color32(
                (byte)Mathf.RoundToInt(Mathf.Lerp(a.r, b.r, t)),
                (byte)Mathf.RoundToInt(Mathf.Lerp(a.g, b.g, t)),
                (byte)Mathf.RoundToInt(Mathf.Lerp(a.b, b.b, t)),
                (byte)Mathf.RoundToInt(Mathf.Lerp(a.a, b.a, t)));
        }

        private static void PaintRivet(Color32[] pixels, int width, int height, int cx, int cy, int radius, Color32 metal)
        {
            for (var y = -radius; y <= radius; y++)
            {
                for (var x = -radius; x <= radius; x++)
                {
                    var px = cx + x;
                    var py = cy + y;
                    if (px < 0 || px >= width || py < 0 || py >= height)
                        continue;

                    var d = Mathf.Sqrt(x * x + y * y);
                    if (d > radius)
                        continue;

                    var light = Mathf.Clamp01((radius - d) / radius);
                    var shade = x + y < 0 ? 1.18f : 0.72f;
                    pixels[py * width + px] = Scale(metal, Mathf.Lerp(0.76f, shade, light));
                }
            }
        }

        private readonly struct PlateSpec
        {
            public readonly int Width;
            public readonly int Height;
            public readonly Color32 Fill;
            public readonly Color32 Edge;
            public readonly Color32 Accent;
            public readonly int CornerCut;
            public readonly int Border;
            public readonly bool Brushed;
            public readonly bool Glow;

            public PlateSpec(int width, int height, Color32 fill, Color32 edge, Color32 accent, int cornerCut, int border, bool brushed, bool glow)
            {
                Width = width;
                Height = height;
                Fill = fill;
                Edge = edge;
                Accent = accent;
                CornerCut = cornerCut;
                Border = border;
                Brushed = brushed;
                Glow = glow;
            }
        }
    }
}
