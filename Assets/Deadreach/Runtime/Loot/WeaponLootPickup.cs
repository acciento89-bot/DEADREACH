using Kamilunavo.Deadreach.Inventory;
using Kamilunavo.Deadreach.Player;
using Kamilunavo.Deadreach.Weapons;
using UnityEngine;

namespace Kamilunavo.Deadreach.Loot
{
    [RequireComponent(typeof(Collider))]
    public sealed class WeaponLootPickup : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float normalizedDepth = 0.35f;
        [SerializeField] private int rollSeed = 101;
        [SerializeField, Min(0f)] private float rotateSpeed = 52f;
        [SerializeField, Min(0f)] private float bobHeight = 0.16f;
        [SerializeField, Min(0f)] private float bobSpeed = 2.2f;

        private WeaponInstanceData _weapon;
        private Vector3 _basePosition;
        private Renderer _renderer;

        public WeaponInstanceData Weapon => _weapon;

        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
            _basePosition = transform.position;
            _renderer = GetComponentInChildren<Renderer>();
        }

        private void Start()
        {
            var rarity = WeaponLootFactory.RollRarity(normalizedDepth, rollSeed);
            _weapon = WeaponLootFactory.CreatePrototypeRifle(rarity, rollSeed * 7919 + 17);
            ApplyRarityPresentation(rarity);
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
            transform.position = _basePosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        }

        public void Configure(float depth, int seed)
        {
            normalizedDepth = Mathf.Clamp01(depth);
            rollSeed = seed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_weapon == null || other.GetComponentInParent<PlayerMotor>() == null)
                return;

            var inventory = RunInventory.Current;
            if (inventory == null || !inventory.TryAddWeapon(_weapon))
                return;

            Destroy(gameObject);
        }

        private void ApplyRarityPresentation(WeaponRarity rarity)
        {
            if (_renderer == null || _renderer.material == null)
                return;

            var color = rarity switch
            {
                WeaponRarity.Common => new Color(0.78f, 0.82f, 0.84f),
                WeaponRarity.Uncommon => new Color(0.22f, 0.9f, 0.42f),
                WeaponRarity.Rare => new Color(0.18f, 0.58f, 1f),
                WeaponRarity.Epic => new Color(0.7f, 0.28f, 1f),
                WeaponRarity.Legendary => new Color(1f, 0.58f, 0.12f),
                _ => Color.white
            };

            if (_renderer.material.HasProperty("_BaseColor"))
                _renderer.material.SetColor("_BaseColor", color);
            else
                _renderer.material.color = color;
        }
    }
}
