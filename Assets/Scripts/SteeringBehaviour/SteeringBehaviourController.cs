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

        public void AddBehaviour(ISteeringBehaviour behaviour)
        {
            m_behaviours.Add(behaviour);
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

            var steeringForceComponents = Vector2.zero;
            foreach (var behaviour in m_behaviours)
            {
                var forceComponentsResult = behaviour.CalculateForceComponents();
                if (forceComponentsResult == null)
                    continue;

                var forceComponents = forceComponentsResult.Value;
                steeringForceComponents += forceComponents;
            }

            steeringForceComponents = ClampForceComponents(steeringForceComponents);
            m_lastAppliedForce = steeringForceComponents;
            var totalForce = CalculateForceFromComponents(steeringForceComponents) + brakingForce;

            body.AddForce(totalForce);
        }

        private Vector2 ClampForceComponents(Vector2 steeringForceComponents)
        {
            var maxTangentForce = steeringForceComponents.y > 0.0f ? maxAccelerationForce : maxBrakingForce;
            var maxNormalForce = maxSteeringForce;

            steeringForceComponents.x = Mathf.Min(steeringForceComponents.x, maxNormalForce);
            steeringForceComponents.y = Mathf.Min(steeringForceComponents.y, maxTangentForce);

            return steeringForceComponents;
        }

        private Vector2 CalculateForceFromComponents(Vector2 steeringForceComponents)
        {
            var normalForce = steeringForceComponents.x;
            var tangentForce = steeringForceComponents.y;

            return tangentForce * forward + normalForce * right;
        }
    }
}
