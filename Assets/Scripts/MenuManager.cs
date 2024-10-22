using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image imageToFade;

    public static MenuManager instance;

    private void Start()
    {
        instance = this; // no singelton pattern needed
    }

    // activate trigger for animator to fade
    public void FadeImage()
    {
        imageToFade.GetComponent<Animator>().SetTrigger("Start Fading");
    }
}
