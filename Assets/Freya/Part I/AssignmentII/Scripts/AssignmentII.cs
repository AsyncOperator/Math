using UnityEngine;

namespace Freya.Part_I.AssignmentII.Scripts
{
    /// <summary>
    /// Bouncing laser
    /// </summary>
    public class AssignmentII : MonoBehaviour
    {
        [SerializeField] private Transform m_Laser;
        [SerializeField] private int m_BounceCount;
        [SerializeField] private float m_NormalLength;

        private void OnValidate()
        {
            m_BounceCount = Mathf.Clamp(m_BounceCount, 0, 100);
            m_NormalLength = Mathf.Max(0.05f, m_NormalLength);
        }

        private Vector3 Reflect(Vector3 inDir, Vector3 inNormal)
        {
            float scalarProjection = Vector3.Dot(inNormal, inDir); 
            Vector3 vectorProjection = scalarProjection * inNormal;
            return inDir + vectorProjection * -2.0f;
        }

        private void OnDrawGizmos()
        {
            Vector3 origin = m_Laser.position;
            Vector3 dir = m_Laser.right;

            for (int i = 0; i <= m_BounceCount; i++)
            {
                Ray ray = new Ray(origin, dir);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    Vector3 point = hitInfo.point; // Hit point
                    Vector3 normal = hitInfo.normal; // Surface normal

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(origin, point);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(point, point + normal * m_NormalLength); // Draw surface normal

                    // ## Solution 1 ##
                    // // For a hit to occur, we know that the dot product of the direction vector and the surface normal vector
                    // // must always be negative. This is way we are passing flipped second argument vector here.
                    // float scalarProjection = Vector3.Dot(normal, -(point - origin));
                    //
                    // // We are building equilateral triangle...
                    // Vector3 vectorProjection = point + normal * scalarProjection;
                    // Vector3 to = origin + (vectorProjection - origin) * 2.0f;

                    // // Update 'origin' and 'dir' variables for possible next iteration
                    // origin = point;
                    // dir = (to - origin).normalized;

                    // ## Solution 2 ## // Freya solution, this one is more fast, compact and elegant
                    
                    // Update 'origin' and 'dir' variables for possible next iteration
                    origin = point;
                    dir = Reflect(dir, normal);
                }
                else
                {
                    break;
                }
            }
        }
    }
}