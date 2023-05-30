using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class AssetType
{
    public string type;
    public List<string> lists;
}

[Serializable]
public class EbookType
{
    public string type;
    public List<EbookCategory> lists;
}

[Serializable]
public class EbookCategory
{
    public string category;
    public int total;
}

[Serializable]
public class EbookObject
{
    public string type;
    public List<EbookTitle> lists;
}

[Serializable]
public class EbookTitle
{
    public string title;
    public int total;
}

public class APIManager : MonoBehaviour
{
    public static APIManager instance;

    [Header("URLs")]
    public string rootURL;
    public string audiobookDir;
    public string ebookDir;
    public string video2dDir;

    [Header("File Names & Extensions")]
    public string fileDetailName = "detail.json";
    public string videoExt = ".m4v";
    public string ebookExt = ".jpg";
    public string audioExt = ".mp3";

    [Header("Audiobook Details")]
    public AudioLinkList audioLinkList;
    public AudioPlaylistHandler audioPlaylistHandler;
    [SerializeField] public AssetType audiobookDetail;

    [Header("Video 2D Details")]
    public VideoLinkList videoLinkList;
    [SerializeField] public AssetType video2dDetail;

    [Header("Ebook Details")]
    public BookManager bookManager;
    public List<EbookLinkHandler> ebookLinkHandlers;
    [SerializeField] public EbookType ebookDetail;
    [SerializeField] public List<EbookObject> ebookObjects;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        //get datas from audiobook
        StartCoroutine(LoadJSON(StringCombiner(rootURL, StringCombiner(audiobookDir, fileDetailName)), res =>
        {
            audiobookDetail = JsonUtility.FromJson<AssetType>(res.ToString());
            for (int i = 0; i < audiobookDetail.lists.Count; i++)
            {
                AudioLinkDetail obj = new AudioLinkDetail();
                obj.audioUrl = StringCombiner(rootURL, StringCombiner(audiobookDir, audiobookDetail.lists[i] + audioExt));
                obj.audioTitle = audiobookDetail.lists[i];

                audioLinkList.audioLinkDetails.Add(obj);
            }

            audioPlaylistHandler.DisplayAudioPlaylist();
        }));

        //get datas from video
        videoLinkList.playButton.GetComponent<Button>().interactable = false;
        StartCoroutine(LoadJSON(StringCombiner(rootURL, StringCombiner(video2dDir, fileDetailName)), res =>
        {
            video2dDetail = JsonUtility.FromJson<AssetType>(res.ToString());
            for (int i = 0; i < video2dDetail.lists.Count; i++)
            {
                VideoLinkDetail obj = new VideoLinkDetail();
                obj.url = StringCombiner(rootURL, StringCombiner(video2dDir, video2dDetail.lists[i] + videoExt));
                obj.videoTitle = video2dDetail.lists[i];

                videoLinkList.videoLinkDetails.Add(obj);
            }

            videoLinkList.playButton.GetComponent<Button>().interactable = true;
        }));

        //get datas from ebook
        StartCoroutine(LoadJSON(StringCombiner(rootURL, StringCombiner(ebookDir, fileDetailName)), res =>
        {
            ebookDetail = JsonUtility.FromJson<EbookType>(res.ToString());
            for (int i = 0; i < ebookDetail.lists.Count; i++)
            {
                int index = i;
                StartCoroutine(LoadJSON(StringCombiner(rootURL, StringCombiner(ebookDir, StringCombiner(ebookDetail.lists[index].category, fileDetailName))), res =>
                {
                    ebookObjects[index] = JsonUtility.FromJson<EbookObject>(res.ToString());
                    ebookLinkHandlers[index].bookLinkDetails.Clear();

                    for (int i = 0; i < ebookObjects[index].lists.Count; i++)
                    {
                        BookLinkDetail book = new BookLinkDetail();
                        book.fileTitle = ebookObjects[index].lists[i].title;
                        book.totalPage = ebookObjects[index].lists[i].total;
                        book.fileUrl = StringCombiner(rootURL, StringCombiner(ebookDir, StringCombiner(ebookObjects[index].type, ebookObjects[index].lists[i].title)));
                        ebookLinkHandlers[index].bookLinkDetails.Add(book);
                    }

                    bookManager.DisplayBookPlaylist(index, ebookLinkHandlers[index].bookLinkDetails);
                }));
            }
        }));
    }

    public string StringCombiner(string root, string dir)
    {
        return Path.Combine(root, dir).Replace(@"\", @"/");
    }

    private IEnumerator LoadJSON(string url, Action<string> json)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                json(www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Failed to fetch JSON: " + www.error + " from " + url);
            }
        }
    }
}
