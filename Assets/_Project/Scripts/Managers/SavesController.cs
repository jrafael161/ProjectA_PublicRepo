using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class SavesController : MonoBehaviour
{
    public static SavesController _instance;

    public List<ISaveDataPersistance> dataPersistanceObjects;

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
        {
            _instance = this;
        }
    }

    public void SaveGame()
    {
        string json;
        //FLOW.FlowSnapshot flowSnapshot = new FLOW.FlowSnapshot();

        int activeSave = -1;
        activeSave = GetActiveSave();
        PlayerPrefs.SetInt("LastSavedGame", activeSave);
        PlayerPrefs.SetString("Save" + activeSave + "_LastTimeSaved", System.DateTime.Now.ToString());

        SaveData save = GameController._instance.GetPlayerData();
        save.saveSlot = activeSave;
        save.lastTimeSaved = System.DateTime.Now.ToString();
        save.isUsed = true;

        dataPersistanceObjects = FindAllDataPersistanceObjects();
        foreach (ISaveDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.StoreData(ref save);
        }

        json = JsonUtility.ToJson(save,true);

        string savePath = PlayerPrefs.GetString("Save" + activeSave + "_path");

        #if UNITY_EDITOR
            File.WriteAllText(savePath, json);
            //PlayerPrefs.SetString("Save" + activeSave + "_path", savePath);
            //File.WriteAllText(Application.dataPath + "/_Project/Saves/Save" + activeSave + "/save" + activeSave + ".json", json);
            //PlayerPrefs.SetString("Save" + activeSave + "_path", Application.dataPath + "/_Project/Saves/Save" + activeSave + "/save" + activeSave + ".json");
        #elif UNITY_STANDALONE_WIN
            File.WriteAllText(savePath, json);
            //File.WriteAllText(Application.dataPath + "/Saves/Save" + activeSave + "/save" + activeSave + ".json", json);
            //PlayerPrefs.SetString("Save" + activeSave + "_path", Application.dataPath + "/Saves/Save" + activeSave + "/save" + activeSave + ".json");
        #endif

        //SaveDiggerStateToSaveFile(activeSave);

        //SaveFlowStateToSaveFile(activeSave);

        Debug.Log("Saved player data");
    }

    public int GetActiveSave()
    {
        int saveSlot = PlayerPrefs.GetInt("AccessedSave");
        return saveSlot;
    }

    void SetActiveSave(int saveSlot)
    {
        PlayerPrefs.SetInt("AccessedSave", saveSlot);
    }

    public void CreateNewGame()
    {
        if (!NewGameController._instance.DataVerification())//Not all the neccesary info for a new character is met
            return;

        NewGameController._instance.DumpPlayerData();
        SaveData saveData = NewGameController._instance.GetPlayerCharacter();

        int numSaves = -1;        

        numSaves = PlayerPrefs.GetInt("NumberOfSaves");

        for (int i = 0; i <= numSaves; i++)
        {
            if (!PlayerPrefs.HasKey("Save" + i + "_path"))
            {
                CreateSaveFile(i,saveData);
                GameController._instance.StartGame(saveData, true);
                break;
            }
            string path = PlayerPrefs.GetString("Save" + i + "_path");
            if (path == "")
            {
                CreateSaveFile(i,saveData);
                GameController._instance.StartGame(saveData, true);
                break;
            }
        }
    }

    void CreateSaveFile(int saveSlot, SaveData saveData)
    {
        SetActiveSave(saveSlot);
        string json;
        saveData.saveSlot = saveSlot;
        saveData.isUsed = true;
        json = JsonUtility.ToJson(saveData,true);

        #if UNITY_EDITOR
            if (!File.Exists(Application.dataPath + "/_Project/Saves/Save" + saveSlot + "/save" + saveSlot + ".json"))
            {
                Directory.CreateDirectory(Application.dataPath + "/_Project/Saves/Save" + saveSlot);
            }
            File.WriteAllText(Application.dataPath + "/_Project/Saves/Save" + saveSlot + "/save" + saveSlot + ".json", json);
        #elif UNITY_STANDALONE_WIN
            if (!File.Exists(Application.dataPath + "/Saves/Save" + saveSlot + "/save" + saveSlot + ".json"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Saves/Save" + saveSlot);
            }
            File.WriteAllText(Application.dataPath + "/Saves/Save" + saveSlot + "/save" + saveSlot + ".json", json);
        #endif

        int numSaves = PlayerPrefs.GetInt("NumberOfSaves");
        if (saveSlot+1 > numSaves)
        {
            PlayerPrefs.SetInt("NumberOfSaves", saveSlot + 1);
        }        
        PlayerPrefs.SetInt("AccessedSave", saveSlot);
        PlayerPrefs.SetInt("LastSavedGame", saveSlot);
        PlayerPrefs.SetString("Save" + saveSlot + "_LastTimeSaved", System.DateTime.Now.ToString());

        #if UNITY_EDITOR
            PlayerPrefs.SetString("Save" + saveSlot + "_path", Application.dataPath + "/_Project/Saves/Save" + saveSlot + "/save" + saveSlot + ".json");
        #elif UNITY_STANDALONE_WIN
            PlayerPrefs.SetString("Save" + saveSlot + "_path", Application.dataPath + "/Saves/Save" + saveSlot + "/save" + saveSlot + ".json");
        #endif
    }

    public void LoadLastGame()
    {
        //int saveSlot = PlayerPrefs.GetInt("LastSavedGame"); //This if you always want to load the last SAVED game
        int saveSlot = PlayerPrefs.GetInt("AccessedSave"); //This if you want to load the last accesed game, even if the player didnt save it
        if (!CheckSaveStatus(saveSlot))
            Debug.LogError("Error there is no saved game");
        else
        {
            //SetActiveSave(saveSlot);
            SaveData saveData = new SaveData();
            string saveSlotPath = PlayerPrefs.GetString("Save" + saveSlot + "_path");
            string saveJsonData = File.ReadAllText(saveSlotPath);
            saveData = JsonUtility.FromJson<SaveData>(saveJsonData);
            GameController._instance.StartGame(saveData);
        }
    }

    public void LoadSpecificGame(int saveSlot)
    {
        if (!CheckSaveStatus(saveSlot))
            Debug.LogError("Error there is no saved game");
        else
        {
            SaveData saveData;
            SetActiveSave(saveSlot);
            string saveSlotPath = PlayerPrefs.GetString("Save" + saveSlot + "_path");
            string saveJsonData = File.ReadAllText(saveSlotPath);
            saveData = JsonUtility.FromJson<SaveData>(saveJsonData);
            GameController._instance.StartGame(saveData);
        }
    }

    public List<SaveData> GetAllSaves()
    {        
        SaveData saveData = new SaveData();
        List<SaveData> savesInfo = new List<SaveData>();
        int numSaves = PlayerPrefs.GetInt("NumberOfSaves");

        for (int i = 0; i < numSaves; i++)
        {
            string saveSlotPath = PlayerPrefs.GetString("Save" + i + "_path");
            if (saveSlotPath!="")
            {
                string saveJsonData = File.ReadAllText(saveSlotPath);
                saveData = JsonUtility.FromJson<SaveData>(saveJsonData);
                savesInfo.Add(saveData);
            }
            else
            {
                saveData = new SaveData();
                saveData.saveSlot = i;
                savesInfo.Add(saveData);
            }
        }
        return (savesInfo);
    }

    public void DeleteSave(int saveSlot)
    {
        string json;
        SaveData saveData = new SaveData();
        saveData.saveSlot = saveSlot;
        saveData.isUsed = false;
        json = JsonUtility.ToJson(saveData,true);

        #if UNITY_EDITOR
            File.WriteAllText(Application.dataPath + "/_Project/Saves/Save" + saveSlot + "/save" + saveSlot + ".json", json);
        #elif UNITY_STANDALONE_WIN
            File.WriteAllText(Application.dataPath + "/Saves/Save" + saveSlot + "/save" + saveSlot + ".json", json);
        #endif

        PlayerPrefs.SetString("Save" + saveSlot + "_path","");
    }

    bool CheckSaveStatus(int saveSlot)
    {
        string saveSlotPath = PlayerPrefs.GetString("Save" + saveSlot + "_path");
        if (saveSlotPath == "")
            return false;
        else
            return true;
    }

    public int CheckLastSave()
    {
        if (PlayerPrefs.HasKey("LastSavedGame"))
        {
            int saveSlot = PlayerPrefs.GetInt("LastSavedGame");
            if (saveSlot >= 0)
                if(CheckSaveStatus(saveSlot))
                    return saveSlot;
        }
        return -1;
    }

    //public void SaveFlowStateTemp()
    //{
    //    FLOW.FlowSnapshot flowSnapshot = FindObjectOfType<FLOW.FlowSnapshot>();
    //    flowSnapshot.SaveToTemp();
    //}

    //public void LoadFlowStateTemp()
    //{
    //    FLOW.FlowSnapshot flowSnapshot = FindObjectOfType<FLOW.FlowSnapshot>();
    //    flowSnapshot.LoadFromTemp();
    //}

    //public void SaveFlowStateToSaveFile(int saveSlot)
    //{
    //    FLOW.FlowSnapshot flowSnapshot = FindObjectOfType<FLOW.FlowSnapshot>();
    //    #if UNITY_EDITOR
    //        flowSnapshot.SaveToFile(Application.dataPath + "/_Project/Saves/Save" + saveSlot);
    //    #elif UNITY_STANDALONE_WIN
    //        flowSnapshot.SaveToFile(Application.dataPath + "/Saves/Save" + saveSlot);
    //    #endif
    //}

    //public void LoadFlowStateFromSaveFile(int saveSlot)
    //{
    //    FLOW.FlowSnapshot flowSnapshot = FindObjectOfType<FLOW.FlowSnapshot>();
    //    #if UNITY_EDITOR
    //        flowSnapshot.LoadFromFile(Application.dataPath + "/_Project/Saves/Save" + saveSlot);
    //    # elif UNITY_STANDALONE_WIN
    //        flowSnapshot.LoadFromFile(Application.dataPath + "/Saves/Save" + saveSlot);
    //    #endif
    //}

    //public void SaveDiggerStateToSaveFile(int saveSlot)
    //{
    //    DiggerMasterRuntime diggerMasterRuntime = FindObjectOfType<DiggerMasterRuntime>();

    //    #if UNITY_EDITOR
    //            //if (!Directory.Exists(Application.persistentDataPath + "/DiggerData/Saves/Save" + saveSlot))
    //            //    Directory.CreateDirectory(Application.persistentDataPath + "/DiggerData/Saves/Save" + saveSlot);
    //            //diggerMasterRuntime.SetPersistenceDataPathPrefix("/Saves/Save" + saveSlot);
    //    #elif UNITY_STANDALONE_WIN
    //            //if (!Directory.Exists(Application.persistentDataPath + "/DiggerData/Saves/Save" + saveSlot))
    //            //    Directory.CreateDirectory(Application.persistentDataPath + "/DiggerData/Saves/Save" + saveSlot);
    //            //diggerMasterRuntime.SetPersistenceDataPathPrefix("/Saves/Save" + saveSlot);
    //    #endif

    //    diggerMasterRuntime.PersistAll();
    //}

    public List<ISaveDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<ISaveDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveDataPersistance>();
        return new List<ISaveDataPersistance>(dataPersistanceObjects);
    }

    public void LoadAllDataPersistanceObjects(SaveData save)
    {
        List<string> LoadedObjects = new List<string>();
        string itemID = "";
        List<string> keys = new List<string>(save._serializedWorldItems.Keys);
        
        for (int i = 0; i < dataPersistanceObjects.Count; i++)
        {
            itemID = dataPersistanceObjects[i].LoadData(save, itemID);
            if (itemID != "")
            {
                keys.Remove(itemID);
            }
        }

        foreach (var uniqueItemID in keys)
        {
            string serializedItem;
            save._serializedWorldItems.TryGetValue(uniqueItemID, out serializedItem);            
            if (serializedItem != null)
            {
                //SerializableItem item = SerializationController._instance.DeserializeItemFromSaveFile1(serializedItem);
                LoadItem(serializedItem);
            }
        }
    }

    public void LoadItem(string serializedItem)
    {
        SerializableItem serializableItem = SerializationController._instance.DeserializeItemFromSaveFile1(serializedItem);
        if (serializableItem.inWorldPosition == Vector3.zero)//If item as placed in game by design (because only those objects in world have a position of 0,0,0)
            return;

        Item deserializedItem = SerializationController._instance.DeserializeItemFromSaveFile(serializedItem);

        GameObject environment = GameObject.Find("Environment");
        GameObject itemInWorld = Instantiate(deserializedItem.inWorldVersion, serializableItem.inWorldPosition, new Quaternion(0, 0, 0, 0), environment.transform);
        itemInWorld.GetComponent<PickableItem>().item = deserializedItem;//Pass the properties of the inventory item to the scriptable in the in world version after a new inWorldVersionPrefab has been instantiated, otherwise it would override the original prefab
        deserializedItem.inWorldVersion = itemInWorld;
        deserializedItem.inWorldVersion.GetComponent<PrefabGUID>().uniqueObjectId = deserializedItem.uniqueItemID;

        if (deserializedItem.itemType == ItemType.Ammo)
        {
            AmmoItem ammoItem = deserializedItem as AmmoItem;
            if (ammoItem.isStuck)
            {
                Rigidbody ammoRigidbody = deserializedItem.inWorldVersion.GetComponent<Rigidbody>();
                ammoRigidbody.useGravity = false;
                ammoRigidbody.isKinematic = true;
            }
        }
        if (deserializedItem.itemType == ItemType.Weapon || deserializedItem.itemType == ItemType.Trinket)
        {
            deserializedItem.inWorldVersion.GetComponentInChildren<GemsInObjectsManager>().SpawnGemsInItem(deserializedItem);
            WeaponItem weaponItem = deserializedItem as WeaponItem;
            if (weaponItem.isBeingDisplayed)
            {
                Collider[] colliders;
                colliders = Physics.OverlapSphere(deserializedItem.inWorldVersion.transform.position, 1);

                foreach (Collider collider in colliders)//Not the best thing to do buut, will work in the meantime until i implement again the displayer serialization
                {
                    if (collider.gameObject.GetComponent<WeaponDisplayerManager>() != null)
                    {
                        if (collider.gameObject.GetComponent<WeaponDisplayerManager>().WeaponPivot.GetComponentInChildren<PickableItem>() == null)
                        {
                            GameObject weaponPivot = collider.gameObject.GetComponent<WeaponDisplayerManager>().WeaponPivot;
                            deserializedItem.inWorldVersion.transform.SetPositionAndRotation(weaponPivot.transform.position, weaponPivot.transform.rotation);
                            deserializedItem.inWorldVersion.transform.parent = weaponPivot.transform;
                            break;
                        }
                    }
                }
                deserializedItem.inWorldVersion.GetComponent<Rigidbody>().useGravity = false;
                deserializedItem.inWorldVersion.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
}
