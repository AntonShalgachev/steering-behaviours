using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringArrivalBehaviour : ISteeringBehaviour
    {
        [SerializeField] private Transform m_target = null;
        [SerializeField] private float m_radius = 1.0f;

        protected override Vector2? CalculateForceComponentsInternal()
        {
            if (m_target == null)
                return null;

            return SteeringBehaviourUtils.Arrival(m_target.position, m_radius, this);
        }

        private void OnDrawGizmos()
        {
            if (m_target == null)
                return;

            Gizmos.color = Color.magenta;
            GizmosHelper.DrawCircle(m_target.position, m_radius);
        }
    }
}
