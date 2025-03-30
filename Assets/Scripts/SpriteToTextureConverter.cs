using UnityEngine;
using UnityEngine.UI;

public class SpriteToTextureConverter : MonoBehaviour
{
    private RawImage img;

    private void Awake()
    {
        img = GetComponent<RawImage>();
    }

    public void LoadImage(Sprite sprite)
    {
        if (!sprite) return;

        img.texture = sprite.texture;
        img.SetNativeSize();
    }
}
