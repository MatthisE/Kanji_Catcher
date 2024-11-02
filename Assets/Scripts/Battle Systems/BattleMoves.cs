using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // this class can be used anywhere

public class BattleMoves // should not be attached to anything, is just source of information
{
    public string moveName;
    public int movePower;
    public int manaCost;
    public AttackEffect effectToUse;
}
