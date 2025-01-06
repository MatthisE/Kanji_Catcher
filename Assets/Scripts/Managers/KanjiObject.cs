using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KanjiObject : MonoBehaviour
{
    [SerializeField] string kanjiName;
    private KanjiManager kanji;

    public float amplitude = 0.5f;  // how far the sprite moves up and down
    public float frequency = 1f;   // how fast the sprite moves up and down
    private Vector3 startPosition;

    void Start()
    {
        // when loading scene only display kanji objects that player has not collected already
        Transform child = KanjiListManager.instance.transform.Find(kanjiName);
        kanji = child.GetComponent<KanjiManager>();

        KanjiManager[] collectedKanjiList = GameManager.instance.GetCollectedKanji();

        foreach(KanjiManager collectedKanji in collectedKanjiList)
        {
            if(collectedKanji.kanjiSymbol == kanji.kanjiSymbol)
            {
                SelfDestroy();
            }
        }

        // save the starting position of the sprite
        startPosition = transform.position;
    }

    // make sprite move a bit up and down
     void Update()
    {
        // calculate the new Y position
        float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // update the position
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    // collecting a kanji on the map
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            string[] sentences = {"The cursed kanji has been sucked back into the kanji book.", "You can now use the kanji " +kanjiName+ " in battle."};

            AudioManager.instance.PlaySFX(8);
            
            // add kanji to collected kanji of player
            PlayerStats[] playerStats = GameManager.instance.GetPlayerStats();
            Array.Resize(ref playerStats[0].collectedKanji, playerStats[0].collectedKanji.Length + 1);
            playerStats[0].collectedKanji[playerStats[0].collectedKanji.Length - 1] = kanji;

            SelfDestroy();

            DialogController.instance.ActivateDialog(sentences, kanjiName); // open box with first sentence
        }
    }

    // make kanji disappear from map
    public void SelfDestroy()
    {
        gameObject.SetActive(false);
    }
}
