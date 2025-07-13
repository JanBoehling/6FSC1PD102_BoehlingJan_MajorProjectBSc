using UnityEngine;

public class ProfileController : MonoBehaviour, IToggleVisibility
{
    private TopIslandController _topIsland;

    private void Awake() => _topIsland = FindFirstObjectByType<TopIslandController>();

    public void Hide() => gameObject.SetActive(false);

    public void Show() => gameObject.SetActive(true);

    public void ToggleVisibility() => gameObject.SetActive(!gameObject.activeInHierarchy);

    public void DisplayUserName() => _topIsland.DisplayTitle(CurrentUser.Data.UserName);
}
