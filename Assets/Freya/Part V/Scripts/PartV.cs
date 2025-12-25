using System.Collections.Generic;
using UnityEngine;

namespace Freya.Part_V.Scripts
{
    /// <summary>
    /// Trajectory
    /// </summary>
    public class PartV : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_Rigidbody;
        [SerializeField] private float m_Speed;
        [SerializeField] private MeshFilter m_MeshFilter;

        private Mesh m_Mesh;

        private Vector3 Position => transform.position;
        private Vector3 Velocity => transform.right * m_Speed;
        private Vector3 Acceleration => Physics.gravity;

        private void Awake()
        {
            m_Rigidbody.velocity = Velocity;
        }

        private Vector3 GetPointAtTrajectory(float t)
        {
            return Position + Velocity * t + (Acceleration * 0.5f) * t * t;
        }

        [ContextMenu("Generate Mesh")]
        private void GenerateMesh()
        {
            if (m_Mesh == null)
            {
                m_Mesh = new Mesh();
                m_MeshFilter.sharedMesh = m_Mesh;
            }

            List<Vector3> verts = new List<Vector3>()
            {
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 0.0f, 0.0f)
            };
            List<int> tris = new List<int>()
            {
                3, 0, 1, 3, 1, 2
            };
            List<Vector3> vertsNormals = new List<Vector3>()
            {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up
            };

            m_Mesh.Clear();
            m_Mesh.SetVertices(verts);
            m_Mesh.SetTriangles(tris, 0);
            m_Mesh.SetNormals(vertsNormals);
            // m_Mesh.RecalculateNormals();
        }

        private void OnDrawGizmos()
        {
            const int detail = 80;
            const float total_time = 3.0f;

            Vector3 prev = GetPointAtTrajectory(0.0f);

            for (int i = 1; i < detail; i++)
            {
                float t = i / (detail - 1.0f);
                float time = t * total_time;
                Vector3 point = GetPointAtTrajectory(time);

                Gizmos.DrawLine(prev, point);
                prev = point;
            }
        }
    }
}