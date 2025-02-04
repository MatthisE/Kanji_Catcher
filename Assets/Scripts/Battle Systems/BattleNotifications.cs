using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// set a notification text and make it appear and disappear after certain time, part of BattleManager
public class BattleNotifications : MonoBehaviour
{
    [SerializeField] float timeAlive;
    [SerializeField] TextMeshProUGUI textNotice;

    public void SetText(string text)
    {
        textNotice.text = text;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        StartCoroutine(MakeNoticeDisappear());
    }

    IEnumerator MakeNoticeDisappear()
    {
        yield return new WaitForSeconds(timeAlive);
        gameObject.SetActive(false);
    }
}
