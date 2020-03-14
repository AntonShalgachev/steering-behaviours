using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringCollisionAvoidanceBehaviour : ISteeringBehaviour
    {
        [SerializeField] private float m_minDistance = 1.0f;
        [SerializeField] private float m_maxDistance = 5.0f;
        [SerializeField] private float m_hullRadius = 1.0f;

        private Sensor m_sensor = null;

        private List<Vector2> targetDirections = new List<Vector2>();

        protected override void Initialize()
        {
            m_sensor = GetComponentInChildren<PerceptionSensor>()?.GetComponent<Sensor>();
        }

        protected override Vector2? CalculateForceComponentsInternal()
        {
            // beware, this code stinks

            activation = 0.0f;
            targetDirections.Clear();

            // float? minDistance = null;
            // var forceForClosestThreat = 0.0f;

            (float force, float distance, float weight)? targetData = null;

            foreach (var obstacle in m_sensor.touchingObjects)
            {
                var data = CalculateForceForObstacle(obstacle as CircleCollider2D); // so far only circles are handled
                if (!data.HasValue)
                    continue;

                if (!targetData.HasValue || targetData.Value.distance > data.Value.distance)
                    targetData = data;
            }

            if (!targetData.HasValue)
                return null;

            activation = targetData.Value.weight;
            return new Vector2(targetData.Value.force, 0.0f);
        }

        private (float force, float distance, float weight)? CalculateForceForObstacle(CircleCollider2D obstacle)
        {
            if (obstacle == null)
                return null;

            var obstaclePosition = (Vector2)obstacle.transform.position;
            var scale = obstacle.transform.lossyScale;
            var radius = obstacle.radius * Mathf.Max(scale.x, scale.y);
            var radiusWithHull = radius + m_hullRadius;

            var toCenter = obstaclePosition - position;
            var distanceToCenter = toCenter.magnitude;

            if (distanceToCenter < radiusWithHull)
            {
                Debug.LogWarning("Agent is inside the obstacle, skipping");
                return null;
            }

            var distanceToBound = distanceToCenter - radius;
            var weight = Mathf.InverseLerp(m_maxDistance, m_minDistance, distanceToBound);

            var deltaAngleFromCenter = Mathf.Asin(radiusWithHull / distanceToCenter) * Mathf.Rad2Deg;
            var deltaAngleToCenter = Vector2.SignedAngle(m_forward, toCenter);

            if (Mathf.Abs(deltaAngleToCenter) > deltaAngleFromCenter)
                return null;

            var directionMultiplier = -Mathf.Sign(deltaAngleToCenter);

            var targetAngle = angle + deltaAngleToCenter + directionMultiplier * deltaAngleFromCenter;

#if DEBUG
            var targetDirection = Vector2.up.Rotate(targetAngle) * Mathf.Sqrt(distanceToCenter * distanceToCenter - radius * radius);
            targetDirections.Add(targetDirection);
#endif

            var force = CalculateNormalForce(targetAngle) * weight;

            return (force, distanceToBound, weight);
        }

        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            Gizmos.color = Color.magenta;
            GizmosHelper.DrawCircle(position, m_minDistance);
            GizmosHelper.DrawCircle(position, m_maxDistance);

            Gizmos.color = Color.black;
            GizmosHelper.DrawCircle(position, m_hullRadius);

            if (m_sensor != null)
            {
                Gizmos.color = Color.blue;
                foreach (var obstacle in m_sensor.touchingObjects)
                    Gizmos.DrawLine(position, obstacle.transform.position);
            }

            if (targetDirections != null)
            {
                Gizmos.color = Color.white;
                foreach (var direction in targetDirections)
                    GizmosHelper.DrawVector(position, direction);
            }
        }
    }
}
