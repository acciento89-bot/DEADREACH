using System;
using System.Collections.Generic;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;

namespace Kamilunavo.Deadreach.Inventory
{
    public sealed class RunInventory : MonoBehaviour
    {
        public static RunInventory Current { get; private set; }

        [SerializeField, Min(1)] private int weaponCapacity = 6;

        public event Action Changed;

        private readonly List<WeaponInstanceData> _weapons = new();

        public IReadOnlyList<WeaponInstanceData> Weapons => _weapons;
        public int WeaponCapacity => weaponCapacity;
        public bool IsFull => _weapons.Count >= weaponCapacity;

        private void Awake()
        {
            if (Current != null && Current != this)
            {
                Destroy(gameObject);
                return;
            }

            Current = this;
        }

        public bool TryAddWeapon(WeaponInstanceData weapon)
        {
            if (weapon == null || IsFull)
                return false;

            _weapons.Add(weapon.Clone());
            Changed?.Invoke();
            return true;
        }

        public bool TryRemoveWeapon(string instanceId)
        {
            var index = _weapons.FindIndex(item => item != null && item.instanceId == instanceId);
            if (index < 0)
                return false;

            _weapons.RemoveAt(index);
            Changed?.Invoke();
            return true;
        }

        public List<WeaponInstanceData> CreateExtractionSnapshot()
        {
            var snapshot = new List<WeaponInstanceData>(_weapons.Count);
            foreach (var weapon in _weapons)
            {
                if (weapon != null)
                    snapshot.Add(weapon.Clone());
            }

            return snapshot;
        }

        public void Clear()
        {
            if (_weapons.Count == 0)
                return;

            _weapons.Clear();
            Changed?.Invoke();
        }

        private void OnDestroy()
        {
            if (Current == this)
                Current = null;
        }
    }
}
