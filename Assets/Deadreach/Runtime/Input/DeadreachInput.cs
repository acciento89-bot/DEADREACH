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
        [SerializeField, Range(0f, 0.5f)] private float moveDeadZone = 0.12f;
        [SerializeField, Range(0f, 0.5f)] private float aimDeadZone = 0.10f;
        [SerializeField, Range(0.5f, 2f)] private float moveResponseExponent = 0.92f;
        [SerializeField, Range(0.5f, 2f)] private float aimResponseExponent = 0.88f;
        [SerializeField, Range(1f, 2f)] private float touchCaptureMultiplier = 1.55f;

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
        public Vector2 MoveTouchOrigin => MoveStickCenter;
        public Vector2 MoveTouchPosition => _movePosition;
        public Vector2 AimTouchOrigin => AimStickCenter;
        public Vector2 AimTouchPosition => _aimPosition;
        public float VirtualStickRadius => GetEffectiveStickRadius();
        public Vector2 MoveStickCenter => GetStickCenter(false);
        public Vector2 AimStickCenter => GetStickCenter(true);

        private int _moveTouchId = -1;
        private int _aimTouchId = -1;
        private int _abilityTouchId = -1;
        private Vector2 _movePosition;
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
            // The visible button gets a generous invisible hit target. This makes the control reliable
            // on real phones without letting it overlap the lower-right aim/fire stick.
            var padding = Mathf.Max(14f, screenSpaceRegion.width * 0.18f);
            _abilityTouchRegion = new Rect(
                screenSpaceRegion.x - padding,
                screenSpaceRegion.y - padding,
                screenSpaceRegion.width + padding * 2f,
                screenSpaceRegion.height + padding * 2f);
            _abilityTouchRegionValid = _abilityTouchRegion.width > 1f && _abilityTouchRegion.height > 1f;
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

            // Device Simulator can mirror touch through the mouse pointer. Touch owns mobile aiming,
            // otherwise the simulated pointer would fight the right stick.
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

            var moveCenter = MoveStickCenter;
            var aimCenter = AimStickCenter;
            var radius = GetEffectiveStickRadius();
            var captureRadius = radius * touchCaptureMultiplier;

            if (Touch.activeTouches.Count == 0)
            {
                ResetTouchControls(moveCenter, aimCenter);
                return;
            }

            _touchModeUntil = Time.unscaledTime + 0.35f;
            Move = Vector2.zero;
            Aim = Vector2.zero;
            HasDirectionalAim = false;
            HasPointerAim = false;
            HasAim = false;
            FireHeld = false;

            foreach (var touch in Touch.activeTouches)
            {
                var id = touch.touchId;
                var position = touch.screenPosition;
                var ended = touch.phase is UnityEngine.InputSystem.TouchPhase.Ended or UnityEngine.InputSystem.TouchPhase.Canceled;

                if (id == _abilityTouchId)
                {
                    if (ended)
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

                    if (_moveTouchId < 0 && Vector2.Distance(position, moveCenter) <= captureRadius)
                    {
                        _moveTouchId = id;
                        _movePosition = position;
                    }
                    else if (_aimTouchId < 0 && Vector2.Distance(position, aimCenter) <= captureRadius)
                    {
                        _aimTouchId = id;
                        _aimPosition = position;
                    }
                }

                if (id == _moveTouchId)
                {
                    _movePosition = position;
                    Move = ShapeStick((_movePosition - moveCenter) / radius, moveDeadZone, moveResponseExponent);

                    if (ended)
                    {
                        _moveTouchId = -1;
                        _movePosition = moveCenter;
                        Move = Vector2.zero;
                    }
                    continue;
                }

                if (id == _aimTouchId)
                {
                    _aimPosition = position;
                    Aim = ShapeStick((_aimPosition - aimCenter) / radius, aimDeadZone, aimResponseExponent);
                    HasDirectionalAim = Aim.sqrMagnitude > 0.0001f;
                    HasAim = HasDirectionalAim;
                    // Twin-stick shooter behavior: once the right stick leaves its deadzone it fires.
                    FireHeld = HasDirectionalAim;

                    if (ended)
                    {
                        _aimTouchId = -1;
                        _aimPosition = aimCenter;
                        Aim = Vector2.zero;
                        HasDirectionalAim = false;
                        HasAim = false;
                        FireHeld = false;
                    }
                }
            }
        }

        private void ResetTouchControls(Vector2 moveCenter, Vector2 aimCenter)
        {
            _moveTouchId = -1;
            _aimTouchId = -1;
            _abilityTouchId = -1;
            _movePosition = moveCenter;
            _aimPosition = aimCenter;
            Move = Vector2.zero;
            Aim = Vector2.zero;
            HasDirectionalAim = false;
            HasPointerAim = false;
            HasAim = false;
            FireHeld = false;
        }

        private Vector2 GetStickCenter(bool right)
        {
            var safe = Screen.safeArea;
            var radius = GetEffectiveStickRadius();
            var bottomPadding = Mathf.Max(18f, safe.height * 0.028f);
            var sidePadding = Mathf.Max(22f, safe.width * 0.016f);
            return right
                ? new Vector2(safe.xMax - radius - sidePadding, safe.yMin + radius + bottomPadding)
                : new Vector2(safe.xMin + radius + sidePadding, safe.yMin + radius + bottomPadding);
        }

        private float GetEffectiveStickRadius()
        {
            return Mathf.Clamp(Mathf.Max(virtualStickRadius, Screen.safeArea.height * 0.145f), 100f, 170f);
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
