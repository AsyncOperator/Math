using UnityEditor;
using UnityEngine;

namespace Freya.Final_Assignments.Scripts
{
    public class BudgetCutsInventory : MonoBehaviour
    {
        [SerializeField] private float[] m_ItemsRadii;
        [SerializeField] private float m_Distance;

        private float GetAngleForRadii(float radii)
        {
            float a = Mathf.Asin(radii * 0.5f / m_Distance);

            return 4 * a * Mathf.Rad2Deg;
        }

        private void OnDrawGizmos()
        {
            float angle = 0.0f;
            for (int i = 0; i < m_ItemsRadii.Length; i++)
            {
                angle += GetAngleForRadii(m_ItemsRadii[i]);
            }

            Handles.matrix = transform.localToWorldMatrix;
            Handles.DrawWireArc(Vector3.zero, Vector3.back, Quaternion.AngleAxis(angle * 0.5f, Vector3.forward) * Vector3.up, angle, m_Distance);

            angle *= 0.5f;

            for (int i = 0; i < m_ItemsRadii.Length; i++)
            {
                float radii = m_ItemsRadii[i];
                float halfAngle = GetAngleForRadii(radii) * 0.5f;
                angle -= halfAngle;
                Vector3 center = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up * m_Distance;
                Handles.DrawWireDisc(center, Vector3.back, radii);
                angle -= halfAngle;
            }
        }
    }
}