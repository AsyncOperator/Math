using TMPro;
using UnityEngine;

namespace Freya.Part_II.Scripts
{
    public class MatrixMonitorUI : MonoBehaviour
    {
        // @formatter:off
        [SerializeField] private TextMeshProUGUI m_M00Text, m_M01Text, m_M02Text, m_M03Text,
                                                 m_M10Text, m_M11Text, m_M12Text, m_M13Text,
                                                 m_M20Text, m_M21Text, m_M22Text, m_M23Text,
                                                 m_M30Text, m_M31Text, m_M32Text, m_M33Text;
        // @formatter:on

        public void UpdateView(Matrix4x4 matrix)
        {
            // X-axis (first column)
            m_M00Text.SetText("{0}", matrix.m00);
            m_M10Text.SetText("{0}", matrix.m10);
            m_M20Text.SetText("{0}", matrix.m20);
            m_M30Text.SetText("{0}", matrix.m30);

            // Y-axis (second column)
            m_M01Text.SetText("{0}", matrix.m01);
            m_M11Text.SetText("{0}", matrix.m11);
            m_M21Text.SetText("{0}", matrix.m21);
            m_M31Text.SetText("{0}", matrix.m31);

            // Z-axis (third column)
            m_M02Text.SetText("{0}", matrix.m02);
            m_M12Text.SetText("{0}", matrix.m12);
            m_M22Text.SetText("{0}", matrix.m22);
            m_M32Text.SetText("{0}", matrix.m32);

            // Position (fourth column)
            m_M03Text.SetText("{0}", matrix.m03);
            m_M13Text.SetText("{0}", matrix.m13);
            m_M23Text.SetText("{0}", matrix.m23);
            m_M33Text.SetText("{0}", matrix.m33);
        }
    }
}