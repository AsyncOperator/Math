using UnityEditor;
using UnityEngine;

namespace Freya.Part_II.Assignment_I.Scripts
{
    /// <summary>
    /// Turret
    /// </summary>
    public class AssignmentI : MonoBehaviour
    {
        [SerializeField] private Transform m_TurretPlayerLookDirectionRepresent, m_Turret, m_TurretHead;
        [SerializeField] private Transform m_Target;
        [SerializeField] private float m_DetectionRadius, m_DetectionAngleInDegrees, m_DetectionHeight;

        private void OnValidate()
        {
            m_DetectionRadius = Mathf.Max(m_DetectionRadius, 1.0f);
            m_DetectionAngleInDegrees = Mathf.Clamp(m_DetectionAngleInDegrees, 10.0f, 360.0f);
            m_DetectionHeight = Mathf.Max(m_DetectionHeight, 0.5f);
        }

        /// <summary>
        /// Also updates turret head rotation to look towards to target
        /// </summary>
        private bool IsTargetInsideTurretRange(Matrix4x4 turretWorldSpace)
        {
            Vector3 relativeToTarget = turretWorldSpace.MultiplyPoint3x4(m_Target.position);

            // The y-value should be out of consideration since the generated volume it's kinda cheese wedge (cylindrical)
            bool insideRadius = new Vector2(relativeToTarget.x, relativeToTarget.z).sqrMagnitude <= m_DetectionRadius * m_DetectionRadius;

            // This 'Mathf.Acos()' function always returns angle(in radians) between [0, PI]
            float angleInDegrees = Mathf.Acos(Vector2.Dot(Vector2.up, new Vector2(relativeToTarget.x, relativeToTarget.z).normalized)) * Mathf.Rad2Deg;
            bool insideAngle = angleInDegrees <= m_DetectionAngleInDegrees * 0.5f;
            // bool insideAngle = -m_DetectionAngleInDegrees * 0.5f <= Mathf.Atan2(relativeToTarget.z, relativeToTarget.x) * Mathf.Rad2Deg - 90.0f && Mathf.Atan2(relativeToTarget.z, relativeToTarget.x) * Mathf.Rad2Deg - 90.0f <= +m_DetectionAngleInDegrees * 0.5f;

            // Is height(relativeTarget.y) within [0.0f, m_DetectionHeight]
            bool insideHeight = 0.0f <= relativeToTarget.y && relativeToTarget.y <= m_DetectionHeight;

            bool inside = insideRadius && insideAngle && insideHeight;
            if (inside)
            {
                const float smooth_factor = 5.0f;

                Vector3 localSpaceVectorRelativeToHead = relativeToTarget - turretWorldSpace.MultiplyPoint3x4(m_TurretHead.position);
                Quaternion fromLocalRotation = m_TurretHead.localRotation;
                // turretWorldSpace.GetColumn(1) => m_Turret.up
                Quaternion toLocalRotation = Quaternion.LookRotation(localSpaceVectorRelativeToHead, turretWorldSpace.GetColumn(1));

                // If application is playing then smoothly rotate it otherwise snap it
                m_TurretHead.localRotation = Application.isPlaying
                    ? Quaternion.Slerp(fromLocalRotation, toLocalRotation, smooth_factor * Time.deltaTime)
                    : toLocalRotation;
            }

            return inside;
        }

        private void OnDrawGizmos()
        {
            Ray ray = new Ray(m_TurretPlayerLookDirectionRepresent.position, m_TurretPlayerLookDirectionRepresent.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 point = hitInfo.point;

                Gizmos.color = Color.white;
                Gizmos.DrawLine(ray.origin, point + ray.direction * 0.5f);

                Vector3 yAxis = hitInfo.normal;
                Vector3 xAxis = Vector3.Cross(yAxis, ray.direction).normalized;
                Vector3 zAxis = Vector3.Cross(xAxis, yAxis); // Since the xAxis and yAxis are orthonormal(perpendicular and normalized) to each other we are sure that zAxis is already normalized

                Gizmos.color = Color.red;
                Gizmos.DrawLine(point, point + xAxis * 0.5f);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(point, point + yAxis * 0.5f);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(point, point + zAxis * 0.5f);

                m_Turret.SetPositionAndRotation(point, Quaternion.LookRotation(zAxis, yAxis));

                // xAxis => new Vector4(xAxis, 0.0f)
                // yAxis => new Vector4(yAxis, 0.0f)
                // zAxis => new Vector4(zAxis, 0.0f)
                Matrix4x4 turretLocalSpace = new Matrix4x4(xAxis, yAxis, zAxis, new Vector4(point.x, point.y, point.z, 1.0f));

                // Draw detection volume ~cheese wedge
                Vector3 worldArcBottomLeftPoint = turretLocalSpace.MultiplyPoint3x4(Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionRadius));
                Vector3 worldArcBottomMidPoint = turretLocalSpace.MultiplyPoint3x4(Vector3.forward * m_DetectionRadius);
                // Same as above calculation but with a different approach
                Vector3 localArcBottomRightPoint = Quaternion.AngleAxis(+m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionRadius);
                Vector3 worldArcBottomRightPoint = turretLocalSpace * new Vector4(localArcBottomRightPoint.x, localArcBottomRightPoint.y, localArcBottomRightPoint.z, 1.0f);
                // Vector3 worldArcBottomRightPoint = (Vector3)(turretLocalSpace * localArcBottomRightPoint) + point;

                Vector3 worldArcTopLeftPoint = turretLocalSpace.MultiplyPoint3x4(Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionRadius) + Vector3.up * m_DetectionHeight);
                Vector3 worldArcTopMidPoint = turretLocalSpace.MultiplyPoint3x4(Vector3.forward * m_DetectionRadius + Vector3.up * m_DetectionHeight);
                Vector3 worldArcTopRightPoint = turretLocalSpace.MultiplyPoint3x4(Quaternion.AngleAxis(+m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionRadius) + Vector3.up * m_DetectionHeight);

                bool inside = IsTargetInsideTurretRange(turretLocalSpace.inverse);
                Handles.color = inside ? Color.red : Color.yellow;

                if (inside)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(m_TurretHead.position, m_Target.position);
                }

                // turretLocalSpace.GetPosition() == 'point' position
                Handles.DrawLine(turretLocalSpace.GetPosition(), worldArcBottomLeftPoint);
                Handles.DrawLine(point, worldArcBottomRightPoint);
                Handles.DrawLine(turretLocalSpace.MultiplyPoint3x4(Vector3.up * m_DetectionHeight), worldArcTopLeftPoint);
                Handles.DrawLine(turretLocalSpace.GetPosition() + yAxis * m_DetectionHeight, worldArcTopRightPoint);

                Handles.DrawLine(worldArcBottomLeftPoint, worldArcTopLeftPoint);
                Handles.DrawLine(worldArcBottomMidPoint, worldArcTopMidPoint);
                Handles.DrawLine(worldArcBottomRightPoint, worldArcTopRightPoint);
                Handles.DrawLine(point, turretLocalSpace.MultiplyPoint3x4(Vector3.up * m_DetectionHeight));

                Handles.DrawWireArc(point, yAxis, Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, yAxis) * zAxis, m_DetectionAngleInDegrees, m_DetectionRadius);
                Handles.DrawWireArc(point + yAxis * m_DetectionHeight, yAxis, Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, yAxis) * zAxis, m_DetectionAngleInDegrees, m_DetectionRadius);
            }
            else
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 50.0f);
            }
        }
    }
}