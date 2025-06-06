using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class CompletionTracker : MonoSingleton<CompletionTracker>, IDisposable
{
    [field:SerializeField] public UnitData[] Units { get; private set; }
    [field:SerializeField] public AssignmentData[] Assignments { get; private set; }

    [field: SerializeField] public bool[] AssignmentCompletionState { get; private set; }
    [field: SerializeField] public bool[] UnitCompletionState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        AssignmentCompletionState = new bool[Assignments.Length];
        UnitCompletionState = new bool[Units.Length];
    }

    public async void DownloadCompletionData()
    {
        var assignmentCompletionState = await DB.Query($"SELECT isCompleted FROM UserData INNER JOIN AssignmentProgress ON UserData.userID = AssignmentProgress.userID WHERE UserData.userID = {CurrentUser.UserID} ORDER BY assignmentLink");
        var assignmentLinks = await DB.Query($"SELECT assignmentLink FROM AssignmentProgress INNER JOIN UserData ON UserData.userID = AssignmentProgress.userID WHERE UserData.userID = {CurrentUser.UserID} ORDER BY assignmentLink");
        for (int i = 0; i < assignmentCompletionState.Length; i++)
        {
            int link = int.Parse(assignmentLinks[i]);
            bool state = int.Parse(assignmentCompletionState[i]) != 0;

            AssignmentCompletionState[link] = state;
        }

        var unitCompletionState = await DB.Query($"SELECT isCompleted FROM UserData INNER JOIN UnitProgress ON UserData.userID = UnitProgress.userID WHERE UserData.userID = {CurrentUser.UserID} ORDER BY unitLink");
        var unitLinks = await DB.Query($"SELECT unitLink FROM UnitProgress INNER JOIN UserData ON UserData.userID = UnitProgress.userID WHERE UserData.userID = {CurrentUser.UserID} ORDER BY unitLink");
        for (int i = 0; i < unitCompletionState.Length; i++)
        {
            int link = int.Parse(unitLinks[i]);
            bool state = int.Parse(unitCompletionState[i]) != 0;

            UnitCompletionState[link] = state;
        }
    }

    /// <summary>
    /// Loops through AssignmentCompletionState and UnitCompletionState arrays and sets the respective bool values in the DB
    /// </summary>
    public async void UploadCompletionStates()
    {
        var unitValues = new List<KeyValuePair<string, string>>();
        for (int i = 0; i < UnitCompletionState.Length; i++)
        {
            unitValues.Add(new(i.ToString(), UnitCompletionState[i] ? "1" : "0"));
        }
        await DB.Update("UnitProgress", unitValues.ToArray(), "UserID", CurrentUser.UserID.ToString());

        var assignmentValues = new List<KeyValuePair<string, string>>();
        for (int i = 0; i < AssignmentCompletionState.Length; i++)
        {
            assignmentValues.Add(new(i.ToString(), AssignmentCompletionState[i] ? "1" : "0"));
        }
        await DB.Update("AssignmentProgress", unitValues.ToArray(), "UserID", CurrentUser.UserID.ToString());
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

    public void Dispose()
    {
        CurrentUser.SyncUser();
    }
}
