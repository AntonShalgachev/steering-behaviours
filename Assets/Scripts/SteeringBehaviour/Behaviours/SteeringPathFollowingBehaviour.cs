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
        [SerializeField] private bool m_slowNearLastPoint = true;
        [SerializeField, Min(0.0001f), ShowIf("m_slowNearLastPoint")] private float m_brakingRadiusMultiplier = 1.0f;
        [SerializeField, Min(0.0001f), ShowIf("m_slowNearLastPoint")] private float m_epsilonRadius = 0.1f;

        private float m_brakingRadius => m_maxStoppingDistance * m_brakingRadiusMultiplier;

        private int m_pointIndex = 0;

        protected override Vector2? CalculateForceComponentsInternal(float dt)
        {
            if (m_path.pointsCount <= 0)
                return null;

            var isLast = m_pointIndex == m_path.pointsCount - 1;
            var targetIndex = Mathf.Clamp(m_pointIndex, 0, m_path.pointsCount - 1);

            var target = m_path.GetPoint(targetIndex);

            if (!isLast && Vector2.Distance(target, position) < m_pointRadius)
                m_pointIndex++;

            var brakingRadius = 0.0f;
            var epsilonRadius = 0.0f;

            if (m_slowNearLastPoint && isLast)
            {
                brakingRadius = m_brakingRadius;
                epsilonRadius = m_epsilonRadius;
            }

            return SteeringBehaviourUtils.Seek(target, brakingRadius, epsilonRadius, this);
        }

        protected override void DrawGizmos()
        {
            if (m_path == null)
                return;

            m_path.DrawCurveGizmos(m_slowNearLastPoint ? m_brakingRadius : 0.0f);
        }
    }
}
