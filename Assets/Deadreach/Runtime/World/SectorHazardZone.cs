using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Player;
using UnityEngine;

namespace Kamilunavo.Deadreach.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider))]
    public sealed class SectorHazardZone : MonoBehaviour
    {
        [SerializeField] private SectorHazardKind kind = SectorHazardKind.Contamination;
        [SerializeField] private Color accent = new(0.25f, 1f, 0.45f, 0.95f);
        [SerializeField] private Vector3 zoneSize = new(4f, 2.4f, 4f);
        [SerializeField, Min(0f)] private float damagePerPulse = 4f;
        [SerializeField, Min(0.2f)] private float pulseInterval = 1.2f;
        [SerializeField, Min(0.05f)] private float ringRadius = 2.2f;

        private PlayerMotor _player;
        private Damageable _playerHealth;
        private float _nextPulse;
        private LineRenderer _ring;
        private Light _light;
        private float _phase;

        public SectorHazardKind Kind => kind;
        public string DisplayName => kind switch
        {
            SectorHazardKind.ElectricalArc => "ARC FIELD",
            SectorHazardKind.Fireline => "THERMAL HAZARD",
            _ => "CONTAMINATION"
        };
        public Color Accent => accent;

        public void Configure(
            SectorHazardKind newKind,
            Color newAccent,
            Vector3 newZoneSize,
            float newDamagePerPulse,
            float newPulseInterval,
            float newRingRadius)
        {
            kind = newKind;
            accent = newAccent;
            zoneSize = new Vector3(
                Mathf.Max(0.5f, newZoneSize.x),
                Mathf.Max(0.5f, newZoneSize.y),
                Mathf.Max(0.5f, newZoneSize.z));
            damagePerPulse = Mathf.Max(0f, newDamagePerPulse);
            pulseInterval = Mathf.Max(0.2f, newPulseInterval);
            ringRadius = Mathf.Max(0.05f, newRingRadius);

            var box = GetComponent<BoxCollider>();
            if (box != null)
            {
                box.isTrigger = true;
                box.center = new Vector3(0f, zoneSize.y * 0.5f, 0f);
                box.size = zoneSize;
            }
        }

        private void Awake()
        {
            var box = GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.center = new Vector3(0f, zoneSize.y * 0.5f, 0f);
            box.size = zoneSize;
            BuildPresentation();
        }

        private void Update()
        {
            _phase += Time.deltaTime * 2.2f;
            var pulse = 0.5f + Mathf.Sin(_phase) * 0.5f;

            if (_ring != null)
            {
                _ring.widthMultiplier = Mathf.Lerp(0.045f, 0.1f, pulse);
                var c = accent;
                c.a = Mathf.Lerp(0.28f, 0.82f, pulse);
                _ring.startColor = c;
                _ring.endColor = c;
            }

            if (_light != null)
                _light.intensity = Mathf.Lerp(1.8f, 4.6f, pulse);

            if (_playerHealth == null || _playerHealth.IsDead || Time.time < _nextPulse)
                return;

            _nextPulse = Time.time + pulseInterval;
            var direction = _player != null
                ? (_player.transform.position - transform.position).normalized
                : Vector3.up;
            if (direction.sqrMagnitude < 0.01f)
                direction = Vector3.up;

            _playerHealth.TakeDamage(new DamageInfo(
                damagePerPulse,
                CombatFaction.Neutral,
                _playerHealth.transform.position + Vector3.up * 0.7f,
                direction));
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<PlayerMotor>();
            if (player == null)
                return;

            _player = player;
            _playerHealth = player.GetComponent<Damageable>();
            _nextPulse = Time.time + Mathf.Min(0.45f, pulseInterval * 0.35f);
            SectorDirector.Current?.NotifyHazardEnter(this);
        }

        private void OnTriggerExit(Collider other)
        {
            var player = other.GetComponentInParent<PlayerMotor>();
            if (player == null || player != _player)
                return;

            SectorDirector.Current?.NotifyHazardExit(this);
            _player = null;
            _playerHealth = null;
        }

        private void OnDisable()
        {
            if (_player != null)
                SectorDirector.Current?.NotifyHazardExit(this);
            _player = null;
            _playerHealth = null;
        }

        private void BuildPresentation()
        {
            var ringObject = new GameObject("Hazard_Ring");
            ringObject.transform.SetParent(transform, false);
            ringObject.transform.localPosition = new Vector3(0f, 0.055f, 0f);
            _ring = ringObject.AddComponent<LineRenderer>();
            _ring.useWorldSpace = false;
            _ring.loop = true;
            _ring.positionCount = 48;
            _ring.widthMultiplier = 0.065f;
            _ring.numCornerVertices = 2;
            _ring.numCapVertices = 2;

            var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color");
            if (shader != null)
            {
                var material = new Material(shader) { name = $"Runtime_Hazard_{kind}" };
                if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", accent);
                if (material.HasProperty("_Color")) material.SetColor("_Color", accent);
                _ring.material = material;
            }

            var radiusX = Mathf.Max(ringRadius, zoneSize.x * 0.48f);
            var radiusZ = Mathf.Max(ringRadius, zoneSize.z * 0.48f);
            for (var i = 0; i < _ring.positionCount; i++)
            {
                var angle = Mathf.PI * 2f * i / _ring.positionCount;
                _ring.SetPosition(i, new Vector3(Mathf.Cos(angle) * radiusX, 0f, Mathf.Sin(angle) * radiusZ));
            }

            var lightObject = new GameObject("Hazard_Light");
            lightObject.transform.SetParent(transform, false);
            lightObject.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            _light = lightObject.AddComponent<Light>();
            _light.type = LightType.Point;
            _light.color = accent;
            _light.range = Mathf.Max(zoneSize.x, zoneSize.z) * 1.3f;
            _light.intensity = 3f;
            _light.shadows = LightShadows.None;
        }
    }
}
