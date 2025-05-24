using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CompletionTracker : MonoSingleton<CompletionTracker>
{
    [SerializeField] private AssignmentData[] _assignments;
    //[SerializeField] private Milestone[] _milestones;
    [SerializeField] private UnitData[] _units;

    [field:SerializeField, Tooltip("Serialized for debugging purposes")] public bool[] AssignmentCompletionState { get; private set; }
    [field:SerializeField, Tooltip("Serialized for debugging purposes")] public bool[] UnitCompletionState { get; private set; }

    public UnitData CurrentUnit { get; private set; }
    public Milestone CurrentMilestone { get; private set; }
    public AssignmentData CurrentAssignment { get; private set; }

    //public int CompletedAssignmentCount
    //{
    //    get
    //    {
    //        int completedAssignments = 0;

    //        foreach (var isCompleted in AssignmentCompletionState)
    //        {
    //            if (isCompleted) completedAssignments++;
    //        }

    //        return completedAssignments;
    //    }
    //}

    //public int CompletedVideoCount
    //{
    //    get
    //    {
    //        int completedAssignments = 0;

    //        System.Collections.Generic.List<int> videoIndices = new();

    //        for (int i = 0; i < _assignments.Length; i++)
    //        {
    //            if (_assignments[i] is VideoAssignment) videoIndices.Add(i);
    //        }

    //        for (int i = 0; i < videoIndices.Count; i++)
    //        {
    //            if (AssignmentCompletionState[videoIndices[i]]) completedAssignments++;
    //        }

    //        return completedAssignments;
    //    }
    //}

    protected override void Awake()
    {
        base.Awake();

        AssignmentCompletionState = new bool[_assignments.Length];
        UnitCompletionState = new bool[_units.Length];
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

    public AssignmentData GetAssignmentByID(uint id) => _assignments[id];
    public UnitData GetUnitByID(uint id) => _units[id];

    public uint GetID(AssignmentData assignmentData, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (uint i = 0; i < _assignments.Length; i++)
        {
            if (_assignments[i] != assignmentData) continue;

            return i;
        }

        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find assignment {assignmentData.name} in {name}");
        return 0;
    }

    public uint GetID(UnitData unitData, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (uint i = 0; i < _units.Length; i++)
        {
            if (_units[i] != unitData) continue;

            return i;
        }

        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find unit {unitData.name} in {name}");
        return 0;
    }

    public void SetCompletionState(AssignmentData assignment, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (int i = 0; i < _assignments.Length; i++)
        {
            if (_assignments[i] != assignment) continue;
            AssignmentCompletionState[i] = true;
            return;
        }

        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find the completion state of assignment {assignment.name}.");
    }

    public void SetCompletionState(UnitData unitData, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (int i = 0; i < _units.Length; i++)
        {
            if (_units[i] != unitData) continue;
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
