using UnityEngine;

// given to kanji list object
public class KanjiListManager : MonoBehaviour
{
    public static KanjiListManager instance;
    private void Awake()
    {
        //singelton pattern --> avoid duplicate Players in new scenes
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
}