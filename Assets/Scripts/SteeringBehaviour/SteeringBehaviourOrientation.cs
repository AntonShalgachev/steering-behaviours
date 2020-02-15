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

        private SteeringBehaviourController m_cachedController = null;
        private SteeringBehaviourController m_controller
        {
            get
            {
                if (m_cachedController == null)
                    m_cachedController = GetComponentInParent<SteeringBehaviourController>();
                return m_cachedController;
            }
        }
        private bool m_hardSyncRotation = true;

        private void LateUpdate()
        {
            UpdateOrientation(m_smooth && !m_hardSyncRotation);
            m_hardSyncRotation = false;
        }

        public void UpdateOrientation(bool smooth)
        {
            var targetAngle = m_controller.angle;
            var angle = targetAngle;
            if (smooth)
            {
                var currentAngle = Vector2.SignedAngle(Vector2.up, transform.up);
                angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, Time.deltaTime * m_rotationSpeed);
            }

            transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        }
    }
}
