using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BasicVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject menuPanel;
    public GameObject isPlayingPanel;
    public GameObject isNotPlayingPanel;

    void EndReached(VideoPlayer vp)
    {
        StopVideo();
    }

    public void PlayVideo()
    {
        isPlayingPanel.SetActive(true);
        isNotPlayingPanel.SetActive(false);

        videoPlayer.Play();
        videoPlayer.loopPointReached += EndReached;
    }

    public void PauseVideo()
    {
        isPlayingPanel.SetActive(false);
        isNotPlayingPanel.SetActive(true);

        videoPlayer.Pause();
    }

    public void StopVideo()
    {
        videoPlayer.Pause();
        videoPlayer.time = 0;

        menuPanel.SetActive(true);
        isPlayingPanel.SetActive(false);
        isNotPlayingPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SkipForwardVideo()
    {
        videoPlayer.time += 5f;
    }

    public void SkipBackwardVideo()
    {
        videoPlayer.time -= 5f;
    }
}
