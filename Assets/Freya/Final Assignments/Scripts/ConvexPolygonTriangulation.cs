using System.Collections.Generic;
using UnityEngine;

namespace Freya.Final_Assignments.Scripts
{
    public class ConvexPolygonTriangulation : MonoBehaviour
    {
        [SerializeField] private MeshFilter m_MeshFilter;

        private Mesh m_Mesh;

        [ContextMenu("Triangulation")]
        private void Triangulation()
        {
            int childCount = transform.childCount;

            if (childCount < 3)
            {
                Debug.LogError("Need at least 3 points");
                return;
            }

            List<Vector3> points = new List<Vector3>(childCount);
            for (int i = 0; i < childCount; i++)
            {
                points.Add(transform.GetChild(i).localPosition);
            }

            int lowestLeftestPointIndex = 0;
            for (int i = 1; i < childCount; i++)
            {
                Vector3 currentLowestLeftestPoint = points[lowestLeftestPointIndex];
                Vector3 candidateLowestLeftestPoint = points[i];

                int yCompare = candidateLowestLeftestPoint.y.CompareTo(currentLowestLeftestPoint.y);
                if (yCompare < 0)
                {
                    lowestLeftestPointIndex = i;
                }
                else if (yCompare == 0)
                {
                    int xCompare = candidateLowestLeftestPoint.x.CompareTo(currentLowestLeftestPoint.x);
                    if (xCompare <= 0)
                    {
                        lowestLeftestPointIndex = i;
                    }
                }
            }

            Vector3 lowestLeftestPoint = points[lowestLeftestPointIndex];
            points.RemoveAt(lowestLeftestPointIndex);
            points.Insert(0, lowestLeftestPoint);

            points.Sort(1, childCount - 1, new VectorAngleComparer(lowestLeftestPoint));

            int[] triangles = new int[(childCount - 2) * 3];

            for (int i = 0; i < childCount - 2; i++)
            {
                int tris0 = 0;
                int tris1 = i + 1;
                int tris2 = i + 2;

                triangles[i * 3] = tris0;
                triangles[i * 3 + 1] = tris1;
                triangles[i * 3 + 2] = tris2;
            }

            if (m_Mesh == null)
            {
                m_Mesh = new Mesh();
                m_MeshFilter.sharedMesh = m_Mesh;
            }

            m_Mesh.Clear();
            m_Mesh.SetVertices(points);
            m_Mesh.SetTriangles(triangles, 0);
            m_Mesh.RecalculateNormals();
        }

        private class VectorAngleComparer : Comparer<Vector3>
        {
            private Vector3 m_Origin;

            public VectorAngleComparer(Vector3 origin)
            {
                m_Origin = origin;
            }

            private float VectorToAngle(Vector3 v)
            {
                float angleInRadians = Mathf.Atan2(v.y, v.x);

                return (double)angleInRadians >= 0.0 ? angleInRadians : Mathf.PI * 2 + angleInRadians;
            }

            public override int Compare(Vector3 v0, Vector3 v1)
            {
                Vector3 toV0 = v0 - m_Origin;
                Vector3 toV1 = v1 - m_Origin;

                float v0Angle = VectorToAngle(toV0);
                float v1Angle = VectorToAngle(toV1);

                return v0Angle > v1Angle ? -1 : Mathf.Approximately(v0Angle, v1Angle) ? 0 : +1;
            }
        }
    }
}