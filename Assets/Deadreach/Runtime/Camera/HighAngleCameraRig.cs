using Kamilunavo.Deadreach.Player;
using UnityEngine;

namespace Kamilunavo.Deadreach.CameraSystem
{
    [RequireComponent(typeof(Camera))]
    public sealed class HighAngleCameraRig : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(0f, 13.5f, -10.5f);
        [SerializeField] private Vector3 lookOffset = new(0f, 1.1f, 0f);
        [SerializeField, Min(0f)] private float positionSharpness = 10f;
        [SerializeField, Min(0f)] private float rotationSharpness = 12f;

        public void SetTarget(Transform value) => target = value;

        private void Start()
        {
            if (target == null)
            {
                var player = FindFirstObjectByType<PlayerMotor>();
                if (player != null)
                    target = player.transform;
            }

            if (target != null)
                Snap();
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            var desiredPosition = target.position + offset;
            var positionT = 1f - Mathf.Exp(-positionSharpness * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, positionT);

            var direction = target.position + lookOffset - transform.position;
            if (direction.sqrMagnitude < 0.001f)
                return;

            var desiredRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            var rotationT = 1f - Mathf.Exp(-rotationSharpness * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationT);
        }

        public void Snap()
        {
            if (target == null)
                return;

            transform.position = target.position + offset;
            transform.rotation = Quaternion.LookRotation((target.position + lookOffset - transform.position).normalized, Vector3.up);
        }
    }
}
