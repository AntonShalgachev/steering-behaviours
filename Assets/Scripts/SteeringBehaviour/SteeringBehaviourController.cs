using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace UnityPrototype
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SteeringBehaviourController : MonoBehaviour
    {
        [SerializeField] private float m_maxSpeed = 10.0f;

        [Header("Forces")]
        [SerializeField] private float m_maxSteeringForce = 10.0f;
        [SerializeField] private float m_maxAccelerationForce = 10.0f;
        [SerializeField] private float m_maxBrakingForce = 10.0f;

        [Header("Speed control")]
        [SerializeField] private float m_speedControlRate = 1.0f;

        public float maxSpeed => m_maxSpeed;
        public float maxSteeringForce => m_maxSteeringForce;
        public float maxAccelerationForce => m_maxAccelerationForce;
        public float maxBrakingForce => m_maxBrakingForce;

        private Rigidbody2D m_body = null;
        public Rigidbody2D body
        {
            get
            {
                if (m_body == null)
                    m_body = GetComponent<Rigidbody2D>();
                return m_body;
            }
        }

        private List<ISteeringBehaviour> m_behaviours = new List<ISteeringBehaviour>();

        public Vector2 position => transform.position;
        public Vector2 velocity => body.velocity;
        [ShowNativeProperty] public float speed => velocity.magnitude;
        public Vector2 forward => velocity.magnitude > Mathf.Epsilon ? velocity.normalized : Vector2.up;
        public Vector2 right => forward.Rotate(-90);

        [ShowNonSerializedField] private Vector2 m_lastAppliedForce = Vector2.zero;
        [ShowNativeProperty] private float m_lastAppliedForceMagnitude => m_lastAppliedForce.magnitude;

        public int AddBehaviour(ISteeringBehaviour behaviour)
        {
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
            var brakingForce = Vector2.zero;
            if (speed > maxSpeed)
                brakingForce += velocity.normalized * (maxSpeed - speed) * m_speedControlRate;

            var steeringForce = CalculateSteeringForce();

            var totalForce = steeringForce + brakingForce;

            body.AddForce(totalForce);
        }

        private Vector2 CalculateSteeringForce()
        {
            var steeringForceComponents = Vector2.zero;
            var weightsSum = 0.0f;
            foreach (var behaviour in m_behaviours)
            {
                var forceComponents = behaviour.CalculateForceComponents();
                if (forceComponents == null)
                    continue;

                Debug.Assert(behaviour.weight >= 0.0f);

                steeringForceComponents += forceComponents.Value * behaviour.weight;
                weightsSum += behaviour.weight;
            }

            Debug.Assert(weightsSum >= 0.0f);

            if (weightsSum > 0.0f)
                steeringForceComponents /= weightsSum;
            else
                Debug.Assert(Vector2.Distance(steeringForceComponents, Vector2.zero) < Mathf.Epsilon);

            steeringForceComponents = ClampForceComponents(steeringForceComponents);
            m_lastAppliedForce = steeringForceComponents;

            return CalculateForceFromComponents(steeringForceComponents);
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

            return tangentForce * forward + normalForce * right;
        }
    }
}
