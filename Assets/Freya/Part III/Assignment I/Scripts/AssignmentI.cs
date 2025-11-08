using System;
using UnityEditor;
using UnityEngine;

namespace Freya.Part_III.Assignment_I.Scripts
{
    /// <summary>
    /// Clock
    /// </summary>
    public class AssignmentI : MonoBehaviour
    {
        [SerializeField] private ClockStyle m_ClockStyle;
        [SerializeField] private ClockBehaviour m_ClockBehaviour;

        private void OnDrawGizmos()
        {
            DateTime now = DateTime.Now;

            long milliseconds = m_ClockBehaviour switch
            {
                ClockBehaviour.Snap => now.Millisecond,
                ClockBehaviour.Smooth => now.Millisecond * TimeSpan.TicksPerMillisecond,
            };
            long seconds = m_ClockBehaviour switch
            {
                ClockBehaviour.Snap => now.Second,
                ClockBehaviour.Smooth => now.Second * TimeSpan.TicksPerSecond,
            };
            long minutes = m_ClockBehaviour switch
            {
                ClockBehaviour.Snap => now.Minute,
                ClockBehaviour.Smooth => now.Minute * TimeSpan.TicksPerMinute,
            };
            long hours = m_ClockBehaviour switch
            {
                ClockBehaviour.Snap => now.Hour,
                ClockBehaviour.Smooth => now.Hour * TimeSpan.TicksPerHour,
            };

            long segmentCount = m_ClockStyle switch
            {
                ClockStyle.TwelveHoursClock => 12L,
                ClockStyle.TwentyFourHoursClock => 24L
            };

            const float clock_radius = 1.0f;

            Handles.DrawWireDisc(Vector3.zero, Vector3.back, clock_radius);

            long lcm = 120; // Least common multiple for (12, 24, 60) ~ to support both 12H and 24H clock with tick marks
            long divider = lcm / segmentCount;

            for (int i = 0; i < lcm; i++)
            {
                float angleInTurns = i / (float)lcm;
                float angleInDegrees = angleInTurns * 360.0f;

                int remainder = i % (int)divider;

                // Draw if it is either hour mark or tick mark
                if (i % 2 == 0 || remainder == 0)
                {
                    // If it is hour mark draw it longer otherwise shorter
                    Vector3 p1 = Quaternion.AngleAxis(angleInDegrees, Vector3.back) * Vector3.up * clock_radius * (remainder == 0 ? 0.90f : 0.95f);
                    Vector3 p2 = Quaternion.AngleAxis(angleInDegrees, Vector3.back) * Vector3.up * clock_radius;
                    Handles.DrawLine(p1, p2);
                }

                if (remainder == 0)
                {
                    Handles.Label(
                        Quaternion.AngleAxis(angleInDegrees, Vector3.back) * Vector3.up * clock_radius * 1.1f,
                        (i != 0 ? i / divider : segmentCount).ToString());
                }
            }

            long n1 = m_ClockBehaviour switch
            {
                ClockBehaviour.Snap => hours,
                ClockBehaviour.Smooth => milliseconds + seconds + minutes + hours
            };
            long n2 = m_ClockBehaviour switch
            {
                ClockBehaviour.Snap => minutes,
                ClockBehaviour.Smooth => milliseconds + seconds + minutes
            };
            long n3 = m_ClockBehaviour switch
            {
                ClockBehaviour.Snap => seconds,
                ClockBehaviour.Smooth => milliseconds + seconds
            };

            long d1 = m_ClockBehaviour switch
            {
                ClockBehaviour.Snap => segmentCount,
                ClockBehaviour.Smooth => m_ClockStyle == ClockStyle.TwelveHoursClock ? TimeSpan.TicksPerDay / 2L : TimeSpan.TicksPerDay
            };
            long d2 = m_ClockBehaviour switch
            {
                ClockBehaviour.Snap => 60L,
                ClockBehaviour.Smooth => TimeSpan.TicksPerHour
            };
            long d3 = m_ClockBehaviour switch
            {
                ClockBehaviour.Snap => 60L,
                ClockBehaviour.Smooth => TimeSpan.TicksPerMinute
            };

            DrawClockArm(n1, d1, 0.025f * 2.0f, 0.55f, Color.blue); // Draw hours arm
            DrawClockArm(n2, d2, 0.025f * 1.5f, 0.70f, Color.green); // Draw minutes arm
            DrawClockArm(n3, d3, 0.025f, 0.85f, Color.red); // Draw seconds arm

            void DrawClockArm(long numerator, long denominator, float radius, float armLength, Color color)
            {
                const float arm_pivot_arc_angle = 300.0f;

                Vector3 p = Quaternion.AngleAxis((numerator / (float)denominator) * 360.0f, Vector3.back) * Vector3.up * armLength;
                Handles.color = color;
                Handles.DrawWireArc(
                    Vector3.zero,
                    Vector3.back,
                    -(Quaternion.AngleAxis((numerator / (float)denominator) * 360.0f - (arm_pivot_arc_angle * 0.5f), Vector3.back) * Vector3.up),
                    arm_pivot_arc_angle,
                    radius);
                Handles.DrawLine(
                    -(Quaternion.AngleAxis((numerator / (float)denominator) * 360.0f - (arm_pivot_arc_angle * 0.5f), Vector3.back) * Vector3.up) * radius,
                    p);
                Handles.DrawLine(
                    -(Quaternion.AngleAxis((numerator / (float)denominator) * 360.0f + (arm_pivot_arc_angle * 0.5f), Vector3.back) * Vector3.up) * radius,
                    p);

                // To easily visualize where the arm intersect with clock radius
                Handles.DrawWireDisc(p.normalized * clock_radius, Vector3.back, 0.015f);
            }
        }

        private enum ClockStyle
        {
            TwelveHoursClock,
            TwentyFourHoursClock
        }

        private enum ClockBehaviour
        {
            Snap,
            Smooth
        }
    }
}