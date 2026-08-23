using UnityEngine;

namespace Kamilunavo.Deadreach.UI
{
    public static class BunkerWeaponPreviewBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsurePreview()
        {
            if (Object.FindFirstObjectByType<BunkerWeaponPreviewUI>() != null)
                return;

            var root = new GameObject("Systems_BunkerWeaponInspector");
            root.AddComponent<BunkerWeaponPreviewUI>();
            Object.DontDestroyOnLoad(root);
        }
    }
}
