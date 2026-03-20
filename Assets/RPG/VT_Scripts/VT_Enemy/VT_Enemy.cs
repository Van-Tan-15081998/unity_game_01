using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VT_Enemy : MonoBehaviour
{
    [Header("Idle data a")]
    public float idleTime;

    [Header("Move data")]
    public float moveSpeed;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public NavMeshAgent agent {  get; private set; }

    public VT_EnemyStateMachine stateMachine {  get; private set; }

    protected virtual void Awake() 
    {
        stateMachine = new VT_EnemyStateMachine();

        agent = GetComponent<NavMeshAgent>();   
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
}
