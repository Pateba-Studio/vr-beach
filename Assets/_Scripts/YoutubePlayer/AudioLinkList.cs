using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class AudioLinkDetail
{
    public string audioTitle;
    public string audioUrl;
    public float audioLength;
    public TextMeshProUGUI downloadProgressText;
}

public class AudioLinkList : MonoBehaviour
{
    [SerializeField] public List<AudioLinkDetail> audioLinkDetails;
}