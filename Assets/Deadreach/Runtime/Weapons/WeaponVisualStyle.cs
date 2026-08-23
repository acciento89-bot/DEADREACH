using UnityEngine;

namespace Kamilunavo.Deadreach.Weapons
{
    public static class WeaponVisualStyle
    {
        private static readonly string[] StandardFinishIds =
        {
            "factory",
            "rustwalker",
            "hazard",
            "nightwatch",
            "toxic",
            "bloodline"
        };

        public static string ResolveFinishId(WeaponInstanceData weapon)
        {
            if (weapon == null)
                return "factory";

            if (!string.IsNullOrWhiteSpace(weapon.visualSkinId))
                return weapon.visualSkinId;

            unchecked
            {
                var hash = 17;
                var source = weapon.instanceId ?? string.Empty;
                for (var i = 0; i < source.Length; i++)
                    hash = hash * 31 + source[i];
                hash = hash * 31 + weapon.itemPower;
                hash = hash * 31 + (int)weapon.rarity;
                var index = Mathf.Abs(hash % StandardFinishIds.Length);
                return StandardFinishIds[index];
            }
        }

        public static string RollStandardFinishId(int seed)
        {
            var index = Mathf.Abs(seed % StandardFinishIds.Length);
            return StandardFinishIds[index];
        }

        public static string GetDisplayName(string finishId)
        {
            if (string.IsNullOrWhiteSpace(finishId))
                return "FACTORY ISSUE";

            if (finishId.StartsWith("mutation-", System.StringComparison.OrdinalIgnoreCase))
                return "MUTATION CORE";

            return finishId switch
            {
                "rustwalker" => "RUSTWALKER",
                "hazard" => "HAZARD STRIPE",
                "nightwatch" => "NIGHTWATCH",
                "toxic" => "TOXIC SALVAGE",
                "bloodline" => "BLOODLINE",
                _ => "FACTORY ISSUE"
            };
        }

        public static Color ResolveColor(WeaponInstanceData weapon)
        {
            var finishId = ResolveFinishId(weapon);
            if (finishId.StartsWith("mutation-", System.StringComparison.OrdinalIgnoreCase))
            {
                var tier = 1;
                var suffix = finishId.Substring("mutation-".Length);
                int.TryParse(suffix, out tier);
                tier = Mathf.Clamp(tier, 1, 5);
                return tier switch
                {
                    1 => new Color(0.52f, 0.38f, 0.95f, 1f),
                    2 => new Color(0.20f, 0.72f, 0.88f, 1f),
                    3 => new Color(0.92f, 0.45f, 0.18f, 1f),
                    4 => new Color(0.58f, 0.28f, 0.82f, 1f),
                    _ => new Color(0.92f, 0.18f, 0.22f, 1f)
                };
            }

            return finishId switch
            {
                "rustwalker" => new Color(0.92f, 0.62f, 0.40f, 1f),
                "hazard" => new Color(0.95f, 0.76f, 0.24f, 1f),
                "nightwatch" => new Color(0.38f, 0.52f, 0.72f, 1f),
                "toxic" => new Color(0.38f, 0.82f, 0.48f, 1f),
                "bloodline" => new Color(0.86f, 0.30f, 0.25f, 1f),
                _ => Color.white
            };
        }

        public static void Apply(Renderer renderer, WeaponInstanceData weapon)
        {
            if (renderer == null)
                return;

            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            var color = ResolveColor(weapon);
            block.SetColor("_BaseColor", color);
            block.SetColor("_Color", color);
            renderer.SetPropertyBlock(block);
        }

        public static void Apply(GameObject root, WeaponInstanceData weapon)
        {
            if (root == null)
                return;

            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                Apply(renderer, weapon);
        }
    }
}
