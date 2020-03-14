using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringEvadeBehaviour : ISteeringBehaviour
    {
        [SerializeField] private SteeringBehaviourController m_target = null;
        [SerializeField] private float m_predictionTime = 0.0f;

        private Vector2 m_targetPosition => m_target.position + m_target.velocity * m_predictionTime;

        protected override Vector2? CalculateForceComponentsInternal()
        {
            activation = 1.0f;
            return SteeringBehaviourUtils.Flee(m_targetPosition, this);
        }

        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            if (m_target == null)
                return;

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(m_targetPosition, 0.1f);
        }
    }
}
