using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoviePlayer : MonoBehaviour
{
    [SerializeField] private GameObject image1;
    [SerializeField] private GameObject image2;
    [SerializeField] private GameObject image3;
    [SerializeField] private GameObject image4;
    [SerializeField] private GameObject image5;

    [SerializeField] private int musicToPlay;


    private void Start()
    {
        _ = StartCoroutine(Slideshow());

        AudioManager.instance.PlayBackgroundMusic(musicToPlay);
        AudioManager.instance.prevMusic = musicToPlay; // set prevMusic
    }

    public IEnumerator Slideshow()
    {

        yield return new WaitForSeconds(3f);

        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(0.5f);
        image1.SetActive(false);
        MenuManager.instance.FadeOut();

        yield return new WaitForSeconds(3f);

        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(0.5f);
        image2.SetActive(false);
        MenuManager.instance.FadeOut();

        yield return new WaitForSeconds(3f);

        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(0.5f);
        image3.SetActive(false);
        MenuManager.instance.FadeOut();

        yield return new WaitForSeconds(3f);

        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(0.5f);
        image4.SetActive(false);
        MenuManager.instance.FadeOut();

        yield return new WaitForSeconds(3f);

        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("Library1");
    }

}