using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// given to area exits
public class AreaExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string transitionAreaName;

    // load new scene when player enters exit's collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.goThroughExit = true; //set condition for player to stop moving
            Player.instance.transitionAreaName = transitionAreaName; // make player's transition name to this area exit's transition name
            MenuManager.instance.FadeImage(); // fade to black
            _ = StartCoroutine(LoadSceneCoroutine());
        }
    }

    private IEnumerator LoadSceneCoroutine()
    {
        yield return new WaitForSeconds(0.5f); // wait 1sec before loading new scene, so Fade_End animation can play out 
        SceneManager.LoadScene(sceneToLoad); // load new scene
    }
}