using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPrototype
{
    public class Sensor : MonoBehaviour
    {
        private List<Collider2D> m_touchingObjects = new List<Collider2D>();

        public IReadOnlyList<Collider2D> touchingObjects => m_touchingObjects;
        public Collider2D anyTouchingObject => m_touchingObjects.Count > 0 ? m_touchingObjects[0] : null;

        public event System.Action<Collider2D> onEnter;
        public event System.Action<Collider2D> onExit;

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnEnter(other);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            OnEnter(other.collider);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnExit(other);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            OnExit(other.collider);
        }

        private void OnEnter(Collider2D other)
        {
            m_touchingObjects.Add(other);
            onEnter?.Invoke(other);
        }

        private void OnExit(Collider2D other)
        {
            m_touchingObjects.Remove(other);
            onExit?.Invoke(other);
        }

        public bool Collides(Collider2D obj)
        {
            if (obj == null)
                return false;

            return m_touchingObjects.Contains(obj);
        }
    }
}
