using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonScript : MonoBehaviour
{
    public static CommonScript instance;
    public PositionName position;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ChangePosAfterThreeSixty()
    {
        VideoPlaylistHandler.instance.continuousMove.enabled = true;
        VideoPlaylistHandler.instance.continuousTurn.enabled = true;

        for (int i = 0; i < VideoPlaylistHandler.instance.xRRayInteractors.Count; i++)
        {
            VideoPlaylistHandler.instance.xRRayInteractors[i].maxRaycastDistance = VideoPlaylistHandler.instance.lineInteractorLengthTemp;
            VideoPlaylistHandler.instance.xRInteractorLineVisuals[i].lineLength = VideoPlaylistHandler.instance.lineInteractorLengthTemp;
        }

        FindObjectOfType<ThreeSixtyVideoPlayer>().StopVideo();
        FindObjectOfType<ThreeSixtyVideoPlayer>().gameObject.SetActive(false);
        FindObjectOfType<PlayerPosManager>().startingPoint = position;
        FindObjectOfType<PlayerPosManager>().isInitiated = false;
    }
}
