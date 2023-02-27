using SharpCompress.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class AudioPlaylistHandler : MonoBehaviour
{
    public bool isProcessing;
    public int currentAudioIndex;
    public AudioLinkList audioLinkList;
    public AudioSource audioSource;
    public Transform parentList;
    public GameObject playerButtonGroup;
    public GameObject templateList;

    [Header("Player Component")]
    public float currentPlayingTime;
    public Button playButton;
    public Button pauseButton;
    public Slider playerSlider;
    public TextMeshProUGUI audioStatusText;
    public TextMeshProUGUI audioTitleText;
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI finishedTimeText;
    public AudioLinkDetail currentAudioDetail;

    string rootPath;

    private void Start()
    {
        rootPath = $"{Application.persistentDataPath}/AudioBook";
        DisplayAudioPlaylist();
    }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            currentPlayingTime = audioSource.time;
            playerSlider.value = currentPlayingTime;

            float minutes = Mathf.Floor(currentPlayingTime / 60);
            float seconds = Mathf.RoundToInt(currentPlayingTime % 60);
            currentTimeText.text = minutes + ":" + seconds;
        }
        else if (currentAudioDetail.audioLength > 0 && !audioSource.isPlaying && 
                 (int)currentPlayingTime >= (int)currentAudioDetail.audioLength - 1)
        {
            playButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);

            audioStatusText.text = "Finished:";
            audioTitleText.text = currentAudioDetail.audioTitle;
        }
    }

    public void PlayAudioBook()
    {
        if (!string.IsNullOrEmpty(currentAudioDetail.audioTitle))
        {
            audioSource.Play();
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
            audioStatusText.text = "Playing:";
            audioTitleText.text = currentAudioDetail.audioTitle;
        }
    }
    
    public void PauseAudioBook()
    {
        if (!string.IsNullOrEmpty(currentAudioDetail.audioTitle))
        {
            audioSource.Pause();
            playButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            audioStatusText.text = "Paused:";
            audioTitleText.text = currentAudioDetail.audioTitle;
        }
    }
    
    public void StopAudioBook()
    {
        if (!string.IsNullOrEmpty(currentAudioDetail.audioTitle))
        {
            audioSource.Stop();
            currentPlayingTime = 0;
            playerSlider.value = 0;
            playButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            audioStatusText.text = "Stopped:";
            audioTitleText.text = currentAudioDetail.audioTitle;
        }
    }

    public void ClearList()
    {
        for (int i = 1; i < parentList.childCount; i++)
        {
            Destroy(parentList.GetChild(i).gameObject);
        }
    }

    public void DisplayAudioPlaylist()
    {
        ClearList();

        int index = 0;
        foreach (AudioLinkDetail list in audioLinkList.audioLinkDetails)
        {
            int i = index;
            var obj = Instantiate(templateList);
            obj.SetActive(true);

            obj.transform.parent = parentList;
            obj.transform.localPosition = templateList.transform.localPosition;
            obj.transform.localScale = templateList.transform.localScale;
            obj.transform.localRotation = templateList.transform.localRotation;

            string filePath = Path.Combine(rootPath, list.audioTitle + ".mp3");
            list.downloadProgressText = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            if (File.Exists(filePath)) { list.downloadProgressText.text = "100%"; }
            else { list.downloadProgressText.text = "0%"; }

            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{index + 1} | {list.audioTitle}";
            obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                if (audioSource.isPlaying) audioSource.Stop();
                AudioProcessing(i, list);
            });

            index++;
        }
    }

    IEnumerator GetAudioClip(AudioLinkDetail detail)
    {
        audioStatusText.text = $"Please Wait...";
        yield return null;

        if (!Directory.Exists(rootPath)) { Directory.CreateDirectory(rootPath); }
        if (!File.Exists(Path.Combine(rootPath, currentAudioDetail.audioTitle + ".mp3")))
        {
            playerButtonGroup.SetActive(true);
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);
            FileDownloader(detail);
        }
        else
        {
            StartCoroutine(PlayAudio(detail));
        }
    }

    public void FileDownloader(AudioLinkDetail detail)
    {
        FileDownloader fileDownloader = new FileDownloader();
        fileDownloader.DownloadProgressChanged += (sender, e) =>
        {
            //audioStatusText.text = $"Processing...";
            detail.downloadProgressText.text = $"{e.BytesReceived * 100 / e.TotalBytesToReceive}%";
            audioTitleText.text = currentAudioDetail.audioTitle;
        };

        fileDownloader.DownloadFileCompleted += (sender, e) =>
        {
            StartCoroutine(PlayAudio(detail));
        };

        fileDownloader.DownloadFileAsync(currentAudioDetail.audioUrl, Path.Combine(rootPath, currentAudioDetail.audioTitle + ".mp3"));
    }

    public IEnumerator PlayAudio(AudioLinkDetail detail)
    {
        isProcessing = false;
        currentAudioDetail = detail;

        string filePath = "file://" + Path.Combine(rootPath, currentAudioDetail.audioTitle + ".mp3");
        using UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip((filePath), AudioType.MPEG);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            playerButtonGroup.SetActive(true);
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);
            Debug.Log(uwr.error);
        }
        else
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(uwr);
            //if (!audioSource.isPlaying)
            //{
                if (audioClip.length > 0)
                {
                    playerButtonGroup.SetActive(true);
                    playButton.gameObject.SetActive(false);
                    pauseButton.gameObject.SetActive(true);

                    audioStatusText.text = "Playing...";
                    audioTitleText.text = currentAudioDetail.audioTitle;

                    float minutes = Mathf.Floor(audioClip.length / 60);
                    float seconds = Mathf.RoundToInt(audioClip.length % 60);
                    finishedTimeText.text = minutes + ":" + seconds;
                    currentAudioDetail.audioLength = audioClip.length;
                    playerSlider.maxValue = audioClip.length;

                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
                else
                {
                    audioStatusText.text = "Verifying...";
                    audioTitleText.text = currentAudioDetail.audioTitle;
                    Debug.Log("Re-downloading file...");
                    FileDownloader(detail);
                }
            //}
        }
    }

    public void NextAudio()
    {
        if (currentAudioIndex + 1 <= audioLinkList.audioLinkDetails.Count - 1)
        {
            int index = currentAudioIndex + 1;
            AudioProcessing(index, audioLinkList.audioLinkDetails[index]);
            audioSource.Stop();
        }
    }

    public void PreviousAudio()
    {
        if (currentAudioIndex - 1 >= 0)
        {
            int index = currentAudioIndex - 1;
            AudioProcessing(index, audioLinkList.audioLinkDetails[index]);
            audioSource.Stop();
        }
    }

    public void AudioProcessing(int index, AudioLinkDetail detail)
    {
        if (isProcessing)
        {
            StopCoroutine(GetAudioClip(null));
            isProcessing = false;
        }

        currentAudioIndex = index;
        currentAudioDetail = detail;
        playerButtonGroup.SetActive(false);
        StartCoroutine(GetAudioClip(detail));
    }
}
