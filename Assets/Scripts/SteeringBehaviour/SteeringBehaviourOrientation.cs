using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringBehaviourOrientation : MonoBehaviour
    {
        [SerializeField] private float m_rotationSpeed = 1.0f;

        private SteeringBehaviourController m_controller = null;

        private void Awake()
        {
            m_controller = GetComponentInParent<SteeringBehaviourController>();
        }

        private void LateUpdate()
        {
            var targetAngle = Vector2.SignedAngle(Vector2.up, m_controller.forward);
            var currentAngle = Vector2.SignedAngle(Vector2.up, transform.up);
            var angle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * m_rotationSpeed);
            angle = targetAngle;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        }
    }
}
