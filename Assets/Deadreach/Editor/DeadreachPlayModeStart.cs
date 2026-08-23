using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    [InitializeOnLoad]
    public static class DeadreachPlayModeStart
    {
        static DeadreachPlayModeStart()
        {
            EditorApplication.delayCall += Configure;
        }

        [MenuItem("DEADREACH/Project/Use Bunker As Play Mode Start")]
        public static void Configure()
        {
            var bunker = AssetDatabase.LoadAssetAtPath<SceneAsset>(DeadreachBuildSettings.BunkerScenePath);
            if (bunker == null)
                return;

            if (EditorSceneManager.playModeStartScene == bunker)
                return;

            EditorSceneManager.playModeStartScene = bunker;
            Debug.Log("DEADREACH Editor Play Mode start scene locked to Bunker_Hub. You can edit Dead City and press Play without bypassing the main menu/Bunker flow.");
        }
    }
}
