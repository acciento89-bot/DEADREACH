using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    /// <summary>
    /// Production 0.16 mobile geometry gate for the accepted Meshy Dead City layout.
    /// This does not destructively simplify meshes. It identifies exactly which source models
    /// need Meshy retopology before the city can be considered mobile-ready.
    /// </summary>
    public static class Production16MeshyDeadCityOptimizationGate
    {
        private const string RootName = "P16_Meshy_DeadCity";
        private const long TotalReleaseTarget = 1_200_000;

        private readonly struct Entry
        {
            public readonly string Name;
            public readonly long Triangles;
            public readonly long Target;

            public Entry(string name, long triangles, long target)
            {
                Name = name;
                Triangles = triangles;
                Target = target;
            }
        }

        [MenuItem("DEADREACH/Production 0.16/Audit Meshy Dead City Mobile Budget", priority = 16)]
        public static void Audit()
        {
            var root = GameObject.Find(RootName);
            if (root == null)
            {
                Debug.LogError("DEADREACH 0.16 mobile budget audit requires the P16_Meshy_DeadCity layout in the open Dead City scene.");
                return;
            }

            var entries = new List<Entry>();
            long total = 0;

            foreach (Transform child in root.transform)
            {
                if (child == null)
                    continue;

                var triangles = CountTriangles(child.gameObject);
                if (triangles <= 0)
                    continue;

                var target = GetTarget(child.name);
                entries.Add(new Entry(child.name, triangles, target));
                total += triangles;
            }

            entries = entries.OrderByDescending(entry => entry.Triangles).ToList();

            var overBudgetCount = 0;
            foreach (var entry in entries)
            {
                if (entry.Triangles <= entry.Target)
                {
                    Debug.Log($"DEADREACH 0.16 MOBILE GEO PASS // {entry.Name} // {entry.Triangles:N0} tris <= {entry.Target:N0} target.");
                    continue;
                }

                overBudgetCount++;
                var multiplier = entry.Target > 0 ? entry.Triangles / (float)entry.Target : 0f;
                Debug.LogWarning(
                    $"DEADREACH 0.16 RETOPO REQUIRED // {entry.Name} // {entry.Triangles:N0} tris // " +
                    $"target <= {entry.Target:N0} // {multiplier:0.0}x over target.");
            }

            var totalStatus = total <= TotalReleaseTarget ? "PASS" : "FAIL";
            var totalMultiplier = total / (float)TotalReleaseTarget;
            var message =
                $"DEADREACH 0.16 MOBILE GEOMETRY GATE: {totalStatus} // {total:N0} tris total // " +
                $"release target <= {TotalReleaseTarget:N0} // {totalMultiplier:0.0}x target // " +
                $"{overBudgetCount}/{entries.Count} instance(s) require source retopology.";

            if (total <= TotalReleaseTarget && overBudgetCount == 0)
                Debug.Log(message);
            else
                Debug.LogError(message);
        }

        private static long GetTarget(string instanceName)
        {
            if (instanceName.IndexOf("Building", StringComparison.OrdinalIgnoreCase) >= 0)
                return 150_000;
            if (instanceName.IndexOf("Storefront", StringComparison.OrdinalIgnoreCase) >= 0)
                return 120_000;
            if (instanceName.IndexOf("Industrial", StringComparison.OrdinalIgnoreCase) >= 0)
                return 120_000;
            if (instanceName.IndexOf("Rubble", StringComparison.OrdinalIgnoreCase) >= 0)
                return 80_000;
            if (instanceName.IndexOf("Checkpoint", StringComparison.OrdinalIgnoreCase) >= 0)
                return 80_000;
            if (instanceName.IndexOf("Van", StringComparison.OrdinalIgnoreCase) >= 0 ||
                instanceName.IndexOf("Vehicle", StringComparison.OrdinalIgnoreCase) >= 0)
                return 60_000;
            return 100_000;
        }

        private static long CountTriangles(GameObject root)
        {
            long triangles = 0;

            foreach (var filter in root.GetComponentsInChildren<MeshFilter>(true))
            {
                if (filter == null || filter.sharedMesh == null)
                    continue;
                triangles += CountMeshTriangles(filter.sharedMesh);
            }

            foreach (var skinned in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (skinned == null || skinned.sharedMesh == null)
                    continue;
                triangles += CountMeshTriangles(skinned.sharedMesh);
            }

            return triangles;
        }

        private static long CountMeshTriangles(Mesh mesh)
        {
            long triangles = 0;
            for (var subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
            {
                if (mesh.GetTopology(subMesh) != MeshTopology.Triangles)
                    continue;
                triangles += (long)mesh.GetIndexCount(subMesh) / 3L;
            }
            return triangles;
        }
    }
}
