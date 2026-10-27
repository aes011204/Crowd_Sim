using UnityEngine;
using UnityEngine.AI;

//이 Component를 사용하려면 NavMeshAgent가 반드시 필요하다고 명시
//Unity에서 Component끼리 의존성을 다루는 방식 중 하나
[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovement : MonoBehaviour
{

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    //void Start()
    //{
    //    agent.SetDestination(destination.position); // 이거 하면 NavMeshAgent 자체가 계속 목적지를 향해 이동
    //}

    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public bool HasArrived()
    {
        if (agent == null)
            return false;

        if(!agent.hasPath||agent.pathStatus != NavMeshPathStatus.PathComplete) return false;

        return agent.remainingDistance <= agent.stoppingDistance + 0.05f;
    }
    //void Update()
    //{
    // 100~1000개 만들거라 호출할 대상이 늘어날 수 있어.빈 함수니까 내부 연산은 없지만 호출 자체의 관리 비용이 있음
    //}
}
