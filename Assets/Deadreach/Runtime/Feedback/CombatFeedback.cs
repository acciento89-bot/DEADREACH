using System;
using UnityEngine;

namespace Kamilunavo.Deadreach.Feedback
{
    public readonly struct ShotFeedback
    {
        public ShotFeedback(Vector3 origin, Vector3 endPoint, bool hitDamageable, bool critical, float tracerDuration, float tracerWidth, float hapticStrength)
        {
            Origin = origin;
            EndPoint = endPoint;
            HitDamageable = hitDamageable;
            Critical = critical;
            TracerDuration = tracerDuration;
            TracerWidth = tracerWidth;
            HapticStrength = hapticStrength;
        }

        public Vector3 Origin { get; }
        public Vector3 EndPoint { get; }
        public bool HitDamageable { get; }
        public bool Critical { get; }
        public float TracerDuration { get; }
        public float TracerWidth { get; }
        public float HapticStrength { get; }
    }

    public readonly struct ImpactFeedback
    {
        public ImpactFeedback(Vector3 point, Vector3 normal, bool hitDamageable, bool critical)
        {
            Point = point;
            Normal = normal;
            HitDamageable = hitDamageable;
            Critical = critical;
        }

        public Vector3 Point { get; }
        public Vector3 Normal { get; }
        public bool HitDamageable { get; }
        public bool Critical { get; }
    }

    public enum CombatAbilityKind
    {
        FieldPatch = 0,
        VectorDash = 1,
        Shockwave = 2
    }

    public readonly struct AbilityImpactFeedback
    {
        public AbilityImpactFeedback(CombatAbilityKind kind, Vector3 origin, Vector3 endPoint, Vector3 direction, float radius)
        {
            Kind = kind;
            Origin = origin;
            EndPoint = endPoint;
            Direction = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward;
            Radius = Mathf.Max(0f, radius);
        }

        public CombatAbilityKind Kind { get; }
        public Vector3 Origin { get; }
        public Vector3 EndPoint { get; }
        public Vector3 Direction { get; }
        public float Radius { get; }
    }

    public enum EnemySpecialKind
    {
        RunnerBurst = 0,
        BruteSlam = 1,
        StalkerFlank = 2
    }

    public readonly struct EnemySpecialImpactFeedback
    {
        public EnemySpecialImpactFeedback(EnemySpecialKind kind, Vector3 origin, Vector3 endPoint, Vector3 direction, float radius)
        {
            Kind = kind;
            Origin = origin;
            EndPoint = endPoint;
            Direction = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward;
            Radius = Mathf.Max(0f, radius);
        }

        public EnemySpecialKind Kind { get; }
        public Vector3 Origin { get; }
        public Vector3 EndPoint { get; }
        public Vector3 Direction { get; }
        public float Radius { get; }
    }

    public static class CombatFeedback
    {
        public static event Action<ShotFeedback> ShotFired;
        public static event Action<ImpactFeedback> Impacted;
        public static event Action<AbilityImpactFeedback> AbilityActivated;
        public static event Action<EnemySpecialImpactFeedback> EnemySpecialActivated;
        public static event Action<float> PlayerDamaged;
        public static event Action PlayerDied;

        public static void RaiseShot(ShotFeedback feedback) => ShotFired?.Invoke(feedback);
        public static void RaiseImpact(ImpactFeedback feedback) => Impacted?.Invoke(feedback);
        public static void RaiseAbility(AbilityImpactFeedback feedback) => AbilityActivated?.Invoke(feedback);
        public static void RaiseEnemySpecial(EnemySpecialImpactFeedback feedback) => EnemySpecialActivated?.Invoke(feedback);
        public static void RaisePlayerDamaged(float normalizedDamage) => PlayerDamaged?.Invoke(Mathf.Clamp01(normalizedDamage));
        public static void RaisePlayerDied() => PlayerDied?.Invoke();
    }
}
