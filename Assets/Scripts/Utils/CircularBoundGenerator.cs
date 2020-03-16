using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

namespace UnityPrototype
{
    public class CircularBoundGenerator : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D m_prefab = null;
        [SerializeField] private float m_radius = 5.0f;

        private void Awake()
        {
            var scale = m_prefab.transform.lossyScale;
            var r = m_prefab.radius * Mathf.Max(scale.x, scale.y);
            var deltaAngleRad = Mathf.Acos(1.0f - 2.0f * r * r / m_radius / m_radius);

            var obstaclesCount = Mathf.CeilToInt(2.0f * Mathf.PI / deltaAngleRad);

            var prevDir = Vector2.up;
            var deltaAngle = 360.0f / obstaclesCount;

            for (var i = 0; i < obstaclesCount; i++)
            {
                var dir = prevDir.Rotate(deltaAngle);
                var pos = dir * m_radius;

                Instantiate(m_prefab, pos, Quaternion.identity, transform);

                prevDir = dir;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            GizmosHelper.DrawCircle(transform.position, m_radius);
        }
    }
}
