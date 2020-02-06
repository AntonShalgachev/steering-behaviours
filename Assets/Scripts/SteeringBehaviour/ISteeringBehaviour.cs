using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    [RequireComponent(typeof(SteeringBehaviourController))]
    public abstract class ISteeringBehaviour : MonoBehaviour
    {
        [SerializeField] private float m_maxSpeedMultiplier = 0.5f;
        [SerializeField] private float m_maxForceMultiplier = 0.5f;

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

        protected Vector2 m_position => m_controller.position;
        protected Vector2 m_velocity => m_controller.velocity;
        protected float m_maxSpeed => m_controller.maxSpeed * m_maxSpeedMultiplier;
        protected float m_maxForce => m_controller.maxForce * m_maxForceMultiplier;
        protected Vector2 m_forward => m_controller.forward;
        protected Vector2 m_right => m_controller.right;

        private void OnEnable()
        {
            m_controller.AddBehaviour(this);
        }

        private void OnDisable()
        {
            m_controller.RemoveBehaviour(this);
        }

        public abstract Vector2? CalculateForce();
    }
}
