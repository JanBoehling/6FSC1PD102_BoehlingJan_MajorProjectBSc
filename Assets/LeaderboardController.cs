using System.Linq;
using TMPro;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _nameTexts;
    [SerializeField] private TMP_Text[] _levelTexts;

    // Start
    private void OnEnable()
    {
        DB.Instance.Query(ShowEntries, "SELECT username, XP FROM UserData ORDER BY XP DESC");
    }

    private void ShowEntries(string[] entriesRaw)
    {
        // Trim entries to top 10 (10 names, 10 values)
        System.Array.Resize(ref entriesRaw, 20);

        var entries = new (string name, string XP)[10];

        // Structure raw data into entries
        int i = 0;
        for (int j = 0; i < entriesRaw.Length; j += 2)
        {
            // No more entries
            if (string.IsNullOrWhiteSpace(entriesRaw[j])) break;

            entries[i].name = entriesRaw[j];
            entries[i].XP = entriesRaw[j + 1];
            i++;
        }

        // Display entries
        for (int j = 0; j < entries.Length; j++)
        {
            _nameTexts[j].text = entries[j].name;
            _levelTexts[j].text = entries[j].XP;
        }
    }
}
