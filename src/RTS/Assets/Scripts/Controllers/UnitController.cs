using System.Globalization;
using RTS.Definitions;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    public FactionDefinition FactionDefinition;
    public UnitDefinition UnitDefinition;
    public UnitManager UnitManager;

    public GameObject MoveTargetIndicatorPrefab;

    public float DeflectorStrength { get; protected set; } = 1f;
    public float Health { get; protected set; } = 1f;

    /// <summary>
    /// TODO: Make the name something unique per unit
    /// </summary>
    public string Name => gameObject.name;

    private Rigidbody _rigidBody;
    private NavMeshAgent _agent;
    private Animator _animator;
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
        _rigidBody = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

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

        if (_animator != null && _rigidBody != null)
        {
            var vFwd = (_agent.velocity.magnitude / _agent.speed);
            if (vFwd < 0.1)
            {
                vFwd = 0f;
            }
            _animator.SetFloat("VFwd", vFwd);
            // TODO: How to control the turn animation?
            //var vRight = _rigidBody.angularVelocity.z;
            //_animator.SetFloat("VRight", vRight);

            DebugInfoPanel.Log($"{Name} VFwd", vFwd.ToString(CultureInfo.InvariantCulture));
            //DebugInfoPanel.Log($"{Name} VRight", vRight.ToString(CultureInfo.InvariantCulture));

            //var x = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            //DebugInfoPanel.Log($"{Name} State", x.ToString() );
        }
        else
        {
            DebugInfoPanel.Remove($"{Name} VFwd");
            //DebugInfoPanel.Remove($"{Name} VRight");
            //DebugInfoPanel.Remove($"{Name} State");
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
