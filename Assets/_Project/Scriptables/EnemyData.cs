using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "GameEntities/Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public ElementTypes enemyAttribute;
    public WeaponProficiency enemyWeaponProficiency;
    public Handedness enemyHandedness;
    public int enemyLevel;
    public int enemyExp;

    public int healthLevel;
    public int staminaLevel;
    public int strenghtLevel;
    public int agilityLevel;
    public int dexterityLevel;
    public int inteligence;
    public int elementalControl;

    public int staminaRegenRate;

    public WeaponItem RightHandWeapon;
    public WeaponItem LeftHandWeapon;

    public State startingState;
    public List<State> possibleAIStates;

    public List<dropTable> dropTable;
}

[Serializable]
public struct dropTable
{
    public Item itemDrop;
    public int dropPercentage;
}