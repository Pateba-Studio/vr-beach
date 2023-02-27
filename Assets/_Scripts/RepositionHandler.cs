using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PositionName
{
    PasirPantai,
    VideoPlayer
}

[Serializable]
public class PositionDetail
{
    public PositionName posName;
    public Transform posObjectReference;
}

public class RepositionHandler : MonoBehaviour
{
    public GameObject playerXR;
    public PositionName startingPoint;
    [SerializeField] public List<PositionDetail> positionDetails;

    public void InitPlayerPos()
    {
        foreach (PositionDetail pos in positionDetails)
        {
            if (startingPoint == pos.posName)
            {
                playerXR.transform.position = pos.posObjectReference.position;
                playerXR.transform.rotation = pos.posObjectReference.rotation;
            }
        }
    }
}
