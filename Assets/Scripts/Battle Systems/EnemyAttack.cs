using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// given to enemy attack menu
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI kanjiToRead;
    [SerializeField] TextMeshProUGUI kanjiMeaning1;
    [SerializeField] TextMeshProUGUI kanjiMeaning2;
    [SerializeField] TextMeshProUGUI kanjiMeaning3;
    [SerializeField] TextMeshProUGUI kanjiMeaning4;

    public void setWords(TrainingWord trainingWord){
        kanjiToRead.text = trainingWord.inKanji;
    }

    void Start()
    {
    }

    void Update()
    {
    }
}
