using UnityEngine;
using TMPro;

public class LeaderboardController : MonoBehaviour
{
    [SerializeField] private RectTransform _nameColumn;
    [SerializeField] private RectTransform _levelColumn;

    // The size of each entry in the database data
    private const int EntrySize = 2;

    // The max amount of entries times the size of an entry. The range needs to be multiplied because one entry takes up multiple slots in the array
    private const int MaxEntryCount = 10 * EntrySize;

    private TMP_Text[] _nameTexts;
    private TMP_Text[] _levelTexts;

    private void Awake()
    {
        _nameTexts = _nameColumn.GetComponentsInChildren<TMP_Text>(true);
        _levelTexts = _levelColumn.GetComponentsInChildren<TMP_Text>(true);
    }

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
            _nameColumn.GetChild(i).gameObject.SetActive(true);
            _levelColumn.GetChild(i).gameObject.SetActive(true);

            _nameTexts[i].text = entries[j];
            _levelTexts[i].text = entries[j + 1];
        }
    }
}
