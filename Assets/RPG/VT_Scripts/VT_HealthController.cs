using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_HealthController : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void ReduceHealth()
    {
        currentHealth--;
    }

    public virtual void IncreaseHealth()
    {
        currentHealth++;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public bool ShouldDie()
    {
        return currentHealth <= 0;
    }
}
