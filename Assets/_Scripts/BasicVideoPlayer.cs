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

    public void SetupPlayer(string url)
    {
        menuPanel.SetActive(false);
        gameObject.SetActive(true);
        videoPlayer.url = url;
        PlayVideo();
    }

    void EndReached(VideoPlayer vp)
    {
        StopVideo();
    }

    public void PlayVideo()
    {
        videoPlayer.Play();
        isPlayingPanel.SetActive(true);
        isNotPlayingPanel.SetActive(false);
        videoPlayer.loopPointReached += EndReached;
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
        isPlayingPanel.SetActive(false);
        isNotPlayingPanel.SetActive(true);
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
        menuPanel.SetActive(true);
        isPlayingPanel.SetActive(true);
        isNotPlayingPanel.SetActive(false);
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
