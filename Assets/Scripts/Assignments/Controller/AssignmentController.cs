using UnityEngine;

public abstract class AssignmentController<T> : MonoBehaviour where T : AssignmentData
{
    [Header("Assignment ID")]
    [SerializeField] protected uint _assignmentID;
    [SerializeField] protected bool _useDebugAssignment;

    protected T _assignmentData;
    protected ParticleSystem _confettiCanon;

    protected virtual void Awake()
    {
        _confettiCanon = FindFirstObjectByType<ParticleSystem>();
    }

    protected virtual void Start()
    {
#if UNITY_EDITOR
        if (_useDebugAssignment)
        {
            RuntimeDataHolder.CurrentMilestone = new()
            {
                Assignments = new[] { _assignmentID }
            };
        }
#endif
        _assignmentID = RuntimeDataHolder.CurrentMilestone.Assignments[0];
        _assignmentData = CompletionTracker.Instance.GetAssignmentByID(_assignmentID) as T;

        Init(_assignmentID);
    }

    public abstract void Init(uint assignmentID);
}
