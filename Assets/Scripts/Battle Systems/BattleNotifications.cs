using System.Collections;
using UnityEngine;
using TMPro;

// set a notification text and make it appear and disappear after certain time, part of BattleManager
public class BattleNotifications : MonoBehaviour
{
    [SerializeField] private float timeAlive;
    [SerializeField] private TextMeshProUGUI textNotice;

    public void SetText(string text)
    {
        textNotice.text = text;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        _ = StartCoroutine(MakeNoticeDisappear());
    }

    private IEnumerator MakeNoticeDisappear()
    {
        yield return new WaitForSeconds(timeAlive);
        gameObject.SetActive(false);
    }
}