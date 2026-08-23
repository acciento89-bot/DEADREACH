using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Kamilunavo.Deadreach.Input
{
    public sealed class DeadreachInput : MonoBehaviour
    {
        public static DeadreachInput Current { get; private set; }

        [SerializeField, Min(40f)] private float virtualStickRadius = 140f;
        [SerializeField, Range(0f, 0.5f)] private float moveDeadZone = 0.14f;
        [SerializeField, Range(0f, 0.5f)] private float aimDeadZone = 0.16f;
        [SerializeField, Range(0.5f, 2f)] private float moveResponseExponent = 1.05f;
        [SerializeField, Range(0.5f, 2f)] private float aimResponseExponent = 0.9f;
        [SerializeField, Range(0.02f, 0.5f)] private float aimFireThreshold = 0.12f;

        public Vector2 Move { get; private set; }
        public Vector2 Aim { get; private set; }
        public Vector2 AimScreenPosition { get; private set; }
        public bool HasAim { get; private set; }
        public bool HasDirectionalAim { get; private set; }
        public bool HasPointerAim { get; private set; }
        public bool FireHeld { get; private set; }

        public bool HasMoveTouch => _moveTouchId >= 0;
        public bool HasAimTouch => _aimTouchId >= 0;
        public bool HasAbilityTouch => _abilityTouchId >= 0;
        public bool TouchModeActive => HasMoveTouch || HasAimTouch || HasAbilityTouch || Time.unscaledTime < _touchModeUntil;
        public Vector2 MoveTouchOrigin => _moveOrigin;
        public Vector2 MoveTouchPosition => _movePosition;
        public Vector2 AimTouchOrigin => _aimOrigin;
        public Vector2 AimTouchPosition => _aimPosition;
        public float VirtualStickRadius => GetEffectiveStickRadius();

        private int _moveTouchId = -1;
        private int _aimTouchId = -1;
        private int _abilityTouchId = -1;
        private Vector2 _moveOrigin;
        private Vector2 _movePosition;
        private Vector2 _aimOrigin;
        private Vector2 _aimPosition;
        private Rect _abilityTouchRegion;
        private bool _abilityTouchRegionValid;
        private bool _abilityQueued;
        private float _touchModeUntil;

        private static bool PreferTouchControls => Application.isMobilePlatform || Touchscreen.current != null;

        private void Awake()
        {
            if (Current != null && Current != this)
            {
                Destroy(gameObject);
                return;
            }

            Current = this;
        }

        private void OnEnable()
        {
            if (!EnhancedTouchSupport.enabled)
                EnhancedTouchSupport.Enable();
        }

        private void Update()
        {
            ReadDesktopAndGamepad();
            ReadTouches();
        }

        public void SetAbilityTouchRegion(Rect screenSpaceRegion)
        {
            _abilityTouchRegion = screenSpaceRegion;
            _abilityTouchRegionValid = screenSpaceRegion.width > 1f && screenSpaceRegion.height > 1f;
        }

        public bool ConsumeAbilityPress()
        {
            if (!_abilityQueued)
                return false;

            _abilityQueued = false;
            return true;
        }

        private void ReadDesktopAndGamepad()
        {
            var move = Vector2.zero;
            var gamepadAim = Vector2.zero;

            HasDirectionalAim = false;
            HasPointerAim = false;
            HasAim = false;
            FireHeld = false;
            Aim = Vector2.zero;

            if (Keyboard.current != null)
            {
                move.x = (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
                move.y = (Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed ? 1f : 0f);

                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    _abilityQueued = true;
            }

            if (Gamepad.current != null)
            {
                var left = Gamepad.current.leftStick.ReadValue();
                if (left.sqrMagnitude > move.sqrMagnitude)
                    move = left;

                gamepadAim = ShapeStick(Gamepad.current.rightStick.ReadValue(), aimDeadZone, aimResponseExponent);
                if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                    _abilityQueued = true;
            }

            Move = ShapeStick(move, moveDeadZone, moveResponseExponent);

            if (gamepadAim.sqrMagnitude > 0.0001f)
            {
                Aim = gamepadAim;
                HasDirectionalAim = true;
                HasAim = true;
                FireHeld = Gamepad.current != null && Gamepad.current.rightTrigger.ReadValue() > 0.35f;
            }

            // On a phone / Device Simulator the mouse often mirrors the simulated finger. Never let
            // that mirrored pointer become a second aim source; touch owns the mobile control scheme.
            if (!PreferTouchControls && Mouse.current != null)
            {
                AimScreenPosition = Mouse.current.position.ReadValue();
                HasPointerAim = true;
                HasAim = true;
                FireHeld |= Mouse.current.leftButton.isPressed;
            }
        }

        private void ReadTouches()
        {
            if (!PreferTouchControls)
                return;

            if (Touch.activeTouches.Count == 0)
            {
                _moveTouchId = -1;
                _aimTouchId = -1;
                _abilityTouchId = -1;
                Move = Vector2.zero;
                Aim = Vector2.zero;
                HasDirectionalAim = false;
                HasPointerAim = false;
                HasAim = false;
                FireHeld = false;
                return;
            }

            _touchModeUntil = Time.unscaledTime + 0.35f;
            Move = Vector2.zero;
            Aim = Vector2.zero;
            HasDirectionalAim = false;
            HasPointerAim = false;
            HasAim = false;
            FireHeld = false;

            var safe = Screen.safeArea;
            var controlBandTop = safe.yMin + safe.height * 0.62f;
            var radius = GetEffectiveStickRadius();

            foreach (var touch in Touch.activeTouches)
            {
                var id = touch.touchId;
                var position = touch.screenPosition;

                if (id == _abilityTouchId)
                {
                    if (touch.phase is UnityEngine.InputSystem.TouchPhase.Ended or UnityEngine.InputSystem.TouchPhase.Canceled)
                        _abilityTouchId = -1;
                    continue;
                }

                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    if (_abilityTouchRegionValid && _abilityTouchRegion.Contains(position) && _abilityTouchId < 0)
                    {
                        _abilityTouchId = id;
                        _abilityQueued = true;
                        continue;
                    }

                    if (position.y <= controlBandTop && position.x < safe.center.x && _moveTouchId < 0)
                    {
                        _moveTouchId = id;
                        _moveOrigin = position;
                        _movePosition = position;
                    }
                    else if (position.y <= controlBandTop && position.x >= safe.center.x && _aimTouchId < 0)
                    {
                        _aimTouchId = id;
                        _aimOrigin = position;
                        _aimPosition = position;
                    }
                }

                if (id == _moveTouchId)
                {
                    _movePosition = position;
                    Move = ShapeStick((_movePosition - _moveOrigin) / radius, moveDeadZone, moveResponseExponent);

                    if (touch.phase is UnityEngine.InputSystem.TouchPhase.Ended or UnityEngine.InputSystem.TouchPhase.Canceled)
                    {
                        _moveTouchId = -1;
                        Move = Vector2.zero;
                    }
                    continue;
                }

                if (id == _aimTouchId)
                {
                    _aimPosition = position;
                    var rawAim = Vector2.ClampMagnitude((_aimPosition - _aimOrigin) / radius, 1f);
                    Aim = ShapeStick(rawAim, aimDeadZone, aimResponseExponent);
                    HasDirectionalAim = Aim.sqrMagnitude > 0.0001f;
                    HasAim = HasDirectionalAim;
                    FireHeld = rawAim.magnitude >= aimFireThreshold;

                    if (touch.phase is UnityEngine.InputSystem.TouchPhase.Ended or UnityEngine.InputSystem.TouchPhase.Canceled)
                    {
                        _aimTouchId = -1;
                        Aim = Vector2.zero;
                        HasDirectionalAim = false;
                        HasAim = false;
                        FireHeld = false;
                    }
                }
            }
        }

        private float GetEffectiveStickRadius()
        {
            return Mathf.Clamp(Mathf.Max(virtualStickRadius, Screen.safeArea.height * 0.15f), 110f, 190f);
        }

        private static Vector2 ShapeStick(Vector2 raw, float deadZone, float exponent)
        {
            raw = Vector2.ClampMagnitude(raw, 1f);
            var magnitude = raw.magnitude;
            if (magnitude <= deadZone)
                return Vector2.zero;

            var normalized = Mathf.Clamp01((magnitude - deadZone) / Mathf.Max(0.001f, 1f - deadZone));
            normalized = Mathf.Pow(normalized, Mathf.Max(0.1f, exponent));
            return raw.normalized * normalized;
        }
    }
}
