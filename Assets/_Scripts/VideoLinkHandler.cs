using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "ScriptableObjects/VideoLinkHandler", order = 1)]
public class VideoLinkHandler : ScriptableObject
{
    public Vector3 playerPosTemp;
    public Vector3 playerRotTemp;
    [SerializeField] public VideoLinkDetail videoLinkDetail;

    public void SetLinkDetails(VideoLinkDetail link)
    {
        videoLinkDetail = link;
    }

    public VideoLinkDetail GetLinkDetails()
    {
        return videoLinkDetail;
    }
}