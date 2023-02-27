using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFollowCam : MonoBehaviour
{
    public Camera camera;
    public float distance = 1f;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        Vector3 targetPosition = camera.transform.TransformPoint(new Vector3(0, 0, distance));

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        var lookAtPos = new Vector3(camera.transform.position.x, transform.position.y, camera.transform.position.z);
        transform.LookAt(lookAtPos);
    }
}
