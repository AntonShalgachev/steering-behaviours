using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace UnityPrototype
{
    [RequireComponent(typeof(SteeringBehaviourController))]
    public abstract class ISteeringBehaviour : MonoBehaviour
    {
        [System.Serializable]
        public class CommonBehaviourParameters
        {
            [Min(0.0f)] public float maxSpeedMultiplier = 1.0f;
            [Min(0.0f)] public float maxForceMultiplier = 1.0f;

            [Min(0.0f)] public float velocityAngleAttenuation = 15.0f;
            [Min(0.0f)] public float velocityMagnitudeAttenuation = 3.0f;
        }

        [SerializeField, Min(0.0f)] private float m_weight = 1.0f;
        [SerializeField] private CommonBehaviourParameters m_commonParameters = new CommonBehaviourParameters();

        public float weight => m_weight;

        private SteeringBehaviourController m_cachedController = null;
        private SteeringBehaviourController m_controller
        {
            get
            {
                if (m_cachedController == null)
                {
                    m_cachedController = GetComponent<SteeringBehaviourController>();
                    if (m_cachedController == null)
                        Debug.LogError($"No {nameof(SteeringBehaviourController)} was found");
                }

                return m_cachedController;
            }
        }

        public Vector2 position => m_controller.position;
        public Vector2 velocity => m_controller.velocity;
        [ShowNativeProperty] public float maxSpeed => m_controller.maxSpeed * m_commonParameters.maxSpeedMultiplier;
        [ShowNativeProperty] public float maxSteeringForce => m_controller.maxSteeringForce * m_commonParameters.maxForceMultiplier;
        [ShowNativeProperty] public float maxAccelerationForce => m_controller.maxAccelerationForce * m_commonParameters.maxForceMultiplier;
        [ShowNativeProperty] public float maxBrakingForce => m_controller.maxBrakingForce * m_commonParameters.maxForceMultiplier;
        protected Vector2 m_forward => m_controller.forward;
        protected Vector2 m_right => m_controller.right;

        private Vector2 m_lastAppliedForce = Vector2.zero;
        [ShowNativeProperty] private float m_lastAppliedForceMagnitude => m_lastAppliedForce.magnitude;

        private int m_behaviourIndex = -1;
        private Color[] m_debugColors = new Color[]{
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.magenta,
            Color.cyan,
            Color.black,
            Color.white,
            Color.gray,
        };

        private void OnEnable()
        {
            m_behaviourIndex = m_controller.AddBehaviour(this);
            Debug.Assert(m_behaviourIndex >= 0);

            Debug.Assert(m_weight >= 0.0f);
        }

        private void OnDisable()
        {
            m_controller.RemoveBehaviour(this);
        }

        public Vector2? CalculateForceComponents()
        {
            var force = CalculateForceComponentsInternal();

            m_lastAppliedForce = m_controller.CalculateForceFromComponents(force.GetValueOrDefault(Vector2.zero) * m_weight);

            return force;
        }

        protected abstract Vector2? CalculateForceComponentsInternal();

        public Vector2 CalculateForceForVelocity(Vector2 targetVelocity)
        {
            var speed = velocity.magnitude;
            var targetSpeed = targetVelocity.magnitude;

            var angle = Vector2.SignedAngle(Vector2.up, velocity);
            var targetAngle = Vector2.SignedAngle(Vector2.up, targetVelocity);

            var deltaSpeed = targetSpeed - speed;
            var deltaAngle = Mathf.DeltaAngle(targetAngle, angle);

            if (Mathf.Abs(targetSpeed) < Mathf.Epsilon)
                deltaAngle = 0.0f;

            deltaSpeed = Mathf.Clamp(deltaSpeed, -m_commonParameters.velocityMagnitudeAttenuation, m_commonParameters.velocityMagnitudeAttenuation) / m_commonParameters.velocityMagnitudeAttenuation;
            deltaAngle = Mathf.Clamp(deltaAngle, -m_commonParameters.velocityAngleAttenuation, m_commonParameters.velocityAngleAttenuation) / m_commonParameters.velocityAngleAttenuation;

            var maxTangentForce = deltaSpeed > 0.0f ? maxAccelerationForce : maxBrakingForce;

            var tangentForce = deltaSpeed * maxTangentForce;
            var normalForce = deltaAngle * maxSteeringForce;

            return new Vector2(normalForce, tangentForce);
        }

        private void OnDrawGizmos()
        {
            if (m_behaviourIndex < 0)
                return;

            var colorIndex = m_behaviourIndex % m_debugColors.Length;
            Gizmos.color = m_debugColors[colorIndex];

            Gizmos.DrawLine(transform.position, transform.position + (Vector3)m_lastAppliedForce);
        }
    }
}
