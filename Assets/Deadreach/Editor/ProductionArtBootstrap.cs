using Kamilunavo.Deadreach.Presentation;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class ProductionArtBootstrap
    {
        public const string CatalogPath = "Assets/Deadreach/Resources/Deadreach/ProductionAssetCatalog.asset";

        [MenuItem("DEADREACH/Production/Create or Select Asset Catalog")]
        public static ProductionAssetCatalog EnsureCatalog()
        {
            EnsureFolder("Assets/Deadreach", "Resources");
            EnsureFolder("Assets/Deadreach/Resources", "Deadreach");

            var catalog = AssetDatabase.LoadAssetAtPath<ProductionAssetCatalog>(CatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<ProductionAssetCatalog>();
                AssetDatabase.CreateAsset(catalog, CatalogPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"DEADREACH production asset catalog created at {CatalogPath}");
            }

            Selection.activeObject = catalog;
            EditorGUIUtility.PingObject(catalog);
            return catalog;
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
