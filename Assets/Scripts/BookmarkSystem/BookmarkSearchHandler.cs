using TMPro;
using UnityEngine;

public class BookmarkSearchHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField _searchInputField;
    [SerializeField] private BookmarkHandler _bookmarkHandler;

    /// <summary>
    /// Searches the bookmarks in the BookmarkHander field for the string in the search input field and jumps to it
    /// </summary>
    public void Search()
    {
        int index = _bookmarkHandler.GetBookmarkIndexByID(_searchInputField.text.TrimEnd());
        
        if (index == -1)
        {
            _searchInputField.image.color = Color.red;
            return;
        }
        else _searchInputField.image.color = Color.white;

        _bookmarkHandler.JumpTo(_bookmarkHandler.Bookmarks[index].Element);
    }
}
