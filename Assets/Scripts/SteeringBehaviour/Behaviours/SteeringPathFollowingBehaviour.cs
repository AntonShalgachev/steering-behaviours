using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringPathFollowingBehaviour : ISteeringBehaviour
    {
        [SerializeField] private Path m_path = null;
        [SerializeField] private float m_pointRadius = 1.0f;
        [SerializeField] private bool m_loop = false;
        [SerializeField, ShowIf("m_canSlowNearLastPoint")] private bool m_slowNearLastPoint = true;
        [SerializeField, Min(0.0001f), ShowIf("m_shouldSlowNearLastPoint")] private float m_brakingRadiusMultiplier = 1.0f;
        [SerializeField, Min(0.0001f), ShowIf("m_shouldSlowNearLastPoint")] private float m_epsilonRadius = 0.1f;

        private float m_brakingRadius => m_maxStoppingDistance * m_brakingRadiusMultiplier;
        private bool m_shouldSlowNearLastPoint => m_canSlowNearLastPoint && m_slowNearLastPoint;
        private bool m_canSlowNearLastPoint => !m_loop;

        private int m_pointIndex = 0;

        protected override Vector2? CalculateForceComponentsInternal()
        {
            if (m_path.pointsCount <= 0)
                return null;

            activation = 1.0f;
            var isLast = m_pointIndex == m_path.pointsCount - 1;
            var canIncrementIndex = m_loop || !isLast;

            var target = m_path.GetPoint(m_pointIndex);

            if (canIncrementIndex && Vector2.Distance(target, position) < m_pointRadius)
                m_pointIndex = (m_pointIndex + 1) % m_path.pointsCount;

            var brakingRadius = 0.0f;
            var epsilonRadius = 0.0f;

            if (m_shouldSlowNearLastPoint && isLast)
            {
                brakingRadius = m_brakingRadius;
                epsilonRadius = m_epsilonRadius;
            }

            return SteeringBehaviourUtils.Seek(target, brakingRadius, epsilonRadius, this);
        }

        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            if (m_path == null)
                return;

            m_path.DrawCurveGizmos(m_pointRadius, m_shouldSlowNearLastPoint ? m_brakingRadius : 0.0f, m_loop, m_pointIndex);
        }
    }
}
