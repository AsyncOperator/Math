using UnityEditor;
using UnityEngine;

namespace Freya.Part_III.Scripts
{
    /// <summary>
    /// Trigonometry
    /// </summary>
    public class PartIII : MonoBehaviour
    {
        [SerializeField, Range(0.0f, 360.0f)] private float m_AngleInDegrees;

        // Euler angles order: Y -> X -> Z

        private void OnDrawGizmos()
        {
            float angleInRadians = m_AngleInDegrees * Mathf.Deg2Rad;
            Vector2 v = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

            Gizmos.DrawLine(Vector2.zero, v);
            Handles.DrawWireDisc(Vector2.zero, Vector3.back, 1.0f); // Unit circle
        }
    }
}