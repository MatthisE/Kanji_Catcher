using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// given to area exits
public class AreaExit : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] string transitionAreaName;

    // load new scene when player enters exit's collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player.instance.transitionAreaName = transitionAreaName; // make player's transition name to this area exit's transition name
            MenuManager.instance.FadeImage(); // fade to black
            StartCoroutine(LoadSceneCoroutine());
        }
    }

    IEnumerator LoadSceneCoroutine()
    {
        yield return new WaitForSeconds(1f); // wait 1sec before loading new scene, so Fade_End animation can play out 
        SceneManager.LoadScene(sceneToLoad); // load new scene
    }
}
