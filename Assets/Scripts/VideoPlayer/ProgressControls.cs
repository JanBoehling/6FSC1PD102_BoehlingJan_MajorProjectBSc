using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProgressControls : VideoControlsBase, IPointerDownHandler
{
    protected override void Awake()
    {
        base.Awake();
        ProgressBar = GetComponent<Image>();
    }

    private void Update() => ProgressBar.fillAmount = VideoProgress;

    public void OnPointerDown(PointerEventData eventData) => Wind(eventData.position);
}
