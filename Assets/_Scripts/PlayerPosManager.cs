using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosManager : MonoBehaviour
{
    public static PlayerPosManager instance;
    public PositionName startingPoint;
    public bool isInitiated;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (FindObjectOfType<RepositionHandler>() &&
            !isInitiated)
        {
            FindObjectOfType<RepositionHandler>().startingPoint = startingPoint;
            FindObjectOfType<RepositionHandler>().InitPlayerPos();
            isInitiated = true;
        }
    }
}
