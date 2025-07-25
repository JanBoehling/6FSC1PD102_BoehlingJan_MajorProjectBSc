using System;
using TMPro;
using UnityEngine;

public class BookmarkSearchHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField _searchInputField;
    [SerializeField] private BookmarkHandler _bookmarkHandler;

    /// <summary>
    /// Searches the bookmarks in the BookmarkHander field for the string in the search input field and jumps to it
    /// </summary>
    public void Search() => Search(_searchInputField.text);

    /// <summary>
    /// Searches the bookmarks in the BookmarkHander field for the string in the search input field and jumps to it
    /// </summary>
    public void Search(string query)
    {
        if (!_bookmarkHandler)
        {
            Debug.LogErrorFormat("{0}: Bookmark Handler could not be found.", name);
            return;
        }

        int index = _bookmarkHandler.GetBookmarkIndexByID(query.TrimEnd());

        if (index == -1)
        {
            _searchInputField.image.color = Color.red;
            return;
        }
        else _searchInputField.image.color = Color.white;

        _bookmarkHandler.JumpTo(_bookmarkHandler.Bookmarks[index].Element);
    }
}
