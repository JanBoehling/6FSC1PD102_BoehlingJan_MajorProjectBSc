using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BookmarkHandler : MonoBehaviour, IPointerClickHandler
{
    [field: SerializeField] public Bookmark[] Bookmarks { get; private set; }
    
    private ScrollRect _scrollRect;

    private void OnEnable() => _scrollRect = FindFirstObjectByType<ScrollRect>();

    public void OnPointerClick(PointerEventData eventData)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).TryGetComponent<TMP_Text>(out var item)) continue;

            int bookmarkIndex = FindBookmark(item, eventData.pointerPressRaycast.worldPosition);
            if (bookmarkIndex == -1) continue;

            JumpTo(Bookmarks[bookmarkIndex].Element);
            break;
        }
    }

    /// <summary>
    /// Retrieves the link at the given position and checks if a bookmark for that link exists.<br/> A link is marked with &lt;link="ID"&gt;&lt;/link&gt;
    /// </summary>
    /// <param name="textElement">The text GameObject</param>
    /// <param name="position">the position of the click</param>
    /// <param name="bookmarkIndex">The index in the bookmarks array. -1 if no bookmark was found.</param>
    /// <returns>True, if a bookmark was found, otherwise false</returns>
    private int FindBookmark(TMP_Text textElement, Vector3 position)
    {
        int wordIndex = TMP_TextUtilities.FindIntersectingLink(textElement, position, null);
        
        if (wordIndex == -1) return -1;

        string id = textElement.textInfo.linkInfo[wordIndex].GetLinkID();

        return GetBookmarkIndexByID(id);
    }

    /// <summary>
    /// Scrolls to the given RectTransform
    /// </summary>
    /// <param name="target">The RectTransform that should be scrolled to</param>
    public void JumpTo(RectTransform target)
    {
        var rectTransform = transform as RectTransform;

        Canvas.ForceUpdateCanvases();

        var jumpPos = (Vector2)_scrollRect.transform.InverseTransformPoint(rectTransform.position) - (Vector2)_scrollRect.transform.InverseTransformPoint(target.position);

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, jumpPos.y);
    }

    /// <summary>
    /// Searches all bookmarks for the given id
    /// </summary>
    /// <param name="id">The ID of the desired bookmark</param>
    /// <returns>The index of the bookmark, or -1 if not found</returns>
    public int GetBookmarkIndexByID(string id)
    {
        for (int i = 0; i < Bookmarks.Length; i++)
        {
            if (!Bookmarks[i].ID.Equals(id, System.StringComparison.InvariantCultureIgnoreCase)) continue;

            return i;
        }

        return -1;
    }
}

[System.Serializable]
public struct Bookmark
{
    [field:SerializeField] public string ID { get; private set; }
    [field:SerializeField] public RectTransform Element { get; private set; }
}
