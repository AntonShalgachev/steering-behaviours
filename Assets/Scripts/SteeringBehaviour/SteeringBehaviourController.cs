using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace UnityPrototype
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SteeringBehaviourController : MonoBehaviour
    {
        [SerializeField] private float m_maxSpeed = 10.0f;
        [SerializeField] private float m_maxForce = 10.0f;

        public float maxSpeed => m_maxSpeed;
        public float maxForce => m_maxForce;

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

        [ShowNativeProperty] public Vector2 position => transform.position;
        [ShowNativeProperty] public Vector2 velocity => body.velocity;
        [ShowNativeProperty] public float speed => velocity.magnitude;

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
            var totalForce = Vector2.zero;
            foreach (var behaviour in m_behaviours)
            {
                var forceResult = behaviour.CalculateForce();
                if (forceResult == null)
                    continue;

                var force = forceResult.Value;
                totalForce += force;
            }

            body.AddForce(totalForce);
        }
    }
}
