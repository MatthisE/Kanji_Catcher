using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KanjiManager : MonoBehaviour
{
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

    private PlayerStats[] playerStats;

    // collecting a kanji on the map
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            AudioManager.instance.PlaySFX(5);
            
            // add kanji to collected kanji of player
            playerStats = GameManager.instance.GetPlayerStats();
            Array.Resize(ref playerStats[0].collectedKanji, playerStats[0].collectedKanji.Length + 1);
            playerStats[0].collectedKanji[playerStats[0].collectedKanji.Length - 1] = this;

            SelfDestroy();
        }
    }

    // make kanji disappear from map
    public void SelfDestroy()
    {
        gameObject.SetActive(false);
    }
}
