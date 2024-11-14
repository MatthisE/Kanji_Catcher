using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // this class can be used anywhere

// define a battle move, BattleManager has a list of them
public class BattleMoves // should not be attached to an object, is just source of information (framework)
{
    public string moveName;
    public int movePower;
    public int manaCost;
    public AttackEffect effectToUse;
}
