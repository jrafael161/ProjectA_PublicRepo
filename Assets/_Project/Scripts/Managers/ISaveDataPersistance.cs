using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveDataPersistance
{
    string LoadData(SaveData saveData, string itemID = null)
    {
        return itemID="";
    }

    //void LoadData(SaveData saveData, ref string itemID)
    //{

    //}

    void StoreData(ref SaveData saveData)
    {

    }
}
