using UnityEngine;
using TMPro;

public class LeaderboardController : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _nameTexts;
    [SerializeField] private TMP_Text[] _levelTexts;

    // The size of each entry in the database data
    private const int EntrySize = 2;

    // The max amount of entries times the size of an entry. The range needs to be multiplied because one entry takes up multiple slots in the array
    private const int MaxEntryCount = 10 * EntrySize;

    private void Start()
    {
        // Resets every text element
        foreach (var item in _nameTexts) item.text = "";
        foreach (var item in _levelTexts) item.text = "";
    }

    // Start
    private void OnEnable()
    {
        DB.Instance.Query(ShowEntries, "SELECT username, XP FROM UserData WHERE XP > 0 ORDER BY XP DESC");
    }

    /// <summary>
    /// Loops through database data and displays name and xp
    /// </summary>
    /// <param name="entries">The data from the DB. Every odd index is the name and every even index is the xp</param>
    private void ShowEntries(string[] entries)
    {
        // This is a wild way to use a for loop xD
        for (int i = 0, j = 0; (entries.Length <= MaxEntryCount && j < entries.Length) || (entries.Length > MaxEntryCount && j < MaxEntryCount); i++, j += EntrySize)
        {
            // Break if there are no more entries
            if (string.IsNullOrWhiteSpace(entries[j])) break;

            _nameTexts[i].text = entries[j];
            _levelTexts[i].text = entries[j + 1];
        }
    }
}
