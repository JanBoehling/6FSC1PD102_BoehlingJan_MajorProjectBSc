using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProfilePictureController : MonoBehaviour
{
    [SerializeField] private Sprite[] _profilePictures;
    public Sprite[] ProfilePictures => _profilePictures;

    private Image _profilePictureImage;

    private void Awake() => _profilePictureImage = GetLowestChild(transform).GetComponent<Image>();

    private void OnEnable() => _profilePictureImage.sprite = _profilePictures[CurrentUser.ProfilePictureIndex];

    private Transform GetLowestChild(Transform parent) => parent.childCount == 0 ? parent : GetLowestChild(parent.GetChild(0));

    public void SetProfilePicture(uint profilePictureIndex)
    {
        if (profilePictureIndex >= _profilePictures.Length) return;
        CurrentUser.SetProfilePicture(() => _profilePictureImage.sprite = _profilePictures[profilePictureIndex], profilePictureIndex);
    }

    public void ToggleProfilePictureSelection() => SetProfilePicture(++CurrentUser.Data.ProfilePictureIndex >= _profilePictures.Length ? 0 : CurrentUser.ProfilePictureIndex);
}

#if UNITY_EDITOR
[CustomEditor(typeof(ProfilePictureController))]
public class ProfilePictureControllerEditor : Editor
{
    private ProfilePictureController _controller;
    private int _profilePictureIndex;

    private string[] _profilePicNames;
    private int[] _profilePicIndices;

    private void OnEnable()
    {
        _controller = (ProfilePictureController)target;

        _profilePicNames = new string[_controller.ProfilePictures.Length];
        _profilePicIndices = new int[_controller.ProfilePictures.Length];

        for (int i = 0; i < _profilePicIndices.Length; i++)
        {
            _profilePicNames[i] = _controller.ProfilePictures[i].name;
            _profilePicIndices[i] = i;
        }

        if (CurrentUser.Data != null) _profilePictureIndex = (int)CurrentUser.ProfilePictureIndex;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GUI.enabled = Application.isPlaying;

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();

        _profilePictureIndex = EditorGUILayout.IntPopup("Profile Picture", _profilePictureIndex, _profilePicNames, _profilePicIndices);
        if (GUILayout.Button("Set Profile Picture")) _controller.SetProfilePicture((uint)_profilePictureIndex);

        EditorGUILayout.EndHorizontal();
    }
}
#endif
