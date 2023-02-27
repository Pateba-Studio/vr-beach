using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "ScriptableObjects/EbookLinkHandler", order = 1)]
public class EbookLinkHandler : ScriptableObject
{
    [SerializeField] public List<BookLinkDetail> bookLinkDetails;

    public void SetLinkDetails(List<BookLinkDetail> link)
    {
        bookLinkDetails = link;
    }

    public List<BookLinkDetail> GetLinkDetails()
    {
        return bookLinkDetails;
    }
}
