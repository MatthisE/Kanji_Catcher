using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KanjiManager : MonoBehaviour
{
    public static KanjiManager instance; //constant across all projects, makes functions and vars usable in other scripts

    public Sprite kanjiImage;
    public Sprite strokeOrder;

    public string kanjiSymbol;

    public string[] kunyomi;
    public string[] onyomi;

    public string[] meanings;

    public TrainingWord[] trainingWords;

    public bool isCollected;
    public int currentXP;
    public int xpReward = 0;
}
