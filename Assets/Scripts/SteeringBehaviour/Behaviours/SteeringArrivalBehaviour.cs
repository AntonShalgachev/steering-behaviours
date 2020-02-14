using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringArrivalBehaviour : ISteeringBehaviour
    {
        [SerializeField] private Transform m_target = null;
        [SerializeField] private bool m_slowNearTarget = true;
        [SerializeField, Min(0.0001f), ShowIf("m_slowNearTarget")] private float m_brakingRadius = 1.0f;
        [SerializeField, Min(0.0001f), ShowIf("m_slowNearTarget")] private float m_epsilonRadius = 0.1f;

        protected override Vector2? CalculateForceComponentsInternal(float dt)
        {
            if (m_target == null)
                return null;

            if (!m_slowNearTarget)
                return SteeringBehaviourUtils.Seek(m_target.position, this);
            return SteeringBehaviourUtils.Arrival(m_target.position, m_brakingRadius, m_epsilonRadius, this);
        }

        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            if (m_target == null)
                return;

            Gizmos.color = Color.magenta;
            GizmosHelper.DrawCircle(m_target.position, m_brakingRadius);
            GizmosHelper.DrawCircle(m_target.position, m_epsilonRadius);
        }
    }
}
