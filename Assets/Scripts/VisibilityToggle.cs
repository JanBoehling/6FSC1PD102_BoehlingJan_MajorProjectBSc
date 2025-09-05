using UnityEngine;

public class VisibilityToggle : MonoBehaviour, IToggleVisibility
{
    public void Hide() => gameObject.SetActive(false);

    public void Show() => gameObject.SetActive(true);

    public void ToggleVisibility()
    {
        if (gameObject.activeSelf) Hide();
        else Show();
    }
}
