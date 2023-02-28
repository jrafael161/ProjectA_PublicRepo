using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates{
    Ambush,
    Iddle,
    Pursuing,
    CombatStance,
    Attacking,
    Falling
}

public abstract class State : MonoBehaviour
{
    public EnemyStates StateID;
    public abstract State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimControler enemyAnim);
}
