using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using TMPro;

public class UILibraryDownloader : MonoBehaviour
{
    public BookManager bookManager;

    [Header("File Component")]
    public string fileTitle;
    public string fileUrl;

    [Header("UI Component")]
    public TextMeshProUGUI downloadProgressText;
    public TextMeshProUGUI fileText;
    public Slider downloadBar;
    public Button playButton;

    public void SetupAttribute(string title, string url)
    {
        fileTitle = title;
        fileUrl = url;
    }

    public void OnClickPlayButton()
    {
        if (FileChecker(fileTitle, ".zip"))
        {
            OpenEbookFile();
        }
        else
        {
            FileDownloader();
        }
    }

    public void FileDownloader()
    {
        string detail = $"{Application.persistentDataPath}/Ebook/Cache";
        if (!Directory.Exists(detail)) { Directory.CreateDirectory(detail); }

        FileDownloader fileDownloader = new FileDownloader();
        fileDownloader.DownloadProgressChanged += (sender, e) =>
        {
            downloadProgressText.text = $"{e.BytesReceived * 100 / e.TotalBytesToReceive}%";
            downloadBar.maxValue = e.TotalBytesToReceive;
            downloadBar.value = e.BytesReceived;
        };

        fileDownloader.DownloadFileCompleted += (sender, e) =>
        {
            File.Move(Path.Combine(detail, fileTitle + ".zip"), Path.Combine($"{Application.persistentDataPath}/Ebook", fileTitle + ".zip"));
            downloadBar.value = downloadBar.maxValue;
            OnClickPlayButton();
        };

        fileDownloader.DownloadFileAsync(fileUrl, Path.Combine(detail, fileTitle + ".zip"));
    }

    public void OpenEbookFile()
    {
        string detail = $"{Application.persistentDataPath}/Ebook";
        bookManager.InitializeBook(this, detail, fileTitle, ".zip");
    }

    public bool FileChecker(string name, string extension)
    {
        string detail = $"{Application.persistentDataPath}/Ebook";
        return File.Exists(Path.Combine(detail, name + extension));
    }
}
