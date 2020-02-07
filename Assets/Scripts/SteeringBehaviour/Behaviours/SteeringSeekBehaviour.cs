using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringSeekBehaviour : ISteeringBehaviour
    {
        [SerializeField] private Transform m_target = null;

        protected override Vector2? CalculateForceComponentsInternal()
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

            var deltaSpeed = targetSpeed - speed;
            var deltaAngle = Mathf.DeltaAngle(targetAngle, angle);

            var maxTangentForce = deltaSpeed > 0.0f ? m_maxAccelerationForce : m_maxBrakingForce;

            var tangentForce = Mathf.Sign(deltaSpeed) * maxTangentForce;
            var normalForce = Mathf.Sign(deltaAngle) * m_maxSteeringForce;

            return new Vector2(normalForce, tangentForce);

            // var force = tangentForce * m_forward + normalForce * m_right;

            // return force;
        }
    }
}
