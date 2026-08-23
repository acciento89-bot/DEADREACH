using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Weapons;
using UnityEditor;
using UnityEngine;

namespace Kamilunavo.Deadreach.Editor
{
    public static class Production08ProgressionDebugTools
    {
        [MenuItem("DEADREACH/Dev/0.8 Grant 1500 Workshop Scrap", priority = 110)]
        private static void GrantWorkshopScrap()
        {
            SaveService.Data.securedScrap += 1500;
            SaveService.Save();
            Debug.Log($"DEADREACH 0.8 DEV: granted 1500 secured Scrap. Balance = {SaveService.Data.securedScrap}.");
        }

        [MenuItem("DEADREACH/Dev/0.8 Seed Four Weapon Families", priority = 111)]
        private static void SeedWeaponFamilies()
        {
            var data = SaveService.Data;
            data.stashWeapons ??= new System.Collections.Generic.List<WeaponInstanceData>();

            var rifle = WeaponLootFactory.CreateFieldWeapon(WeaponRarity.Rare, 8011, WeaponFamily.Rifle);
            var smg = WeaponLootFactory.CreateFieldWeapon(WeaponRarity.Epic, 8012, WeaponFamily.Smg);
            var pistol = WeaponLootFactory.CreateFieldWeapon(WeaponRarity.Rare, 8013, WeaponFamily.Pistol);
            var shotgun = WeaponLootFactory.CreateFieldWeapon(WeaponRarity.Epic, 8014, WeaponFamily.Shotgun);

            data.stashWeapons.Add(rifle);
            data.stashWeapons.Add(smg);
            data.stashWeapons.Add(pistol);
            data.stashWeapons.Add(shotgun);
            data.equippedPrimaryWeaponId = rifle.instanceId;
            SaveService.Save();

            Debug.Log("DEADREACH 0.8 DEV: seeded Rifle / SMG / Pistol / Shotgun and equipped the Rifle.");
        }

        [MenuItem("DEADREACH/Dev/0.8 Set Workshop Test Profile", priority = 112)]
        private static void SetWorkshopTestProfile()
        {
            var data = SaveService.Data;
            data.securedScrap = Mathf.Max(data.securedScrap, 2500);
            data.workbenchLevel = 2;
            data.medbayLevel = 1;
            data.cargoRigLevel = 1;
            data.scavengerNetworkLevel = 1;

            if (data.stashWeapons == null || data.stashWeapons.Count == 0)
                SeedWeaponFamilies();
            else
                SaveService.Save();

            Debug.Log("DEADREACH 0.8 DEV: Workshop test profile ready // Workbench 2 // Medbay 1 // Cargo 1 // Scavenger 1 // >=2500 Scrap.");
        }
    }
}
