using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UnitAndAssignmentManager : MonoSingleton<UnitAndAssignmentManager>, IDisposable
{
    [field:SerializeField] public UnitData[] Units { get; private set; }
    [field:SerializeField] public AssignmentDataBase[] Assignments { get; private set; }

    public bool[] AssignmentCompletionState { get; private set; }
    public bool[] UnitCompletionState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        AssignmentCompletionState = new bool[Assignments.Length];
        UnitCompletionState = new bool[Units.Length];
    }

    /// <summary>
    /// Retrieves the completion states of the units and assignments from the DB
    /// </summary>
    /// <param name="onDownloadCompletedCallback">This action is invoked when the download has been completed</param>
    public void DownloadCompletionData(Action onDownloadCompletedCallback)
    {
        DB.Instance.Query(assignmentCompletionState =>
        {
            DB.Instance.Query(assignmentLinks =>
            {
                for (int i = 0; i < assignmentCompletionState.Length; i++)
                {
                    int link = int.Parse(assignmentLinks[i]);
                    bool state = int.Parse(assignmentCompletionState[i]) != 0;

                    AssignmentCompletionState[link] = state;
                }

                DB.Instance.Query(unitCompletionState =>
                {
                    DB.Instance.Query(unitLinks =>
                    {
                        for (int i = 0; i < unitCompletionState.Length; i++)
                        {
                            int link = int.Parse(unitLinks[i]);
                            bool state = int.Parse(unitCompletionState[i]) != 0;

                            UnitCompletionState[link] = state;
                        }

                        // Download finished
                        onDownloadCompletedCallback?.Invoke();

                    }, $"SELECT unitLink FROM UnitProgress INNER JOIN UserData ON UserData.userID = UnitProgress.userID WHERE UserData.userID = {CurrentUser.UserID} ORDER BY unitLink");

                }, $"SELECT isCompleted FROM UserData INNER JOIN UnitProgress ON UserData.userID = UnitProgress.userID WHERE UserData.userID = {CurrentUser.UserID} ORDER BY unitLink");

            }, $"SELECT assignmentLink FROM AssignmentProgress INNER JOIN UserData ON UserData.userID = AssignmentProgress.userID WHERE UserData.userID = {CurrentUser.UserID} ORDER BY assignmentLink");

        }, $"SELECT isCompleted FROM UserData INNER JOIN AssignmentProgress ON UserData.userID = AssignmentProgress.userID WHERE UserData.userID = {CurrentUser.UserID} ORDER BY assignmentLink");
    }

    /// <summary>
    /// Loops through AssignmentCompletionState and UnitCompletionState arrays and sets the respective bool values in the DB
    /// </summary>
    public void UploadCompletionStates()
    {
        for (int i = 0; i < UnitCompletionState.Length; i++)
        {
            DB.Instance.UpdateQuery(null, tableName: "UnitProgress", set: $"isCompleted={(UnitCompletionState[i] ? "1" : "0")}", predicate: $"unitLink={i}");
        }

        for (int i = 0; i < AssignmentCompletionState.Length; i++)
        {
            DB.Instance.UpdateQuery(null, tableName: "AssignmentProgress", set: $"isCompleted={(AssignmentCompletionState[i] ? "1" : "0")}", predicate: $"assignmentLink={i}");
        }
    }

    /// <summary>
    /// Sets the completion state of the assignment with the given ID
    /// </summary>
    /// <param name="id">The ID of the assignment of which the completion state should be updated</param>
    public void SetAssignmentCompletionState(uint id)
    {
        if (id >= AssignmentCompletionState.Length)
        {
#if UNITY_EDITOR
            Debug.LogError($"Assignment with ID {id} could not be found.");
#endif
            return;
        }
        AssignmentCompletionState[id] = true;
    }

    /// <summary>
    /// Sets the completion state of the unit with the given ID
    /// </summary>
    /// <param name="id">The ID of the unit of which the completion state should be updated</param>
    public void SetUnitCompletionState(uint id)
    {
        if (id >= UnitCompletionState.Length)
        {
#if UNITY_EDITOR
            Debug.LogError($"Unit with ID {id} could not be found.");
#endif
            return;
        }
        UnitCompletionState[id] = true;
    }

    /// <summary>
    /// Retrieves the assignment data of the given assignment ID
    /// </summary>
    /// <param name="id">The ID of the assignment of which the data should be retrieved</param>
    /// <returns>The data of the assignment with the given ID</returns>
    public AssignmentDataBase GetAssignmentByID(uint id) => Assignments[id];

    /// <summary>
    /// Retrieves the unit data of the given unit ID
    /// </summary>
    /// <param name="id">The ID of the unit of which the data should be retrieved</param>
    /// <returns>The data of the unit with the given ID</returns>
    public UnitData GetUnitByID(uint id) => Units[id];

