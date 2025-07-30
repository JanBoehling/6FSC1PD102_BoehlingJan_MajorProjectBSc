using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class ImageLoader : MonoBehaviour
{
    private MaskableGraphic _imgComponent;

    private void Awake() => _imgComponent = GetComponent<MaskableGraphic>();

    /// <summary>
    /// Loads a sprite and either sets it in the image component or converts it to a texture and sets it in the raw image component
    /// </summary>
    /// <param name="sprite"></param>
    public void LoadImage(Sprite sprite)
    {
        if (!sprite)
        {
            gameObject.SetActive(false);
            return;
        }

        if (_imgComponent is Image image)
        {
            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
        }

        else if (_imgComponent is RawImage rawImage)
        {
            rawImage.texture = sprite.texture;
            rawImage.SetNativeSize();
        }

#if UNITY_EDITOR
        else Debug.LogError($"Error: {name} has neither a Image nor a RawImage component. Instead found: {_imgComponent}");
#endif

        _imgComponent.enabled = true;
    }
}
