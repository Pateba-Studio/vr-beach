using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using SharpCompress.Archives.Zip;
using SharpCompress.Archives;

[Serializable]
public class BookLinkDetail
{
    [TextArea(3, 3)] public string fileTitle;
    public string fileUrl;
}

public class BookManager : MonoBehaviour
{
    [Header("Book Sheet Component")]
    public int sheetIndex;
    public RectTransform sheetTransform;
    public List<float> posSheetX;

    [Header("Book UI Component")]
    public GameObject templateList;
    [SerializeField] public List<Transform> ebookSheetsOverlay;
    [SerializeField] public List<Transform> ebookSheetsParent;

    [Header("Book Object Component")]
    public int pageBefore = 0;
    public int pageAfter = 1;
    public int minBookScale = 1;
    public int maxBookScale = 2;
    public float scaleFactor;
    public Image leftPage;
    public Image rightPage;
    public GameObject bookMenu;
    public GameObject bookObj;

    [Header("Book Link Component")]
    [SerializeField] public List<EbookLinkHandler> ebookLinkHandlers;
    [SerializeField] public List<BookLinkDetail> bookLinkDetails;
    public List<string> ebookPagePath;

    void Start()
    {
        for (int i = 0; i < ebookLinkHandlers.Count; i++)
        {
            DisplayBookPlaylist(i);
        }
    }

    public void PrevSheet()
    {
        if (sheetIndex > 0)
        {
            sheetIndex--;
            OpenSheet(sheetIndex);
            sheetTransform.anchoredPosition3D = new Vector3(posSheetX[sheetIndex],
                                                            sheetTransform.anchoredPosition3D.y,
                                                            sheetTransform.anchoredPosition3D.z);
        }
    }

    public void NextSheet()
    {
        if (sheetIndex < posSheetX.Count - 1)
        {
            sheetIndex++;
            OpenSheet(sheetIndex);
            sheetTransform.anchoredPosition3D = new Vector3(posSheetX[sheetIndex],
                                                            sheetTransform.anchoredPosition3D.y,
                                                            sheetTransform.anchoredPosition3D.z);
        }
    }

    public void DisplayBookPlaylist(int sheet)
    {
        ClearList(sheet);
        bookLinkDetails.Clear();
        foreach (BookLinkDetail link in ebookLinkHandlers[sheet].bookLinkDetails)
        {
            bookLinkDetails.Add(link);
        }

        int index = 0;
        foreach (BookLinkDetail list in bookLinkDetails)
        {
            var obj = Instantiate(templateList);
            obj.SetActive(true);

            obj.transform.parent = ebookSheetsParent[sheet];
            obj.transform.localPosition = templateList.transform.localPosition;
            obj.transform.localScale = templateList.transform.localScale;
            obj.transform.localRotation = templateList.transform.localRotation;

            obj.GetComponent<UILibraryDownloader>().SetupAttribute(list.fileTitle, list.fileUrl);
            obj.GetComponent<UILibraryDownloader>().fileText.text = $"{index + 1} | {list.fileTitle}";

            if (FileChecker(list.fileTitle, ".zip"))
            {
                obj.GetComponent<UILibraryDownloader>().downloadProgressText.text = "100%";
                obj.GetComponent<UILibraryDownloader>().downloadBar.value = obj.GetComponent<UILibraryDownloader>().downloadBar.maxValue;
            }

            index++;
        }
    }

    public void OpenSheet(int sheet)
    {
        for (int i = 0; i < ebookSheetsOverlay.Count; i++)
        {
            if (i == sheet)
            {
                ebookSheetsOverlay[i].gameObject.SetActive(true);
                ebookSheetsOverlay[i].GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
            }
            else
            {
                ebookSheetsOverlay[i].gameObject.SetActive(false);
            }
        }
    }

    public void ClearList(int sheet)
    {
        for (int i = 1; i < ebookSheetsParent[sheet].childCount; i++)
        {
            Destroy(ebookSheetsParent[sheet].GetChild(i).gameObject);
        }
    }

    public bool FileChecker(string name, string extension)
    {
        string detail = $"{Application.persistentDataPath}/Ebook";
        return File.Exists(Path.Combine(detail, name + extension));
    }

    public bool DirectoryChecker(string name)
    {
        string detail = $"{Application.persistentDataPath}/Ebook";
        return Directory.Exists(Path.Combine(detail, name));
    }

