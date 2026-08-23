using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    [InitializeOnLoad]
    internal static class DeadreachProjectBootstrap
    {
        private const string ConfigurationVersion = "1";
        private const string ConfigurationKey = "DEADREACH.ProjectConfiguration.Version";
        private const string BundleIdentifier = "de.kamilunavo.deadzone";

        static DeadreachProjectBootstrap()
        {
            EditorApplication.delayCall += EnsureConfigured;
        }

        [MenuItem("DEADREACH/Project/Apply Production Settings")]
        public static void ApplyProductionSettings()
        {
            PlayerSettings.companyName = "Kamilunavo";
            PlayerSettings.productName = "DEADREACH";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.colorSpace = ColorSpace.Linear;

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, BundleIdentifier);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, BundleIdentifier);
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);

            PlayerSettings.iOS.buildNumber = "1";
            PlayerSettings.Android.bundleVersionCode = 1;

            EditorSettings.serializationMode = SerializationMode.ForceText;
            EditorSettings.lineEndingsForNewScripts = LineEndingsMode.Unix;

            AssetDatabase.SaveAssets();
            EditorPrefs.SetString(ConfigurationKey, ConfigurationVersion);

            Debug.Log("DEADREACH production project settings applied.");
        }

        private static void EnsureConfigured()
        {
            if (EditorPrefs.GetString(ConfigurationKey, string.Empty) == ConfigurationVersion)
            {
                return;
            }

            ApplyProductionSettings();
        }
    }
}
