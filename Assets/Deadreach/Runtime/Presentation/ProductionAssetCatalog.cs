using System;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;

namespace Kamilunavo.Deadreach.Presentation
{
    [CreateAssetMenu(menuName = "DEADREACH/Production/Asset Catalog", fileName = "ProductionAssetCatalog")]
    public sealed class ProductionAssetCatalog : ScriptableObject
    {
        [Header("Characters")]
        [SerializeField] private GameObject survivorPrefab;
        [SerializeField] private GameObject survivorScoutPrefab;
        [SerializeField] private GameObject survivorWardenPrefab;
        [SerializeField] private GameObject[] infectedPrefabs;

        [Header("Weapons")]
        [SerializeField] private GameObject primaryWeaponPrefab;
        [SerializeField] private GameObject pistolWeaponPrefab;
        [SerializeField] private GameObject smgWeaponPrefab;
        [SerializeField] private GameObject shotgunWeaponPrefab;

        [Header("Survivor Transform")]
        [SerializeField] private Vector3 survivorLocalPosition;
        [SerializeField] private Vector3 survivorLocalEuler;
        [SerializeField, Min(0.01f)] private float survivorScale = 1f;

        [Header("Infected Transform")]
        [SerializeField] private Vector3 infectedLocalPosition;
        [SerializeField] private Vector3 infectedLocalEuler;
        [SerializeField, Min(0.01f)] private float infectedScale = 1f;

        public GameObject SurvivorPrefab => survivorPrefab;
        public GameObject SurvivorScoutPrefab => survivorScoutPrefab;
        public GameObject SurvivorWardenPrefab => survivorWardenPrefab;
        public GameObject PrimaryWeaponPrefab => primaryWeaponPrefab;
        public GameObject PistolWeaponPrefab => pistolWeaponPrefab;
        public GameObject SmgWeaponPrefab => smgWeaponPrefab;
        public GameObject ShotgunWeaponPrefab => shotgunWeaponPrefab;
        public int InfectedPrefabCount => infectedPrefabs?.Length ?? 0;
        public Vector3 SurvivorLocalPosition => survivorLocalPosition;
        public Vector3 SurvivorLocalEuler => survivorLocalEuler;
        public float SurvivorScale => survivorScale;
        public Vector3 InfectedLocalPosition => infectedLocalPosition;
        public Vector3 InfectedLocalEuler => infectedLocalEuler;
        public float InfectedScale => infectedScale;

        public GameObject GetSurvivorPrefab(string operatorId)
        {
            if (string.Equals(operatorId, "scout", StringComparison.OrdinalIgnoreCase) && survivorScoutPrefab != null)
                return survivorScoutPrefab;

            if (string.Equals(operatorId, "warden", StringComparison.OrdinalIgnoreCase) && survivorWardenPrefab != null)
                return survivorWardenPrefab;

            return survivorPrefab;
        }

        public GameObject GetWeaponPrefab(WeaponFamily family)
        {
            return family switch
            {
                WeaponFamily.Pistol when pistolWeaponPrefab != null => pistolWeaponPrefab,
                WeaponFamily.Smg when smgWeaponPrefab != null => smgWeaponPrefab,
                WeaponFamily.Shotgun when shotgunWeaponPrefab != null => shotgunWeaponPrefab,
                _ => primaryWeaponPrefab
            };
        }

        public GameObject GetInfectedPrefab(int variantIndex)
        {
            if (infectedPrefabs == null || infectedPrefabs.Length == 0)
                return null;

            var index = Mathf.Abs(variantIndex) % infectedPrefabs.Length;
            return infectedPrefabs[index];
        }

        public void ConfigureAssets(GameObject survivor, GameObject[] infected, GameObject primaryWeapon)
        {
            survivorPrefab = survivor;
            infectedPrefabs = infected;
            primaryWeaponPrefab = primaryWeapon;
        }

        public void ConfigureWeaponFamilies(GameObject rifle, GameObject smg, GameObject pistol, GameObject shotgun)
        {
            if (rifle != null)
                primaryWeaponPrefab = rifle;
            smgWeaponPrefab = smg;
            pistolWeaponPrefab = pistol;
            shotgunWeaponPrefab = shotgun;
        }

        public void ConfigureSurvivorOperators(GameObject ranger, GameObject scout, GameObject warden)
        {
            if (ranger != null)
                survivorPrefab = ranger;
            survivorScoutPrefab = scout;
            survivorWardenPrefab = warden;
        }

        public void ConfigureVisualOffsets(
            Vector3 survivorPosition,
            Vector3 survivorEuler,
            float newSurvivorScale,
            Vector3 infectedPosition,
            Vector3 infectedEuler,
            float newInfectedScale)
        {
            survivorLocalPosition = survivorPosition;
            survivorLocalEuler = survivorEuler;
            survivorScale = Mathf.Max(0.01f, newSurvivorScale);
            infectedLocalPosition = infectedPosition;
            infectedLocalEuler = infectedEuler;
            infectedScale = Mathf.Max(0.01f, newInfectedScale);
        }
    }
}
