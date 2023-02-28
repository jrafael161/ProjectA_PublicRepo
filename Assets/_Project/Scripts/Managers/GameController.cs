using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public enum GameState
{
    InMenu,
    InGame,
    Paused,
    StartScreen
}
public class GameController : MonoBehaviour
{
    public static GameController _instance;

    public GameState state;

    public static event Action<GameState> OnGameStateChanged;

    GameObject player;

    bool isNewGame = false;
    public bool settingsChanged = false;
    public GameSettings gameSettings;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }            
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        ChangeGameState(GameState.StartScreen);
        Application.targetFrameRate = 60;
        gameSettings = new GameSettings();
        #if UNITY_STANDALONE_WIN
        DebugManager.instance.enableRuntimeUI = false;//There is a bug with the URP in the development build
        #endif
        #if UNITY_EDITOR
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "WorldScene")
        {
            ChangeGameState(GameState.InGame);
        }
        #endif
        if (!PlayerPrefs.HasKey("PlayerSettingsCreated"))
            CreatePlayerPrefsGameSettings();
    }

    private void CreatePlayerPrefsGameSettings()
    {
        CreateScreenRelatedPlayerPrefs();
        CreateDisplayRelatedPlayerPrefs();
        CreateGameGraphicsQualityPlayerPrefs();
        CreateSoundRelatedPlayerPrefs();
        CreateControlsRelatedPlayerPrefs();
        PlayerPrefs.SetInt("PlayerSettingsCreated", 1);
    }

    private void CreateScreenRelatedPlayerPrefs()
    {
        PlayerPrefs.SetInt("ScreenResolutionX", 1920);
        PlayerPrefs.SetInt("ScreenResolutionY", 1080);
        PlayerPrefs.SetInt("Framerate", 60);
        PlayerPrefs.SetInt("DisplayMode", 0);
        PlayerPrefs.SetInt("VSync", 0);
    }

    public void CreateDisplayRelatedPlayerPrefs()
    {
        PlayerPrefs.SetInt("HUD", 1);
        PlayerPrefs.SetInt("UIScale", 1);
        PlayerPrefs.SetInt("Subtitles", 1);
    }

    public void CreateGameGraphicsQualityPlayerPrefs()
    {
        PlayerPrefs.SetInt("AntiAliasing", 1);
        PlayerPrefs.SetInt("MotionBlur", 1);
        PlayerPrefs.SetInt("ChromaticAberration", 1);
    }

    public void CreateSoundRelatedPlayerPrefs()
    {
        PlayerPrefs.SetInt("MasterVolume", 100);
        PlayerPrefs.SetInt("EnvironmentVolume", 100);
        PlayerPrefs.SetInt("MusicVolume", 100);
        PlayerPrefs.SetInt("VoicesVolume", 100);
        PlayerPrefs.SetInt("SFXVolume", 100);
    }

    public void CreateControlsRelatedPlayerPrefs()
    {
        PlayerPrefs.SetInt("XAxis", 1);
        PlayerPrefs.SetInt("YAxis", 1);
        PlayerPrefs.SetFloat("XAxisCameraSensivity", (float) 1);
        PlayerPrefs.SetFloat("YAxisCameraSensivity", (float) 1.1);
        PlayerPrefs.SetInt("Vibration", 1);

        //TODO: Create the buttons rebind listing
    }

    public void SaveGameSettings()
    {
        //TODO: when there is a change save the settings
        PlayerPrefs.SetInt("MasterVolume", gameSettings.masterVolume);
        settingsChanged = false;
    }

    public void CheckIfSettingsChanged()
    {
        if (settingsChanged)
        {
            SaveGameSettings();
        }
    }

    public void StartGame(SaveData saveData, bool isFirstTime = false)
    {
        isNewGame = isFirstTime;
        //SceneManager.LoadScene("WorldScene");        
        StartCoroutine(LoadYourAsyncScene(saveData));
    }

    public void ChangeGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.InMenu:
                CameraHandler._instance.DisableCameraInputs();
                state = GameState.InMenu;
                break;
            case GameState.InGame:
                CameraHandler._instance.EnableCameraInputs();
                state = GameState.InGame;
                GameStarted();
                break;
            case GameState.Paused:
                CameraHandler._instance.DisableCameraInputs();
                state = GameState.Paused;
                break;
            case GameState.StartScreen:
                state = GameState.StartScreen;
                break;
            default:
                Debug.LogError("Error unknown game state");
                break;
        }
        OnGameStateChanged?.Invoke(newState);
    }
    public SaveData GetPlayerData()
    {
        SaveData saveData = new SaveData();//Instead of instanciating a new save data atch it to the player and modify it as the game goes so its always ready to save
        player = FindObjectOfType<PlayerManager>().gameObject;//the game controller will need an array of players to manage the save of each player?

        saveData.playerWorldPos = player.transform.position;
        PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();
        playerInventory.SerializeInventory();
        saveData._serializedInventory = playerInventory._serializedItemsInventory;
        for (int i = 0; i < 2; i++)
        {
            if(playerInventory.weaponsInRightHandSlot[i] != null)
                if (playerInventory.weaponsInRightHandSlot[i].itemID != 0)
                    saveData.weaponsInRightHandSlotUniqueID[i] = playerInventory.weaponsInRightHandSlot[i].uniqueItemID;
                
        }
        for (int i = 0; i < 2; i++)
        {
            if (playerInventory.weaponsInLeftHandSlot[i] != null)
                if (playerInventory.weaponsInLeftHandSlot[i].itemID != 0)
                    saveData.weaponsInLeftHandSlotUniqueID[i] = playerInventory.weaponsInLeftHandSlot[i].uniqueItemID;
        }

        saveData.currentRightWeaponIndex = playerInventory.currentRightWeaponIndex;
        saveData.currentLeftWeaponIndex = playerInventory.currentLeftWeaponIndex;

        if (player.GetComponent<PlayerManager>().isTwoHandingWeapon)
        {
            saveData.isTwoHanding = true;
            //if ((playerInventory.leftHandWeapon != null && playerInventory.leftHandWeapon.isUnarmed == false) && (playerInventory.rightHandWeapon != null && playerInventory.rightHandWeapon.isUnarmed == false))
            //{
            //    switch (playerStats.Handedness)
            //    {
            //        case Handedness.RightHanded:
            //            saveData.twoHandingLeftHandWeapon = false;
            //            break;
            //        case Handedness.LeftHanded:
            //            saveData.twoHandingLeftHandWeapon = true;
            //            break;
            //        case Handedness.Ambidextrous:
            //            saveData.twoHandingLeftHandWeapon = false;
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //else
            //{
            //    if (playerInventory.leftHandWeapon != null && playerInventory.leftHandWeapon.isUnarmed == false)
            //    {
            //        saveData.twoHandingLeftHandWeapon = true;
            //    }
            //    if (playerInventory.rightHandWeapon != null && playerInventory.rightHandWeapon.isUnarmed == false)
            //    {
            //        saveData.twoHandingLeftHandWeapon = false;
            //    }
            //}
        }
        else
        {
            saveData.isTwoHanding = false;
        }

        if (playerInventory.ammoSlot != null)
        {
            saveData.ammoItem = playerInventory.ammoSlot.itemName;
        }

        PlayerStats playerStats = player.GetComponent<PlayerStats>();

        saveData.characterName = playerStats.CharacterName;//Some of this things should inly be set the first time a save is done
        saveData.race = playerStats.characterKin;
        saveData.weaponProficiency = playerStats.characterWeaponProf;
        saveData.handedness = playerStats.Handedness;
        saveData.level = playerStats.Level;
        saveData.exp = playerStats.Exp;
        saveData.health = playerStats.currentHealth;

        //get the current player data to send this info to the save controller in order to overwrite the save data

        return saveData;
    }

    public void GameStarted()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void QuitGame()
    {
        CheckIfSettingsChanged();
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        CheckIfSettingsChanged();
        SceneManager.LoadScene("StartScreen");
    }

    IEnumerator LoadYourAsyncScene(SaveData saveData)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("WorldScene");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (asyncLoad.isDone)
        {
            ChangeGameState(GameState.InGame);            
            SavesController._instance.dataPersistanceObjects = SavesController._instance.FindAllDataPersistanceObjects();
            SavesController._instance.LoadAllDataPersistanceObjects(saveData);
            //if (isNewGame)
            //{
            //    FLOW.FlowSnapshot flowSnapshot = FindObjectOfType<FLOW.FlowSnapshot>();
            //    #if UNITY_EDITOR
            //        flowSnapshot.LoadFromFile(Application.dataPath + "/_Project/Scenes");
            //    #elif UNITY_STANDALONE_WIN
            //        flowSnapshot.LoadFromFile(Application.dataPath + "/StreamingAssets");
            //    #endif
            //}
            //else
            //{
            //    SavesController._instance.LoadFlowStateFromSaveFile(saveData.saveSlot);
            //}
            StartCoroutine(CheckForPlayer(saveData));
        }        
    }

    IEnumerator CheckForPlayer(SaveData saveData)
    {
        while (GameObject.FindGameObjectWithTag("Player") == null)
        {
            yield return null;
        }
        PreparePlayer(saveData);
    }

    public void RespawnPlayerAfterDeath()
    {
        SavesController._instance.LoadLastGame();
    }

    public void PreparePlayer(SaveData saveData)
    {
        player = FindObjectOfType<PlayerManager>().gameObject;

        if (player != null)
        {
            Debug.Log("Spawn position " + saveData.playerWorldPos);

            player.transform.position = saveData.playerWorldPos;

            Debug.Log("Player position " + player.transform.position);

            FindObjectOfType<PlayerManager>().SetPlayerSkin(saveData.race);

            PlayerStats playerStats = player.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.CharacterName = saveData.characterName;
                playerStats.characterKin = saveData.race;
                playerStats.characterWeaponProf = saveData.weaponProficiency;
                playerStats.Handedness = saveData.handedness;
                playerStats.Level = saveData.level;
                playerStats.Exp = saveData.exp;
                playerStats.currentHealth = saveData.health;
            }            

            if (!isNewGame)
            {
                PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();
                SerializableItem serializableItem;
                if (saveData._serializedInventory != null)
                {
                    foreach (string serializedItem in saveData._serializedInventory)//All items in the save file are deserialized
                    {
                        serializableItem = SerializationController._instance.DeserializeItemFromSaveFile1(serializedItem);
                        Item deserializedItem = SerializationController._instance.DeserializeItemFromSaveFile(serializedItem);
                        if (deserializedItem.itemType == ItemType.Weapon)
                        {
                            playerInventory.AddWeaponFromSaveFile(deserializedItem);
                        }
                        else
                        {
                            playerInventory.AddItem(deserializedItem);
                        }
                    }

                    foreach (Item item in playerInventory.itemsInventory)//All weapons are "reconstructed" to its save state
                    {
                        if (item.itemType == ItemType.Weapon)
                        {
                            playerInventory.ReconstructWeapon(item);
                        }
                        if (item.itemType == ItemType.Trinket)
                        {
                            //TODO : reconstruct trinkets the same way as weapons
                        }
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        if (saveData.weaponsInRightHandSlotUniqueID[i] != "")
                            playerInventory.weaponsInRightHandSlot[i] = playerInventory.itemsInventory.Find(x=>x.uniqueItemID == saveData.weaponsInRightHandSlotUniqueID[i]) as WeaponItem;

                    }
                    for (int i = 0; i < 2; i++)
                    {
                        if (saveData.weaponsInLeftHandSlotUniqueID[i] != "")
                            playerInventory.weaponsInLeftHandSlot[i] = playerInventory.itemsInventory.Find(x => x.uniqueItemID == saveData.weaponsInLeftHandSlotUniqueID[i]) as WeaponItem;
                    }

                    playerInventory.currentRightWeaponIndex = saveData.currentRightWeaponIndex;
                    playerInventory.currentLeftWeaponIndex = saveData.currentLeftWeaponIndex;

                    if (playerInventory.currentRightWeaponIndex >=0)
                    {
                        if (playerInventory.weaponsInRightHandSlot[playerInventory.currentRightWeaponIndex] != null)
                        {
                            playerInventory.rightHandWeapon = playerInventory.weaponsInRightHandSlot[playerInventory.currentRightWeaponIndex];
                            playerInventory._weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightHandWeapon, false);
                        }
                    }

                    if (playerInventory.currentLeftWeaponIndex >= 0)
                    {
                        if (playerInventory.weaponsInLeftHandSlot[playerInventory.currentLeftWeaponIndex] != null)
                        {
                            playerInventory.leftHandWeapon = playerInventory.weaponsInLeftHandSlot[playerInventory.currentLeftWeaponIndex];
                            playerInventory._weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftHandWeapon, true);
                        }
                    }

                    if (saveData.isTwoHanding)
                    {
                        player.GetComponent<InputHandler>().twoHandedFlag = true;//Not the best thing to do but it will do in the meantime
                        player.GetComponentInChildren<PlayerCombatHandler>().HandleTwoHandingWeapon();
                        player.GetComponent<InputHandler>().twoHandedFlag = false;
                    }

                    if (saveData.ammoItem != "")//Not too fancy but will work in the meantime
                    {
                        playerInventory.ammoSlot = playerInventory.itemsInventory.Find(x => x.itemName == saveData.ammoItem) as AmmoItem;
                        UIManager uIManager = FindObjectOfType<UIManager>();
                        if (uIManager != null)
                        {
                            if (playerInventory.ammoSlot != null)
                            {
                                uIManager.ammoSlotCuantity_txt.text = playerInventory.itemsInventory.FindAll(x => x.itemName == playerInventory.ammoSlot.itemName).Count.ToString();
                                uIManager.ammoSlotCuantity_txt.gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }

            /*
            Debug.Log(playerStats.CharacterName);
            Debug.Log(playerStats.characterKin);
            Debug.Log(playerStats.Level);
            Debug.Log(player.transform.position);
            */

            SavesController._instance.SaveGame();
        }
    }
}

public struct GameSettings
{
    public int masterVolume;
}