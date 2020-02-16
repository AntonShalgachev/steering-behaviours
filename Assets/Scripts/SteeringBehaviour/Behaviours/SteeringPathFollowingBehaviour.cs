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
        [SerializeField, Min(0.0001f), ShowIf("m_slowNearLastPoint")] private float m_brakingRadius = 1.0f;
        [SerializeField, Min(0.0001f), ShowIf("m_slowNearLastPoint")] private float m_epsilonRadius = 0.1f;

        private int m_pointIndex = 0;

        protected override Vector2? CalculateForceComponentsInternal(float dt)
        {
            var isLast = m_pointIndex == m_path.pointsCount - 1;
            var targetIndex = Mathf.Clamp(m_pointIndex, 0, m_path.pointsCount - 1);

            var target = m_path.GetPoint(targetIndex);

            if (!isLast && Vector2.Distance(target, position) < m_pointRadius)
                m_pointIndex++;

            if (m_slowNearLastPoint && isLast)
                return SteeringBehaviourUtils.Arrival(target, m_brakingRadius, m_epsilonRadius, this);

            return SteeringBehaviourUtils.Seek(target, this);
        }
    }
}
