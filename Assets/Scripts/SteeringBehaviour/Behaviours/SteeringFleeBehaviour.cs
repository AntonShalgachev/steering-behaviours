using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringFleeBehaviour : ISteeringBehaviour
    {
        [SerializeField] private Transform m_threat = null;

        protected override Vector2? CalculateForceComponentsInternal(float dt)
        {
            if (m_threat == null)
                return null;

            return SteeringBehaviourUtils.Flee(m_threat.position, this);
        }
    }
}
