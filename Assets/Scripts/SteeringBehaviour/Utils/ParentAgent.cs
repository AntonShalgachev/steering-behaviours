using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class ParentAgent : MonoBehaviour
    {
        public SteeringBehaviourController agent { get; private set; }

        private void Awake()
        {
            agent = GetComponentInParent<SteeringBehaviourController>();
        }
    }
}
