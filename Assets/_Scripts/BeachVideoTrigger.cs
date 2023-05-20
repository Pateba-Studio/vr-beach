using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachVideoTrigger : MonoBehaviour
{
    public GameObject videoPlayerPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            videoPlayerPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            videoPlayerPanel.SetActive(false);
            videoPlayerPanel.transform.GetChild(1).gameObject.SetActive(true);
            videoPlayerPanel.transform.GetChild(2).gameObject.SetActive(false);
        }
    }
}
