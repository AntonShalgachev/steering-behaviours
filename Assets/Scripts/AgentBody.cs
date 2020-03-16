using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class AgentBody : MonoBehaviour
    {
        public SteeringBehaviourController agent { get; private set; }

        private void Awake()
        {
            agent = GetComponentInParent<SteeringBehaviourController>();
        }
    }
}
