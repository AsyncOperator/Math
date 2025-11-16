using TMPro;
using UnityEngine;

namespace Freya.Part_II.Scripts
{
    public class MatrixMonitorUI : MonoBehaviour
    {
        private const string FORMAT = "{0:0.00}";
        
        // @formatter:off
        [SerializeField] private TextMeshProUGUI m_M00Text, m_M01Text, m_M02Text, m_M03Text,
                                                 m_M10Text, m_M11Text, m_M12Text, m_M13Text,
                                                 m_M20Text, m_M21Text, m_M22Text, m_M23Text,
                                                 m_M30Text, m_M31Text, m_M32Text, m_M33Text;
        // @formatter:on

        public void UpdateView(Matrix4x4 matrix)
        {
            // X-axis (first column)
            m_M00Text.SetText(FORMAT, matrix.m00);
            m_M10Text.SetText(FORMAT, matrix.m10);
            m_M20Text.SetText(FORMAT, matrix.m20);
            m_M30Text.SetText(FORMAT, matrix.m30);

            // Y-axis (second column)
            m_M01Text.SetText(FORMAT, matrix.m01);
            m_M11Text.SetText(FORMAT, matrix.m11);
            m_M21Text.SetText(FORMAT, matrix.m21);
            m_M31Text.SetText(FORMAT, matrix.m31);

            // Z-axis (third column)
            m_M02Text.SetText(FORMAT, matrix.m02);
            m_M12Text.SetText(FORMAT, matrix.m12);
            m_M22Text.SetText(FORMAT, matrix.m22);
            m_M32Text.SetText(FORMAT, matrix.m32);

            // Position (fourth column)
            m_M03Text.SetText(FORMAT, matrix.m03);
            m_M13Text.SetText(FORMAT, matrix.m13);
            m_M23Text.SetText(FORMAT, matrix.m23);
            m_M33Text.SetText(FORMAT, matrix.m33);
        }
    }
}