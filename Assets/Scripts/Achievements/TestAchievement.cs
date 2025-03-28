using System;
using UnityEngine;

[CreateAssetMenu()]
public class TestAchievement : Achievement
{
    protected override bool CompletionCondition()
    {
        return true;
    }
}
