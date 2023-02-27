//using Shubham.PDF;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class PDFHandler : MonoBehaviour
{
    //    public string path;
    //    public TMP_Dropdown dropdownPath;
    //    public TextMeshProUGUI pathText;
    //    public List<string> folderName;

    //    [Header("File And Texture Result")]
    //    public List<string> fileNameBefore;
    //    public List<string> fileNameAfter;
    //    public List<Texture2D> fileTexture;

    //    [Header("Book Components")]
    //    public int pageBefore = 0;
    //    public int pageAfter = 1;
    //    public Image leftPage;
    //    public Image rightPage;

    //    void Start()
    //    {
    //        BetterStreamingAssets.Initialize();
    //        List<string> folders = BetterStreamingAssets.GetFiles(path, "*", SearchOption.AllDirectories)
    //                                .Select(x => Path.GetDirectoryName(x))
    //                                .Distinct()
    //                                .ToList();

    //        foreach (string dir in folders)
    //        {
    //            string name = dir.Replace('\\', '/');
    //            string[] nameResult = name.Split('/');
    //            folderName.Add(nameResult[nameResult.Length - 1]);
    //        }

    //        dropdownPath.ClearOptions();
    //        dropdownPath.AddOptions(folderName);
    //        DropdownValueChanged(dropdownPath);
    //    }

    //    public void DropdownValueChanged(TMP_Dropdown change)
    //    {
    //        fileNameBefore = BetterStreamingAssets.GetFiles($"{path}/{folderName[change.value]}", "*").ToList();
    //        fileNameAfter = fileNameBefore.OrderBy(o => Convert.ToInt32(o.Split('(')[1].Split(')')[0])).ToList();

    //        pathText.text = "Files Detected:\n";
    //        for (int i = 0; i < fileNameAfter.Count; i++)
    //        {
    //            string[] name = fileNameAfter[i].Split('/');
    //            fileNameAfter[i] = name[name.Length - 1];
    //            pathText.text += fileNameAfter[i] + "\n";
    //        }

    //        pageBefore = 0;
    //        pageAfter = 1;
    //        AssignTextureToBook();
    //    }

    //    public void PrevPage()
    //    {
    //        //if (pageBefore - 2 >= 0)
    //        //{
    //        //    pageBefore -= 2;
    //        //    pageAfter -= 2;
    //        //    AssignTextureToPage();
    //        //}

    //        if (pageBefore - 2 >= 0)
    //        {
    //            pageBefore -= 2;
    //            pageAfter -= 2;
    //            AssignTextureToBook();
    //        }
    //    }

    //    public void NextPage()
    //    {
    //        //if (pageAfter + 2 <= fileTexture.Count)
    //        //{
    //        //    pageBefore += 2;
    //        //    pageAfter += 2;
    //        //    AssignTextureToPage();
    //        //}

    //        if (pageAfter + 2 <= fileNameAfter.Count)
    //        {
    //            pageBefore += 2;
    //            pageAfter += 2;
    //            AssignTextureToBook();
    //        }
    //    }

    //    public void AssignTextureToPage()
    //    {
    //        leftPage.sprite = Sprite.Create(fileTexture[pageBefore], new Rect(0, 0, fileTexture[pageBefore].width, fileTexture[pageBefore].height), new Vector2(0, 0));
    //        if (pageAfter < fileTexture.Count)
    //        {
    //            rightPage.sprite = Sprite.Create(fileTexture[pageAfter], new Rect(0, 0, fileTexture[pageAfter].width, fileTexture[pageAfter].height), new Vector2(0, 0));
    //        }
    //        else
    //        {
    //            rightPage.sprite = null;
    //        }
    //    }

    //    public void AssignTextureToBook()
    //    {
    //        leftPage.sprite = LoadSpriteFromDisk(fileNameAfter[pageBefore]);
    //        if (pageAfter < fileNameAfter.Count)
    //        {
    //            rightPage.sprite = LoadSpriteFromDisk(fileNameAfter[pageAfter]);
    //        }
    //        else
    //        {
    //            rightPage.sprite = null;
    //        }
    //    }

    //    public void LoadImageFromDisk()
    //    {
    //        fileTexture.Clear();
    //        foreach (string file in fileNameAfter)
    //        {
    //            byte[] textureBytes = BetterStreamingAssets.ReadAllBytes($"{path}/{folderName[dropdownPath.value]}/{file}");
    //            Texture2D loadedTexture = new Texture2D(0, 0);
    //            loadedTexture.LoadImage(textureBytes);
    //            fileTexture.Add(loadedTexture);
    //        }

    //        AssignTextureToPage();
    //    }

    //    public Sprite LoadSpriteFromDisk(string fileName)
    //    {
    //        Texture2D loadedTexture = new Texture2D(0, 0);
    //        byte[] textureBytes = BetterStreamingAssets.ReadAllBytes($"{path}/{folderName[dropdownPath.value]}/{fileName}");

    //        loadedTexture.LoadImage(textureBytes);
    //        return Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0, 0));
    //    }
}
