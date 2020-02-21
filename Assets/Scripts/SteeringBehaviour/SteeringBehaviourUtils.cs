using UnityEngine;

namespace UnityPrototype
{
    public static class SteeringBehaviourUtils
    {
        public static Vector2 Flee(Vector2 threatPosition, ISteeringBehaviour agent)
        {
            return agent.CalculateForceForDirection((threatPosition - agent.position).normalized);
        }

        public static Vector2 Seek(Vector2 targetPosition, float targetRadius, float epsilonRadius, ISteeringBehaviour agent)
        {
            var dPos = targetPosition - agent.position;

            var distanceToTarget = dPos.magnitude;
            var speedMultiplier = 1.0f;
            if (targetRadius > 0.0f)
                speedMultiplier = Mathf.Clamp01(Mathf.InverseLerp(0.0f, targetRadius, distanceToTarget));
            if (distanceToTarget < epsilonRadius)
                speedMultiplier = 0.0f;

            return agent.CalculateForceForDirection(dPos.normalized, speedMultiplier);
        }
    }
}
