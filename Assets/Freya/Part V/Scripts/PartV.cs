using UnityEngine;

namespace Freya.Part_V.Scripts
{
    /// <summary>
    /// Trajectory
    /// </summary>
    public class PartV : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_Rigidbody;
        [SerializeField] private float m_Speed;

        private Vector3 Position => transform.position;
        private Vector3 Velocity => transform.right * m_Speed;
        private Vector3 Acceleration => Physics.gravity;

        private void Awake()
        {
            m_Rigidbody.velocity = Velocity;
        }

        private Vector3 GetPointAtTrajectory(float t)
        {
            return Position + Velocity * t + (Acceleration * 0.5f) * t * t;
        }

        private void OnDrawGizmos()
        {
            const int detail = 80;
            const float total_time = 3.0f;

            Vector3 prev = GetPointAtTrajectory(0.0f);

            for (int i = 1; i < detail; i++)
            {
                float t = i / (detail - 1.0f);
                float time = t * total_time;
                Vector3 point = GetPointAtTrajectory(time);

                Gizmos.DrawLine(prev, point);
                prev = point;
            }
        }
    }
}