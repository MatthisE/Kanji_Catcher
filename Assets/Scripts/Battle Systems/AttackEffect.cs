using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// part of a battle move
public class AttackEffect : MonoBehaviour
{
    [SerializeField] float effectTime;
    [SerializeField] int SFXNumberToPlay;

    void Start()
    {
        // play the right sound when the effect appears
        AudioManager.instance.PlaySFX(SFXNumberToPlay);
    }

    void Update()
    {
        // destroy the effect after a certain time
        Destroy(gameObject, effectTime);
    }
}
