using UnityEngine;

// part of a battle move
public class AttackEffect : MonoBehaviour
{
    [SerializeField] private float effectTime;
    [SerializeField] private int SFXNumberToPlay;

    private void Start()
    {
        // play the right sound when the effect appears
        AudioManager.instance.PlaySFX(SFXNumberToPlay);
    }

    private void Update()
    {
        // destroy the effect after a certain time
        Destroy(gameObject, effectTime);
    }
}