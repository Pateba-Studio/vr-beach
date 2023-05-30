using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ThreeSixtyVideoPlayer : MonoBehaviour
{
    public VideoLinkHandler videoLinkHandler;
    public VideoPlayer videoPlayer;
    public GameObject isPlayingPanel;
    public GameObject isNotPlayingPanel;
    [SerializeField] public VideoLinkDetail linkDetail;

    public void SetupPlayer()
    {
        linkDetail = videoLinkHandler.GetLinkDetails();
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = linkDetail.clip;
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
        videoPlayer.gameObject.SetActive(true);
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
        isPlayingPanel.SetActive(false);
        isNotPlayingPanel.SetActive(true);
    }

    public void SkipForwardVideo()
    {
        videoPlayer.time += 5;
    }

    public void SkipBackwardVideo()
    {
        videoPlayer.time -= 5f;
    }
}
