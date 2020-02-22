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

            // [Min(0.0f)] public float velocityAngleAttenuation = 15.0f;
            // [Min(0.0f)] public float velocityMagnitudeAttenuation = 3.0f;
            [Min(0.0f)] public float tangentForceMultiplier = 1.0f;
            [Min(0.0f)] public float normalForceMultiplier = 1.0f;
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

        [ShowNativeProperty] protected float m_maxStoppingDistance => 0.5f * maxSpeed * maxSpeed / (maxBrakingForce / m_mass);
        [ShowNativeProperty] protected float m_currentStoppingDistance => 0.5f * m_controller.speed * m_controller.speed / (maxBrakingForce / m_mass);

        protected float m_mass => m_controller.mass;
        protected float m_time => m_controller.time;
        protected float m_timeStep => m_controller.timeStep;

        private Vector2 m_lastAppliedForce = Vector2.zero;
        private Vector2 m_lastAppliedForceComponents = Vector2.zero;
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

            Initialize();
        }

        private void OnDisable()
        {
            m_controller.RemoveBehaviour(this);
        }

        public Vector2? CalculateLocalForce()
        {
            var force = CalculateForceComponentsInternal();

            m_lastAppliedForceComponents = force.GetValueOrDefault(Vector2.zero) * m_weight;
            m_lastAppliedForce = m_controller.CalculateForceFromComponents(m_lastAppliedForceComponents);

            return force;
        }

        protected virtual void Initialize() { }

        protected abstract Vector2? CalculateForceComponentsInternal();

        public Vector2 CalculateForceForDirection(Vector2 direction, float speedMultiplier = 1.0f)
        {
            float epsilon = 1e-5f;

            Debug.Assert(Mathf.Abs(direction.magnitude - 1.0f) < epsilon, "Direction isn't normalized");
            return CalculateForceForVelocity(direction * maxSpeed * speedMultiplier);
        }

        public Vector2 CalculateForceForVelocity(Vector2 targetVelocity)
        {
            var targetSpeed = targetVelocity.magnitude;
            var tangentForce = CalculateTangentForce(targetSpeed);

            var normalForce = 0.0f;
            var targetAngle = m_controller.angle;
            if (Mathf.Abs(targetSpeed) > Mathf.Epsilon)
                targetAngle = Vector2.SignedAngle(Vector2.up, targetVelocity);
            normalForce = CalculateNormalForce(targetAngle);

            return ClampForceComponents(new Vector2(normalForce, tangentForce), targetSpeed, targetAngle);
        }

        private Vector2 ClampForceComponents(Vector2 steeringForceComponents, float targetSpeed, float targetAngle)
        {
            var forceToTargetSpeed = (targetSpeed - m_controller.speed) * m_mass / m_timeStep;

            var tan = Mathf.Tan(-(targetAngle - m_controller.angle) * Mathf.Deg2Rad);
            var forceToTargetAngle = m_controller.speed * tan * m_mass / m_timeStep;

            var minTangentForce = -maxBrakingForce;
            var maxTangentForce = maxAccelerationForce;
            var minNormalForce = -maxSteeringForce;
            var maxNormalForce = maxSteeringForce;

            if (forceToTargetSpeed > 0.0f)
                maxTangentForce = Mathf.Min(maxTangentForce, forceToTargetSpeed);
            else
                minTangentForce = Mathf.Max(minTangentForce, forceToTargetSpeed);

            if (forceToTargetAngle > 0.0f)
                maxNormalForce = Mathf.Min(maxNormalForce, forceToTargetAngle);
            else
                minNormalForce = Mathf.Max(minNormalForce, forceToTargetAngle);

            steeringForceComponents.x = Mathf.Clamp(steeringForceComponents.x, minNormalForce, maxNormalForce);
            steeringForceComponents.y = Mathf.Clamp(steeringForceComponents.y, minTangentForce, maxTangentForce);

            return steeringForceComponents;
        }

        public float CalculateNormalForce(float targetAngle)
        {
            var angle = m_controller.angle;
            var deltaAngle = Mathf.DeltaAngle(angle, targetAngle);
            return CalculateForceFromDelta(deltaAngle, -m_commonParameters.normalForceMultiplier);
        }

        public float CalculateTangentForce(float targetSpeed)
        {
            var speed = m_controller.speed;
            var deltaSpeed = targetSpeed - speed;
            return CalculateForceFromDelta(deltaSpeed, m_commonParameters.tangentForceMultiplier);
        }

        private float CalculateForceFromDelta(float delta, float multiplier)
        {
            // var power = 1.0f;
            // var powDelta = Mathf.Sign(delta) * Mathf.Pow(Mathf.Abs(delta), power);
            // return powDelta * multiplier;
            return delta * multiplier;
        }

        protected virtual void DrawGizmos()
        {
            if (m_behaviourIndex < 0)
                return;

            var colorIndex = m_behaviourIndex % m_debugColors.Length;
            Gizmos.color = m_debugColors[colorIndex];

            GizmosHelper.DrawVector(transform.position, m_right * m_lastAppliedForceComponents.x);
            GizmosHelper.DrawVector(transform.position, m_forward * m_lastAppliedForceComponents.y);
        }

        private void OnDrawGizmos()
        {
            if (!enabled)
                return;

            DrawGizmos();
        }
    }
}
