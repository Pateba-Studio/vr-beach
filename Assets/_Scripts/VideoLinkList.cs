using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public class VideoLinkDetail
{
    public string videoTitle;
    public string url;
    public VideoClip clip;
}

public class VideoLinkList : MonoBehaviour
{
    public GameObject playButton;
    [SerializeField] public List<VideoLinkDetail> videoLinkDetails;
}
