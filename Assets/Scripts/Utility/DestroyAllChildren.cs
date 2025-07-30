using UnityEngine;

/// <summary>
/// Murders every child of the parent who has this curse applied to them.
/// </summary>
public class DestroyAllChildren : MonoBehaviour
{
    private void OnDisable()
    {
        var children = new GameObject[transform.childCount];

        // Get all children
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }

        // Destroys all children
        foreach (var child in children)
        {
            Destroy(child);
        }
    }
}
