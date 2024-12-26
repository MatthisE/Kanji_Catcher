using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanjiManager : MonoBehaviour
{
    [SerializeField] Sprite kanjiImage;
    [SerializeField] Sprite strokeOrder;

    [SerializeField] string kanjiSymbol;

    [SerializeField] string[] kunyomi;
    [SerializeField] string[] onyomi;

    [SerializeField] string[] meanings;

    [SerializeField] TrainingWord[] trainingWords;

    [SerializeField] bool isCollected;
    [SerializeField] int currentXP;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
