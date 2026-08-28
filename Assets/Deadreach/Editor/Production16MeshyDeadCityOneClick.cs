using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production16MeshyDeadCityOneClick
    {
        [MenuItem("DEADREACH/Production 0.16/Integrate Meshy Dead City", priority = 9)]
        public static void Integrate()
        {
            Debug.Log("DEADREACH 0.16 Meshy Dead City integration started...");
            Production16MeshyDeadCityGlbBridge.RepairFromMenu();
            Production16MeshyDeadCityMaterialRepair.Repair();
            Production16MeshyDeadCityLayout.ValidateAssets();
            Production16MeshyDeadCityLayout.Build();
            Production16MeshyDeadCityLayoutPolish.Apply();
            Debug.Log("DEADREACH 0.16 Meshy Dead City integration pass finished. Meshy assets prepared, materials hardened, base layout rebuilt and final placement polish applied. Inspect Game view next.");
        }
    }
}
