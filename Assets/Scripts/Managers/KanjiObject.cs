using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KanjiObject : MonoBehaviour
{
    [SerializeField] KanjiManager kanji;

    // when loading scene only display kanji objects that player has not collected already
    void Start()
    {
        KanjiManager[] collectedKanjiList = GameManager.instance.GetCollectedKanji();

        foreach(KanjiManager collectedKanji in collectedKanjiList)
        {
            if(collectedKanji.kanjiSymbol == kanji.kanjiSymbol)
            {
                SelfDestroy();
            }
        }
    }

    // collecting a kanji on the map
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            AudioManager.instance.PlaySFX(5);
            
            // add kanji to collected kanji of player
            PlayerStats[] playerStats = GameManager.instance.GetPlayerStats();
            Array.Resize(ref playerStats[0].collectedKanji, playerStats[0].collectedKanji.Length + 1);
            playerStats[0].collectedKanji[playerStats[0].collectedKanji.Length - 1] = kanji;

            SelfDestroy();
        }
    }

    // make kanji disappear from map
    public void SelfDestroy()
    {
        gameObject.SetActive(false);
    }
}
