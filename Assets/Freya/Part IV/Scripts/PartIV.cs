using UnityEngine;

namespace Freya.Part_IV.Scripts
{
    /// <summary>
    /// Interpolation & Point physics
    /// </summary>
    public class PartIV : MonoBehaviour
    {
        [SerializeField] private Transform m_A, m_B, m_C, m_P;

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
        }
    }
}