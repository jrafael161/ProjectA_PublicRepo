using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class UniqueIdentifierAttribute : Attribute {}
public class PrefabGUID : MonoBehaviour
{
    [SerializeField]
    public string uniqueObjectId;
    [ContextMenu("Generate guid for id")]
    private void Awake()
    {
        if (uniqueObjectId == null || uniqueObjectId == "")
        {
            if (GetComponent<PickableItem>()!=null)
            {
                uniqueObjectId = GetComponent<PickableItem>().item.uniqueItemID;
            }
            else
            {
                Debug.LogWarning("GUID of " + gameObject + "changed");
                uniqueObjectId = System.Guid.NewGuid().ToString();
            }
        }
    }
    private void GenerateGuid()
    {
        uniqueObjectId = System.Guid.NewGuid().ToString();
    }
}

