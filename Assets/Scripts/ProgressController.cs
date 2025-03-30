using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressController : MonoBehaviour
{
    [SerializeField] private Slider _videoProgressDisplay;
    [SerializeField] private TMP_Text _videoProgressDisplayText;

    [SerializeField] private Slider _assignmentProgressDisplay;
    [SerializeField] private TMP_Text _assignmentProgressDisplayText;

    [SerializeField] private Image _totalProgressDisplay;
    [SerializeField] private TMP_Text _totalProgressDisplayText;

    public float VideoProgress { get; private set; }
    public float AssignmentProgress { get; private set; }
    public float TotalProgress { get; private set; }

    private void Start()
    {
        UpdateProgressDisplay(UnitCarousel.Instance.GetCurrentUnitData());
    }

    public void UpdateProgressDisplay(UnitData currentUnitData)
    {
        (VideoProgress, AssignmentProgress, TotalProgress) = CalculateProgress(currentUnitData);

        if (_videoProgressDisplay) _videoProgressDisplay.value = VideoProgress * 100f;
        if (_videoProgressDisplayText) _videoProgressDisplayText.text = (VideoProgress * 100f).ToString("0");

        if (_assignmentProgressDisplay) _assignmentProgressDisplay.value = AssignmentProgress * 100f;
        if (_assignmentProgressDisplayText) _assignmentProgressDisplayText.text = (AssignmentProgress * 100f).ToString("0");

        if (_totalProgressDisplay) _totalProgressDisplay.fillAmount = TotalProgress * 100f;
        if (_totalProgressDisplayText) _totalProgressDisplayText.text = (TotalProgress * 100f).ToString("0");
    }

    private (float videoProgress, float assignmentProgress, float totalProgress) CalculateProgress(UnitData currentUnitData)
    {
        var milestones = currentUnitData.Milestones;
        int milestoneCount = milestones.Count;

        int completedVideos = 0;
        int completedAssignments = 0;
        int totalProgress = 0;

        int videoCount = 0;
        int assignmentCount = 0;

        for (int i = 0; i < milestoneCount; i++)
        {
            if (!milestones[0].IsCompleted) continue;

            if (milestones[0].IsVideo) completedVideos++;
            else completedAssignments++;

            totalProgress++;
        }

        for (int i = 0; i < milestoneCount; i++)
        {
            if (milestones[0].IsVideo) videoCount++;
            else assignmentCount++;
        }

        return (
            videoCount == 0 ? 1f : MathF.Round(completedVideos / videoCount, 2),
            assignmentCount == 0 ? 1f : MathF.Round(completedAssignments / assignmentCount, 2),
            MathF.Round(totalProgress / milestoneCount, 2)
            );
    }
}
