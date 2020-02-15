using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityPrototype
{
    [ExecuteInEditMode]
    public class SteeringBehaviourEditModeOrientation : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
                return;

            GetComponent<SteeringBehaviourOrientation>().UpdateOrientation(false);
        }
# endif
    }
}
