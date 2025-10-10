using UnityEditor;
using UnityEngine;

namespace Freya.Part_I.Assignment_I.Scripts
{
    /// <summary>
    /// Radial trigger
    /// </summary>
    public class AssignmentI : MonoBehaviour
    {
        [SerializeField] private Transform m_ExplosiveBarrel, m_Player;
        [SerializeField] private float m_ExplosiveBarrelRadius;

        private void OnDrawGizmos()
        {
            Vector3 barrelPosition = m_ExplosiveBarrel.position;
            Vector3 playerPosition = m_Player.position;

            // ## Solution 1 ##
            // float distance = (barrelPosition - playerPosition).magnitude;
            // bool inside = distance <= m_ExplosiveBarrelRadius;

            // ## Solution 2 ##
            // Vector3 displacement = barrelPosition - playerPosition;
            // float distance = Mathf.Sqrt
            // (
            //     displacement.x * displacement.x +
            //     displacement.y * displacement.y +
            //     displacement.z * displacement.z
            // );
            // bool inside = distance <= m_ExplosiveBarrelRadius;

            // ## Solution 3 ##
            // Vector3 displacement = barrelPosition - playerPosition;
            // float distanceSqr = displacement.x * displacement.x +
            //                     displacement.y * displacement.y +
            //                     displacement.z * displacement.z;
            // bool inside = distanceSqr <= m_ExplosiveBarrelRadius * m_ExplosiveBarrelRadius;

            // ## Solution 4 ##
            // Vector3 displacement = barrelPosition - playerPosition;
            // float distanceSqr = Vector3.Dot(displacement, displacement);
            // bool inside = distanceSqr <= m_ExplosiveBarrelRadius * m_ExplosiveBarrelRadius;

            // ## Solution 5 ##
            // Since we are just interested in whether player is inside or outside the explosive barrel radius
            // this solution is more performant since we got rid of calculating expensive sqrt operation
            float distanceSqr = (barrelPosition - playerPosition).sqrMagnitude;
            bool inside = distanceSqr <= m_ExplosiveBarrelRadius * m_ExplosiveBarrelRadius;

            Handles.color = inside ? Color.red : Color.green;
            Handles.DrawWireDisc(barrelPosition, Vector3.back, m_ExplosiveBarrelRadius, 2.0f);
        }
    }
}