using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceController : MonoBehaviour, ISaveDataPersistance
{
    [SerializeField]
    public string uniqueObjectId;
    [SerializeField]
    GemsInObjectsManager gemsInObjectsManager;
    public PlayerManager InteractingPlayer { get => _interactingPlayer; set => _interactingPlayer = value; } [SerializeField] PlayerManager _interactingPlayer;

    [SerializeField]
    Animator[] animator;

    public GameObject ObjectInventoryWindow;

    public Transform insideObjPos;

    public GameObject InsideGameObject;

    public UnityEngine.UI.Image gemSocket_sprite;
    public UnityEngine.UI.Image objectInside_sprite;

    public UnityEngine.UI.Button unequipGemButton;

    public float temperature;
    public GemItem gemSocket;
    public MaterialItem objectInside;
    public bool isOpen;
    public bool isPourOpen;
    public bool isWorking;
    public bool isEmpty = true;

    public BoxCollider insideTrigger;
    public string _serializedFurnaceState;
   
    private void Start()
    {
        if (uniqueObjectId == "")
        {
            uniqueObjectId = GetComponent<PrefabGUID>().uniqueObjectId;
        }
        if (gemsInObjectsManager == null)
        {
            gemsInObjectsManager = GetComponent<GemsInObjectsManager>();
        }
    }

    private void Update()
    {
        UpdateTemperature();
        UseEnergySource();
    }

    public string LoadData(SaveData saveData, string itemID)
    {
        saveData._serializedFurnaces.TryGetValue(uniqueObjectId, out _serializedFurnaceState);
        if (_serializedFurnaceState != null)
        {
            SerializableFurnace serializableFurnace = JsonUtility.FromJson<SerializableFurnace>(_serializedFurnaceState);
            temperature = serializableFurnace.temperature;
            gemSocket = SerializationController._instance.DeserializeItem(serializableFurnace.gemItem) as GemItem;
            unequipGemButton.interactable = false;
            if (gemSocket != null)
            {
                gemSocket_sprite.sprite = gemSocket.itemIcon;
                unequipGemButton.interactable = true;
                gemsInObjectsManager.ManageGemInObject(0);
            }            
            isOpen = serializableFurnace.isOpen;
            if (isOpen)
            {
                animator[0].Play("FurnaceOpen");
                EnablePutSomethingInteraction();
            }
            isWorking = serializableFurnace.isWorking;
            isEmpty = serializableFurnace.isEmpty;
            if (!isEmpty)
            {
                objectInside = SerializationController._instance.DeserializeItem(serializableFurnace.item) as MaterialItem;
                GameObject materialInside = Instantiate(objectInside.inWorldVersion, insideObjPos.position, new Quaternion(0, 0, 0, 0), insideObjPos);
                SpawnObjInsideInWorld();
            }
        }
        return itemID = "";
    }

    public void StoreData(ref SaveData saveData)
    {
        SerializableFurnace serializableFurnaceState = new SerializableFurnace(this);
        string json = JsonUtility.ToJson(serializableFurnaceState,true);
        if (uniqueObjectId == "")
        {
            uniqueObjectId = uniqueObjectId = GetComponent<PrefabGUID>().uniqueObjectId;
        }
        else if(saveData._serializedFurnaces.ContainsKey(uniqueObjectId))
        {
            saveData._serializedFurnaces.Remove(uniqueObjectId);
        }
        saveData._serializedFurnaces.Add(uniqueObjectId, json);
    }

    public void OpenDoor(PlayerManager playerOpeningDoor)
    {
        _interactingPlayer = playerOpeningDoor;
        isOpen = !isOpen;
        animator[0].Play("FurnaceOpen");
        EnablePutSomethingInteraction();
    }

    public void CloseDoor(PlayerManager playerClosingDoor)
    {
        _interactingPlayer = playerClosingDoor;
        isOpen = !isOpen;
        animator[0].Play("FurnaceClose");
        DisablePutSomethingInteraction();
    }

    public void OpenPour(PlayerManager playerOpeningDoor)
    {
        _interactingPlayer = playerOpeningDoor;
        isPourOpen = !isPourOpen;
        animator[1].Play("MetalFurnacePourDoorOpen");
        EnablePutSomethingInteraction();
    }

    public void ClosePour(PlayerManager playerClosingDoor)
    {
        _interactingPlayer = playerClosingDoor;
        isPourOpen = !isPourOpen;
        animator[1].Play("MetalFurnacePourDoorClose");
        DisablePutSomethingInteraction();
    }

    public void EnablePutSomethingInteraction()
    {
        insideTrigger.enabled = true;
    }

    public void DisablePutSomethingInteraction()
    {
        insideTrigger.enabled = false;
    }

    public void OpenObjectGemSlot(PlayerManager interactingPlayer)
    {
        _interactingPlayer = interactingPlayer;
        ObjectInventoryWindow.SetActive(true);

        if (gemSocket != null)
        {
            gemSocket_sprite.sprite = gemSocket.itemIcon;
            unequipGemButton.interactable = true;
        }
        else
        {
            gemSocket_sprite.sprite = null;
            unequipGemButton.interactable = false;
        }

        UIManager uIManager = interactingPlayer.UIManager;
        uIManager.AddOpenWindow(ObjectInventoryWindow);//Maybe move to being an event?
        InputHandler inputHandler = interactingPlayer.gameObject.GetComponent<InputHandler>();
        inputHandler.OpenUI();//Maybe move to being an event?
    }
    public void SetChangeGem(PlayerManager playerChangingGem)
    {
        _interactingPlayer = playerChangingGem;

        ItemReceptacle itemReceptacle = _interactingPlayer.gameObject.GetComponent<ItemReceptacle>();
        PlayerInventory playerInventory = _interactingPlayer.gameObject.GetComponent<PlayerInventory>();

        if (gemSocket != null)
        {
            playerInventory.AddItem(gemSocket);
            gemSocket = null;
        }            

        gemSocket = itemReceptacle.gemItem;
        gemSocket_sprite.sprite = gemSocket.itemIcon;
        unequipGemButton.interactable = true;
        playerInventory.RemoveItem(gemSocket);
        itemReceptacle.ClearGemToEquip();
        gemsInObjectsManager.ManageGemInObject(0);
    }

    public void UnequipGem()
    {
        if (gemSocket != null)
        {
            PlayerInventory playerInventory = _interactingPlayer.gameObject.GetComponent<PlayerInventory>();
            playerInventory.AddItem(gemSocket);
            _interactingPlayer.UIManager.ShowGemItemSlots();
            gemSocket = null;
            gemSocket_sprite.sprite = null;
            unequipGemButton.interactable = false;
            gemsInObjectsManager.ManageGemInObject(0);
        }
    }

    public void PutObjectInside(PlayerManager playerPuttingSomethingInFurncace)
    {
        _interactingPlayer = playerPuttingSomethingInFurncace;

        ItemReceptacle itemReceptacle = _interactingPlayer.gameObject.GetComponent<ItemReceptacle>();
        PlayerInventory playerInventory = _interactingPlayer.gameObject.GetComponent<PlayerInventory>();

        if (objectInside != null)
        {
            playerInventory.AddItem(objectInside);
            Transform objTransform = insideObjPos.GetComponentInChildren<Transform>();
            Destroy(objTransform);
        }            

        objectInside = itemReceptacle.materialItem;
        objectInside_sprite.sprite = objectInside.itemIcon;
        playerInventory.RemoveItem(objectInside);
        itemReceptacle.ClearMaterialItem();
        SpawnObjInsideInWorld();
    }

    public void SpawnObjInsideInWorld()
    {
        InsideGameObject = Instantiate(objectInside.inWorldVersion, insideObjPos.position, new Quaternion(0, 0, 0, 0), insideObjPos);
        InsideGameObject.GetComponent<BoxCollider>().enabled = false;
        InsideGameObject.GetComponent<Rigidbody>().useGravity = false;
    }

    public void UseEnergySource()
    {
        //TODO: consume energy from the enrgy source
    }
    public void UpdateTemperature()
    {
        //TODO: calculate the temperature
    }
}

[System.Serializable]
public class SerializableFurnace
{
    public string uniqueObjectId;
    public float temperature;
    public SerializableGemItem gemItem;
    public SerializableItem item;
    public bool isOpen;
    public bool isWorking;
    public bool isEmpty;

    public SerializableFurnace(FurnaceController furnaceController)
    {
        uniqueObjectId = furnaceController.uniqueObjectId;
        temperature = furnaceController.temperature;
        gemItem = null;
        if (furnaceController.gemSocket != null)
        {
            gemItem = new SerializableGemItem(furnaceController.gemSocket);
        }
        isOpen = furnaceController.isOpen;
        isWorking = furnaceController.isWorking;
        isEmpty = furnaceController.isEmpty;
        if (!furnaceController.isEmpty)
        {
            item = new SerializableItem(furnaceController.objectInside);
        }
    }
}