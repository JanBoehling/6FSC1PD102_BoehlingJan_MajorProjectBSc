using UnityEngine;

[CreateAssetMenu()]
public class TestAchievement : AchievementBase
{
    protected override bool CompletionCondition()
    {
        return true;
    }
}
