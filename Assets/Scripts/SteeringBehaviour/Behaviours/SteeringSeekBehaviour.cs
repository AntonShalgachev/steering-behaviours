using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringSeekBehaviour : ISteeringBehaviour
    {
        [SerializeField] private Transform m_target = null;

        protected override Vector2? CalculateForceComponentsInternal(float dt)
        {
            if (m_target == null)
                return null;

            return SteeringBehaviourUtils.Seek(m_target.position, this);
        }
    }
}
