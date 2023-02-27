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
    [SerializeField] public YoutubeLinkDetail linkDetail;

    public void SetupPlayer()
    {
        linkDetail = videoLinkHandler.GetLinkDetails();
        if (videoLinkHandler.useVideoFile)
        {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = linkDetail.videoClip;
        }
        else
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = linkDetail.convertedUrl;
        }
    }

    void EndReached(VideoPlayer vp)
    {
        StopVideo();
    }

    public void PlayVideo()
    {
        isPlayingPanel.SetActive(true);
        isNotPlayingPanel.SetActive(false);
        videoPlayer.gameObject.SetActive(true);

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
        videoPlayer.Stop();
        videoPlayer.time = 0f;

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
