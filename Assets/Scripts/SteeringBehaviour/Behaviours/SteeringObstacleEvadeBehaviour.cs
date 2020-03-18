using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringObstacleEvadeBehaviour : ISteeringBehaviour
    {
        [SerializeField] private float m_sensorRange = 5.0f;
        [SerializeField, Layer] private int m_sensorLayer = 0;

        private Sensor m_sensor = null;

        protected override void Initialize()
        {
            m_sensor = m_controller.AddSensor(m_sensorRange, m_sensorLayer);
        }

        protected override Vector2? CalculateForceComponentsInternal()
        {
            activation = 0.0f;

            if (m_sensor == null)
                return null;

            var visibleObstaclesCount = m_sensor.touchingObjects.Count;

            var meanPos = Vector2.zero;
            foreach (var obstacle in m_sensor.touchingObjects)
                meanPos += (Vector2)obstacle.transform.position;

            activation = visibleObstaclesCount > 0 ? 1.0f : 0.0f;
            var force = visibleObstaclesCount > 0 ? SteeringBehaviourUtils.Flee(meanPos, this) : Vector2.zero;

            return force;
        }
    }
}
