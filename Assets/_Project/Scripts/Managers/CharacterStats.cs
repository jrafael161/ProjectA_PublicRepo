using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Kins
{
    FireKin,
    WaterKin,
    RockKin,
    ElectricKin,
    WindKin,
    VoidKin
}

public enum WeaponProficiency
{
    Swords,
    Bows,
    Axes,
    Maces,
    Lances,
    Fists
}

public enum Handedness
{
    RightHanded,
    LeftHanded,
    Ambidextrous
}

public class CharacterStats : MonoBehaviour
{
    public string CharacterName;
    public int characterKin = (int)Kins.VoidKin;
    public int characterWeaponProf = (int)WeaponProficiency.Fists;
    public Handedness Handedness;
    public int Level;
    public int Exp;

    public int healthLevel = 10;//Vitality
    public int maxHealth;
    public int currentHealth;

    public int staminaLevel = 10;//Endurance
    public float maxStamina;
    public float currentStamina;

    public int strenghtLevel=1;
    public int agilityLevel=1;
    public int dexterityLevel=1;
    public int inteligence=1;
    public int elementalControl=1;

    public bool isDead;
}

