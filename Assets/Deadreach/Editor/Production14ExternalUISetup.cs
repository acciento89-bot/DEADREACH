using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Removes the rejected external HUD experiment from local projects.
    /// The experiment downloaded Devdog sprites into Resources, so a normal git pull
    /// cannot remove those generated local files. This cleanup runs once after compile.
    /// </summary>
    public static class Production14ExternalUISetup
    {
        private const string RejectedFolder = "Assets/Deadreach/Resources/Production14/UI/External";

        [InitializeOnLoadMethod]
        private static void QueueCleanup()
        {
            EditorApplication.delayCall += CleanupRejectedPack;
        }

        [MenuItem("DEADREACH/Cleanup Rejected 0.14 External UI Pack", priority = 8)]
        public static void CleanupRejectedPack()
        {
            if (!AssetDatabase.IsValidFolder(RejectedFolder))
                return;

            if (AssetDatabase.DeleteAsset(RejectedFolder))
            {
                AssetDatabase.Refresh();
                Debug.Log("DEADREACH 0.14 cleanup: rejected external HUD sprites removed. Clean command-center skin is active.");
            }
            else
            {
                Debug.LogWarning("DEADREACH 0.14 cleanup: rejected external HUD folder could not be removed automatically. Delete Assets/Deadreach/Resources/Production14/UI/External manually if it still exists.");
            }
        }
    }
}
