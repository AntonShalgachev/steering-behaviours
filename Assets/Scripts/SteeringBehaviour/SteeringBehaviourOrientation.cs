using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace UnityPrototype
{
    public class SteeringBehaviourOrientation : MonoBehaviour
    {
        [SerializeField] private bool m_smooth = true;
        [SerializeField, ShowIf("m_smooth")] private float m_rotationSpeed = 90.0f;

        private SteeringBehaviourController m_controller = null;
        private bool m_hardSyncRotation = true;

        private void Awake()
        {
            m_controller = GetComponentInParent<SteeringBehaviourController>();
        }

        private void LateUpdate()
        {
            var targetAngle = Vector2.SignedAngle(Vector2.up, m_controller.forward);
            var angle = targetAngle;
            if (m_smooth && !m_hardSyncRotation)
            {
                var currentAngle = Vector2.SignedAngle(Vector2.up, transform.up);
                angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, Time.deltaTime * m_rotationSpeed);
            }
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        }
    }
}
