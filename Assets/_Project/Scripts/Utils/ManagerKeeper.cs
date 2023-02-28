using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerKeeper : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
