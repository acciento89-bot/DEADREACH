using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Production 0.14 release invariant: DEADREACH is landscape-only on mobile.
    /// Keeps PlayerSettings deterministic on fresh/recovered checkouts before validation/build.
    /// </summary>
    public static class Production14LandscapeSettings
    {
        [InitializeOnLoadMethod]
        private static void QueueEnforcement()
        {
            EditorApplication.delayCall += () => EnsureLandscapeOnly(false);
        }

        [MenuItem("DEADREACH/Enforce Production 0.14 Landscape Only", priority = 7)]
        public static void EnforceFromMenu()
        {
            EnsureLandscapeOnly(true);
        }

        public static bool EnsureLandscapeOnly(bool verbose)
        {
            var changed = false;

            // AutoRotation is intentional: both landscape directions are supported while
            // portrait directions are forbidden. This is friendlier on phones/tablets than
            // locking users to one physical landscape side.
            if (PlayerSettings.defaultInterfaceOrientation != UIOrientation.AutoRotation)
            {
                PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
                changed = true;
            }

            if (PlayerSettings.allowedAutorotateToPortrait)
            {
                PlayerSettings.allowedAutorotateToPortrait = false;
                changed = true;
            }

            if (PlayerSettings.allowedAutorotateToPortraitUpsideDown)
            {
                PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
                changed = true;
            }

            if (!PlayerSettings.allowedAutorotateToLandscapeLeft)
            {
                PlayerSettings.allowedAutorotateToLandscapeLeft = true;
                changed = true;
            }

            if (!PlayerSettings.allowedAutorotateToLandscapeRight)
            {
                PlayerSettings.allowedAutorotateToLandscapeRight = true;
                changed = true;
            }

            if (changed)
            {
                AssetDatabase.SaveAssets();
                Debug.Log("DEADREACH 0.14 orientation repaired: landscape left/right enabled; portrait rotations disabled.");
            }
            else if (verbose)
            {
                Debug.Log("DEADREACH 0.14 orientation already valid: landscape-only autorotation.");
            }

            return !PlayerSettings.allowedAutorotateToPortrait &&
                   !PlayerSettings.allowedAutorotateToPortraitUpsideDown &&
                   PlayerSettings.allowedAutorotateToLandscapeLeft &&
                   PlayerSettings.allowedAutorotateToLandscapeRight;
        }
    }
}
