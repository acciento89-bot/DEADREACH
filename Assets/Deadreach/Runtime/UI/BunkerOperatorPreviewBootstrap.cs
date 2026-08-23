using UnityEngine;

namespace Kamilunavo.Deadreach.UI
{
    public static class BunkerOperatorPreviewBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsurePreview()
        {
            if (Object.FindFirstObjectByType<BunkerOperatorPreviewUI>() != null)
                return;

            var root = new GameObject("Systems_BunkerOperatorInspector");
            root.AddComponent<BunkerOperatorPreviewUI>();
            Object.DontDestroyOnLoad(root);
        }
    }
}
