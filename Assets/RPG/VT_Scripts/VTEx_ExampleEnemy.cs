using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VTEx_ExampleEnemy : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform target;

    private void Update()
    {
        agent.destination = target.position;    
    }
}
