using UnityEngine;

namespace Kamilunavo.Deadreach.Player
{
    /// <summary>
    /// Runtime backstop for the generated Dead City. Physical world-bound colliders should prevent
    /// leaving the playspace; this guard recovers the player if a spawn/physics edge case still gets
    /// below the map or outside the authored rectangle.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerFallSafety : MonoBehaviour
    {
        [SerializeField] private Vector2 xBounds = new(-8.35f, 8.35f);
        [SerializeField] private Vector2 zBounds = new(-10.5f, 21.4f);
        [SerializeField] private float minimumY = -0.8f;
        [SerializeField] private float safeSampleMinimumY = -0.15f;

        private CharacterController _controller;
        private Vector3 _lastSafePosition;
        private bool _hasSafePosition;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _lastSafePosition = transform.position;
            _hasSafePosition = true;
        }

        private void LateUpdate()
        {
            var position = transform.position;
            var insideHorizontal = position.x >= xBounds.x && position.x <= xBounds.y &&
                                   position.z >= zBounds.x && position.z <= zBounds.y;

            if (insideHorizontal && position.y >= safeSampleMinimumY)
            {
                _lastSafePosition = position;
                _hasSafePosition = true;
                return;
            }

            if (position.y >= minimumY && insideHorizontal)
                return;

            var recovery = _hasSafePosition ? _lastSafePosition : new Vector3(0f, 0.15f, -7.5f);
            recovery.x = Mathf.Clamp(recovery.x, xBounds.x + 0.35f, xBounds.y - 0.35f);
            recovery.z = Mathf.Clamp(recovery.z, zBounds.x + 0.35f, zBounds.y - 0.35f);
            recovery.y = Mathf.Max(recovery.y, 0.12f);

            if (_controller != null)
                _controller.enabled = false;
            transform.position = recovery;
            if (_controller != null)
                _controller.enabled = true;

            Debug.LogWarning("DEADREACH world-safety recovered player before an out-of-bounds fall could continue.");
        }
    }
}
