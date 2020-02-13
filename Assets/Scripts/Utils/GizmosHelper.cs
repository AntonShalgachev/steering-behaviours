using UnityEngine;
using Gamelogic.Extensions;
using System.Collections.Generic;

namespace UnityPrototype
{
    public static class GizmosHelper
    {
        public static void DrawPoint(Vector2 pos, float scale = 1.0f)
        {
            Gizmos.DrawLine(pos + new Vector2(-0.5f * scale, -0.5f * scale), pos + new Vector2(+0.5f * scale, +0.5f * scale));
            Gizmos.DrawLine(pos + new Vector2(+0.5f * scale, -0.5f * scale), pos + new Vector2(-0.5f * scale, +0.5f * scale));
        }

        public static void DrawVector(Vector2 pos, Vector2 dir, float scale = 1.0f)
        {
            Gizmos.DrawLine(pos, pos + dir * scale);
        }

        public static void DrawCircle(Vector2 origin, float radius, int segments = 16)
        {
            DrawEllipse(origin, Vector2.one * radius, segments);
        }

        public static void DrawEllipse(Vector2 origin, Vector2 axesLength, int segments = 16)
        {
            segments = Mathf.Max(segments, 2);

            var prevDir = Vector2.up;
            var prevPoint = prevDir * axesLength;
            var deltaAngle = 360.0f / segments;

            for (var i = 0; i < segments; i++)
            {
                var dir = prevDir.Rotate(deltaAngle);
                var pos = dir * axesLength;

                Gizmos.DrawLine(origin + prevPoint, origin + pos);

                prevPoint = pos;
                prevDir = dir;
            }
        }

        public static void DrawCurve(IEnumerable<Vector2> points, float pointSize = -1.0f, bool wireframePoint = false)
        {
            Vector2? prevPoint = null;

            foreach (var point in points)
            {
                if (prevPoint.HasValue)
                    Gizmos.DrawLine(prevPoint.Value, point);

                if (pointSize > 0.0f)
                {
                    if (wireframePoint)
                        GizmosHelper.DrawCircle(point, pointSize);
                    else
                        Gizmos.DrawSphere(point, pointSize);
                }

                prevPoint = point;
            }
        }

        public static float ScreenSizeToWorldSize(float size)
        {
            var camera = Camera.current;
            var center = camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
            var offset = camera.ScreenToWorldPoint(new Vector3(size, 0, 0));

            return (center - offset).magnitude;
        }

        public static void DrawOutline(Vector2 position, string text)
        {
            DrawOutline(position, text, Color.white, Color.black);
        }

        public static void DrawOutline(Vector2 position, string text, Color color, Color outlineColor)
        {
            var style = new GUIStyle();
            var outlineStyle = new GUIStyle();
            style.normal.textColor = color;
            outlineStyle.normal.textColor = outlineColor;

            DrawOutline(position, text, style, outlineStyle);
        }

        public static void DrawOutline(Vector2 position, string text, GUIStyle style, GUIStyle outlineStyle)
        {
#if UNITY_EDITOR
            var offset = ScreenSizeToWorldSize(1.0f);
            UnityEditor.Handles.Label(position + new Vector2(-offset, 0), text, outlineStyle);
            UnityEditor.Handles.Label(position + new Vector2(+offset, 0), text, outlineStyle);
            UnityEditor.Handles.Label(position + new Vector2(0, -offset), text, outlineStyle);
            UnityEditor.Handles.Label(position + new Vector2(0, +offset), text, outlineStyle);

            UnityEditor.Handles.Label(position, text, style);
#endif
        }
    }
}
