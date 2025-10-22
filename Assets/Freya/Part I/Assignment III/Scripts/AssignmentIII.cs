using Attributes;
using UnityEngine;

namespace Freya.Part_I.Assignment_III.Scripts
{
    /// <summary>
    /// Vector transformation
    /// </summary>
    public class AssignmentIII : MonoBehaviour
    {
        [SerializeField] private Transform m_LocalSpace;
        [SerializeField] private Vector2 m_LocalSpaceCoords, m_WorldSpaceCoords;
        [SerializeField] private bool m_DrawLocalSpaceToWorldSpace, m_DrawWorldSpaceToLocalSpace;
        [SerializeField, ReadOnly] private Vector2 m_LocalToWorldPosition, m_WorldToLocalPosition;

        private void OnDrawGizmos()
        {
            Vector3 localSpaceOrigin = m_LocalSpace.position; // In world space
            Vector3 localRight = m_LocalSpace.right; // In world space
            Vector3 localUp = m_LocalSpace.up; // In world space

            // Draw local space orientation
            Gizmos.color = Color.red;
            Gizmos.DrawLine(localSpaceOrigin, localSpaceOrigin + localRight * 0.125f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(localSpaceOrigin, localSpaceOrigin + localUp * 0.125f);

            // Local to world transformation
            if (m_DrawLocalSpaceToWorldSpace)
            {
                Vector3 localSpaceCoordsRelativeToWorld = localRight * m_LocalSpaceCoords.x +
                                                          localUp * m_LocalSpaceCoords.y;
                m_LocalToWorldPosition = localSpaceOrigin + localSpaceCoordsRelativeToWorld;

                // Draw line from local space origin (m_LocalSpace.position) to final position in world space
                Gizmos.color = Color.white;
                Gizmos.DrawLine(localSpaceOrigin, m_LocalToWorldPosition);

                // Draw line from world space origin (Vector3.zero) to final position in world
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(Vector3.zero, m_LocalToWorldPosition);
            }

            // World to local transformation
            if (m_DrawWorldSpaceToLocalSpace)
            {
                Vector3 displacement = (Vector3)m_WorldSpaceCoords - localSpaceOrigin; // Relative vector from local space origin to world space position, but it's still in world space
                m_WorldToLocalPosition = new Vector2
                (
                    Vector3.Dot(localRight, displacement),
                    Vector3.Dot(localUp, displacement)
                );
                Vector3 localSpaceCoordsRelativeToWorld = localRight * m_WorldToLocalPosition.x +
                                                          localUp * m_WorldToLocalPosition.y;

                // Draw a line from world space origin (Vector3.zero) to world space position
                Gizmos.color = Color.white;
                Gizmos.DrawLine(Vector3.zero, m_WorldSpaceCoords);

                // Draw a line from local space origin (m_LocalSpace.position) to final position
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(localSpaceOrigin, localSpaceOrigin + localSpaceCoordsRelativeToWorld);
            }
        }
    }
}