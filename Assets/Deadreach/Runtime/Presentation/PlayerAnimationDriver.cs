using Kamilunavo.Deadreach.Combat;
using Kamilunavo.Deadreach.Input;
using Kamilunavo.Deadreach.Player;
using UnityEngine;

namespace Kamilunavo.Deadreach.Presentation
{
    [RequireComponent(typeof(PlayerMotor), typeof(Damageable))]
    public sealed class PlayerAnimationDriver : MonoBehaviour
    {
        private static readonly int SpeedId = Animator.StringToHash("Speed");
        private static readonly int IsMovingId = Animator.StringToHash("IsMoving");
        private static readonly int IsAimingId = Animator.StringToHash("IsAiming");
        private static readonly int IsDeadId = Animator.StringToHash("IsDead");
        private static readonly int HitId = Animator.StringToHash("Hit");

        [SerializeField] private Animator animator;
        [SerializeField, Min(0f)] private float speedDampTime = 0.08f;

        private PlayerMotor _motor;
        private Damageable _health;

        private void Awake()
        {
            _motor = GetComponent<PlayerMotor>();
            _health = GetComponent<Damageable>();

            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            if (_health != null)
            {
                _health.Damaged += HandleDamaged;
                _health.Died += HandleDied;
            }
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.Damaged -= HandleDamaged;
                _health.Died -= HandleDied;
            }
        }

        private void Update()
        {
            if (animator == null || _motor == null)
                return;

            var planarSpeed = new Vector2(_motor.Velocity.x, _motor.Velocity.z).magnitude;
            var input = DeadreachInput.Current;

            animator.SetFloat(SpeedId, planarSpeed, speedDampTime, Time.deltaTime);
            animator.SetBool(IsMovingId, _motor.IsMoving);
            animator.SetBool(IsAimingId, input != null && input.HasAim);
            animator.SetBool(IsDeadId, _health != null && _health.IsDead);
        }

        public void SetAnimator(Animator newAnimator)
        {
            animator = newAnimator;
        }

        private void HandleDamaged(DamageInfo info)
        {
            if (animator != null && !(_health?.IsDead ?? false))
                animator.SetTrigger(HitId);
        }

        private void HandleDied()
        {
            if (animator != null)
                animator.SetBool(IsDeadId, true);
        }
    }
}
