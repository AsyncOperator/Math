using System;
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
        [SerializeField] private TriggerShapes m_TriggerShape;
        [SerializeField] private float m_DetectionMinRadius, m_DetectionMaxRadius, m_DetectionAngleInDegrees, m_DetectionHeight;

        private void OnValidate()
        {
            m_DetectionMinRadius = Mathf.Max(m_DetectionMinRadius, 0.5f);
            m_DetectionMaxRadius = Mathf.Max(m_DetectionMaxRadius, 1.0f);

            if (m_DetectionMaxRadius < m_DetectionMinRadius)
            {
                m_DetectionMinRadius = m_DetectionMaxRadius;
            }

            m_DetectionAngleInDegrees = Mathf.Clamp(m_DetectionAngleInDegrees, 10.0f, 180.0f);
            m_DetectionHeight = Mathf.Max(m_DetectionHeight, 0.5f);
        }

        private void OnEnable()
        {
            Camera.onPostRender += OnPostRender;
        }

        private void OnDisable()
        {
            Camera.onPostRender -= OnPostRender;
        }

        /// <summary>
        /// Also updates turret head rotation to look towards to target
        /// </summary>
        private bool IsTargetInsideTurretRange(Matrix4x4 turretWorldSpace)
        {
            bool inside = false;

            Vector3 relativeToTarget = turretWorldSpace.MultiplyPoint3x4(m_Target.position);

            switch (m_TriggerShape)
            {
                case TriggerShapes.CylindricalSector:
                {
                    // The y-value should be out of consideration since the generated volume it's kinda cheese wedge (cylindrical)
                    float distanceSqrToTarget = new Vector2(relativeToTarget.x, relativeToTarget.z).sqrMagnitude;
                    bool insideRadius = m_DetectionMinRadius * m_DetectionMinRadius <= distanceSqrToTarget && distanceSqrToTarget <= m_DetectionMaxRadius * m_DetectionMaxRadius;

                    // This 'Mathf.Acos()' function always returns angle(in radians) between [0, PI]
                    float angleInDegrees = Mathf.Acos(Vector2.Dot(Vector2.up, new Vector2(relativeToTarget.x, relativeToTarget.z).normalized)) * Mathf.Rad2Deg;
                    bool insideAngle = angleInDegrees <= m_DetectionAngleInDegrees * 0.5f;
                    // bool insideAngle = -m_DetectionAngleInDegrees * 0.5f <= Mathf.Atan2(relativeToTarget.z, relativeToTarget.x) * Mathf.Rad2Deg - 90.0f && Mathf.Atan2(relativeToTarget.z, relativeToTarget.x) * Mathf.Rad2Deg - 90.0f <= +m_DetectionAngleInDegrees * 0.5f;

                    // Is height(relativeTarget.y) within [0.0f, m_DetectionHeight]
                    bool insideHeight = 0.0f <= relativeToTarget.y && relativeToTarget.y <= m_DetectionHeight;

                    inside = insideRadius && insideAngle && insideHeight;
                    break;
                }
                case TriggerShapes.Spherical:
                {
                    float distanceSqrToTarget = relativeToTarget.sqrMagnitude;
                    inside = m_DetectionMinRadius * m_DetectionMinRadius <= distanceSqrToTarget && distanceSqrToTarget <= m_DetectionMaxRadius * m_DetectionMaxRadius;
                    break;
                }
                case TriggerShapes.SphericalSector:
                {
                    float distanceSqrToTarget = relativeToTarget.sqrMagnitude;
                    bool insideRadius = m_DetectionMinRadius * m_DetectionMinRadius <= distanceSqrToTarget && distanceSqrToTarget <= m_DetectionMaxRadius * m_DetectionMaxRadius;

                    // This 'Mathf.Acos()' function always returns angle(in radians) between [0, PI]
                    float angleInDegrees = Mathf.Acos(Vector3.Dot(Vector3.forward, relativeToTarget.normalized)) * Mathf.Rad2Deg;
                    bool insideAngle = angleInDegrees <= m_DetectionAngleInDegrees * 0.5f;

                    inside = insideRadius && insideAngle;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

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

        // This part talked somewhere in trigonometry lecture (5th of the series)
        // What would you do if you want to draw the wedge shape in built just rather than on editor
        // and Freya suggest that one solution to that is using GL commands

        // This event will be called only if this script attached same GameObject that has Camera Component, thus not proper solution for our case
        private void OnPostRender()
        {
            Debug.Log("Should never be printed in current setup");
        }

        // We want to draw our things exactly here to avoid possible errors that might occur
        // like if we draw on Update call instead of here
        // since the Unity's event execution order for camera render run after Update method calls
        // the screen probably will be cleared before Draw function call thus cause to lose our drawings
        private void OnPostRender(Camera camera)
        {
            // Want to be sure just catching targeted camera
            if (camera == Camera.current)
            {
                GL.PushMatrix();
                GL.MultMatrix(m_Turret.localToWorldMatrix);
                GL.Begin(GL.LINES);

                // This is placeholder not actually drawing wedge shape
                for (int i = 0; i < 5; ++i)
                {
                    float a = i / (float)5;
                    float angleInRadians = a * Mathf.PI * 2.0f;
                    // Vertex colors change from red to green
                    GL.Color(new Color(a, 1.0f - a, 0.0f, 0.8f));
                    // One vertex at transform position
                    GL.Vertex3(0, 0, 0);
                    // Another vertex at edge of circle
                    GL.Vertex3(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians), 0.0f);
                }

                GL.End();
                GL.PopMatrix();
            }
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

                bool inside = IsTargetInsideTurretRange(turretLocalSpace.inverse);
                Gizmos.color = Handles.color = inside ? Color.red : Color.yellow;

                if (inside)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(m_TurretHead.position, m_Target.position);
                }

                switch (m_TriggerShape)
                {
                    case TriggerShapes.CylindricalSector:
                    {
                        // Calculate detection volume(cheese wedge) vertices' world points
                        Vector3 worldInnerArcBottomLeftPoint = turretLocalSpace.MultiplyPoint3x4(Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionMinRadius));
                        // Vector3 worldInnerArcBottomMidPoint = turretLocalSpace.MultiplyPoint3x4(Vector3.forward * m_DetectionMinRadius);
                        Vector3 localInnerArcBottomRightPoint = Quaternion.AngleAxis(+m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionMinRadius);
                        Vector3 worldInnerArcBottomRightPoint = turretLocalSpace * new Vector4(localInnerArcBottomRightPoint.x, localInnerArcBottomRightPoint.y, localInnerArcBottomRightPoint.z, 1.0f);
                        // Vector3 worldInnerArcBottomRightPoint = (Vector3)(turretLocalSpace * localInnerArcBottomRightPoint) + point;

                        Vector3 worldInnerArcTopLeftPoint = turretLocalSpace.MultiplyPoint3x4(Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionMinRadius) + Vector3.up * m_DetectionHeight);
                        // Vector3 worldInnerArcTopMidPoint = turretLocalSpace.MultiplyPoint3x4(Vector3.forward * m_DetectionMinRadius + Vector3.up * m_DetectionHeight);
                        Vector3 worldInnerArcTopRightPoint = turretLocalSpace.MultiplyPoint3x4(Quaternion.AngleAxis(+m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionMinRadius) + Vector3.up * m_DetectionHeight);

                        Vector3 worldOuterArcBottomLeftPoint = turretLocalSpace.MultiplyPoint3x4(Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionMaxRadius));
                        // Vector3 worldOuterArcBottomMidPoint = turretLocalSpace.MultiplyPoint3x4(Vector3.forward * m_DetectionMaxRadius);
                        Vector3 localOuterArcBottomRightPoint = Quaternion.AngleAxis(+m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionMaxRadius);
                        Vector3 worldOuterArcBottomRightPoint = turretLocalSpace * new Vector4(localOuterArcBottomRightPoint.x, localOuterArcBottomRightPoint.y, localOuterArcBottomRightPoint.z, 1.0f);
                        // Vector3 worldOuterArcBottomRightPoint = (Vector3)(turretLocalSpace * localOuterArcBottomRightPoint) + point;

                        Vector3 worldOuterArcTopLeftPoint = turretLocalSpace.MultiplyPoint3x4(Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionMaxRadius) + Vector3.up * m_DetectionHeight);
                        // Vector3 worldOuterArcTopMidPoint = turretLocalSpace.MultiplyPoint3x4(Vector3.forward * m_DetectionMaxRadius + Vector3.up * m_DetectionHeight);
                        Vector3 worldOuterArcTopRightPoint = turretLocalSpace.MultiplyPoint3x4(Quaternion.AngleAxis(+m_DetectionAngleInDegrees * 0.5f, Vector3.up) * (Vector3.forward * m_DetectionMaxRadius) + Vector3.up * m_DetectionHeight);

                        // Draw wedge edges
                        Handles.DrawLine(worldInnerArcBottomLeftPoint, worldOuterArcBottomLeftPoint);
                        Handles.DrawLine(worldInnerArcBottomRightPoint, worldOuterArcBottomRightPoint);
                        Handles.DrawLine(worldInnerArcTopLeftPoint, worldOuterArcTopLeftPoint);
                        Handles.DrawLine(worldInnerArcTopRightPoint, worldOuterArcTopRightPoint);

                        // Draw wire arc for inner radius
                        Handles.DrawWireArc(point, yAxis, Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, yAxis) * zAxis, m_DetectionAngleInDegrees, m_DetectionMinRadius);
                        Handles.DrawWireArc(point + yAxis * m_DetectionHeight, yAxis, Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, yAxis) * zAxis, m_DetectionAngleInDegrees, m_DetectionMinRadius);

                        // Draw wire arc edges for inner radius
                        Handles.DrawLine(worldInnerArcBottomLeftPoint, worldInnerArcTopLeftPoint);
                        // Handles.DrawLine(worldInnerArcBottomMidPoint, worldInnerArcTopMidPoint);
                        Handles.DrawLine(worldInnerArcBottomRightPoint, worldInnerArcTopRightPoint);

                        // Draw wire arc for outer radius
                        Handles.DrawWireArc(point, yAxis, Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, yAxis) * zAxis, m_DetectionAngleInDegrees, m_DetectionMaxRadius);
                        Handles.DrawWireArc(point + yAxis * m_DetectionHeight, yAxis, Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, yAxis) * zAxis, m_DetectionAngleInDegrees, m_DetectionMaxRadius);

                        // Draw wire arc edges for outer radius
                        Handles.DrawLine(worldOuterArcBottomLeftPoint, worldOuterArcTopLeftPoint);
                        // Handles.DrawLine(worldOuterArcBottomMidPoint, worldOuterArcTopMidPoint);
                        Handles.DrawLine(worldOuterArcBottomRightPoint, worldOuterArcTopRightPoint);

                        break;
                    }
                    case TriggerShapes.Spherical:
                    {
                        Gizmos.DrawWireSphere(point, m_DetectionMinRadius);
                        Gizmos.DrawWireSphere(point, m_DetectionMaxRadius);

                        break;
                    }
                    case TriggerShapes.SphericalSector:
                    {
                        const int segment_count = 16;

                        // Bottom cap
                        Handles.DrawWireArc
                        (
                            point,
                            zAxis,
                            Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, yAxis) * zAxis,
                            360.0f,
                            m_DetectionMinRadius
                        );

                        // Top cap
                        Handles.DrawWireArc
                        (
                            point,
                            zAxis,
                            Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, yAxis) * zAxis,
                            360.0f,
                            m_DetectionMaxRadius
                        );

                        for (int i = 0; i < segment_count; i++)
                        {
                            float angleInDegrees = (i / (float)segment_count) * 180.0f;
                            Vector3 normal = Quaternion.AngleAxis(angleInDegrees, zAxis) * yAxis;

                            Vector3 from = Quaternion.AngleAxis(-m_DetectionAngleInDegrees * 0.5f, normal) * zAxis;
                            Vector3 end = Quaternion.AngleAxis(+m_DetectionAngleInDegrees * 0.5f, normal) * zAxis;

                            Handles.DrawWireArc
                            (
                                point,
                                normal,
                                from,
                                m_DetectionAngleInDegrees,
                                m_DetectionMinRadius
                            );

                            Handles.DrawWireArc
                            (
                                point,
                                normal,
                                from,
                                m_DetectionAngleInDegrees,
                                m_DetectionMaxRadius
                            );

                            Handles.DrawLine(point + (from * m_DetectionMinRadius), point + (from * m_DetectionMaxRadius));
                            Handles.DrawLine(point + (end * m_DetectionMinRadius), point + (end * m_DetectionMaxRadius));
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 50.0f);
            }
        }

        private enum TriggerShapes : byte
        {
            CylindricalSector,
            Spherical,
            SphericalSector
        }
    }
}