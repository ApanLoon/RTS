using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    public FactionDefinition FactionDefinition;
    public UnitDefinition UnitDefinition;
    public UnitManager UnitManager;

    private NavMeshAgent _agent;

    public void Command_MoveTo (Vector3 target)
    {
        _agent.SetDestination(target);
    }

    public void Command_Stop()
    {
        _agent.isStopped = true;
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
}
