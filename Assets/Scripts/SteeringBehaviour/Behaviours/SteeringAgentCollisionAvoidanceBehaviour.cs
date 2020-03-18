using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringAgentCollisionAvoidanceBehaviour : ISteeringBehaviour
    {
        [SerializeField] private float m_sensorRange = 5.0f;
        [SerializeField, Layer] private int m_sensorLayer = 0;
        [SerializeField] private float m_safetyDistance = 0.5f;

        private Sensor m_sensor = null;

        protected override void Initialize()
        {
            m_sensor = m_controller.AddSensor(m_sensorRange, m_sensorLayer);
        }

        protected override Vector2? CalculateForceComponentsInternal()
        {
            activation = 0.0f;

            foreach (var collider in m_sensor.touchingObjects)
            {
                var agent = collider.GetComponent<AgentBody>()?.agent;

                if (agent == null)
                    continue;

                var dPos0 = agent.position - position;
                var dVel0 = agent.velocity - velocity;

                if (dVel0.sqrMagnitude < Mathf.Epsilon)
                    continue;

                var t = -Vector2.Dot(dPos0, dVel0) / dVel0.sqrMagnitude; // time of closest distance

                if (t < 0.0f)
                    continue;

                var selfPredictedPosition = position + velocity * t;
                var otherPredictedPosition = agent.position + agent.velocity * t;

                var distanceSqr = (otherPredictedPosition - selfPredictedPosition).sqrMagnitude;
                if (distanceSqr > m_safetyDistance * m_safetyDistance)
                    continue;

                Debug.DrawLine(position, selfPredictedPosition, Color.magenta);
            }

            return null;
        }

        private void SolveQuadratic(float a, float b, float c, out float? t1, out float? t2)
        {
            // at^2 + bt + c = 0

            var discriminant = b * b - 4 * a * c;

            if (discriminant < 0.0f)
            {
                t1 = null;
                t2 = null;
                return;
            }

            var sqrtDiscriminant = Mathf.Sqrt(discriminant);

            t1 = (-b - sqrtDiscriminant) / (2.0f * a);
            t2 = (-b + sqrtDiscriminant) / (2.0f * a);
        }
    }
}
