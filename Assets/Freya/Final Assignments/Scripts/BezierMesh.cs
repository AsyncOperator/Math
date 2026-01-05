using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Freya.Final_Assignments.Scripts
{
    public class BezierMesh : MonoBehaviour
    {
        [SerializeField] private Transform[] m_Points;
        [SerializeField, Range(0.0f, 1.0f)] private float m_TValue;
        [SerializeField] private MeshFilter m_MeshFilter;

        private List<Vector3> BezierPoints
        {
            get
            {
                int childCount = m_Points.Length;
                bool populate = false;

                if (m_BezierPoints == null)
                {
                    m_BezierPoints = new List<Vector3>(childCount);
                    populate = true;
                }
                else if (m_BezierPoints.Count != childCount)
                {
                    m_BezierPoints.Clear();
                    populate = true;
                }

                if (populate)
                {
                    for (int i = 0; i < childCount; i++)
                    {
                        m_BezierPoints.Add(m_Points[i].position);
                    }
                }

                return m_BezierPoints;
            }
        }

        private List<Vector3> m_BezierPoints;
        private Mesh m_Mesh;

        private Matrix4x4 GetPoint(List<Vector3> points, float t)
        {
            List<Vector3> source = ListPool<Vector3>.Get();
            source.AddRange(points);
            List<Vector3> temp = ListPool<Vector3>.Get();

            do
            {
                temp.Clear();

                for (int i = 0; i < source.Count - 1; i++)
                {
                    temp.Add(Vector3.Lerp(source[i], source[i + 1], t));
                }

                source.Clear();
                source.AddRange(temp);
            } while (temp.Count > 2);

            Vector3 a = temp[0];
            Vector3 b = temp[1];

            ListPool<Vector3>.Release(source);
            ListPool<Vector3>.Release(temp);

            Vector3 origin = Vector3.Lerp(a, b, t);
            Vector3 tangent = (b - a).normalized; // Z-axis
            Vector3 normal = Vector2.Perpendicular(tangent); // Y-axis
            Vector3 binormal = Vector3.back; // X-axis

            return new Matrix4x4(binormal, normal, tangent, new Vector4(origin.x, origin.y, origin.z, 1.0f));
        }

        private (Vector3 left, Vector3 right, Vector3 normal) GetProfileDataAt(List<Vector3> points, float t)
        {
            Matrix4x4 mtx = GetPoint(points, t);

            Vector3 left = mtx.MultiplyPoint3x4(-Vector3.right * 0.15f);
            Vector3 right = mtx.MultiplyPoint3x4(Vector3.right * 0.15f);
            Vector3 normal = mtx.GetColumn(1);

            return (left, right, normal);
        }

        [ContextMenu("Generate Bezier Mesh")]
        private void GenerateBezierMesh()
        {
            if (m_Mesh == null)
            {
                m_Mesh = new Mesh();
            }

            m_MeshFilter.sharedMesh = m_Mesh;

            const int segment_count = 64;

            List<Vector3> verts = ListPool<Vector3>.Get();
            List<Vector3> normals = ListPool<Vector3>.Get();
            for (int i = 0; i < segment_count + 1; i++)
            {
                float t = i / (float)segment_count;
                (Vector3 left, Vector3 right, Vector3 normal) = GetProfileDataAt(BezierPoints, t);

                verts.Add(transform.worldToLocalMatrix.MultiplyPoint3x4(right));
                verts.Add(transform.InverseTransformPoint(left));

                normals.Add(normal);
                normals.Add(normal);
            }

            List<int> tris = ListPool<int>.Get();
            for (int i = 0; i < segment_count; i++)
            {
                int root = i * 2;
                int neighbour = root + 1;
                int next = root + 2;
                int nextNeighbour = root + 3;

                // First triangle
                tris.Add(root);
                tris.Add(neighbour);
                tris.Add(next);

                // Second triangle
                tris.Add(neighbour);
                tris.Add(nextNeighbour);
                tris.Add(next);
            }

            m_Mesh.Clear();
            m_Mesh.SetVertices(verts);
            m_Mesh.SetNormals(normals);
            m_Mesh.SetTriangles(tris, 0);

            ListPool<Vector3>.Release(verts);
            ListPool<Vector3>.Release(normals);
            ListPool<int>.Release(tris);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
            for (int i = 0; i < BezierPoints.Count - 1; i++)
            {
                Vector3 from = BezierPoints[i];
                Vector3 to = BezierPoints[i + 1];
                Gizmos.DrawLine(from, to);
            }

            Gizmos.color = Color.white;

            // Visualize bezier mtx at 'TValue'
            if (Selection.activeTransform == transform)
            {
                Matrix4x4 mtx = GetPoint(BezierPoints, m_TValue);
                Gizmos.DrawWireSphere(mtx.GetPosition(), 0.02f);
                _ = Handles.PositionHandle(mtx.GetPosition(), mtx.rotation);
            }

            // Visualize bezier curve
            const int detail = 128;
            Vector3 prev = GetPoint(BezierPoints, 0.0f).GetPosition();
            for (int i = 1; i < detail; i++)
            {
                float t = i / (detail - 1.0f);
                Vector3 point = GetPoint(BezierPoints, t).GetPosition();

                Gizmos.DrawLine(prev, point);
                prev = point;
            }
        }
    }
}