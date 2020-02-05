using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringSeekBehaviour : ISteeringBehaviour
    {
        [SerializeField] private Transform m_target = null;

        public override Vector2? CalculateForce()
        {
            if (m_target == null)
                return null;

            var targetPos = (Vector2)m_target.position;
            var targetVelocity = (targetPos - m_position).normalized * m_maxSpeed;
            var force = (targetVelocity - m_velocity).normalized * m_maxForce;

            return force;
        }
    }
}
