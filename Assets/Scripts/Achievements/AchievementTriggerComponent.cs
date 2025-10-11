using UnityEngine;

public class AchievementTriggerComponent : MonoBehaviour
{
    [SerializeField] private int _achievementID;

    public bool GetAchievementStatus() => AchievementManager.Instance.Achievements[_achievementID].IsCompleted;

    public void SetAchievementStatus(bool completionStatus = true) => SetAchievementStatus(null, completionStatus);
    public void SetAchievementStatus(System.Action callback, bool completionStatus = true)
    {
        AchievementManager.Instance.Achievements[_achievementID].SetCompletionState(completionStatus);
        DB.Instance.UpdateQuery(_ => callback?.Invoke(), tableName: "AchievementProgress", set: $"isCompleted={(completionStatus ? "1" : "0")}", predicate: $"achievementLink={_achievementID}");
    }
}
