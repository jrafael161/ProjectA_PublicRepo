using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewGameController : MonoBehaviour
{
    public static NewGameController _instance;

    [Header ("Character name")]
    [SerializeField]
    TMPro.TMP_InputField CharacterName;

    [Header("Chosen Race")]
    [SerializeField]
    TMPro.TMP_Text ChosenRace;

    [Header("Race Model")]
    [SerializeField]
    Transform ModelPos;

    [Header("Handedness")]
    [SerializeField]
    Toggle rightHanded;
    [SerializeField]
    Toggle leftHanded;

    [Header("Village Spawns")]
    [SerializeField]
    Transform[] SpawnLocations = new Transform[6];

    [Header("Race Models")]
    [SerializeField]
    GameObject[] RaceModels = new GameObject[6];

    [SerializeField]
    Handedness Handedness;

    SaveData saveData;
    int CharacterRace;

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        CharacterRace = 0;
        saveData = new SaveData();
    }
    
    public SaveData GetPlayerCharacter()
    {        
        return saveData;
    }
    public void ChangeCharacterRace(int change)
    {
        CharacterRace += change;
        if (CharacterRace<0)
            CharacterRace =5;
        if (CharacterRace>5)
            CharacterRace = 0;

        UpdateCharacterCreatorScreen((Kins)CharacterRace);
    }

    public void SetRightHandedness()
    {
        if (rightHanded.isOn)
        {
            Handedness = Handedness.RightHanded;
            if (leftHanded.isOn)
            {
                Handedness = Handedness.Ambidextrous;
            }
        }
        else
        {
            if (leftHanded.isOn)
            {
                Handedness = Handedness.LeftHanded;
            }
        }
    }

    public void SetLefHandedness()
    {
        if (leftHanded.isOn)
        {
            Handedness = Handedness.LeftHanded;
            if (rightHanded.isOn)
            {
                Handedness = Handedness.Ambidextrous;
            }
        }
        else
        {
            if (rightHanded.isOn)
            {
                Handedness = Handedness.RightHanded;
            }
        }
    }

    public void UpdateCharacterCreatorScreen(Kins worldRaces)
    {
        Destroy(ModelPos.GetComponentsInChildren<Transform>()[1].gameObject);

        switch (worldRaces)
        {
            case Kins.FireKin:
                ChosenRace.text = "Fire Kin";                       
                GameObject.Instantiate(RaceModels[0], ModelPos);
                break;
            case Kins.WaterKin:
                ChosenRace.text = "Water Kin";
                GameObject.Instantiate(RaceModels[1], ModelPos);
                break;
            case Kins.ElectricKin:
                ChosenRace.text = "Thunder Kin";
                GameObject.Instantiate(RaceModels[2], ModelPos);
                break;
            case Kins.RockKin:
                ChosenRace.text = "Rock Kin";
                GameObject.Instantiate(RaceModels[3], ModelPos);
                break;
            case Kins.WindKin:
                ChosenRace.text = "Wind Kin";
                GameObject.Instantiate(RaceModels[4], ModelPos);
                break;
            case Kins.VoidKin:
                ChosenRace.text = "Void Kin";
                GameObject.Instantiate(RaceModels[5], ModelPos);
                break;
            default:
                break;
        }
    }

    public bool DataVerification()
    {
        if (CharacterName.text != "")
            return true;
        else
        {
            Debug.LogWarning("You must declare your name");
            return false;
        }
    }

    public void DumpPlayerData()
    {
        saveData = new SaveData(CharacterName.text, CharacterRace, Handedness, VillageSpawn(CharacterRace), SavesController._instance.GetActiveSave(), System.DateTime.Now.ToString());
    }

    public Vector3 VillageSpawn(int characterRace)
    {
        Vector3 spawnPos = new Vector3(SpawnLocations[characterRace].position.x, SpawnLocations[characterRace].position.y, SpawnLocations[characterRace].position.z);
        return spawnPos;
    }
}
