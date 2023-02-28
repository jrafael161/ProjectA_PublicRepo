using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStateInitializer : MonoBehaviour
{
    [Header("This enables or disables the game objecto so at the start of the \ngame is correctly initialized")]
    [SerializeField]
    bool enable;
    private void Start()
    {
        //Debug.Log(gameObject + " " + enable);
        //this.gameObject.SetActive(enable);
    }

    public void setEnable(bool en)
    {
        enable = en;
    }

    public bool getEnable()
    {
        return enable;
    }
}
