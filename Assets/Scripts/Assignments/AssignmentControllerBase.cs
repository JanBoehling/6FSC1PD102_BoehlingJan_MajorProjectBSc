using UnityEngine;

public abstract class AssignmentControllerBase<T> : MonoBehaviour where T : AssignmentDataBase
{
    [Header("Assignment ID")]
    [SerializeField] protected uint AssignmentID;
    [SerializeField] protected bool UseDebugAssignment;

    protected T AssignmentData;
    protected ParticleSystem ConfettiCanon;

    protected virtual void Awake()
    {
        ConfettiCanon = FindFirstObjectByType<ParticleSystem>();
    }

    protected virtual void Start()
    {
#if UNITY_EDITOR
        if (UseDebugAssignment)
        {
            RuntimeDataHolder.CurrentMilestone = new()
            {
                Assignments = new[] { AssignmentID }
            };
        }
#endif
        AssignmentID = RuntimeDataHolder.CurrentMilestone.Assignments[0];
        AssignmentData = UnitAndAssignmentManager.Instance.GetAssignmentByID(AssignmentID) as T;

        Init(AssignmentID);
    }

    public abstract void Init(uint assignmentID);
}
