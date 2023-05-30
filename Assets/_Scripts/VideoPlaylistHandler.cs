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
    public VideoLinkList basicVideoList;
    public VideoLinkList threeSixtyVideoList;
    public Transform parentList;
    public GameObject templateList;
    public GameObject videoListPanel;

    [Header("Addon Component For Basic Player")]
    public BasicVideoPlayer basicVideoPlayer;

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

    void Awake()
    {
        instance = this;
    }

    public void ClearList()
    {
        for (int i = 1; i < parentList.childCount; i++)
        {
            Destroy(parentList.GetChild(i).gameObject);
        }
    }

    public void DisplayBasicVideoPlaylist()
    {
        ClearList();

        int index = 0;
        foreach (VideoLinkDetail list in basicVideoList.videoLinkDetails)
        {
            var obj = Instantiate(templateList);
            obj.SetActive(true);

            obj.transform.parent = parentList;
            obj.transform.localPosition = templateList.transform.localPosition;
            obj.transform.localScale = templateList.transform.localScale;
            obj.transform.localRotation = templateList.transform.localRotation;

            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{index + 1} | {list.videoTitle}";
            obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                videoListPanel.SetActive(false);
                basicVideoPlayer.SetupPlayer(list.url);
            });

            index++;
        }
    }

    public void DisplayThreeSixtyVideoPlaylist()
    {
        ClearList();

        int index = 0;
        foreach (VideoLinkDetail list in threeSixtyVideoList.videoLinkDetails)
        {
            var obj = Instantiate(templateList);
            obj.SetActive(true);

            obj.transform.parent = parentList;
            obj.transform.localPosition = templateList.transform.localPosition;
            obj.transform.localScale = templateList.transform.localScale;
            obj.transform.localRotation = templateList.transform.localRotation;

            obj.GetComponentInChildren<TextMeshProUGUI>().text = $"{index + 1} | {list.videoTitle}";
            obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
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
}
