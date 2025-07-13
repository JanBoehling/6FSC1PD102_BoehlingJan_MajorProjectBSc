using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IToggleVisibility
{
    public static IEnumerable<IToggleVisibility> Overlays => GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None).OfType<IToggleVisibility>();

    public void ToggleVisibility();
    public void Hide();
    public void Show();

    public static void HideAllOverlays()
    {
        foreach (var item in Overlays)
        {
            item.Hide();
        }
    }
}
