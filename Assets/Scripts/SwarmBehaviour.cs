using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBehaviour
{
    private Vector3 _velocity;
    private float _smoothTime;

    public AgentBehaviour(bool smoothCohesion, float smoothTime)
    {
        if (smoothCohesion)
            this._smoothTime = smoothTime;
        else
            this._smoothTime = 0.0f;
    }

    // inspired from https://github.com/boardtobits/flocking-algorithm
    public Vector3 CalculateMove(SwarmAgent agent,
        List<Transform> everythingInRange, float[] behaviourWeights, Swarm swarm)
    {
        LayerMask mask = LayerMask.GetMask("Swarm Particle");
        int agentsInRange = 0;

        if (behaviourWeights.Length != 3)
        {
            Debug.LogError("There should be 3 weights for each behaviour: cohesion, " +
                "alignment, and avoidance.");
            return Vector3.zero;
        }

        Vector3 finalMovement = Vector3.zero;

        Vector3 cohesion = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 avoidance = Vector3.zero;

        if (everythingInRange.Count == 0)
        {
            alignment = agent.transform.forward;
        }
        
        int avoidanceCounter = 0;
        foreach (Transform t in everythingInRange)
        {
            // check if the transform corresponds to a swarm agent 
            if (mask == (mask | 1 << t.gameObject.layer))
            {
                cohesion += t.position;
                alignment += t.transform.forward;

                agentsInRange++;
            }

            Vector3 difference = t.position - agent.transform.position;

            if (difference.sqrMagnitude < swarm.AvoidanceRadiusSquared)
            {
                avoidanceCounter++;
                avoidance += agent.transform.position - t.position;
            }
        }

        // avoidance
        if (avoidanceCounter > 0)
            avoidance /= avoidanceCounter;

        // alignment
        alignment /= agentsInRange;

        // cohesion  
        cohesion /= agentsInRange;
        cohesion -= agent.transform.position;
        cohesion = Vector3.Lerp(agent.transform.forward, cohesion, _smoothTime);

        // put individual movements in vector
        Vector3[] behaviourMovements = new[] { cohesion, alignment, avoidance };

        for (int i = 0; i < behaviourMovements.Length; i++)
        {
            Vector3 individualMovement = behaviourMovements[i];
            if (individualMovement != Vector3.zero)
            {
                if (individualMovement.sqrMagnitude >
                    (behaviourWeights[i] * behaviourWeights[i]))
                {
                    individualMovement.Normalize();
                    individualMovement *= behaviourWeights[i];
                }
            }
            finalMovement += individualMovement;
        }
        finalMovement.y = 0.0f;
        return finalMovement;
    }
}
