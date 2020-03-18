using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringSeparationBehaviour : ISteeringBehaviour
    {
        [SerializeField] private float m_sensorRange = 5.0f;
        [SerializeField, Layer] private int m_sensorLayer = 0;
        [SerializeField] private Sensor m_sensor = null;

        protected override void Initialize()
        {
            if (m_sensor == null)
                m_sensor = m_controller.AddSensor(m_sensorRange, m_sensorLayer);
        }

        protected override Vector2? CalculateForceComponentsInternal()
        {
            activation = 0.0f;

            var visibleAgentsCount = m_sensor.touchingObjects.Count;
            var meanPosition = Vector2.zero;

            foreach (var collider in m_sensor.touchingObjects)
            {
                var agent = collider.GetComponent<ParentAgent>()?.agent;
                if (agent == null)
                    continue;
                meanPosition += agent.position;
            }

            if (visibleAgentsCount <= 0)
                return null;

            meanPosition /= visibleAgentsCount;

            activation = 1.0f;
            return SteeringBehaviourUtils.Flee(meanPosition, this);
        }

        private void OnDrawGizmos()
        {
            if (m_sensor != null)
                return;

            Gizmos.color = Color.red;
            GizmosHelper.DrawCircle(transform.position, m_sensorRange);
        }
    }
}
