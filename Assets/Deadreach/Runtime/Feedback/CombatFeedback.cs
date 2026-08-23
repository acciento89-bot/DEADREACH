using System;
using UnityEngine;

namespace Kamilunavo.Deadreach.Feedback
{
    public readonly struct ShotFeedback
    {
        public ShotFeedback(Vector3 origin, Vector3 endPoint, bool hitDamageable)
        {
            Origin = origin;
            EndPoint = endPoint;
            HitDamageable = hitDamageable;
        }

        public Vector3 Origin { get; }
        public Vector3 EndPoint { get; }
        public bool HitDamageable { get; }
    }

    public readonly struct ImpactFeedback
    {
        public ImpactFeedback(Vector3 point, Vector3 normal, bool hitDamageable)
        {
            Point = point;
            Normal = normal;
            HitDamageable = hitDamageable;
        }

        public Vector3 Point { get; }
        public Vector3 Normal { get; }
        public bool HitDamageable { get; }
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
