using System.Runtime.CompilerServices;
using UnityEngine;

public class CompletionTracker : MonoSingleton<CompletionTracker>
{
    [field:SerializeField] public UnitData[] Units { get; private set; }
    [field:SerializeField] public AssignmentData[] Assignments { get; private set; }

    public bool[] AssignmentCompletionState { get; private set; }
    public bool[] UnitCompletionState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        AssignmentCompletionState = new bool[Assignments.Length];
        UnitCompletionState = new bool[Units.Length];
    }

    public void SetAssignmentCompletionState(uint id)
    {
        if (id >= AssignmentCompletionState.Length)
        {
            Debug.LogError($"Assignment with ID {id} could not be found.");
            return;
        }
        AssignmentCompletionState[id] = true;
    }

    public void SetUnitCompletionState(uint id)
    {
        if (id >= UnitCompletionState.Length)
        {
            Debug.LogError($"Unit with ID {id} could not be found.");
            return;
        }
        UnitCompletionState[id] = true;
    }

    public AssignmentData GetAssignmentByID(uint id) => Assignments[id];

    public UnitData GetUnitByID(uint id) => Units[id];

#nullable enable
    public uint GetID(AssignmentData assignmentData, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (uint i = 0; i < Assignments.Length; i++)
        {
            if (Assignments[i] != assignmentData) continue;

            return i;
        }

        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find assignment {assignmentData.name} in {name}");
        return 0;
    }

    public uint GetID(UnitData unitData, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (uint i = 0; i < Units.Length; i++)
        {
            if (Units[i] != unitData) continue;

            return i;
        }

        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find unit {unitData.name} in {name}");
        return 0;
    }

    public void SetCompletionState(AssignmentData assignment, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (int i = 0; i < Assignments.Length; i++)
        {
            if (Assignments[i] != assignment) continue;
            AssignmentCompletionState[i] = true;
            return;
        }

        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find the completion state of assignment {assignment.name}.");
    }

    public void SetCompletionState(UnitData unitData, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (int i = 0; i < Units.Length; i++)
        {
            if (Units[i] != unitData) continue;
            UnitCompletionState[i] = true;
            return;
        }

        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find the completion state of unit {unitData.name}.");
    }

    public bool GetUnitCompletionState(uint id, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        if (id < UnitCompletionState.Length)
        {
            return UnitCompletionState[id];
        }

        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find the completion state of unit with ID {id}.");
        return false;
    }

    public bool GetAssignmentCompletionState(uint id, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        if (id < AssignmentCompletionState.Length)
        {
            return AssignmentCompletionState[id];
        }

        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find the completion state of assignment with ID {id}.");
        return false;
    }
}
