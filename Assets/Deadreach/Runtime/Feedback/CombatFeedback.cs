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

    public static class CombatFeedback
    {
        public static event Action<ShotFeedback> ShotFired;
        public static event Action<ImpactFeedback> Impacted;
        public static event Action<float> PlayerDamaged;
        public static event Action PlayerDied;

        public static void RaiseShot(ShotFeedback feedback) => ShotFired?.Invoke(feedback);
        public static void RaiseImpact(ImpactFeedback feedback) => Impacted?.Invoke(feedback);
        public static void RaisePlayerDamaged(float normalizedDamage) => PlayerDamaged?.Invoke(Mathf.Clamp01(normalizedDamage));
        public static void RaisePlayerDied() => PlayerDied?.Invoke();
    }
}
