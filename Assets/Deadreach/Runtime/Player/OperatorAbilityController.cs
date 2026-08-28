using System.Collections.Generic;
using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Feedback;
using Kamilunavo.Deadreach.Input;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Progression;
using Kamilunavo.Deadreach.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kamilunavo.Deadreach.Player
{
    [DisallowMultipleComponent]
    public sealed class OperatorAbilityController : MonoBehaviour
    {
        public static OperatorAbilityController Current { get; private set; }

        public string AbilityName => _definition.AbilityName;
        public string AbilityDescription => _definition.AbilityDescription;
        public float CooldownDuration => _definition.AbilityCooldown;
        public float CooldownRemaining => Mathf.Max(0f, _nextReadyTime - Time.time);
        public bool IsReady => CooldownRemaining <= 0.001f;

        private OperatorDefinition _definition;
        private Damageable _health;
        private CharacterController _controller;
        private Camera _camera;
        private float _nextReadyTime;
        private GUIStyle _titleStyle;
        private GUIStyle _smallStyle;
        private GUIStyle _centerStyle;
        private int _styleScaleKey;
        private string _feedbackText;
        private float _feedbackUntil;

        private void Awake()
        {
            if (Current != null && Current != this)
            {
                Destroy(this);
                return;
            }

            Current = this;
            _health = GetComponent<Damageable>();
            _controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _definition = OperatorCatalog.Get(SaveService.Data.selectedCharacterId);
            _camera = Camera.main;
            _nextReadyTime = Time.time + 0.65f;
            Debug.Log($"DEADREACH 0.10 ability online // {_definition.Name} // {_definition.AbilityName} // CD {_definition.AbilityCooldown:0.#}s.");
        }

        private void Update()
        {
            var input = DeadreachInput.Current;
            if (input == null || !input.ConsumeAbilityPress())
                return;

            if (TryActivate())
            {
                ShowFeedback("FIRED");
                return;
            }

            if (!IsReady)
            {
                ShowFeedback("COOLDOWN");
                return;
            }

            ShowFeedback(_definition.Id switch
            {
                "warden" => "NO TARGET",
                "ranger" => "FULL HP",
                _ => "BLOCKED"
            });
        }

        public bool TryActivate()
        {
            if (!IsReady || _health == null || _health.IsDead)
                return false;

            var activated = _definition.Id switch
            {
                "scout" => ActivateVectorDash(),
                "warden" => ActivateShockwave(),
                _ => ActivateFieldPatch()
            };

            if (!activated)
                return false;

            _nextReadyTime = Time.time + Mathf.Max(0.1f, _definition.AbilityCooldown);
            Debug.Log($"DEADREACH ability fired // {_definition.Name} // {_definition.AbilityName}.");
            return true;
        }

        private bool ActivateFieldPatch()
        {
            if (_health.CurrentHealth >= _health.MaxHealth - 0.5f)
                return false;

            _health.Heal(_health.MaxHealth * 0.32f);
            var origin = transform.position + Vector3.up * 0.18f;
            CombatFeedback.RaiseAbility(new AbilityImpactFeedback(
                CombatAbilityKind.FieldPatch,
                origin,
                origin,
                Vector3.up,
                3.2f));
            return true;
        }

        private bool ActivateVectorDash()
        {
            if (_controller == null || !_controller.enabled)
                return false;

            var direction = GetMovementDirection();
            if (direction.sqrMagnitude < 0.01f)
                direction = transform.forward;

            direction.Normalize();
            var start = transform.position + Vector3.up * 0.55f;
            _controller.Move(direction * 4.6f);
            var end = transform.position + Vector3.up * 0.55f;
            CombatFeedback.RaiseAbility(new AbilityImpactFeedback(
                CombatAbilityKind.VectorDash,
                start,
                end,
                direction,
                1.45f));
            return true;
        }

        private bool ActivateShockwave()
        {
            var colliders = Physics.OverlapSphere(transform.position, 4.6f, ~0, QueryTriggerInteraction.Collide);
            var damaged = new HashSet<Damageable>();
            var level = Mathf.Clamp(SaveService.Data.selectedLevel, 1, SaveService.MaxCampaignLevel);
            var damage = 58f + (level - 1) * 1.35f;

            foreach (var collider in colliders)
            {
                if (collider == null)
                    continue;

                var target = collider.GetComponentInParent<Damageable>();
                if (target == null || target.Faction != CombatFaction.Infected || target.IsDead || !damaged.Add(target))
                    continue;

                var direction = target.transform.position - transform.position;
                direction.y = 0f;
                if (direction.sqrMagnitude < 0.01f)
                    direction = transform.forward;

                target.TakeDamage(new DamageInfo(
                    damage,
                    CombatFaction.Survivor,
                    target.transform.position + Vector3.up * 0.8f,
                    direction.normalized));
            }

            if (damaged.Count <= 0)
                return false;

            CombatFeedback.RaiseAbility(new AbilityImpactFeedback(
                CombatAbilityKind.Shockwave,
                transform.position + Vector3.up * 0.08f,
                transform.position + Vector3.up * 0.08f,
                transform.forward,
                4.6f));
            return true;
        }

        private Vector3 GetMovementDirection()
        {
            var input = DeadreachInput.Current;
            if (input == null || input.Move.sqrMagnitude < 0.01f)
                return transform.forward;

            var camera = _camera != null ? _camera : Camera.main;
            if (camera == null)
                return new Vector3(input.Move.x, 0f, input.Move.y);

            var forward = camera.transform.forward;
            var right = camera.transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            return Vector3.ClampMagnitude(forward * input.Move.y + right * input.Move.x, 1f);
        }

        private void ShowFeedback(string text)
        {
            _feedbackText = text;
            _feedbackUntil = Time.unscaledTime + 0.8f;
        }

        private void OnGUI()
        {
            if (_health == null || _health.IsDead || string.IsNullOrWhiteSpace(_definition.AbilityName))
                return;

            var safe = Screen.safeArea;
            var touchCapable = Application.isMobilePlatform || Touchscreen.current != null;

            // Production 0.16 owns desktop expedition HUD presentation. Keep this legacy overlay only
            // where it is still a functional mobile touch control; keyboard input continues through Update().
            if (!touchCapable && FindFirstObjectByType<Production16FieldOpsUI>() != null)
                return;

            EnsureStyles(touchCapable);

            if (touchCapable)
                DrawMobileAbility(safe);
            else
                DrawDesktopAbility(safe);
        }

        private void DrawMobileAbility(Rect safe)
        {
            var size = Mathf.Clamp(safe.height * 0.145f, 108f, 158f);
            var center = new Vector2(
                safe.xMax - size * 0.72f,
                safe.yMin + safe.height * 0.78f);
            var screenRect = new Rect(center.x - size * 0.5f, center.y - size * 0.5f, size, size);
            DeadreachInput.Current?.SetAbilityTouchRegion(screenRect);

            var guiRect = ScreenToGuiRect(screenRect);
            var feedbackActive = Time.unscaledTime < _feedbackUntil;
            var old = GUI.color;
            GUI.color = feedbackActive
                ? new Color(1f, 0.78f, 0.2f, 0.96f)
                : IsReady
                    ? new Color(_definition.Accent.r, _definition.Accent.g, _definition.Accent.b, 0.92f)
                    : new Color(0.18f, 0.19f, 0.19f, 0.82f);
            GUI.Box(guiRect, GUIContent.none);
            GUI.color = old;

            var status = feedbackActive
                ? _feedbackText
                : IsReady ? "READY" : $"{CooldownRemaining:0.0}s";
            GUI.Label(new Rect(guiRect.x + 6f, guiRect.y + size * 0.18f, guiRect.width - 12f, size * 0.24f), "ABILITY", _centerStyle);
            GUI.Label(new Rect(guiRect.x + 6f, guiRect.y + size * 0.46f, guiRect.width - 12f, size * 0.22f), status, _centerStyle);
            GUI.Label(new Rect(guiRect.x - size * 0.38f, guiRect.y + guiRect.height + 4f, guiRect.width + size * 0.76f, 30f), _definition.AbilityName, _smallStyle);
        }

        private void DrawDesktopAbility(Rect safe)
        {
            var width = 285f;
            var height = 68f;
            var screenRect = new Rect(safe.xMax - width - 22f, safe.yMin + 24f, width, height);
            var guiRect = ScreenToGuiRect(screenRect);

            var old = GUI.color;
            GUI.color = new Color(0.015f, 0.022f, 0.022f, 0.88f);
            GUI.Box(guiRect, GUIContent.none);
            GUI.color = old;

            GUI.Label(new Rect(guiRect.x + 12f, guiRect.y + 7f, guiRect.width - 24f, 26f), $"[SPACE]  {_definition.AbilityName}", _titleStyle);
            var status = IsReady ? "READY" : $"COOLDOWN {CooldownRemaining:0.0}s";
            GUI.Label(new Rect(guiRect.x + 12f, guiRect.y + 35f, guiRect.width - 24f, 22f), status, _smallStyle);
        }

        private static Rect ScreenToGuiRect(Rect screenRect)
        {
            return new Rect(
                screenRect.x,
                Screen.height - screenRect.y - screenRect.height,
                screenRect.width,
                screenRect.height);
        }

        private void EnsureStyles(bool mobile)
        {
            var scaleKey = mobile ? Mathf.Clamp(Mathf.RoundToInt(Screen.safeArea.height / 50f), 14, 24) : 14;
            if (_titleStyle != null && _styleScaleKey == scaleKey)
                return;

            _styleScaleKey = scaleKey;
            var titleSize = mobile ? Mathf.Clamp(scaleKey, 16, 22) : 14;
            var smallSize = mobile ? Mathf.Clamp(scaleKey - 2, 14, 20) : 11;

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = titleSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            _titleStyle.normal.textColor = Color.white;

            _smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = smallSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _smallStyle.normal.textColor = new Color(0.86f, 0.9f, 0.88f);

            _centerStyle = new GUIStyle(_smallStyle)
            {
                fontSize = titleSize,
                alignment = TextAnchor.MiddleCenter
            };
        }

        private void OnDestroy()
        {
            if (Current == this)
                Current = null;
        }
    }
}
