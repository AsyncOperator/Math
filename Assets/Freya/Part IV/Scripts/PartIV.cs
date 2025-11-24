using Attributes;
using UnityEngine;

namespace Freya.Part_IV.Scripts
{
    /// <summary>
    /// Interpolation & Point physics
    /// </summary>
    public class PartIV : MonoBehaviour
    {
        [Header("Wedge product")]
        [SerializeField] private Transform m_A, m_B, m_C, m_P;

        [Header("Interpolation")]
        [SerializeField] private float m_InMin;

        [SerializeField] private float m_InMax;
        [SerializeField] private float m_OutMin;
        [SerializeField] private float m_OutMax;
        [SerializeField] private float m_Value;
        [SerializeField, ReadOnly] private float m_RemappedValue;

        [Header("Bezier Curve")]
        [SerializeField] private Transform m_P0;

        [SerializeField] private Transform m_P1;
        [SerializeField] private Transform m_P2;
        [SerializeField] private Transform m_P3;
        [SerializeField, Range(0.0f, 1.0f)] private float m_TValue;

        [Header("Movement")]
        [SerializeField] private Transform m_Cube;

        [SerializeField] private float m_AccMag;
        [SerializeField] private float m_Drag;

        private Vector3 m_Vel, m_Acc;

        private void OnValidate()
        {
            m_RemappedValue = Remap(m_InMin, m_InMax, m_OutMin, m_OutMax, m_Value);
        }

        private void Update()
        {
            m_Acc = Vector3.zero;

            TestInput(KeyCode.W, Vector3.up);
            TestInput(KeyCode.S, Vector3.down);
            TestInput(KeyCode.A, Vector3.left);
            TestInput(KeyCode.D, Vector3.right);

            m_Acc = m_Acc.normalized * m_AccMag;

            float dt = Time.deltaTime;
            m_Vel += m_Acc * dt;
            m_Cube.position += m_Vel * dt;

            void TestInput(KeyCode key, Vector3 v)
            {
                if (Input.GetKey(key))
                {
                    m_Acc += v;
                }
            }
        }

        private void FixedUpdate()
        {
            m_Vel /= m_Drag;
        }

        private float Remap(float iMin, float iMax, float oMin, float oMax, float value)
        {
            float t = Mathf.InverseLerp(iMin, iMax, value);
            return Mathf.Lerp(oMin, oMax, t);
        }

        private Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 a = Vector3.Lerp(p0, p1, t);
            Vector3 b = Vector3.Lerp(p1, p2, t);
            Vector3 c = Vector3.Lerp(p2, p3, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);

            return Vector3.Lerp(d, e, t);
        }

        private float Wedge(Vector2 v1, Vector2 v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }

        private bool IsTriangleContains(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
        {
            bool ab = GetSide(a, b, p);
            bool bc = GetSide(b, c, p);
            bool ca = GetSide(c, a, p);

            return ab == bc && bc == ca;
        }

        private bool GetSide(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 ab = b - a;
            Vector2 ap = p - a;
            return Wedge(ab, ap) > 0.0f;
        }

        private void OnDrawGizmos()
        {
            Vector2 a = m_A.position;
            Vector2 b = m_B.position;
            Vector2 c = m_C.position;
            Vector2 p = m_P.position;

            Gizmos.color = IsTriangleContains(a, b, c, p) ? Color.red : Color.white;

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, a);
            Gizmos.DrawWireSphere(p, 0.02f);

            // Draw vel and acc vector
            Gizmos.color = Color.red;
            Gizmos.DrawRay(m_Cube.position, m_Vel);
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(m_Cube.position, m_Acc);

            // Draw cubic bezier curve, it's called cubic since we are doing it on 4 points
            Vector3 p0 = m_P0.position;
            Vector3 p1 = m_P1.position;
            Vector3 p2 = m_P2.position;
            Vector3 p3 = m_P3.position;

            Gizmos.color = Color.white;
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawWireSphere(GetBezierPoint(p0, p1, p2, p3, m_TValue), 0.1f);

            // Visualize bezier curve
            const int detail = 32;
            Vector3 prev = p0;
            for (int i = 0; i < detail; i++)
            {
                float t = (i + 1) / (float)detail;
                Vector3 point = GetBezierPoint(p0, p1, p2, p3, t);

                Gizmos.DrawLine(prev, point);
                prev = point;
            }
        }
    }
}