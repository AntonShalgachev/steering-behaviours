using UnityEngine;

namespace UnityPrototype
{
    public static class SteeringBehaviourUtils
    {
        public static Vector2 Seek(Vector2 targetPosition, ISteeringBehaviour agent)
        {
            var targetVelocity = (targetPosition - agent.position).normalized * agent.maxSpeed;
            return agent.CalculateForceForVelocity(targetVelocity);
        }

        public static Vector2 Flee(Vector2 threatPosition, ISteeringBehaviour agent)
        {
            var targetVelocity = (agent.position - threatPosition).normalized * agent.maxSpeed;
            return agent.CalculateForceForVelocity(targetVelocity);
        }

        public static Vector2 Arrival(Vector2 targetPosition, float targetRadius, float epsilonRadius, ISteeringBehaviour agent)
        {
            var dPos = targetPosition - agent.position;
            var distanceToTarget = dPos.magnitude;
            var speedMultiplier = 1.0f;
            if (targetRadius > 0.0f)
                speedMultiplier = Mathf.Clamp01(Mathf.InverseLerp(0.0f, targetRadius, distanceToTarget));
            if (distanceToTarget < epsilonRadius)
                speedMultiplier = 0.0f;
            var targetVelocity = dPos.normalized * agent.maxSpeed * speedMultiplier;
            return agent.CalculateForceForVelocity(targetVelocity);
        }
    }
}
