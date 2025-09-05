using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IToggleVisibility
{
    public static IEnumerable<IToggleVisibility> Overlays => Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).OfType<IToggleVisibility>();

    /// <summary>
    /// Toggles the visibility of the implementing view
    /// </summary>
    public void ToggleVisibility();

    /// <summary>
    /// Deactivates the visibility of the implementing view
    /// </summary>
    public void Hide();

    /// <summary>
    /// Activates the visibility of the implementing view
    /// </summary>
    public void Show();

    /// <summary>
    /// Hides all overlay views
    /// </summary>
    public static void HideAllOverlays()
    {
        foreach (var item in Overlays)
        {
            item.Hide();
        }
    }

    /// <summary>
    /// Shows all overlay views
    /// </summary>
    public static void ShowAllOverlays()
    {
        foreach (var item in Overlays)
        {
            item.Show();
        }
    }
}
