using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;
using NaughtyAttributes;

namespace UnityPrototype
{
    public class SteeringCollisionAvoidanceBehaviour : ISteeringBehaviour
    {
        private struct ObstacleData
        {
            public Vector2 position;
            public float force;
            public float distance;
            public float weight;
            public Vector2 direction;
        }

        [SerializeField] private float m_visibilityRange = 5.0f;
        [SerializeField, Layer] private int m_visbilityLayer = -1;
        [SerializeField] private float m_minDistance = 1.0f;
        [SerializeField] private float m_maxDistance = 5.0f;

        [SerializeField] private float m_hullRadius = 1.0f;
        [SerializeField, Min(0.0f)] private float m_safetyDistance = 1.0f;

        private Sensor m_sensor = null;

        private ObstacleData? m_currentTargetData = null;

        protected override void Initialize()
        {
            m_sensor = m_controller.AddSensor(m_visibilityRange, m_visbilityLayer);
        }

        protected override Vector2? CalculateForceComponentsInternal()
        {
            // beware, this code stinks

            activation = 0.0f;

            ObstacleData? targetData = null;

            foreach (var obstacle in m_sensor.touchingObjects)
            {
                var data = CalculateForceForObstacle(obstacle as CircleCollider2D); // so far only circles are handled
                if (!data.HasValue)
                    continue;

                if (!targetData.HasValue || targetData.Value.distance > data.Value.distance)
                    targetData = data;
            }

            m_currentTargetData = targetData;

            if (!targetData.HasValue)
                return null;

            activation = targetData.Value.weight;
            return new Vector2(targetData.Value.force, 0.0f);
        }

        private ObstacleData? CalculateForceForObstacle(CircleCollider2D obstacle)
        {
            if (obstacle == null)
                return null;

            var obstaclePosition = (Vector2)obstacle.transform.position;
            var scale = obstacle.transform.lossyScale;
            var radius = obstacle.radius * Mathf.Max(scale.x, scale.y);
            var radiusWithHull = radius + m_hullRadius;
            var radiusWithSafety = radius + m_safetyDistance;

            var toCenter = obstaclePosition - position;
            var distanceToCenter = toCenter.magnitude;
            var distanceToBound = distanceToCenter - radius;
            var weight = Mathf.InverseLerp(m_maxDistance, m_minDistance, distanceToBound);

            var deltaAngleToCenter = Vector2.SignedAngle(m_forward, toCenter);
            var directionMultiplier = -Mathf.Sign(deltaAngleToCenter);

            var targetDirection = Vector2.zero;

            if (distanceToCenter < radiusWithHull)
            {
                return new ObstacleData
                {
                    position = obstaclePosition,
                    force = -maxSteeringForce * directionMultiplier,
                    distance = distanceToBound,
                    weight = weight,
                    direction = targetDirection,
                };
            }

            var dAngle = Mathf.Atan(m_safetyDistance / distanceToCenter);
            var deltaAngleFromCenter = Mathf.Asin(radiusWithHull / distanceToCenter) + dAngle;
            deltaAngleFromCenter *= Mathf.Rad2Deg;
            if (Mathf.Abs(deltaAngleToCenter) > deltaAngleFromCenter)
                return null;

            var targetAngle = angle + deltaAngleToCenter + directionMultiplier * deltaAngleFromCenter;

#if DEBUG
            targetDirection = Vector2.up.Rotate(targetAngle) * Mathf.Sqrt(distanceToCenter * distanceToCenter - radiusWithHull * radiusWithHull);
#endif

            var force = CalculateNormalForce(targetAngle) * weight;

            return new ObstacleData
            {
                position = obstaclePosition,
                force = force,
                distance = distanceToBound,
                weight = weight,
                direction = targetDirection,
            };
        }

        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            Gizmos.color = Color.magenta;
            GizmosHelper.DrawCircle(position, m_minDistance);
            GizmosHelper.DrawCircle(position, m_maxDistance);

            Gizmos.color = Color.black;
            GizmosHelper.DrawCircle(position, m_hullRadius);
            GizmosHelper.DrawCircle(position, m_hullRadius + m_safetyDistance);

            if (m_currentTargetData.HasValue)
            {
                var data = m_currentTargetData.Value;

                Gizmos.color = Color.white;
                GizmosHelper.DrawVector(position, data.direction);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(position, data.position);
            }
        }
    }
}
