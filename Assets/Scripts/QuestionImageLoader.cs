using UnityEngine;
using UnityEngine.UI;

public class QuestionImageLoader : MonoBehaviour
{
    private RawImage img;

    private void Awake()
    {
        img = GetComponent<RawImage>();
    }

    public void LoadImage(Sprite sprite)
    {
        img.texture = sprite.texture;
        img.SetNativeSize();
    }
}
