using UnityEngine;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    private MaskableGraphic imgComponent;

    private void Awake()
    {
        imgComponent = GetComponent<MaskableGraphic>();
    }

    public void LoadImage(Sprite sprite)
    {
        if (!sprite)
        {
            gameObject.SetActive(false);
            return;
        }

        if (imgComponent is Image image)
        {
            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
        }

        else if (imgComponent is RawImage rawImage)
        {
            rawImage.texture = sprite.texture;
            rawImage.SetNativeSize();
        }

        else Debug.LogError($"Error: {name} has neither a Image nor a RawImage component. Instead found: {imgComponent}");

        imgComponent.enabled = true;
    }
}
