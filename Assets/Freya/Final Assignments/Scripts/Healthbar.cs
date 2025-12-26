using UnityEditor;
using UnityEngine;

namespace Freya.Final_Assignments.Scripts
{
    public class Healthbar : MonoBehaviour
    {
        [SerializeField, Range(0.0f, 100.0f)] private float m_HealthPoints;
        [SerializeField] private float m_LowestThreshold, m_HighestThreshold;
        [SerializeField] private Color m_LowestColor, m_HighestColor;

        private void OnDrawGizmos()
        {
            Handles.matrix = transform.localToWorldMatrix;

            Handles.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            Handles.DrawLine(Vector3.zero, Vector3.right, 10.0f);

            float t = Mathf.InverseLerp(m_LowestThreshold, m_HighestThreshold, m_HealthPoints);
            Handles.color = Color.Lerp(m_LowestColor, m_HighestColor, t);

            float width = m_HealthPoints / 100.0f;
            Handles.DrawLine(Vector3.zero, Vector3.right * width, 10.0f);
        }
    }
}