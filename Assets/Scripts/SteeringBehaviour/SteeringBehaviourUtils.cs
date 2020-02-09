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
    }
}
