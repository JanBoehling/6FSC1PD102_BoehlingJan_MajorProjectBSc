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
    public float MilestoneProgress { get; private set; }
    public float TotalProgress { get; private set; }

    private void Start()
    {
        UpdateProgressDisplay(UnitCarousel.Instance.GetCurrentUnitData());
    }

    public void UpdateProgressDisplay(UnitData currentUnitData)
    {
        (VideoProgress, MilestoneProgress, TotalProgress) = CalculateProgress(currentUnitData);

        if (_videoProgressDisplay) _videoProgressDisplay.value = VideoProgress;
        if (_videoProgressDisplayText) _videoProgressDisplayText.text = (VideoProgress * 100f).ToString("0");

        if (_assignmentProgressDisplay) _assignmentProgressDisplay.value = MilestoneProgress;
        if (_assignmentProgressDisplayText) _assignmentProgressDisplayText.text = (MilestoneProgress * 100f).ToString("0");

        if (_totalProgressDisplay) _totalProgressDisplay.fillAmount = TotalProgress;
        if (_totalProgressDisplayText) _totalProgressDisplayText.text = (TotalProgress * 100f).ToString("0");
    }

    private (float videoProgress, float milestoneProgress, float totalProgress) CalculateProgress(UnitData currentUnitData)
    {
        var milestones = currentUnitData.Milestones;
        int milestoneCount = milestones.Count;

        int completedVideos = 0;
        int completedMilestones = 0;
        int totalProgress = 0;

        int videoCount = 0;
        int assignmentCount = 0;

        // Counts the completion count of videos, assignments and the total completion
        for (int i = 0; i < milestoneCount; i++)
        {
            if (!milestones[i].IsCompleted) continue;

            if (CompletionTracker.Instance.GetAssignmentByID(milestones[i].Assignments[0]) is VideoAssignment) completedVideos++;
            else completedMilestones++;

            totalProgress++;
        }

        // Counts videos and assignments
        for (int i = 0; i < milestoneCount; i++)
        {
            if (CompletionTracker.Instance.GetAssignmentByID(milestones[i].Assignments[0]) is VideoAssignment) videoCount++;
            else assignmentCount++;
        }

        float x = (float)totalProgress / milestoneCount;
        float total = MathF.Round(x, 2);

        return (
            videoCount == 0 ? 1 : MathF.Round(completedVideos / videoCount, 2),
            assignmentCount == 0 ? 1 : MathF.Round(completedMilestones / assignmentCount, 2),
            total
            );
    }
}
