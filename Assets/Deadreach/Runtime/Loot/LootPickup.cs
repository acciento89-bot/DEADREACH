using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Player;
using UnityEngine;

namespace Kamilunavo.Deadreach.Loot
{
    [RequireComponent(typeof(Collider))]
    public sealed class LootPickup : MonoBehaviour
    {
        [SerializeField, Min(1)] private int scrapAmount = 1;
        [SerializeField, Min(0f)] private float spinSpeed = 75f;
        [SerializeField, Min(0f)] private float bobHeight = 0.12f;
        [SerializeField, Min(0f)] private float bobSpeed = 2.4f;

        private Vector3 _basePosition;
        private float _phase;

        private void Awake()
        {
            var pickupCollider = GetComponent<Collider>();
            pickupCollider.isTrigger = true;
            _basePosition = transform.position;
            _phase = Random.value * Mathf.PI * 2f;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            if (bobHeight > 0f)
            {
                var position = _basePosition;
                position.y += Mathf.Sin(Time.time * bobSpeed + _phase) * bobHeight;
                transform.position = position;
            }
        }

        public void Configure(int amount)
        {
            scrapAmount = Mathf.Max(1, amount);
            _basePosition = transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerMotor>() == null)
                return;

            var session = RunSession.Current;
            if (session == null || session.IsCompleted || session.IsFailed)
                return;

            session.CollectScrap(scrapAmount);
            Destroy(gameObject);
        }

        public static LootPickup SpawnScrap(Vector3 position, int amount)
        {
            var pickup = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pickup.name = $"Scrap_{Mathf.Max(1, amount)}";
            pickup.transform.position = position;
            pickup.transform.localScale = Vector3.one * 0.42f;

            var collider = pickup.GetComponent<Collider>();
            collider.isTrigger = true;

            var component = pickup.AddComponent<LootPickup>();
            component.Configure(amount);
            return component;
        }
    }
}
