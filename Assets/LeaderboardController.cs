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
        (string name, uint XP) entries;

        // for loopen und spitten (jeder ungerader index name, jeder gerade index xp)
    }
}
