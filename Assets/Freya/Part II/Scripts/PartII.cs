using UnityEngine;

namespace Freya.Part_II.Scripts
{
    /// <summary>
    /// Matrices and cross product 
    /// </summary>
    public class PartII : MonoBehaviour
    {
        [SerializeField] private Transform m_Transform;
        [SerializeField] private Transform m_PlayerShip, m_EnemyShip;

        [SerializeField] private MatrixMonitorUI m_MatrixMonitorUI;
        [SerializeField] private Vector3 m_LocalSpaceCoords;

        [SerializeField] private Transform m_TurretPlayerLookDirectionRepresent, m_Turret;
        [SerializeField] private bool m_TurretDrawLocalSpaceInstead;

        [SerializeField] private Transform m_CrossA, m_CrossB;

        private Matrix4x4 LocalToWorldMatrix => m_Transform.localToWorldMatrix;

        private void Start()
        {
            UpdateMonitorView();
        }

        [ContextMenu("Update matrix monitor view")]
        private void UpdateMonitorView()
        {
            m_MatrixMonitorUI.UpdateView(LocalToWorldMatrix);
        }

        private void OnDrawGizmos()
        {
            // Vector3 v = default;
            // // NOTE: M -> m_Transform.localToWorldMatrix and (M^-1, **inverse of M**) -> m_Transform.worldToLocalMatrix
            // // NOTE: Point(s) are specific to a space thus they represent exact location, however vector(s) can be located anywhere in space, so this is why TransformPoint/InverseTransformPoint takes position into consideration
            // // From local space to world space
            // m_Transform.TransformPoint(v); // Equivalent of M*(new Vector4(v.x, v.y, v.z, 1.0f)); Takes position into consideration
            // // From world space to local space
            // m_Transform.InverseTransformPoint(v); // Equivalent of (M^-1)*(new Vector4(v.x, v.y, v.z, 1.0f)); Takes position into consideration
            //
            // // From local space to world space
            // m_Transform.TransformDirection(v); // Equivalent of M*(new Vector4(v.x, v.y, v.z, 0.0f)); DO NOT take position into consideration and where v vector is normalized
            // // From world space to local space
            // m_Transform.InverseTransformDirection(v); // Equivalent of (M^-1)*(new Vector4(v.x, v.y, v.z, 0.0f)); DO NOT take position into consideration and where v vector is normalized
            //
            // // From local space to world space
            // m_Transform.TransformVector(v); // Equivalent of M*(new Vector4(v.x, v.y, v.z, 0.0f)); DO NOT take position into consideration but take scale into consideration
            // // From world space to local space
            // m_Transform.InverseTransformVector(v); // Equivalent of (M^-1)*(new Vector4(v.x, v.y, v.z, 0.0f)); DO NOT take position into consideration but take scale into consideration

            Vector3 localSpacePosition = m_PlayerShip.InverseTransformPoint(m_EnemyShip.position);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(m_PlayerShip.position, m_PlayerShip.TransformPoint(localSpacePosition));

            // The direction vector should be normalized, Unity do not handle it for you
            Vector3 worldSpaceDirection = (m_EnemyShip.position - m_PlayerShip.position).normalized; // Direction from player ship to enemy ship in world space
            Vector3 localSpaceDirection = m_PlayerShip.InverseTransformDirection(worldSpaceDirection);
            (Vector3 v1, Vector3 v2) = (m_PlayerShip.position, m_PlayerShip.position + m_PlayerShip.TransformDirection(localSpaceDirection));

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(v1, 1.0f); // Draw a unit circle around player ship position

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawSphere(v2, 0.05f);

            Vector3 worldSpaceVector = (m_Transform.position - m_PlayerShip.position);
            Vector3 localSpaceVector = m_PlayerShip.InverseTransformVector(worldSpaceVector);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(m_PlayerShip.position, m_PlayerShip.position + m_PlayerShip.TransformVector(localSpaceVector));

            // Cross product
            Ray ray = new Ray(m_TurretPlayerLookDirectionRepresent.position, m_TurretPlayerLookDirectionRepresent.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 point = hitInfo.point;

                Gizmos.color = Color.white;
                Gizmos.DrawLine(ray.origin, point);

                Vector3 yAxis = hitInfo.normal;
                Vector3 xAxis = Vector3.Cross(yAxis, ray.direction).normalized;
                Vector3 zAxis = Vector3.Cross(xAxis, yAxis); // Since the xAxis and yAxis are orthonormal(perpendicular and normalized) to each other we are sure that zAxis is already normalized

                if (m_TurretDrawLocalSpaceInstead)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(point, point + xAxis * 0.15f);

                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(point, point + yAxis * 0.15f);

                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(point, point + zAxis * 0.15f);
                }
                else
                {
                    m_Turret.SetPositionAndRotation(point, Quaternion.LookRotation(zAxis, yAxis));
                }
            }
            else
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 50.0f);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, m_CrossA.position.normalized);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vector3.zero, m_CrossB.position.normalized);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Vector3.zero, Vector3.Cross(m_CrossA.position.normalized, m_CrossB.position.normalized));
        }
    }
}