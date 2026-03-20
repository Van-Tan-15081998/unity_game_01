using UnityEngine;
using UnityEngine.AI;

public class VT_Enemy : MonoBehaviour
{
    public float turnSpeed;
    public float aggresionRange;

    [Header("Attack data")]
    public float attackRange;
    public float attackMoveSpeed;

    [Header("Idle data")]
    public float idleTime;

    [Header("Move data")]
    public float moveSpeed;
    public float chaseSpeed;
    private bool manualMovement;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public Transform player { get; private set; }

    public Animator anim { get; private set; }

    public NavMeshAgent agent { get; private set; }

    public VT_EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new VT_EnemyStateMachine();

        agent = GetComponent<NavMeshAgent>();

        anim = GetComponentInChildren<Animator>();

        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    private void InitializePatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = null;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;

        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
        {
            currentPatrolIndex = 0;
        }

        return destination;
    }

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(
            currentEulerAngels.y,
            targetRotation.eulerAngles.y,
            turnSpeed * Time.deltaTime
            );

        return Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);  
    }

    public bool PlayerInAggresionRange()
    {
        return Vector3.Distance(transform.position, player.position) < aggresionRange;
    }

    public bool PlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackRange;
    }

    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public void ActiveManualMovement(bool manualMovement)
    {
        this.manualMovement = manualMovement;
    }

    public bool ManualMovementActive()
    {
        return manualMovement;
    }
}
