using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    public bool shouldBeInactive;
    public List<GameObject> objShouldBeInactive;

    void Awake()
    {
#if UNITY_EDITOR
        if (shouldBeInactive)
            foreach (GameObject obj in objShouldBeInactive)
                obj.SetActive(false);
#endif
    }
}
