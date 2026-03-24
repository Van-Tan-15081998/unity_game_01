using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class VT_Enemy : MonoBehaviour
{
    //[Header("Attack data")]
    //public float attackRange;
    //public float attackMoveSpeed;

    [SerializeField] protected int healthPoints = 25;

    [Header("Idle data")]
    public float idleTime;
    public float aggresionRange;

    [Header("Move data")]
    public float moveSpeed;
    public float turnSpeed;
    public float chaseSpeed;
    private bool manualMovement;
    private bool manualRotation;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;

    public bool inBattleMode { get; private set; }

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



    // Update is called once per frame
    protected virtual void Update()
    {
    }

    protected bool ShouldEnterBattleMode()
    {
        bool inAggresionRange = Vector3.Distance(transform.position, player.position) < aggresionRange;

        if (inAggresionRange && !inBattleMode)
        {
            EnterBattleMode();
            return true;
        }

        return false;   
    }

    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }

    public virtual void GetHit()
    {
        EnterBattleMode();
        healthPoints--;
    }

    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(DeathImpactCourutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCourutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public void FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(
            currentEulerAngels.y,
            targetRotation.eulerAngles.y,
            turnSpeed * Time.deltaTime
            );

        transform.rotation = Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);

    }

    #region Animation Events
    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }

    public void ActiveManualMovement(bool manualMovement)
    {
        this.manualMovement = manualMovement;
    }

    public void ActiveManualRotation(bool manualRotation)
    {
        this.manualRotation = manualRotation;
    }

    public bool ManualMovementActive()
    {
        return manualMovement;
    }

    public bool ManualRotationActive()
    {
        return manualRotation;
    }
    #endregion

    #region Patrol logic
    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];

        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
        {
            currentPatrolIndex = 0;
        }

        return destination;
    }

    private void InitializePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }
    #endregion
}
