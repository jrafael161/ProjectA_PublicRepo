using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadSaveController : MonoBehaviour//Change the name to SavesSlotsUIController
{
    [Header("Save Slot")]
    [SerializeField]
    Button SaveSlot;

    [Header("Save Slots Container")]
    [SerializeField]
    Transform SaveSlotContainer;

    [Header("Races Sprites")]
    [SerializeField]
    Sprite[] RacesSprites;

    List<GameObject> SaveSlots;
    List<Button> SaveSlotsAsButtons;

    private void Start()
    {
        DisplayAllSaves();
    }

    private void OnEnable()
    {
        DisplayAllSaves();
    }

    public void ContinueGame()
    {
        SavesController._instance.LoadLastGame();
    }

    public void StartNewGame()
    {
        SavesController._instance.CreateNewGame();
    }

    public void DisplayAllSaves()
    {
        List<SaveData> savesInfo = new List<SaveData>();
        SaveSlots = new List<GameObject>();
        SaveSlotsAsButtons = new List<Button>();
        savesInfo = SavesController._instance.GetAllSaves();

        Button[] Buttons = SaveSlotContainer.GetComponentsInChildren<Button>(true);
        foreach (Button button in Buttons)
        {
            if (button.gameObject.name.Contains("SaveSlot"))//Not ideal but will work in the meantime
            {
                SaveSlots.Add(button.gameObject);
                SaveSlotsAsButtons.Add(button);
            }            
        }

        if (SaveSlots.Count < savesInfo.Count)
        {
            while (SaveSlots.Count < savesInfo.Count)
            {
                Button newSlot = Instantiate(SaveSlot, SaveSlotContainer) as Button;
                SaveSlots.Add(newSlot.gameObject);
                SaveSlotsAsButtons.Add(newSlot);
            }            
        }

        for (int i = 0; i < savesInfo.Count; i++)
        {

            if (savesInfo[i].saveSlot==i && savesInfo[i].isUsed)
            {
                SaveSlotsAsButtons[i].gameObject.SetActive(true);
                SaveSlotsAsButtons[i].gameObject.transform.SetParent(SaveSlotContainer, true);
                SaveSlotsAsButtons[i].GetComponentInChildren<TMP_Text>().text = savesInfo[i].characterName;
                SaveSlotsAsButtons[i].GetComponentsInChildren<Image>()[1].sprite = RacesSprites[savesInfo[i].race];//Assignd the sprite to the second image in the save slot button
            }
            else
            {
                SaveSlotsAsButtons[i].gameObject.SetActive(false);
            }
            
        }

        foreach (Button button in SaveSlotsAsButtons)
        {
            if (button.IsActive())
            {
                button.Select();
                break;
            }                
        }

    }

    public void LoadSelectedSaveSlot(GameObject saveSlotObject)
    {
        int slot = IdentifySaveSlot(saveSlotObject);
        if (slot >= 0)
            SavesController._instance.LoadSpecificGame(slot);
        else
            Debug.LogError("Save slot not found");
    }

    public void DeleteSelectedSaveSlot(GameObject saveSlotObject)
    {
        int slot = IdentifySaveSlot(saveSlotObject);
        if (slot >= 0)
            SavesController._instance.DeleteSave(slot);
        else
            Debug.LogError("Save slot not found");

        saveSlotObject.SetActive(false);
    }

    int IdentifySaveSlot(GameObject saveSlotGameObjectId)
    {
        for (int i = 0; i < SaveSlots.Count; i++)
        {
            if (saveSlotGameObjectId == SaveSlots[i])
            {
                return i;
            }
        }

        return -1;
    }
}
