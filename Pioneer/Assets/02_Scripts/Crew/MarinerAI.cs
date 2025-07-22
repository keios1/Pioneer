using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MarinerAI : MonoBehaviour
{
    [Header("Setup")]
    public int marinerId;
    public LayerMask targetLayer;
    public float detectionRange = 3f;
    public float attackInterval = 0.5f;
    public float speed = 1f;
    public float moveDuration = 2f;
    public float idleDuration = 4f;

    [HideInInspector] public MarinerStateMachine StateMachine;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Transform target;
    private FOVController fovController;

    private float attackCooldown = 0f;
    private Vector3 moveDirection;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        fovController = GetComponent<FOVController>();
        StateMachine = new MarinerStateMachine(this);
    }

    private void Start()
    {
        StateMachine.ChangeState(new MarinerRepairState(this));
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.RegisterMariner(this);

        if (GameManager.Instance.IsDaytime)
        {
            if (!(StateMachine.GetCurrentState() is MarinerRepairState || StateMachine.GetCurrentState() is MarinerSecondPriorityState))
            {
                StateMachine.ChangeState(new MarinerRepairState(this));
            }
        }
        else
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f && DetectTarget() && IsTargetInFOV())
            {
                StateMachine.ChangeState(new MarinerAttackState(this));
                attackCooldown = attackInterval;
            }

            if (!(StateMachine.GetCurrentState() is MarinerWanderState || StateMachine.GetCurrentState() is MarinerIdleState || StateMachine.GetCurrentState() is MarinerAttackState))
            {
                StateMachine.ChangeState(new MarinerWanderState(this));
            }
        }

        StateMachine.Update();
    }

    public bool DetectTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);
        target = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                target = hit.transform;
            }
        }

        return target != null;
    }

    public bool IsTargetInFOV() => target != null && fovController.visibleTargets.Contains(target);

    public IEnumerator FindAndRepairCoroutine()
    {
        List<DefenseObject> repairTargets = GameManager.Instance.GetNeedsRepair();
        foreach (var obj in repairTargets)
        {
            if (GameManager.Instance.TryOccupyRepairObject(obj, marinerId))
            {
                yield return MoveToThenReset(obj.transform.position);
                yield return StartCoroutine(RepairCoroutine(obj));
                GameManager.Instance.ReleaseRepairObject(obj);
                yield break;
            }
        }
    }

    private IEnumerator RepairCoroutine(DefenseObject obj)
    {
        yield return new WaitForSeconds(10f);
        obj.Repair(30);
        Debug.Log($"{marinerId} repaired {obj.name}");
    }

    public IEnumerator MoveToThenReset(Vector3 destination)
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(destination);
            while (!IsArrived()) yield return null;
            agent.ResetPath();
        }
    }

    public bool IsArrived() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;

    public void SetRandomDirection()
    {
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    public void Wander()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public IEnumerator AttackSequence()
    {
        if (target == null) yield break;

        Vector3 attackPos = target.position - (target.position - transform.position).normalized;
        while (Vector3.Distance(transform.position, attackPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackPos, speed * Time.deltaTime);
            yield return null;
        }

        Collider[] hits = Physics.OverlapBox(transform.position + transform.forward, Vector3.one * 0.5f, Quaternion.identity, targetLayer);
        foreach (var hit in hits)
        {
            var status = hit.GetComponent<MarinerStatus>();
            if (status != null)
            {
                status.TakeDamage(6);
                Debug.Log($"{hit.name} attacked for 6 damage.");
            }
        }

        yield return new WaitForSeconds(1f);
    }

    public IEnumerator StartSecondPriorityAction()
    {
        Debug.Log("Second Priority Action Start - Farming");
        yield return new WaitForSeconds(10f);
        Debug.Log("Second Priority Action Complete - Farming Done");
    }
}
