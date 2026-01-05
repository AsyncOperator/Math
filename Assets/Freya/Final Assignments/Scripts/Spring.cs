using UnityEditor;
using UnityEngine;

namespace Freya.Final_Assignments.Scripts
{
    public class Spring : MonoBehaviour
    {
        private const float TAU = Mathf.PI * 2;

        [Header("Coil spring")]
        [SerializeField] private float m_Radius;
        [SerializeField] private float m_Height;
        [SerializeField] private int m_TurnsCount;
        [SerializeField] private Color m_ColorA, m_ColorB;

        [Header("Donut spring")]
        [SerializeField] private float m_MajorRadius, m_MinorRadius;
        [SerializeField] private int m_SpiralsCount;

        private Vector3[] m_Points;
        private Color[] m_Colors;

        private void DrawCoilSpring(Matrix4x4 matrix)
        {
            Handles.matrix = matrix;

            const int detail = 100;

            if (m_Points == null || m_Points.Length != detail)
            {
                m_Points = new Vector3[detail];
            }

            if (m_Colors == null || m_Colors.Length != detail)
            {
                m_Colors = new Color[detail];
            }

            for (int i = 0; i < detail; i++)
            {
                float t = i / (detail - 1.0f);

                float angleInTurns = m_TurnsCount * t;
                float heightT = Mathf.InverseLerp(0.0f, m_TurnsCount, angleInTurns);

                float angleInRadians = angleInTurns * TAU;
                Vector3 point = new Vector3(Mathf.Cos(angleInRadians) * m_Radius, heightT * m_Height, Mathf.Sin(angleInRadians) * m_Radius);
                m_Points[i] = point;

                m_Colors[i] = Color.Lerp(m_ColorA, m_ColorB, t);
            }

            Handles.DrawAAPolyLine(m_Colors, m_Points);
        }

        private void DrawDonutSpring(Matrix4x4 matrix)
        {
            Handles.matrix = matrix;

            const int detail = 2_500;

            using (new Handles.DrawingScope(Color.red))
            {
                Handles.DrawWireDisc(Vector3.zero, Vector3.up, m_MajorRadius);
            }

            Vector3 prev = GetPoint(0.0f);
            for (int i = 1; i < detail; i++)
            {
                float t = i / (detail - 1.0f);

                Vector3 point = GetPoint(t);
                Handles.DrawLine(prev, point);
                prev = point;
            }

            Vector3 GetPoint(float t)
            {
                float angleInRadians = t * TAU;

                Vector3 point = new Vector3(Mathf.Cos(angleInRadians) * m_MajorRadius, 0.0f, Mathf.Sin(angleInRadians) * m_MajorRadius);

                // Either is fine
                // Option 1
                // Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0.0f, -angleInRadians * Mathf.Rad2Deg, 0.0f));
                float angleInTurns = m_SpiralsCount * t;
                // Vector3 spiralPoint = rotationMatrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(angleInTurns * TAU) * m_MinorRadius, Mathf.Sin(angleInTurns * TAU) * m_MinorRadius, 0.0f));

                // Option 2
                Quaternion rotation = Quaternion.Euler(0.0f, -angleInRadians * Mathf.Rad2Deg, 0.0f);
                Vector3 spiralPoint = rotation * new Vector3(Mathf.Cos(angleInTurns * TAU) * m_MinorRadius, Mathf.Sin(angleInTurns * TAU) * m_MinorRadius, 0.0f);

                return point + spiralPoint;
            }
        }

        private void OnDrawGizmos()
        {
            Matrix4x4 matrix = transform.localToWorldMatrix;

            DrawCoilSpring(matrix);

            // Give some free space to draw donut spring
            DrawDonutSpring(matrix * Matrix4x4.Translate(Vector3.right * 5.0f));
        }
    }
}