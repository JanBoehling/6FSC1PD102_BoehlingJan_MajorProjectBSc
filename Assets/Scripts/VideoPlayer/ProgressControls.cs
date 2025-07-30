using UnityEngine.EventSystems;

public class ProgressControls : VideoControlsBase, IPointerDownHandler
{
    private void Update() => ProgressBar.fillAmount = VideoProgress;

    public void OnPointerDown(PointerEventData eventData) => Wind(eventData.position);
}
