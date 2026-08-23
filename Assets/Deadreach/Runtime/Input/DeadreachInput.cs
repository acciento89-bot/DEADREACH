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

        public Vector2 Move { get; private set; }
        public Vector2 AimScreenPosition { get; private set; }
        public bool HasAim { get; private set; }
        public bool FireHeld { get; private set; }

        public bool HasMoveTouch => _moveTouchId >= 0;
        public bool HasAimTouch => _aimTouchId >= 0;
        public Vector2 MoveTouchOrigin => _moveOrigin;
        public Vector2 MoveTouchPosition => _movePosition;
        public float VirtualStickRadius => virtualStickRadius;

        private int _moveTouchId = -1;
        private int _aimTouchId = -1;
        private Vector2 _moveOrigin;
        private Vector2 _movePosition;

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

        private void ReadDesktopAndGamepad()
        {
            var move = Vector2.zero;

            if (Keyboard.current != null)
            {
                move.x = (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
                move.y = (Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed ? 1f : 0f);
            }

            if (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().sqrMagnitude > move.sqrMagnitude)
                move = Gamepad.current.leftStick.ReadValue();

            Move = Vector2.ClampMagnitude(move, 1f);

            if (Mouse.current != null)
            {
                AimScreenPosition = Mouse.current.position.ReadValue();
                HasAim = true;
                FireHeld = Mouse.current.leftButton.isPressed;
            }
            else
            {
                HasAim = false;
                FireHeld = Gamepad.current != null && Gamepad.current.rightTrigger.ReadValue() > 0.35f;
            }
        }

        private void ReadTouches()
        {
            if (Touch.activeTouches.Count == 0)
            {
                _moveTouchId = -1;
                _aimTouchId = -1;
                return;
            }

            foreach (var touch in Touch.activeTouches)
            {
                var id = touch.touchId;
                var position = touch.screenPosition;

                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    if (position.x < Screen.width * 0.5f && _moveTouchId < 0)
                    {
                        _moveTouchId = id;
                        _moveOrigin = position;
                        _movePosition = position;
                    }
                    else if (_aimTouchId < 0)
                    {
                        _aimTouchId = id;
                    }
                }

                if (id == _moveTouchId)
                {
                    _movePosition = position;
                    Move = Vector2.ClampMagnitude((position - _moveOrigin) / virtualStickRadius, 1f);
                    if (touch.phase is UnityEngine.InputSystem.TouchPhase.Ended or UnityEngine.InputSystem.TouchPhase.Canceled)
                    {
                        _moveTouchId = -1;
                        Move = Vector2.zero;
                    }
                }

                if (id == _aimTouchId)
                {
                    AimScreenPosition = position;
                    HasAim = true;
                    FireHeld = touch.phase is not UnityEngine.InputSystem.TouchPhase.Ended and not UnityEngine.InputSystem.TouchPhase.Canceled;

                    if (!FireHeld)
                    {
                        _aimTouchId = -1;
                        HasAim = Mouse.current != null;
                    }
                }
            }
        }
    }
}
