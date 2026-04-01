using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VT_GameManager : MonoBehaviour
{
    public static VT_GameManager instance;

    [Header("Settings")]
    public bool friendlyFire;

    private void Awake()
    {
        instance = this;
    }
}
