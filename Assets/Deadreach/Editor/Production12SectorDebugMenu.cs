using Kamilunavo.Deadreach.World;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production12SectorDebugMenu
    {
        private const string Root = "DEADREACH/Dev/Sector 0.12/";

        [MenuItem(Root + "AUTO", priority = 0)]
        private static void Auto() => SetOverride(-1, "AUTO");

        [MenuItem(Root + "QUARANTINE WARD", priority = 1)]
        private static void QuarantineWard() => SetOverride((int)SectorArchetype.QuarantineWard, "QUARANTINE WARD");

        [MenuItem(Root + "TRANSIT COLLAPSE", priority = 2)]
        private static void TransitCollapse() => SetOverride((int)SectorArchetype.TransitCollapse, "TRANSIT COLLAPSE");

        [MenuItem(Root + "INDUSTRIAL SPILL", priority = 3)]
        private static void IndustrialSpill() => SetOverride((int)SectorArchetype.IndustrialSpill, "INDUSTRIAL SPILL");

        [MenuItem(Root + "BLACKOUT PLAZA", priority = 4)]
        private static void BlackoutPlaza() => SetOverride((int)SectorArchetype.BlackoutPlaza, "BLACKOUT PLAZA");

        private static void SetOverride(int value, string label)
        {
            PlayerPrefs.SetInt(SectorDirector.EditorSectorOverrideKey, value);
            PlayerPrefs.Save();
            Debug.Log($"DEADREACH 0.12 editor sector override // {label}. Takes effect on the next expedition scene load. Player builds always use AUTO selection.");
        }
    }
}
