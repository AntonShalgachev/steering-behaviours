using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringObstacleEvadeBehaviour : ISteeringBehaviour
    {
        private Sensor m_sensor = null;

        protected override void Initialize()
        {
            m_sensor = GetComponentInChildren<PerceptionSensor>()?.GetComponent<Sensor>();
        }

        protected override Vector2? CalculateForceComponentsInternal()
        {
            activation = 0.0f;

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
