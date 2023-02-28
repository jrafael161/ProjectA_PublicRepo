using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{    
    public int saveSlot;
    public string lastTimeSaved;
    public bool isUsed;

    public string characterName;
    public int race;
    public int weaponProficiency;
    public Handedness handedness;
    public int level;
    public int exp;

    public int health;

    public Vector3 playerWorldPos;
    
    public List<string> _serializedInventory;

    public string[] weaponsInRightHandSlotUniqueID = new string[2];
    public string[] weaponsInLeftHandSlotUniqueID = new string[2];

    public int currentRightWeaponIndex = -1;
    public int currentLeftWeaponIndex = -1;

    public bool isTwoHanding;

    public string ammoItem;

    public SerializableDictionary<string, string> _serializedWeaponDisplayers;
    public SerializableDictionary<string, string> _serializedFurnaces;
    public SerializableDictionary<string, string> _serializedWorldItems;

    public SaveData(string charName, int race, Handedness handed, Vector3 playerSpawn, int saveSlot, string date)
    {
        this.saveSlot = saveSlot;
        lastTimeSaved = date;
        isUsed = true;        

        characterName = charName;
        this.race = race;
        handedness = handed;
        level = 1;
        health = 100;

        playerWorldPos = playerSpawn;
        _serializedInventory = new List<string>();
        _serializedWeaponDisplayers = new SerializableDictionary<string, string>();
        _serializedFurnaces = new SerializableDictionary<string, string>();
        _serializedWorldItems = new SerializableDictionary<string, string>();
    }

    public SaveData()
    {
        lastTimeSaved = "NotValid";
        isUsed = false;

        characterName = "NotValid";
        race = -1;
        handedness = Handedness.RightHanded;
        level = -1;
        health = 100;

        _serializedInventory = new List<string>();
        _serializedWeaponDisplayers = new SerializableDictionary<string, string>();
        _serializedFurnaces = new SerializableDictionary<string, string>();
        _serializedWorldItems = new SerializableDictionary<string, string>();
    }
}
