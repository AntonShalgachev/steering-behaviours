using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringSeekBehaviour : ISteeringBehaviour
    {
        [SerializeField] private Transform m_target = null;
        [SerializeField] private bool m_slowNearTarget = true;
        [SerializeField, Min(0.0001f), ShowIf("m_slowNearTarget")] private float m_brakingRadiusMultiplier = 1.0f;
        [SerializeField, Min(0.0001f), ShowIf("m_slowNearTarget")] private float m_epsilonRadius = 0.1f;

        private float m_brakingRadius => m_maxStoppingDistance * m_brakingRadiusMultiplier;

        protected override Vector2? CalculateForceComponentsInternal()
        {
            if (m_target == null)
                return null;

            var brakingRadius = m_slowNearTarget ? m_brakingRadius : 0.0f;
            var epsilonRadius = m_slowNearTarget ? m_epsilonRadius : 0.0f;
            return SteeringBehaviourUtils.Seek(m_target.position, brakingRadius, epsilonRadius, this);
        }

        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            if (m_target == null)
                return;

            if (m_slowNearTarget)
            {
                Gizmos.color = Color.green;
                GizmosHelper.DrawCircle(m_target.position, m_brakingRadius);
                GizmosHelper.DrawCircle(m_target.position, m_epsilonRadius);
            }
        }
    }
}
