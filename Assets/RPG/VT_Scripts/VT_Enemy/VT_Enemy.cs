using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VT_Enemy : MonoBehaviour
{
    [Header("Idle data")]
    public float idleTime;

    [Header("Move data")]
    public float moveSpeed;

    public Transform destination;

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

    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }
}
