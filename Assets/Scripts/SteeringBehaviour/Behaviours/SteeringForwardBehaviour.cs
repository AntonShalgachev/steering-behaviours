using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringForwardBehaviour : ISteeringBehaviour
    {
        protected override Vector2? CalculateForceComponentsInternal()
        {
            activation = 1.0f;
            return SteeringBehaviourUtils.Seek(position + m_forward, 0.0f, 0.0f, this);
        }
    }
}
