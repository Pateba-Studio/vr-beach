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
    public Button prevButton;
    public Button nextButton;
    public Slider playerSlider;
    public TextMeshProUGUI audioStatusText;
    public TextMeshProUGUI audioTitleText;
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI finishedTimeText;
    public AudioLinkDetail currentAudioDetail;

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

    public void SetupAudioBook(int index, AudioLinkDetail list)
    {
        currentAudioIndex = index;
        currentAudioDetail = list;
        audioStatusText.text = "Loading...";
        audioTitleText.text = currentAudioDetail.audioTitle;
        playerButtonGroup.SetActive(false);

        prevButton.interactable = true;
        nextButton.interactable = true;
        if (currentAudioIndex == 0) prevButton.interactable = false;
        if (currentAudioIndex == audioLinkList.audioLinkDetails.Count - 1) nextButton.interactable = false;

        if (audioSource.isPlaying)
            audioSource.Stop();

        StartCoroutine(PlayAudioFromUrl());
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

            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{index + 1} | {list.audioTitle}";
            obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                SetupAudioBook(i, list);
            });

            index++;
        }
    }

    public void ClearList()
    {
        for (int i = 1; i < parentList.childCount; i++)
        {
            Destroy(parentList.GetChild(i).gameObject);
        }
    }

    public void NextAudio()
    {
        audioSource.Stop();

        int index = currentAudioIndex + 1;
        currentAudioDetail = audioLinkList.audioLinkDetails[index];
        SetupAudioBook(index, currentAudioDetail);
    }

    public void PreviousAudio()
    {
        audioSource.Stop();

        int index = currentAudioIndex - 1;
        currentAudioDetail = audioLinkList.audioLinkDetails[index];
        SetupAudioBook(index, currentAudioDetail);
    }

    IEnumerator PlayAudioFromUrl()
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(currentAudioDetail.audioUrl, AudioType.MPEG);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
            if (audioClip.length > 0)
            {
                playerButtonGroup.SetActive(true);
                playButton.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(true);

                audioStatusText.text = "Playing...";
                finishedTimeText.text = Mathf.Floor(audioClip.length / 60) + ":" + Mathf.RoundToInt(audioClip.length % 60);
                currentAudioDetail.audioLength = audioClip.length;
                playerSlider.maxValue = audioClip.length;

                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
        else
        {
            audioStatusText.text = "Re-Tyring...";
            StartCoroutine(PlayAudioFromUrl());
        }
    }
}
