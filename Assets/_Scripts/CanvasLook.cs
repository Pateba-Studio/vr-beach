using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLook : MonoBehaviour
{
    private void LateUpdate()
    {
        Vector3 camPos = transform.position - Camera.main.transform.position;
        camPos.y = 0;
        transform.rotation = Quaternion.LookRotation(camPos);
        
    }
}
