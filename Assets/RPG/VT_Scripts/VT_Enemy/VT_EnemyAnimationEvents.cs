using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_EnemyAnimationEvents : MonoBehaviour
{
    private VT_Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<VT_Enemy>();
    }

    public void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

    public void StartManualMovement()
    {
        enemy.ActiveManualMovement(true);
    }
    public void StopManualMovement()
    {
        enemy.ActiveManualMovement(false);
    }

}
