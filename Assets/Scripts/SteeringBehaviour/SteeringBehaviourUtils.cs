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

        public static Vector2 Arrival(Vector2 targetPosition, float targetRadius, ISteeringBehaviour agent)
        {
            var dPos = targetPosition - agent.position;
            var distanceToTarget = dPos.magnitude;
            var speedMultiplier = Mathf.Clamp01(Mathf.InverseLerp(0.0f, targetRadius, distanceToTarget));
            var targetVelocity = dPos.normalized * agent.maxSpeed * speedMultiplier;
            return agent.CalculateForceForVelocity(targetVelocity);
        }
    }
}
