using SharpCompress.Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

public class VideoPlaylistHandler : MonoBehaviour
{
    public static VideoPlaylistHandler instance;
    public YoutubeLinkList basicVideoList;
    public YoutubeLinkList threeSixtyVideoList;
    public Transform parentList;
    public GameObject templateList;
    public GameObject videoListPanel;

    [Header("Addon Component For Basic Player")]
    public GameObject basicVideoPlayer;

    [Header("Addon Component For 360 Player")]
    public int lineInteractorLength;
    public int lineInteractorLengthTemp;
    public PositionName posTargetName;
    //public Rigidbody playerRB;
    public VideoLinkHandler threeSixtyLinkHandler;
    public ThreeSixtyVideoPlayer threeSixtyVideoPlayer;
    public Transform threeSixtyPlayerPos;
    public ContinuousMoveProviderBase continuousMove;
    public ContinuousTurnProviderBase continuousTurn;
    public List<XRRayInteractor> xRRayInteractors;
    public List<XRInteractorLineVisual> xRInteractorLineVisuals;

    string rootPath = string.Empty;

    void Awake()
    {
        instance = this;
        rootPath = $"{Application.persistentDataPath}/Video";
    }

    public void ClearList()
    {
        for (int i = 1; i < parentList.childCount; i++)
        {
            Destroy(parentList.GetChild(i).gameObject);
        }
    }

    public IEnumerator InitializeVideoPlayer(bool isUrl, string url, YoutubeLinkDetail fileDetail, VideoClip clip, UnityEvent events)
    {
        videoListPanel.SetActive(false);
        basicVideoPlayer.SetActive(true);
        basicVideoPlayer.GetComponent<BasicVideoPlayer>().isNotPlayingPanel.SetActive(false);
        basicVideoPlayer.GetComponent<BasicVideoPlayer>().isPlayingPanel.SetActive(true);

        if (isUrl)
        {
            basicVideoPlayer.GetComponent<BasicVideoPlayer>().videoPlayer.source = VideoSource.Url;
            basicVideoPlayer.GetComponent<BasicVideoPlayer>().videoPlayer.url = url;
        }
        else
        {
            basicVideoPlayer.GetComponent<BasicVideoPlayer>().videoPlayer.source = VideoSource.VideoClip;
            basicVideoPlayer.GetComponent<BasicVideoPlayer>().videoPlayer.clip = clip;
        }

        basicVideoPlayer.GetComponent<BasicVideoPlayer>().videoPlayer.Play();
        yield return new WaitForSeconds(1f);

        if (!basicVideoPlayer.GetComponent<BasicVideoPlayer>().videoPlayer.isPlaying)
        {
            fileDetail.downloadProgressText.text = "File is Corrupted\nRe-downloading...";
            yield return new WaitForSeconds(1f);
            basicVideoPlayer.SetActive(false);
            videoListPanel.SetActive(true);
            events.Invoke();
        }
        else
        {
            fileDetail.playButton.interactable = true;
        }
    }

    public void DisplayBasicVideoPlaylist()
    {
        ClearList();

        int index = 0;
        foreach (YoutubeLinkDetail list in basicVideoList.youtubeLinkDetails)
        {
            var obj = Instantiate(templateList);
            obj.SetActive(true);

            obj.transform.parent = parentList;
            obj.transform.localPosition = templateList.transform.localPosition;
            obj.transform.localScale = templateList.transform.localScale;
            obj.transform.localRotation = templateList.transform.localRotation;

            list.playButton = obj.GetComponentInChildren<Button>();
            list.downloadProgressText = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            if (!File.Exists(Path.Combine(rootPath, list.videoTitle + ".mp4"))) { list.downloadProgressText.text = "0%"; }
            else { list.downloadProgressText.text = "100%"; }

            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{index + 1} | {list.videoTitle}";
            obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                if (list.isLocalFile &&
                    list.isDownloadable)
                {
                    UnityEvent eventWhenExists = new UnityEvent();
                    UnityEvent eventWhenNotExists = new UnityEvent();

                    string filePath = Path.Combine(rootPath, list.videoTitle + ".mp4");
                    eventWhenExists.AddListener(() => StartCoroutine(InitializeVideoPlayer(true, filePath, list, null, eventWhenNotExists)));
                    eventWhenNotExists.AddListener(() => FileDownloader(list, eventWhenExists));
                    list.downloadProgressText.text = "Please Wait...";
                    list.playButton.interactable = false;

                    if (!File.Exists(Path.Combine(rootPath, list.videoTitle + ".mp4")))
                    {
                        FileDownloader(list, eventWhenExists);
                    }
                    else
                    {
                        StartCoroutine(InitializeVideoPlayer(true, filePath, list, null, eventWhenNotExists));
                    }
                }
                //else if (list.isLocalFile)
                //{
                //    StartCoroutine(InitializeVideoPlayer(false, null, list, list.videoClip, null));
                //}
                //else if (list.isStreaming)
                //{
                //    StartCoroutine(InitializeVideoPlayer(true, list.convertedUrl, list, null, null));
                //}
            });

            index++;
        }
    }

    public void DisplayThreeSixtyVideoPlaylist()
    {
        ClearList();

        int index = 0;
        foreach (YoutubeLinkDetail list in threeSixtyVideoList.youtubeLinkDetails)
        {
            var obj = Instantiate(templateList);
            obj.SetActive(true);

            obj.transform.parent = parentList;
            obj.transform.localPosition = templateList.transform.localPosition;
            obj.transform.localScale = templateList.transform.localScale;
            obj.transform.localRotation = templateList.transform.localRotation;

            list.downloadProgressText = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            list.downloadProgressText.text = string.Empty;

            obj.GetComponentInChildren<TextMeshProUGUI>().text = $"{index + 1} | {list.videoTitle}";
            obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                threeSixtyLinkHandler.useVideoFile = list.isLocalFile;
                threeSixtyLinkHandler.SetLinkDetails(list);

                FindObjectOfType<XROrigin>().transform.position = threeSixtyPlayerPos.position;
                FindObjectOfType<XROrigin>().transform.rotation = threeSixtyPlayerPos.rotation;

                continuousMove.enabled = false;
                continuousTurn.enabled = false;

                threeSixtyVideoPlayer.gameObject.SetActive(true);
                threeSixtyVideoPlayer.SetupPlayer();

                for (int i = 0; i < xRRayInteractors.Count; i++)
                {
                    xRRayInteractors[i].maxRaycastDistance = lineInteractorLength;
                    xRInteractorLineVisuals[i].lineLength = lineInteractorLength;
                }

                CommonScript.instance.position = posTargetName;
                //playerRB.isKinematic = true;
            });

            index++;
        }
    }

    public void FileDownloader(YoutubeLinkDetail fileDetail, UnityEvent events)
    {
        if (!Directory.Exists(rootPath)) { Directory.CreateDirectory(rootPath); }

        FileDownloader fileDownloader = new FileDownloader();
        fileDownloader.DownloadProgressChanged += (sender, e) =>
        {
            fileDetail.downloadProgressText.text = $"{e.BytesReceived * 100 / e.TotalBytesToReceive}%";
        };

        fileDownloader.DownloadFileCompleted += (sender, e) =>
        {
            if (!basicVideoPlayer.GetComponent<BasicVideoPlayer>().videoPlayer.isPlaying &&
                !threeSixtyVideoPlayer.gameObject.activeSelf)
            {
                events.Invoke();
            }
        };

        fileDownloader.DownloadFileAsync(fileDetail.convertedUrl, Path.Combine(rootPath, fileDetail.videoTitle + ".mp4"));
    }
}
