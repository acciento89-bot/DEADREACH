using Kamilunavo.Deadreach.AI;
using Kamilunavo.Deadreach.Combat;
using UnityEngine;

namespace Kamilunavo.Deadreach.Presentation
{
    [RequireComponent(typeof(InfectedChaser), typeof(CharacterController), typeof(Damageable))]
    public sealed class InfectedAnimationDriver : MonoBehaviour
    {
        private static readonly int SpeedId = Animator.StringToHash("Speed");
        private static readonly int IsDeadId = Animator.StringToHash("IsDead");
        private static readonly int AttackId = Animator.StringToHash("Attack");
        private static readonly int HitId = Animator.StringToHash("Hit");

        [SerializeField] private Animator animator;
        [SerializeField, Min(0f)] private float speedDampTime = 0.08f;

        private InfectedChaser _brain;
        private CharacterController _controller;
        private Damageable _health;

        private void Awake()
        {
            _brain = GetComponent<InfectedChaser>();
            _controller = GetComponent<CharacterController>();
            _health = GetComponent<Damageable>();

            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            if (_brain != null)
                _brain.Attacked += HandleAttack;

            if (_health != null)
            {
                _health.Damaged += HandleDamaged;
                _health.Died += HandleDied;
            }
        }

        private void OnDisable()
        {
            if (_brain != null)
                _brain.Attacked -= HandleAttack;

            if (_health != null)
            {
                _health.Damaged -= HandleDamaged;
                _health.Died -= HandleDied;
            }
        }

        private void Update()
        {
            if (animator == null || _controller == null)
                return;

            var velocity = _controller.velocity;
            var planarSpeed = new Vector2(velocity.x, velocity.z).magnitude;
            animator.SetFloat(SpeedId, planarSpeed, speedDampTime, Time.deltaTime);
            animator.SetBool(IsDeadId, _health != null && _health.IsDead);
        }

        public void SetAnimator(Animator newAnimator)
        {
            animator = newAnimator;
        }

        private void HandleAttack()
        {
            if (animator != null)
                animator.SetTrigger(AttackId);
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
