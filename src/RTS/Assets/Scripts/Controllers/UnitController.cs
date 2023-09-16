using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    public FactionDefinition FactionDefinition;
    public UnitDefinition UnitDefinition;
    public UnitManager UnitManager;

    public GameObject MoveTargetIndicatorPrefab;

    private NavMeshAgent _agent;
    private GameObject _moveTargetIndicator;

    public void Command_MoveTo (Vector3 target)
    {
        if (UnitDefinition.CanMove == false)
        {
            return;
        }
        _agent.SetDestination(target);
        SetMoveTargetIndicator(target);
    }


    public void Command_Stop()
    {
        if (UnitDefinition.CanMove == false)
        {
            return;
        }
        _agent.isStopped = true;
        HideMoveTargetIndicator();
    }

    private void SetMoveTargetIndicator(Vector3 target)
    {
        if (_moveTargetIndicator == null)
        {
            return;
        }
        _moveTargetIndicator.transform.position = target;
        _moveTargetIndicator.SetActive(true);
    }

    private void HideMoveTargetIndicator()
    {
        if (_moveTargetIndicator == null)
        {
            return;
        }
        _moveTargetIndicator.SetActive(false);
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (MoveTargetIndicatorPrefab == null)
        {
            return;
        }
        _moveTargetIndicator = Instantiate(MoveTargetIndicatorPrefab, Vector3.zero, Quaternion.identity, null); // TODO: Where should this be parented?
        HideMoveTargetIndicator();
    }

    private void Update()
    {
        if (UnitDefinition.CanMove == false)
        {
            return;
        }

        if (HasReachedDestination())
        {
            HideMoveTargetIndicator();
        }
    }

    private bool HasReachedDestination()
    {
        return ((_agent.pathPending == true) // We are waiting for path finding, so we are not there yet
                || (
                        (_agent.remainingDistance <= _agent.stoppingDistance) // We are close enough
                     && (_agent.hasPath == false || _agent.velocity.sqrMagnitude == 0f) // We have no path and are no longer moving
                   )
                );
    }
}
