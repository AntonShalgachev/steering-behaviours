using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

namespace UnityPrototype
{
    public class SteeringWanderBehaviour : ISteeringBehaviour
    {
        private enum NoiseType
        {
            Perlin,
            Sine,
            Random,
        }

        private enum NoiseMode
        {
            NoiseAsValue,
            NoiseAsSpeed,
        }

        [SerializeField] private NoiseType m_noiseType = NoiseType.Perlin;
        [SerializeField] private NoiseMode m_noiseMode = NoiseMode.NoiseAsSpeed;

        [SerializeField] private float m_angleRange = 180.0f;
        [SerializeField] private float m_frequency = 1.0f;

        [SerializeField] private float m_circleDistance = 5.0f;
        [SerializeField] private float m_circleRadius = 4.0f;

        [SerializeField] private float m_seed = 0.0f;

        private float m_wanderAngle = 0.0f;
        private Vector2 m_circleOffset => m_forward * m_circleDistance;
        private Vector2 m_wanderOffset => Vector2.up.Rotate(m_wanderAngle) * m_circleRadius;
        private Vector2 m_targetPoint => position + m_circleOffset + m_wanderOffset;

        private List<Vector2> m_initialApproximation = null;

        protected override void Initialize()
        {
            m_wanderAngle = m_controller.angle;
        }

        protected override Vector2? CalculateForceComponentsInternal(float dt)
        {
            m_wanderAngle = GetNextAngle(m_wanderAngle, m_controller.time, dt);

            return SteeringBehaviourUtils.Seek(m_targetPoint, this);
        }

        private float GetNextAngle(float prevAngle, float t, float dt)
        {
            switch (m_noiseMode)
            {
                case NoiseMode.NoiseAsSpeed:
                    return prevAngle + GetNoise(t) * m_angleRange * dt;
                case NoiseMode.NoiseAsValue:
                    return GetNoise(t) * m_angleRange;
            }

            Debug.Assert(false);
            return 0.0f;
        }

        private float GetNoise(float t)
        {
            switch (m_noiseType)
            {
                case NoiseType.Perlin:
                    return 2.0f * Mathf.PerlinNoise(m_seed, m_seed + t * m_frequency) - 1.0f; ;
                case NoiseType.Sine:
                    return Mathf.Sin(m_seed + t * m_frequency);
                case NoiseType.Random:
                    return Random.Range(-1.0f, 1.0f);
            }

            Debug.Assert(false);
            return 0.0f;
        }

        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            if (Application.isPlaying)
            {
                if (m_initialApproximation == null)
                    m_initialApproximation = CalculateApproximation(position, m_forward, m_wanderAngle);

                Gizmos.color = Color.red;
                GizmosHelper.DrawCurve(m_initialApproximation);

                Gizmos.color = Color.green;
                var circleCenter = position + m_forward * m_circleDistance;
                GizmosHelper.DrawCircle(circleCenter, m_circleRadius);
                Gizmos.color = Color.white;
                GizmosHelper.DrawVector(position, m_forward, m_circleRadius + m_circleDistance);
                Gizmos.color = Color.black;
                GizmosHelper.DrawVector(circleCenter, m_wanderOffset, 1.0f);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(m_targetPoint, 0.2f);
            }
            else
            {
                Gizmos.color = Color.magenta;
                GizmosHelper.DrawFunction(position, GetNoise, 0.1f, 100.0f);

                Gizmos.color = Color.green;
                var initialAngle = Vector2.SignedAngle(Vector2.up, transform.up);
                var points = CalculateApproximation(position, transform.up, initialAngle);

                GizmosHelper.DrawCurve(points);
            }
        }

        private List<Vector2> CalculateApproximation(Vector2 position, Vector2 forward, float initialAngle)
        {
            var maxT = 1000.0f;
            var step = Time.fixedDeltaTime;

            var pos = position;
            var wanderAngle = initialAngle;

            var points = new List<Vector2>();
            points.Add(pos);

            Random.InitState((int)(m_seed * 1000000));

            for (var t = 0.0f; t <= maxT; t += step)
            {
                wanderAngle = GetNextAngle(wanderAngle, t, step);
                var circleOffset = forward * m_circleDistance;
                var wanderOffset = Vector2.up.Rotate(wanderAngle) * m_circleRadius;
                var targetPoint = position + circleOffset + wanderOffset;
                var targetVelocity = (targetPoint - position).normalized * maxSpeed;
                pos += targetVelocity * step;

                forward = targetVelocity.normalized;

                points.Add(pos);
            }

            return points;
        }
    }
}
