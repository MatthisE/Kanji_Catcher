using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogHandler : MonoBehaviour
{
    public string[] sentences;
    private bool canActivateBox; // can only be activated inside objects trigger box

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canActivateBox && Input.GetButtonDown("Fire1") && !DialogController.instance.IsDialogBoxActive()) // only call if the box is not already active
        {
            DialogController.instance.ActivateDialog(sentences);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canActivateBox = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canActivateBox = false;
        }
    }
}
