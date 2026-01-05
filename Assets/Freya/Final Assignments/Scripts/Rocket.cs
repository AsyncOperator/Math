using UnityEngine;

namespace Freya.Final_Assignments.Scripts
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private float m_AngularSpeedInAngles;
        [SerializeField] private float m_AccelerationMag;

        private Vector3 m_Velocity;

        private void Update()
        {
            float angularSpeed = 0.0f;
            if (Input.GetKey(KeyCode.A))
            {
                angularSpeed = +m_AngularSpeedInAngles;
            }

            if (Input.GetKey(KeyCode.D))
            {
                angularSpeed = -m_AngularSpeedInAngles;
            }

            Vector3 acceleration = Physics.gravity;
            if (Input.GetKey(KeyCode.W))
            {
                acceleration += transform.up * m_AccelerationMag;
            }

            float dt = Time.deltaTime;
            transform.rotation *= Quaternion.AngleAxis(angularSpeed * dt, Vector3.forward);

            m_Velocity += acceleration * dt;
            transform.position += m_Velocity * dt;
        }
    }
}