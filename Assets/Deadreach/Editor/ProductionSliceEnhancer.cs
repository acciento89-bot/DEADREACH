using Kamilunavo.Deadreach.Inventory;
using Kamilunavo.Deadreach.Loot;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class ProductionSliceEnhancer
    {
        public static void EnhanceCurrentDeadCityScene()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid() || scene.name != "DeadCity_VerticalSlice")
            {
                Debug.LogWarning("DEADREACH production enhancer expected DeadCity_VerticalSlice as active scene.");
                return;
            }

            if (Object.FindFirstObjectByType<RunInventory>() == null)
                new GameObject("Systems_RunInventory").AddComponent<RunInventory>();

            CreateWeaponCase("WeaponCase_Mid", new Vector3(4.15f, 0.62f, 7.6f), 0.38f, 22031);
            CreateWeaponCase("WeaponCase_Deep", new Vector3(-4.15f, 0.62f, 14.6f), 0.82f, 77291);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("DEADREACH Production Pass 0.2 enhancements added: RunInventory + extractable weapon loot cases.");
        }

        private static void CreateWeaponCase(string name, Vector3 position, float depth, int seed)
        {
            var existing = GameObject.Find(name);
            if (existing != null)
                Object.DestroyImmediate(existing);

            var root = new GameObject(name);
            root.transform.position = position;

            var trigger = root.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1.35f, 0.8f, 0.8f);

            var pickup = root.AddComponent<WeaponLootPickup>();
            pickup.Configure(depth, seed);

            var caseBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
            caseBody.name = "CaseBody";
            caseBody.transform.SetParent(root.transform, false);
            caseBody.transform.localPosition = Vector3.zero;
            caseBody.transform.localScale = new Vector3(1.15f, 0.28f, 0.58f);
            Object.DestroyImmediate(caseBody.GetComponent<Collider>());

            var rifleBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rifleBody.name = "RiflePreview";
            rifleBody.transform.SetParent(root.transform, false);
            rifleBody.transform.localPosition = new Vector3(0f, 0.28f, 0f);
            rifleBody.transform.localScale = new Vector3(0.72f, 0.12f, 0.16f);
            Object.DestroyImmediate(rifleBody.GetComponent<Collider>());

            var barrel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrel.name = "BarrelPreview";
            barrel.transform.SetParent(root.transform, false);
            barrel.transform.localPosition = new Vector3(0.54f, 0.28f, 0f);
            barrel.transform.localScale = new Vector3(0.45f, 0.065f, 0.07f);
            Object.DestroyImmediate(barrel.GetComponent<Collider>());
        }
    }
}