#nullable enable
    /// <summary>
    /// Retrieves the ID of the given assignment data
    /// </summary>
    /// <param name="assignmentData">The assignment data of which the ID should be retieved</param>
    /// <param name="callerFilePath">Informations about the file who called this method</param>
    /// <param name="callerMemberName">Informations about the member who called this method</param>
    /// <returns>The ID of the given assignment data</returns>
    public uint GetID(AssignmentDataBase assignmentData, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (uint i = 0; i < Assignments.Length; i++)
        {
            if (Assignments[i] != assignmentData) continue;

            return i;
        }

#if UNITY_EDITOR
        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find assignment {assignmentData.name} in {name}");
#endif
        return 0;
    }

    /// <summary>
    /// Retrieves the ID of the given unit data
    /// </summary>
    /// <param name="unitData">The unit data of which the ID should be retieved</param>
    /// <param name="callerFilePath">Informations about the file who called this method</param>
    /// <param name="callerMemberName">Informations about the member who called this method</param>
    /// <returns>The ID of the given unit data</returns>
    public uint GetID(UnitData unitData, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (uint i = 0; i < Units.Length; i++)
        {
            if (Units[i] != unitData) continue;

            return i;
        }

#if UNITY_EDITOR
        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find unit {unitData.name} in {name}");
#endif
        return 0;
    }

    /// <summary>
    /// Sets the completion state of the given assignment to true
    /// </summary>
    /// <param name="assignment">The assignment of which the completionstate should be updated</param>
    /// <param name="callerFilePath">Informations about the file who called this method</param>
    /// <param name="callerMemberName">Informations about the member who called this method</param>
    public void SetCompletionState(AssignmentDataBase assignment, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (int i = 0; i < Assignments.Length; i++)
        {
            if (Assignments[i] != assignment) continue;
            AssignmentCompletionState[i] = true;
            return;
        }

#if UNITY_EDITOR
        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find the completion state of assignment {assignment.name}.");
#endif
    }

    /// <summary>
    /// Sets the completion state of the given unit to true
    /// </summary>
    /// <param name="unitData">The unit of which the completion state should be updated</param>
    /// <param name="callerFilePath">Informations about the file who called this method</param>
    /// <param name="callerMemberName">Informations about the member who called this method</param>
    public void SetCompletionState(UnitData unitData, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        for (int i = 0; i < Units.Length; i++)
        {
            if (Units[i] != unitData) continue;
            UnitCompletionState[i] = true;
            return;
        }

#if UNITY_EDITOR
        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find the completion state of unit {unitData.name}.");
#endif
    }

    /// <summary>
    /// Retrieves the completion state of the unit with the given ID
    /// </summary>
    /// <param name="id">The ID of the unit from which the completion state should be retrieved</param>
    /// <param name="callerFilePath">Informations about the file who called this method</param>
    /// <param name="callerMemberName">Informations about the member who called this method</param>
    /// <returns>Whether or not the unit has been completed</returns>
    public bool GetUnitCompletionState(uint id, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        if (id < UnitCompletionState.Length)
        {
            return UnitCompletionState[id];
        }

#if UNITY_EDITOR
        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find the completion state of unit with ID {id}.");
#endif
        return false;
    }

    /// <summary>
    /// Retrieves the completion state of the assignment with the given ID
    /// </summary>
    /// <param name="id">The ID of the assignment from which the completion state should be retrieved</param>
    /// <param name="callerFilePath">Informations about the file who called this method</param>
    /// <param name="callerMemberName">Informations about the member who called this method</param>
    /// <returns>Whether or not the assignment has been completed</returns>
    public bool GetAssignmentCompletionState(uint id, [CallerFilePath] string? callerFilePath = default, [CallerMemberName] string? callerMemberName = default)
    {
        if (id < AssignmentCompletionState.Length)
        {
            return AssignmentCompletionState[id];
        }

#if UNITY_EDITOR
        Debug.LogError($"{callerFilePath}.{callerMemberName}: Could not find the completion state of assignment with ID {id}.");
#endif
        return false;
    }

    public void Dispose() => CurrentUser.SyncUser();
}
