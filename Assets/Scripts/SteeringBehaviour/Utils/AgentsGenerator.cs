using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace UnityPrototype
{
    public class AgentsGenerator : MonoBehaviour
    {
        [SerializeField] private SteeringBehaviourController m_agentPrefab = null;
        [SerializeField] private int m_amount = 5;
        [SerializeField, MinMaxSlider(-180.0f, 180.0f)] private Vector2 m_angleRange = new Vector2(-45.0f, 45.0f);
        [SerializeField] private float m_radius = 0.5f;

        private void Awake()
        {
            for (var i = 0; i < m_amount; i++)
            {
                var offset = Random.insideUnitCircle * m_radius;
                var angle = Random.Range(m_angleRange.x, m_angleRange.y);
                var agent = Instantiate(m_agentPrefab, transform.position + (Vector3)offset, Quaternion.identity, transform);
                agent.SetDirection(angle);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            GizmosHelper.DrawCircle(transform.position, m_radius);
        }
    }
}
