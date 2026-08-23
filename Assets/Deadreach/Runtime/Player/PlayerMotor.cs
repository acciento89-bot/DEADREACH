using Kamilunavo.Deadreach.Input;
using UnityEngine;

namespace Kamilunavo.Deadreach.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 5.4f;
        [SerializeField, Min(0f)] private float acceleration = 22f;
        [SerializeField, Min(0f)] private float gravity = 24f;
        [SerializeField, Min(0f)] private float movementTurnSpeed = 14f;

        private CharacterController _controller;
        private Vector3 _planarVelocity;
        private float _verticalVelocity;
        private Camera _camera;
        private float _moveSpeedMultiplier = 1f;

        public Vector3 Velocity => _planarVelocity + Vector3.up * _verticalVelocity;
        public bool IsMoving => _planarVelocity.sqrMagnitude > 0.04f;
        public float EffectiveMoveSpeed => moveSpeed * _moveSpeedMultiplier;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        public void SetMoveSpeedMultiplier(float multiplier)
        {
            _moveSpeedMultiplier = Mathf.Clamp(multiplier, 0.5f, 1.6f);
        }

        private void Update()
        {
            var input = DeadreachInput.Current;
            var moveInput = input != null ? input.Move : Vector2.zero;

            var forward = _camera != null ? _camera.transform.forward : Vector3.forward;
            var right = _camera != null ? _camera.transform.right : Vector3.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            var desiredDirection = Vector3.ClampMagnitude(forward * moveInput.y + right * moveInput.x, 1f);
            var desiredVelocity = desiredDirection * EffectiveMoveSpeed;

            // Mobile twin-stick movement must feel immediate. The old desktop acceleration made the
            // player feel like it kept sliding after the thumb changed direction or returned to center.
            var touchControl = input != null && input.TouchModeActive;
            var response = touchControl
                ? (desiredDirection.sqrMagnitude > 0.001f ? 52f : 72f)
                : acceleration;
            _planarVelocity = Vector3.MoveTowards(_planarVelocity, desiredVelocity, response * Time.deltaTime);

            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;
            else
                _verticalVelocity -= gravity * Time.deltaTime;

            _controller.Move((_planarVelocity + Vector3.up * _verticalVelocity) * Time.deltaTime);

            if (desiredDirection.sqrMagnitude > 0.05f && (input == null || !input.HasAim))
            {
                var targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, movementTurnSpeed * Time.deltaTime);
            }
        }
    }
}
