using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityPrototype
{
    public class Path : MonoBehaviour
    {
        private PathPoint[] m_cachedPoints = null;
        private PathPoint[] m_points
        {
            get
            {
                if (m_cachedPoints == null)
                    m_cachedPoints = FindPathPoints();
                return m_cachedPoints;
            }
        }

        public int pointsCount => m_points.Length;

        private PathPoint[] FindPathPoints()
        {
            return GetComponentsInChildren<PathPoint>();
        }

        public Vector2 GetPoint(int index)
        {
            return m_points[index].transform.position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var points = FindPathPoints().Select((PathPoint point) => (Vector2)point.transform.position);
            GizmosHelper.DrawCurve(points);
        }
    }
}
