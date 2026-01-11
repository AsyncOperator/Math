using UnityEditor;
using UnityEngine;

namespace Freya.Final_Assignments.Scripts
{
    public class BudgetCutsInventory : MonoBehaviour
    {
        [SerializeField] private float[] m_ItemsRadii;
        [SerializeField] private float m_ArcRadii;

        private void OnValidate()
        {
            int length = m_ItemsRadii.Length;
            for (int i = 0; i < length; i++)
            {
                m_ItemsRadii[i] = Mathf.Max(m_ItemsRadii[i], 0.0f);
            }

            m_ArcRadii = Mathf.Max(m_ArcRadii, 1.0f);
        }

        private float LawOfCosines(float sideA, float sideB, float sideC)
        {
            float cosC = (sideA * sideA + sideB * sideB - sideC * sideC) / (2 * sideA * sideB);
            return Mathf.Acos(Mathf.Clamp(cosC, -1.0f, 1.0f)) * Mathf.Rad2Deg;
        }

        private float GetAngleBetweenCircles(float radii0, float radii1)
        {
            float sum = radii0 + radii1;
            return LawOfCosines(m_ArcRadii, m_ArcRadii, sum);
        }

        private void OnDrawGizmos()
        {
            Handles.matrix = transform.localToWorldMatrix;

            float angle = 0.0f;
            for (int i = 0; i < m_ItemsRadii.Length - 1; i++)
            {
                float radii0 = m_ItemsRadii[i];
                float radii1 = m_ItemsRadii[i + 1];
                angle += GetAngleBetweenCircles(radii0, radii1);
            }

            float offset = GetAngleBetweenCircles(m_ItemsRadii[0], Mathf.Epsilon);
            angle += offset + GetAngleBetweenCircles(m_ItemsRadii[^1], Mathf.Epsilon);

            Vector3 startDirection = Quaternion.AngleAxis(angle * 0.5f, Vector3.forward) * Vector3.up;
            Vector3 endDirection = Quaternion.AngleAxis(-angle * 0.5f, Vector3.forward) * Vector3.up;
            Handles.DrawLine(Vector3.zero, startDirection * m_ArcRadii);
            Handles.DrawLine(Vector3.zero, endDirection * m_ArcRadii);
            Handles.DrawWireArc(Vector3.zero, Vector3.back, startDirection, angle, m_ArcRadii);

            angle = angle * 0.5f - offset;

            for (int i = 0; i < m_ItemsRadii.Length; i++)
            {
                float radii0 = m_ItemsRadii[i];
                float radii1 = i < m_ItemsRadii.Length - 1 ? m_ItemsRadii[i + 1] : 0.0f;

                Vector3 direction = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;
                Vector3 center = direction * m_ArcRadii;
                Handles.DrawWireDisc(center, Vector3.back, radii0);

                angle -= GetAngleBetweenCircles(radii0, radii1);
            }
        }
    }
}