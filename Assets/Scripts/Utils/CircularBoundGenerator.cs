using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

namespace UnityPrototype
{
    public class CircularBoundGenerator : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D m_prefab = null;
        [SerializeField] private Vector2 m_radii = new Vector2(7.0f, 5.0f);

        private void Awake()
        {
            var scale = m_prefab.transform.lossyScale;
            var r = m_prefab.radius * Mathf.Max(scale.x, scale.y);
            var meanRadius = 0.5f * (m_radii.x + m_radii.y);
            var deltaAngleRad = Mathf.Acos(1.0f - 2.0f * r * r / meanRadius / meanRadius);

            var obstaclesCount = Mathf.CeilToInt(2.0f * Mathf.PI / deltaAngleRad);

            var prevDir = Vector2.up;
            var deltaAngle = 360.0f / obstaclesCount;

            for (var i = 0; i < obstaclesCount; i++)
            {
                var dir = prevDir.Rotate(deltaAngle);
                var pos = dir * m_radii;

                Instantiate(m_prefab, pos, Quaternion.identity, transform);

                prevDir = dir;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            GizmosHelper.DrawEllipse(transform.position, m_radii);
        }
    }
}
