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
using TMPro;

[Serializable]
public class BookLinkDetail
{
    [TextArea(3, 3)] public string fileTitle;
    public string fileUrl;
    public int totalPage;
}

public class BookManager : MonoBehaviour
{
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

    [Header("Book Sheet Component")]
    public int sheetIndex;
    public Button prevButton;
    public Button nextButton;
    public RectTransform sheetTransform;
    public GameObject templateList;
    public List<float> posSheetX;
    [SerializeField] public List<Transform> ebookSheetsOverlay;
    [SerializeField] public List<Transform> ebookSheetsParent;

    [Header("Current Book Component")]
    public int currBookTotalPage;
    public string currBookRootURL;
    [SerializeField] public List<EbookLinkHandler> ebookLinkHandlers;

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

    public void DisplayBookPlaylist(int index, List<BookLinkDetail> books)
    {
        int i = 1;
        foreach (BookLinkDetail list in books)
        {
            var obj = Instantiate(templateList);
            obj.SetActive(true);

            obj.transform.parent = ebookSheetsParent[index];
            obj.transform.localPosition = templateList.transform.localPosition;
            obj.transform.localScale = templateList.transform.localScale;
            obj.transform.localRotation = templateList.transform.localRotation;

            obj.GetComponentInChildren<TextMeshProUGUI>().text = i + " | " + list.fileTitle;
            obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                prevButton.interactable = false;
                nextButton.interactable = false;

                currBookRootURL = list.fileUrl;
                currBookTotalPage = list.totalPage;

                bookMenu.SetActive(false);
                bookObj.SetActive(true);
                StartCoroutine(AssignPageSprite());
            });
 
            i++;
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

    public void ResetBookPage()
    {
        prevButton.interactable = false;
        nextButton.interactable = false;

        leftPage.sprite = null;
        rightPage.sprite = null;
        
        pageBefore = 1;
        pageAfter = 2;
    }

    public void PrevPage()
    {
        if (pageBefore - 2 >= 0)
        {
            prevButton.interactable = false;
            nextButton.interactable = false;

            pageBefore -= 2;
            pageAfter -= 2;

            StartCoroutine(AssignPageSprite());
        }
    }

    public void NextPage()
    {
        if (pageAfter + 2 <= currBookTotalPage)
        {
            prevButton.interactable = false;
            nextButton.interactable = false;

            pageBefore += 2;
            pageAfter += 2;

            StartCoroutine(AssignPageSprite());
        }
    }

    public IEnumerator AssignPageSprite()
    {
        StartCoroutine(LoadImageFromUrl(StringCombiner(currBookRootURL, $"{pageBefore}{APIManager.instance.ebookExt}"), res =>
        {
            leftPage.sprite = res;
        }));

        StartCoroutine(LoadImageFromUrl(StringCombiner(currBookRootURL, $"{pageAfter}{APIManager.instance.ebookExt}"), res =>
        {
            rightPage.sprite = res;
        }));

        yield return new WaitUntil(() => leftPage.sprite != null && rightPage.sprite != null);
        prevButton.interactable = true;
        nextButton.interactable = true;
    }

    public string StringCombiner(string root, string dir)
    {
        return Path.Combine(root, dir).Replace(@"\", @"/");
    }

    IEnumerator LoadImageFromUrl(string imageURL, Action<Sprite> sprite)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error downloading image: " + webRequest.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                sprite(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero));
            }
        }
    }
}
