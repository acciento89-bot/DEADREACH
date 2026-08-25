using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// Production 0.14 command-center skin.
    ///
    /// A coherent CC0 Wenrexa panel/button family is preferred when the editor setup has
    /// prepared it. The calm graphite generator remains only as an offline-safe fallback.
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

            var sprite = Build(kind, GetSpec(kind));
            Cache[kind] = sprite;
            return sprite;
        }

        private static string ExternalResourcePath(PlateKind kind)
        {
            return kind switch
            {
                PlateKind.Header => "Production14/UI/Wenrexa/HeaderFrame",
                PlateKind.Tab => "Production14/UI/Wenrexa/TabFrame",
                PlateKind.TabActive => "Production14/UI/Wenrexa/TabActiveFrame",
                PlateKind.Counter => "Production14/UI/Wenrexa/CounterFrame",
                PlateKind.Mission => "Production14/UI/Wenrexa/MissionFrame",
                PlateKind.Glass => "Production14/UI/Wenrexa/GlassFrame",
                PlateKind.Footer => "Production14/UI/Wenrexa/FooterFrame",
                PlateKind.Deploy => "Production14/UI/Wenrexa/DeployFrame",
                _ => "Production14/UI/Wenrexa/TagFrame"
            };
        }

        private static PanelSpec GetSpec(PlateKind kind)
        {
            return kind switch
            {
                PlateKind.Header => new PanelSpec(
                    256, 80,
                    new Color32(18, 24, 27, 252), new Color32(11, 16, 18, 252),
                    new Color32(58, 70, 75, 255), new Color32(47, 211, 225, 255),
                    7, 14, AccentMode.Top),

                PlateKind.Tab => new PanelSpec(
                    256, 72,
                    new Color32(22, 27, 29, 252), new Color32(14, 19, 21, 252),
                    new Color32(70, 80, 83, 255), new Color32(48, 102, 110, 255),
                    6, 13, AccentMode.None),

                PlateKind.TabActive => new PanelSpec(
                    256, 72,
                    new Color32(23, 35, 39, 252), new Color32(14, 23, 26, 252),
                    new Color32(82, 105, 110, 255), new Color32(55, 226, 238, 255),
                    6, 13, AccentMode.Bottom),

                PlateKind.Counter => new PanelSpec(
                    144, 96,
                    new Color32(20, 27, 29, 250), new Color32(12, 18, 20, 250),
                    new Color32(65, 80, 84, 255), new Color32(53, 211, 225, 255),
                    6, 14, AccentMode.Left),

                PlateKind.Mission => new PanelSpec(
                    256, 256,
                    new Color32(31, 27, 24, 248), new Color32(18, 17, 16, 248),
                    new Color32(82, 71, 62, 255), new Color32(242, 103, 31, 255),
                    8, 16, AccentMode.LeftTop),

                PlateKind.Glass => new PanelSpec(
                    256, 256,
                    new Color32(13, 31, 34, 236), new Color32(8, 20, 22, 236),
                    new Color32(52, 83, 88, 255), new Color32(50, 215, 229, 255),
                    8, 16, AccentMode.Top),

                PlateKind.Footer => new PanelSpec(
                    256, 72,
                    new Color32(17, 23, 25, 252), new Color32(10, 15, 17, 252),
                    new Color32(58, 71, 75, 255), new Color32(52, 207, 222, 255),
                    6, 13, AccentMode.Left),

                PlateKind.Deploy => new PanelSpec(
                    256, 72,
                    new Color32(32, 25, 21, 255), new Color32(20, 17, 15, 255),
                    new Color32(101, 75, 59, 255), new Color32(242, 101, 31, 255),
                    6, 13, AccentMode.TopBottom),

                _ => new PanelSpec(
                    160, 56,
                    new Color32(15, 27, 29, 244), new Color32(10, 19, 21, 244),
                    new Color32(53, 79, 84, 255), new Color32(50, 207, 222, 255),
                    5, 11, AccentMode.Left)
            };
        }

        private static Sprite Build(PlateKind kind, PanelSpec spec)
        {
            var texture = new Texture2D(spec.Width, spec.Height, TextureFormat.RGBA32, false)
            {
                name = $"P14_Clean_{kind}_Texture",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var pixels = new Color32[spec.Width * spec.Height];
            var innerEdge = Darken(spec.Edge, 0.48f);

            for (var y = 0; y < spec.Height; y++)
            {
                var fy = y / (float)Mathf.Max(1, spec.Height - 1);
                var fill = Lerp(spec.Bottom, spec.Top, fy);

                for (var x = 0; x < spec.Width; x++)
                {
                    var index = y * spec.Width + x;
                    if (IsCutCorner(x, y, spec.Width, spec.Height, spec.CornerCut))
                    {
                        pixels[index] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    var edge = EdgeDistance(x, y, spec.Width, spec.Height);
                    var color = fill;
                    var variation = (((x * 11) + (y * 17)) & 15) / 255f;
                    color = Add(color, variation * 0.35f);

                    if (edge == 0)
                        color = innerEdge;
                    else if (edge <= 2)
                        color = spec.Edge;
                    else if (edge <= 5)
                        color = Lerp(color, spec.Edge, 0.20f);

                    pixels[index] = color;
                }
            }

            PaintAccent(pixels, spec);
            PaintCornerTicks(pixels, spec);

            texture.SetPixels32(pixels);
            texture.Apply(false, true);

            var maxBorder = Mathf.Min(spec.Width, spec.Height) / 2 - 1;
            var border = Mathf.Clamp(spec.Border, 8, maxBorder);
            var sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, spec.Width, spec.Height),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(border, border, border, border));
            sprite.name = $"P14_Clean_{kind}";
            return sprite;
        }

        private static void PaintAccent(Color32[] pixels, PanelSpec spec)
        {
            var inset = spec.CornerCut + 8;
            var shortLength = Mathf.Max(18, spec.Width / 5);

            if (spec.AccentMode is AccentMode.Top or AccentMode.LeftTop or AccentMode.TopBottom)
                PaintHorizontal(pixels, spec.Width, spec.Height, inset, Mathf.Min(spec.Width - inset, inset + shortLength), spec.Height - 4, 2, spec.Accent);

            if (spec.AccentMode is AccentMode.Bottom or AccentMode.TopBottom)
                PaintHorizontal(pixels, spec.Width, spec.Height, inset, spec.Width - inset, 2, 2, spec.Accent);

            if (spec.AccentMode is AccentMode.Left or AccentMode.LeftTop)
                PaintVertical(pixels, spec.Width, spec.Height, 3, inset, spec.Height - inset, 2, spec.Accent);
        }

        private static void PaintCornerTicks(Color32[] pixels, PanelSpec spec)
        {
            var muted = Lerp(spec.Edge, spec.Accent, 0.24f);
            var inset = spec.CornerCut + 4;
            const int length = 10;

            PaintHorizontal(pixels, spec.Width, spec.Height, inset, Mathf.Min(spec.Width - inset, inset + length), 4, 1, muted);
            PaintHorizontal(pixels, spec.Width, spec.Height, Mathf.Max(inset, spec.Width - inset - length), spec.Width - inset, spec.Height - 5, 1, muted);
        }

        private static void PaintHorizontal(Color32[] pixels, int width, int height, int x0, int x1, int y, int thickness, Color32 color)
        {
            for (var py = Mathf.Max(0, y); py < Mathf.Min(height, y + thickness); py++)
            {
                for (var x = Mathf.Max(0, x0); x < Mathf.Min(width, x1); x++)
                    pixels[py * width + x] = color;
            }
        }

        private static void PaintVertical(Color32[] pixels, int width, int height, int x, int y0, int y1, int thickness, Color32 color)
        {
            for (var px = Mathf.Max(0, x); px < Mathf.Min(width, x + thickness); px++)
            {
                for (var y = Mathf.Max(0, y0); y < Mathf.Min(height, y1); y++)
                    pixels[y * width + px] = color;
            }
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

        private static Color32 Lerp(Color32 a, Color32 b, float t)
        {
            return new Color32(
                (byte)Mathf.RoundToInt(Mathf.Lerp(a.r, b.r, t)),
                (byte)Mathf.RoundToInt(Mathf.Lerp(a.g, b.g, t)),
                (byte)Mathf.RoundToInt(Mathf.Lerp(a.b, b.b, t)),
                (byte)Mathf.RoundToInt(Mathf.Lerp(a.a, b.a, t)));
        }

        private static Color32 Darken(Color32 color, float amount)
        {
            return new Color32(
                (byte)Mathf.RoundToInt(color.r * amount),
                (byte)Mathf.RoundToInt(color.g * amount),
                (byte)Mathf.RoundToInt(color.b * amount),
                color.a);
        }

        private static Color32 Add(Color32 color, float amount)
        {
            var value = Mathf.RoundToInt(amount * 255f);
            return new Color32(
                (byte)Mathf.Clamp(color.r + value, 0, 255),
                (byte)Mathf.Clamp(color.g + value, 0, 255),
                (byte)Mathf.Clamp(color.b + value, 0, 255),
                color.a);
        }

        private enum AccentMode
        {
            None,
            Top,
            Bottom,
            Left,
            LeftTop,
            TopBottom
        }

        private readonly struct PanelSpec
        {
            public readonly int Width;
            public readonly int Height;
            public readonly Color32 Top;
            public readonly Color32 Bottom;
            public readonly Color32 Edge;
            public readonly Color32 Accent;
            public readonly int CornerCut;
            public readonly int Border;
            public readonly AccentMode AccentMode;

            public PanelSpec(
                int width,
                int height,
                Color32 top,
                Color32 bottom,
                Color32 edge,
                Color32 accent,
                int cornerCut,
                int border,
                AccentMode accentMode)
            {
                Width = width;
                Height = height;
                Top = top;
                Bottom = bottom;
                Edge = edge;
                Accent = accent;
                CornerCut = cornerCut;
                Border = border;
                AccentMode = accentMode;
            }
        }
    }
}
