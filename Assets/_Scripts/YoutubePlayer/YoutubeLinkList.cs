using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine;
using YoutubePlayer;
using TMPro;

[Serializable]
public class YoutubeLinkDetail
{
    public string videoTitle;
    public string oriUrl;
    public string convertedUrl;
    public VideoClip videoClip;
    public TextMeshProUGUI downloadProgressText;
    public bool isStreaming;
    public bool isDownloadable;
    public bool isLocalFile;
}

public class YoutubeLinkList : MonoBehaviour
{
    public int index;
    public bool needConvert;
    public InvidiousPlayer invidiousPlayer;
    public GameObject playButton;
    [SerializeField] public List<YoutubeLinkDetail> youtubeLinkDetails;

    // Start is called before the first frame update
    void Start()
    {
        playButton.GetComponent<Button>().interactable = false;
        StartCoroutine(GetYoutubeDetails());
    }

    public IEnumerator GetYoutubeDetails()
    {
        int temp = index;
        if (needConvert) 
        {
            AsyncToCallbackAsync();
        }
        else
        {
            youtubeLinkDetails[index].convertedUrl = youtubeLinkDetails[index].oriUrl;
            index++;
        }

        yield return new WaitUntil(() => temp != index);

        if (index < youtubeLinkDetails.Count) 
        {
            StartCoroutine(GetYoutubeDetails()); 
        }
        else
        {
            playButton.GetComponent<Button>().interactable = true;
        }
    }

    private async Task AsyncToCallbackAsync()
    {
        string[] id = youtubeLinkDetails[index].oriUrl.Split('=');
        invidiousPlayer.videoId = id[1];

        await invidiousPlayer.PrepareVideoAsync();
        youtubeLinkDetails[index].convertedUrl = invidiousPlayer.linkResult;
        index++;
    }
}
