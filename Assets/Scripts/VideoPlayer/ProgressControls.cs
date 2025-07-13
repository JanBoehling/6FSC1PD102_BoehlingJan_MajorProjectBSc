using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProgressControls : VideoControlsBase, IPointerDownHandler
{
    protected override void Awake()
    {
        base.Awake();
        _progressBar = GetComponent<Image>();
    }

    private void Update()
    {
        _progressBar.fillAmount = VideoProgress;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Wind(eventData.position);
    }
}
