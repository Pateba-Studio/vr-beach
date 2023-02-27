using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class LinkDetail
{
    [TextArea(3,3)] public string fileTitle;
    public string fileUrl;
    public string fileDir;
    public bool isDownloaded;
}

public class DownloadManager : MonoBehaviour
{
    public string rootVideoFolder;
    public string rootAudioFolder;
    public string rootEbookFolder;

    [Header("Asset Links")]
    [SerializeField] public List<LinkDetail> videoList;
    [SerializeField] public List<LinkDetail> audioList;
    [SerializeField] public List<LinkDetail> ebookList;

    public void VideoDownloader()
    {
        foreach (LinkDetail detail in videoList)
        {
            StartCoroutine(Downloader(rootVideoFolder, detail.fileTitle, detail.fileUrl));
        }
    }
    
    public void AudioDownloader()
    {
        foreach (LinkDetail detail in audioList)
        {
            StartCoroutine(Downloader(rootAudioFolder, detail.fileTitle, detail.fileUrl));
        }
    }
    
    public void EbookDownloader()
    {
        foreach (LinkDetail detail in ebookList)
        {
            StartCoroutine(Downloader(rootEbookFolder, detail.fileTitle, detail.fileUrl));
        }
    }

    public IEnumerator Downloader(string root, string name, string url)
    {
        Debug.Log($"Downloading: {name}");
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string detail = $"{Application.persistentDataPath}/{root}";
            if (!Directory.Exists(detail))
            {
                Directory.CreateDirectory(detail);
            }

            if (!File.Exists(Path.Combine(detail, name)))
            {
                File.WriteAllBytes($"{detail}/{name}", www.downloadHandler.data);
                Debug.Log($"Downloaded: {name} in {detail}");
            }
        }
    }
}
