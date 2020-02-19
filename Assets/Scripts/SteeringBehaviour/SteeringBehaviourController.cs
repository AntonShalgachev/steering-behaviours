using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using NaughtyAttributes;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityPrototype
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SteeringBehaviourController : MonoBehaviour
    {
        [SerializeField] private float m_maxSpeed = 10.0f;

        [SerializeField, Range(-180.0f, 180.0f)] private float m_initialDirectionAngle = 0.0f;

        [Header("Forces")]
        [SerializeField] private float m_maxAccelerationForce = 10.0f;
        [SerializeField] private float m_maxBrakingForce = 10.0f;
        [SerializeField] private float m_maxSteeringForce = 10.0f;
        [SerializeField] private float m_speedForMaxSteeringForce = 10.0f;

        [Header("Speed control")]
        [SerializeField] private float m_speedControlRate = 1.0f;

        public float maxSpeed => m_maxSpeed;
        [ShowNativeProperty] public float maxAccelerationForce => m_maxAccelerationForce;
        public float maxBrakingForce => m_maxBrakingForce;
        [ShowNativeProperty] public float maxSteeringForce => Application.isPlaying ? Mathf.Lerp(0.0f, m_maxSteeringForce, Mathf.InverseLerp(0.0f, m_speedForMaxSteeringForce, speed)) : m_maxSteeringForce;

        private Rigidbody2D m_cachedBody = null;
        public Rigidbody2D body
        {
            get
            {
                if (m_cachedBody == null)
                    m_cachedBody = GetComponent<Rigidbody2D>();
                return m_cachedBody;
            }
        }

        private List<ISteeringBehaviour> m_behaviours = new List<ISteeringBehaviour>();

        public Vector2 position => transform.position;
        public Vector2 velocity => body.velocity;
        public Vector2 localAcceleration { get; private set; } = Vector2.zero;
        public Vector2 acceleration { get; private set; } = Vector2.zero;
        [ShowNativeProperty] public float speed => velocity.magnitude;

        private Vector2 m_initialForward => Vector2.up.Rotate(m_initialDirectionAngle);
        private Vector2 m_runtimeForward = Vector2.up;

        public Vector2 forward => Application.isPlaying ? m_runtimeForward : m_initialForward;
        public Vector2 right => forward.Rotate270();
        public float angle => Vector2.SignedAngle(Vector2.up, forward);

        public float mass => body.mass;

        [ShowNonSerializedField] private Vector2 m_lastAppliedForce = Vector2.zero;
        [ShowNativeProperty] private float m_lastAppliedForceMagnitude => m_lastAppliedForce.magnitude;

        public float time => Time.fixedTime;
        public float timeStep => Time.fixedDeltaTime;

        private bool m_initialized = false;

        private void TryInitialize()
        {
            if (m_initialized)
                return;

            m_runtimeForward = Vector2.up.Rotate(m_initialDirectionAngle);

            m_initialized = true;
        }

        public int AddBehaviour(ISteeringBehaviour behaviour)
        {
            TryInitialize();

            Debug.Assert(!m_behaviours.Contains(behaviour));

            var index = m_behaviours.Count;
            m_behaviours.Add(behaviour);
            return index;
        }

        public void RemoveBehaviour(ISteeringBehaviour behaviour)
        {
            m_behaviours.Remove(behaviour);
        }

        private void FixedUpdate()
        {
            Step(timeStep);
        }

        private void Step(float dt)
        {
            var newForward = velocity.normalized;
            if (speed > Mathf.Epsilon && newForward.magnitude > Mathf.Epsilon)
                m_runtimeForward = newForward;

            var brakingForce = Vector2.zero;
            if (speed > maxSpeed)
                brakingForce += velocity.normalized * (maxSpeed - speed) * m_speedControlRate;

            var steeringForce = CalculateSteeringForce(dt);

            var totalForce = steeringForce + brakingForce;

            body.AddForce(totalForce);
        }

        private Vector2 CalculateSteeringForce(float dt)
        {
            var localSteeringForce = Vector2.zero;
            var weightsSum = 0.0f;
            foreach (var behaviour in m_behaviours)
            {
                var localForce = behaviour.CalculateLocalForce(dt);
                if (localForce == null)
                    continue;

                Debug.Assert(behaviour.weight >= 0.0f);

                localSteeringForce += localForce.Value * behaviour.weight;
                weightsSum += behaviour.weight;
            }

            Debug.Assert(weightsSum >= 0.0f);

            if (weightsSum > 0.0f)
                localSteeringForce /= weightsSum;
            else
                Debug.Assert(Vector2.Distance(localSteeringForce, Vector2.zero) < Mathf.Epsilon);

            localSteeringForce = ClampForceComponents(localSteeringForce);
            m_lastAppliedForce = localSteeringForce;

            var steeringForce = CalculateForceFromComponents(localSteeringForce);

            localAcceleration = localSteeringForce / mass;
            acceleration = steeringForce / mass;

            return steeringForce;
        }

        private Vector2 ClampForceComponents(Vector2 steeringForceComponents)
        {
            var maxTangentForce = steeringForceComponents.y > 0.0f ? maxAccelerationForce : maxBrakingForce;
            var maxNormalForce = maxSteeringForce;

            steeringForceComponents.x = Mathf.Min(steeringForceComponents.x, maxNormalForce);
            steeringForceComponents.y = Mathf.Min(steeringForceComponents.y, maxTangentForce);

            return steeringForceComponents;
        }

        public Vector2 CalculateForceFromComponents(Vector2 steeringForceComponents)
        {
            var normalForce = steeringForceComponents.x;
            var tangentForce = steeringForceComponents.y;

            return normalForce * right + tangentForce * forward;
        }
    }
}
