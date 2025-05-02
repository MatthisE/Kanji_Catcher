using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// given to loader object of loading scene
public class LoadingScene : MonoBehaviour
{
    [SerializeField] private float waitToLoadTime;

    private void Start()
    {
        if (waitToLoadTime > 0)
        {
            _ = StartCoroutine(LoadScene());
        }
    }

    public IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(waitToLoadTime);

        SceneManager.LoadScene(PlayerPrefs.GetString("Current_Scene")); // load scene that was last saved in PlayerPrefs
        GameManager.instance.LoadData(); // load other PlayerPrefs
        //QuestManager.instance.LoadQuestData();
    }
}