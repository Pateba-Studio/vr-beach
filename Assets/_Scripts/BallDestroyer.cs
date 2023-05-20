using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDestroyer : MonoBehaviour
{
    public string ballDestroyerTag;
    public bool isWork;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(ballDestroyerTag) && 
            isWork)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ballDestroyerTag) && 
            isWork)
        {
            Destroy(gameObject);
        }
    }
}
