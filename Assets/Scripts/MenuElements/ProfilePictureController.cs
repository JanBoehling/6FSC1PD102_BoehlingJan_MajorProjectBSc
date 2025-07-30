using UnityEngine;
using UnityEngine.UI;

public class ProfilePictureController : MonoBehaviour
{
    [SerializeField] private Sprite[] _profilePictures;

    public Sprite[] ProfilePictures => _profilePictures;

    private Image _profilePictureImage;

    private void Awake() => _profilePictureImage = GetLowestChild(transform).GetComponent<Image>();

    private void OnEnable() => _profilePictureImage.sprite = _profilePictures[CurrentUser.ProfilePictureIndex];

    /// <summary>
    /// Recursively gets the lowest child of the given transform
    /// </summary>
    /// <param name="parent">The parent of which the lowest child should be retrieved</param>
    /// <returns>The transform of the lowest child</returns>
    private Transform GetLowestChild(Transform parent) => parent.childCount == 0 ? parent : GetLowestChild(parent.GetChild(0));

    /// <summary>
    /// Sets the users profile picture based on the given index
    /// </summary>
    /// <param name="profilePictureIndex"></param>
    public void SetProfilePicture(uint profilePictureIndex)
    {
        if (profilePictureIndex >= _profilePictures.Length) return;
        CurrentUser.SetProfilePicture(() => _profilePictureImage.sprite = _profilePictures[profilePictureIndex], profilePictureIndex);
    }

    /// <summary>
    /// Toggles through the profile picture carousel
    /// </summary>
    public void ToggleProfilePictureSelection() => SetProfilePicture(++CurrentUser.Data.ProfilePictureIndex >= _profilePictures.Length ? 0 : CurrentUser.ProfilePictureIndex);
}
