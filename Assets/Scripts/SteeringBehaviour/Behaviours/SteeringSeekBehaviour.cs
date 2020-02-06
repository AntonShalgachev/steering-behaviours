using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringSeekBehaviour : ISteeringBehaviour
    {
        [SerializeField] private float m_tangentWeight = 1.0f;
        [SerializeField] private float m_normalWeight = 10.0f;

        [SerializeField] private Transform m_target = null;

        public override Vector2? CalculateForce()
        {
            if (m_target == null)
                return null;

            var targetPos = (Vector2)m_target.position;
            var targetVelocity = (targetPos - m_position).normalized * m_maxSpeed;

            return CalculateForce(targetVelocity);
        }

        private Vector2 CalculateForce(Vector2 targetVelocity)
        {
            var speed = m_velocity.magnitude;
            var targetSpeed = targetVelocity.magnitude;

            var angle = Vector2.SignedAngle(Vector2.up, m_velocity);
            var targetAngle = Vector2.SignedAngle(Vector2.up, targetVelocity);

            var tangentForce = Mathf.Sign(targetSpeed - speed) * m_tangentWeight;
            var normalForce = Mathf.Sign(Mathf.DeltaAngle(targetAngle, angle)) * m_normalWeight;

            // var force = (targetVelocity - m_velocity).normalized * m_maxForce;
            var force = tangentForce * m_forward + normalForce * m_right;
            force = force.normalized * m_maxForce;

            return force;
        }
    }
}
