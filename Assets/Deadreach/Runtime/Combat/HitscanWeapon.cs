using System;
using Kamilunavo.Deadreach.Input;
using UnityEngine;

namespace Kamilunavo.Deadreach.Combat
{
    public sealed class HitscanWeapon : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField, Min(0.1f)] private float damage = 24f;
        [SerializeField, Min(0.1f)] private float roundsPerSecond = 7.5f;
        [SerializeField, Min(1f)] private float range = 40f;
        [SerializeField, Min(0f)] private float aimTurnSpeed = 24f;
        [SerializeField] private LayerMask hitMask = ~0;

        private Camera _camera;
        private Damageable _owner;
        private float _nextShotTime;
        private Vector3 _aimPoint;

        public Vector3 AimPoint => _aimPoint;

        private void Awake()
        {
            _owner = GetComponent<Damageable>();
        }

        private void Start()
        {
            _camera = Camera.main;
            _aimPoint = transform.position + transform.forward * 8f;
        }

        private void Update()
        {
            var input = DeadreachInput.Current;
            if (input == null || _camera == null)
                return;

            UpdateAim(input);

            if (input.FireHeld && Time.time >= _nextShotTime)
            {
                _nextShotTime = Time.time + 1f / roundsPerSecond;
                Fire();
            }
        }

        private void UpdateAim(DeadreachInput input)
        {
            if (input.HasAim)
            {
                var ray = _camera.ScreenPointToRay(input.AimScreenPosition);
                if (Physics.Raycast(ray, out var hit, 250f, hitMask, QueryTriggerInteraction.Ignore))
                {
                    _aimPoint = hit.point;
                }
                else
                {
                    var ground = new Plane(Vector3.up, transform.position);
                    if (ground.Raycast(ray, out var enter))
                        _aimPoint = ray.GetPoint(enter);
                }
            }

            var flatDirection = _aimPoint - transform.position;
            flatDirection.y = 0f;
            if (flatDirection.sqrMagnitude > 0.05f)
            {
                var targetRotation = Quaternion.LookRotation(flatDirection.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aimTurnSpeed * Time.deltaTime);
            }
        }

        private void Fire()
        {
            var origin = muzzle != null ? muzzle.position : transform.position + Vector3.up * 1.15f + transform.forward * 0.4f;
            var direction = (_aimPoint - origin).normalized;
            if (direction.sqrMagnitude < 0.5f)
                direction = transform.forward;

            var hits = Physics.RaycastAll(origin, direction, range, hitMask, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, static (a, b) => a.distance.CompareTo(b.distance));

            var endPoint = origin + direction * range;
            foreach (var hit in hits)
            {
                if (hit.collider.transform.IsChildOf(transform) || transform.IsChildOf(hit.collider.transform))
                    continue;

                endPoint = hit.point;
                var damageable = hit.collider.GetComponentInParent<Damageable>();
                if (damageable != null)
                {
                    var faction = _owner != null ? _owner.Faction : CombatFaction.Survivor;
                    damageable.TakeDamage(new DamageInfo(damage, faction, hit.point, direction));
                }

                break;
            }

            Debug.DrawLine(origin, endPoint, Color.cyan, 0.12f);
        }
    }
}
