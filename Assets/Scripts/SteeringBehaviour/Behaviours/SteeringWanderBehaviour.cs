using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using Unity.Mathematics;

namespace UnityPrototype
{
    public class SteeringWanderBehaviour : ISteeringBehaviour
    {
        private enum NoiseType
        {
            PerlinNoise,
            CNoise,
            SNoise,
            SRDNoise,
            Sine,
            Random,
        }

        private enum NoiseMode
        {
            NoiseAsValue,
            NoiseAsSpeed,
        }

        [SerializeField] private NoiseType m_noiseType = NoiseType.PerlinNoise;
        [SerializeField, Range(0.0f, 1.0f)] private float m_amplitude = 1.0f;
        [SerializeField, Min(0.0f)] private float m_frequency = 1.0f;

        [SerializeField] private float m_seed = 0.0f;
        [SerializeField] private bool m_keepMaxSpeed = false;

        protected override Vector2? CalculateForceComponentsInternal(float dt)
        {
            var normalForce = CalculateNormalForce(m_time);
            var tangentForce = m_keepMaxSpeed ? ClaculateTangentForce(maxSpeed) : 0.0f;

            return new Vector2(normalForce, tangentForce);
        }

        private float CalculateNormalForce(float t)
        {
            return Mathf.Clamp01(m_amplitude) * GetNoise(t) * maxSteeringForce;
        }

        private float GetNoise(float t)
        {
            switch (m_noiseType)
            {
                case NoiseType.PerlinNoise:
                    return 2.0f * Mathf.PerlinNoise(m_seed, m_seed + t * m_frequency) - 1.0f;
                case NoiseType.CNoise:
                    return noise.cnoise(new float2(m_seed, t * m_frequency));
                case NoiseType.SNoise:
                    return noise.snoise(new float2(m_seed, m_seed + t * m_frequency));
                case NoiseType.SRDNoise:
                    return noise.srdnoise(new float2(m_seed, m_seed + t * m_frequency)).x;
                case NoiseType.Sine:
                    return Mathf.Sin(m_seed + t * m_frequency);
                case NoiseType.Random:
                    return UnityEngine.Random.Range(-1.0f, 1.0f);
            }

            Debug.Assert(false);
            return 0.0f;
        }

        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            if (!Application.isPlaying)
            {
                Gizmos.color = Color.green;
                var initialAngle = Vector2.SignedAngle(Vector2.up, transform.up);
                var points = CalculateApproximation(position);

                GizmosHelper.DrawCurve(points);
            }
        }

        private List<Vector2> CalculateApproximation(Vector2 position)
        {
            var maxT = 100.0f;
            var step = Time.fixedDeltaTime;

            var pos = position;
            var velocity = m_forward * maxSpeed;

            var points = new List<Vector2>();
            points.Add(pos);

            for (var t = 0.0f; t <= maxT; t += step)
            {
                var forward = velocity.normalized;
                var right = forward.Rotate270();

                var normalForce = CalculateNormalForce(t);
                var force = right * normalForce;
                var acceleration = force / m_mass;
                velocity += acceleration * step;
                velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
                pos += velocity * step;

                points.Add(pos);
            }

            return points;
        }
    }
}
