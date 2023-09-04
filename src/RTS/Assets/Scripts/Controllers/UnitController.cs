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
        if (UnitDefinition.CanMove == false)
        {
            return;
        }
        _agent.SetDestination(target);
    }

    public void Command_Stop()
    {
        if (UnitDefinition.CanMove == false)
        {
            return;
        }
        _agent.isStopped = true;
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
}
