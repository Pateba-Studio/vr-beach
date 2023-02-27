using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowCanScore : MonoBehaviour
{
    public bool isCounted;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ThrowCanFloor") &&
            !isCounted)
        {
            GameManager.instance.gameComponents[(int)GameManager.instance.gameType].gameScore += 1;
            isCounted = true;
        }
    }
}
