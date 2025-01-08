using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoviePlayer : MonoBehaviour
{
    [SerializeField] GameObject image1;
    [SerializeField] GameObject image2;
    [SerializeField] GameObject image3;
    [SerializeField] GameObject image4;
    [SerializeField] GameObject image5;

    [SerializeField] int musicToPlay;

    
    void Start()
    {
        StartCoroutine(Slideshow());

        AudioManager.instance.PlayBackgroundMusic(musicToPlay);
        AudioManager.instance.prevMusic = musicToPlay; // set prevMusic
    }

    public IEnumerator Slideshow()
    {
        
        yield return new WaitForSeconds(3f);

        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(0.4f);
        image1.SetActive(false);
        MenuManager.instance.FadeOut();

        yield return new WaitForSeconds(3f);

        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(0.4f);
        image2.SetActive(false);
        MenuManager.instance.FadeOut();

        yield return new WaitForSeconds(3f);

        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(0.4f);
        image3.SetActive(false);
        MenuManager.instance.FadeOut();

        yield return new WaitForSeconds(3f);

        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(0.4f);
        image4.SetActive(false);
        MenuManager.instance.FadeOut();

        yield return new WaitForSeconds(3f);

        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(0.4f);

        SceneManager.LoadScene("Library1");
    }

}
