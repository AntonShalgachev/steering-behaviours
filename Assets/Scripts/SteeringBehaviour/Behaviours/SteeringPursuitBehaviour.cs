using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringPursuitBehaviour : ISteeringBehaviour
    {
        [SerializeField] private SteeringBehaviourController m_target = null;
        [SerializeField] private float m_predictionTime = 0.0f;

        private Vector2 m_targetPosition => m_target.position + m_target.velocity * m_predictionTime;

        protected override Vector2? CalculateForceComponentsInternal()
        {
            return SteeringBehaviourUtils.Seek(m_targetPosition, 0.0f, 0.0f, this);
        }

        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(m_targetPosition, 0.1f);
        }
    }
}
