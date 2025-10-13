using Attributes;
using UnityEditor;
using UnityEngine;

namespace Freya.Part_I.Scripts
{
    /// <summary>
    /// Number lines and vectors
    /// </summary>
    public class PartI : MonoBehaviour
    {
        [SerializeField] private Transform m_A, m_B;
        [SerializeField, ReadOnly] private float m_ScalarProjection;
        [SerializeField, ReadOnly] private Vector3 m_VectorProjection;

        private void OnDrawGizmos()
        {
            // Draw unit circle
            Handles.color = Color.black;
            Handles.DrawWireDisc(Vector3.zero, Vector3.back, 1.0f, 2.0f);

            Vector3 a = m_A.position;
            Vector3 b = m_B.position;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, a);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(Vector3.zero, b);

            // Vector3 aNormalized = a / Mathf.Sqrt(a.x * a.x + a.y * a.y);
            Vector3 aNormalized = a.normalized;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(aNormalized, 0.05f);

            // Scalar projection
            // m_ScalarProjection = aNormalized.x * b.x + aNormalized.y * b.y + a.normalized.z * b.z;
            m_ScalarProjection = Vector3.Dot(aNormalized, b);

            // Vector projection
            // m_VectorProjection = aNormalized * m_ScalarProjection;
            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(Vector3.zero, m_VectorProjection);

            // Vector projection, if both of the vectors are not normalized
            m_VectorProjection = (Vector3.Dot(a, b) / Vector3.Dot(a, a)) * a;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vector3.zero, m_VectorProjection);
        }
    }
}