    public IEnumerator Downloader(string name, string url, Image playButton, Action<bool> isDone)
    {
        string detail = $"{Application.persistentDataPath}/Ebook";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest();

        while (!www.isDone)
        {
            playButton.fillAmount = www.downloadProgress;
            yield return null;
        }

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (!Directory.Exists(detail))
            {
                Directory.CreateDirectory(detail);
            }

            if (!File.Exists(Path.Combine(detail, name + ".zip")))
            {
                File.WriteAllBytes($"{detail}/{name}.zip", www.downloadHandler.data);
            }
        }

        isDone(File.Exists(Path.Combine(detail, name + ".zip")));
        if (File.Exists(Path.Combine(detail, name + ".zip")))
        {
            playButton.fillAmount = 1;
        }
    }

    public IEnumerator InitializeBook(UILibraryDownloader handler, string rootPath, string folderName, string extension)
    {
        ebookPagePath.Clear();
        pageBefore = 0;
        pageAfter = 1;

        bool fileCorrupted = false;
        string rarPath = Path.Combine(rootPath, folderName + extension);
        string destinationPath = Path.Combine(rootPath, "Textures");
        if (!DirectoryChecker(destinationPath)) { Directory.CreateDirectory(destinationPath); }

        try
        {
            Debug.Log(folderName + " is not corrupted.");
            DeleteAllFiles(destinationPath);
            UnzipFile(rarPath, destinationPath);
        }
        catch
        {
            Debug.Log(folderName + " is corrupted.");
            fileCorrupted = true;
        }

        if (fileCorrupted)
        {
            handler.downloadProgressText.text = "File is Corrupted\nRe-downloading...";
            yield return new WaitForSeconds(1f);
            handler.downloadProgressText.text = "0%";
            handler.downloadBar.value = 0;
            handler.FileDownloader();
        }
        else
        {
            if (!bookObj.activeSelf)
            {
                var fileInfo = new DirectoryInfo(destinationPath).GetFiles();
                if (fileInfo.Length > 0)
                {
                    GetAllFiles(destinationPath);
                    AssignTextureToBook();

                    bookMenu.SetActive(false);
                    bookObj.SetActive(true);
                    Debug.Log("File opened!");
                }
            }
        }
    }

    public void UnzipFile(string source, string dest)
    {
        var archive = ZipArchive.Open(source);
        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
        {
            entry.WriteToDirectory(dest);
        }

        Debug.Log("File unzipped!");
    }

    public void GetAllFiles(string path)
    {
        var info = new DirectoryInfo(path);
        var fileInfo = info.GetFiles();

        List<string> fileName = new List<string>();
        foreach (FileInfo file in fileInfo)
        {
            fileName.Add(Path.GetFileName(file.FullName));
        }

        List<string> filePath = new List<string>();
        filePath = fileName.OrderBy(o => Convert.ToInt32(o.Split('.')[0])).ToList();
        foreach (string file in filePath)
        {
            ebookPagePath.Add(Path.Combine(path, file));
        }
    }

    public void DeleteAllFiles(string path)
    {
        var info = new DirectoryInfo(path);
        var fileInfo = info.GetFiles();
        foreach (FileInfo file in fileInfo)
        {
            File.Delete(file.FullName);
        }

        Debug.Log("Deleting files...");
    }

    public void PrevPage()
    {
        if (pageBefore - 2 >= 0)
        {
            pageBefore -= 2;
            pageAfter -= 2;
            AssignTextureToBook();
        }
    }

    public void NextPage()
    {
        if (pageAfter + 2 <= ebookPagePath.Count)
        {
            pageBefore += 2;
            pageAfter += 2;
            AssignTextureToBook();
        }
    }

    public Sprite LoadSpriteFromDisk(string filePath)
    {
        byte[] textureBytes = File.ReadAllBytes(filePath);
        Texture2D loadedTexture = new Texture2D(0, 0);
        loadedTexture.LoadImage(textureBytes);
        return Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0, 0));
    }

    public void AssignTextureToBook()
    {
        leftPage.sprite = LoadSpriteFromDisk(ebookPagePath[pageBefore]);
        if (pageAfter < ebookPagePath.Count)
        {
            rightPage.sprite = LoadSpriteFromDisk(ebookPagePath[pageAfter]);
        }
        else
        {
            rightPage.sprite = null;
        }
    }

    public void ScaleUpBook()
    {
        if (bookObj.transform.localScale.x + scaleFactor <= maxBookScale)
        {
            bookObj.transform.localScale += new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }
    public void ScaleDownBook()
    {
        if (bookObj.transform.localScale.x - scaleFactor >= minBookScale)
        {
            bookObj.transform.localScale -= new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }

    public void ResetBookScale()
    {
        bookObj.transform.localScale = Vector3.one;
    }
}
