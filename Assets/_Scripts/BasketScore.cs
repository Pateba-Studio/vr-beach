using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketScore : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            GameManager.instance.gameComponents[(int)GameManager.instance.gameType].gameScore += 1;
        }
    }
}
