using UnityEngine;

namespace Freya.Final_Assignments.Scripts
{
    public class MeshSurfaceArea : MonoBehaviour
    {
        [SerializeField] private Mesh m_Mesh;

        // https://stackoverflow.com/questions/1406029/how-to-calculate-the-volume-of-a-3d-mesh-object-the-surface-of-which-is-made-up
        private float SignedVolumeOfTriangle(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            return Vector3.Dot(p0, Vector3.Cross(p1, p2)) / 6.0f;
        }

        private float CalculateTriangleAreaByHeronFormula(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float side0 = (p0 - p1).magnitude;
            float side1 = (p1 - p2).magnitude;
            float side2 = (p2 - p0).magnitude;

            // s = semiperimeter
            float s = 0.5f * (side0 + side1 + side2);

            // Area of the triangle
            return Mathf.Sqrt(s * (s - side0) * (s - side1) * (s - side2));
        }

        private float CalculateTriangleAreaByDotProduct(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 ab = (p1 - p0);
            Vector3 ac = (p2 - p0);

            float abMag = ab.magnitude;
            float acMag = ac.magnitude;

            float angleInRadians = Mathf.Acos(Vector3.Dot(ab, ac) / (abMag * acMag));

            return 0.5f * abMag * acMag * Mathf.Sin(angleInRadians);
        }

        [ContextMenu("Calculate surface area")]
        private void CalculateSurfaceArea()
        {
            if (m_Mesh == null)
            {
                Debug.Log("No mesh assigned");
                return;
            }

            int[] tris = m_Mesh.triangles;
            Vector3[] vertices = m_Mesh.vertices;

            float accumulatedAreaHeron = 0.0f;
            float accumulatedAreaDot = 0.0f;
            float accumulatedVolume = 0.0f;
            for (int i = 0; i < tris.Length; i += 3)
            {
                int t0 = tris[i + 0];
                int t1 = tris[i + 1];
                int t2 = tris[i + 2];

                Vector3 p0 = vertices[t0];
                Vector3 p1 = vertices[t1];
                Vector3 p2 = vertices[t2];

                accumulatedAreaHeron += CalculateTriangleAreaByHeronFormula(p0, p1, p2);
                accumulatedAreaDot += CalculateTriangleAreaByDotProduct(p0, p1, p2);
                accumulatedVolume += SignedVolumeOfTriangle(p0, p1, p2);
            }

            Debug.Log($"[Heron's formula] {m_Mesh.name} accumulated area: {accumulatedAreaHeron}");
            Debug.Log($"[Dot] {m_Mesh.name} accumulated area: {accumulatedAreaDot}");
            Debug.Log($"<color=green>{m_Mesh.name} accumulated volume:</color> {accumulatedVolume}");
        }
    }
}