using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public bool HasArrived()
    {
        if (agent == null)
            return false;

        if (!agent.hasPath || agent.pathStatus != NavMeshPathStatus.PathComplete) return false;

        return agent.remainingDistance <= agent.stoppingDistance + 0.05f;
    }
}
