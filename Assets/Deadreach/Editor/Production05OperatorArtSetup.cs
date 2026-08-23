using UnityEditor;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Stable public/menu entry point kept for existing builder callers.
    /// The implementation lives in Production05OperatorArtSetupV2 so the failed incremental
    /// AssetDatabase mutation path can never run again.
    /// </summary>
    public static class Production05OperatorArtSetup
    {
        [MenuItem("DEADREACH/Production/Repair 0.5 Operator Art", priority = 24)]
        public static void RepairMenu()
        {
            Production05OperatorArtSetupV2.EnsureOperatorAssetsReady();
        }

        public static bool EnsureOperatorAssetsReady()
        {
            return Production05OperatorArtSetupV2.EnsureOperatorAssetsReady();
        }
    }
}
