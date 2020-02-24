using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringCollisionAvoidanceBehaviour : ISteeringBehaviour
    {
        [SerializeField] private float m_detectionRadius = 3.0f;

        protected override Vector2? CalculateForceComponentsInternal()
        {
            return null;
        }
    }
}
