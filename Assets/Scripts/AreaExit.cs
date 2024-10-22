using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] string transitionAreaName;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // load new scene when Player hits it
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player.instance.transitionAreaName = transitionAreaName; // make Players transition name to this area exits transition name
            MenuManager.instance.FadeImage();
            StartCoroutine(LoadSceneCoroutine()); // call defined coroutine (IEnumerator) with StartCoroutine
        }
    }

    IEnumerator LoadSceneCoroutine() // IEnumerator --> Coroutine --> hold (yield) process until certain condition is met
    {
        yield return new WaitForSeconds(1f);//wait 1sec before loading new scene, so Fade_End animation can play out 
        SceneManager.LoadScene(sceneToLoad);
    }
}
