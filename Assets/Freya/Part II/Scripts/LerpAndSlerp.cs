using UnityEditor;
using UnityEngine;

namespace Freya.Part_II.Scripts
{
    public class LerpAndSlerp : MonoBehaviour
    {
        [SerializeField] private Transform m_A, m_B;
        [SerializeField, Range(0.0f, 1.0f)] private float m_TValue;
        
        private void OnDrawGizmosSelected()
        {
            Vector3 aNormalized = m_A.position.normalized;
            Vector3 bNormalized = m_B.position.normalized;

            Handles.DrawWireDisc(Vector3.zero, Vector3.back, 1.0f, 2.0f);
            Gizmos.color = Color.black;
            Gizmos.DrawLine(Vector3.zero, aNormalized);
            Gizmos.DrawLine(Vector3.zero, bNormalized);

            Vector3 lerpValue = Vector3.Lerp(aNormalized, bNormalized, m_TValue);
            Vector3 slerpValue = Vector3.Slerp(aNormalized, bNormalized, m_TValue);

            Gizmos.color = Handles.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, lerpValue);
            Handles.DrawDottedLine(lerpValue, lerpValue.normalized, 0.2f);
            Gizmos.DrawWireSphere(lerpValue, 0.02f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vector3.zero, slerpValue);
            Gizmos.DrawWireSphere(slerpValue, 0.02f);
        }
    }
}