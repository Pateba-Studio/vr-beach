using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "ScriptableObjects/VideoLinkHandler", order = 1)]
public class VideoLinkHandler : ScriptableObject
{
    public bool useVideoFile;
    public Vector3 playerPosTemp;
    public Vector3 playerRotTemp;
    [SerializeField] public YoutubeLinkDetail youtubeLinkDetail;

    public void SetLinkDetails(YoutubeLinkDetail link)
    {
        youtubeLinkDetail = link;
    }

    public YoutubeLinkDetail GetLinkDetails()
    {
        return youtubeLinkDetail;
    }
}