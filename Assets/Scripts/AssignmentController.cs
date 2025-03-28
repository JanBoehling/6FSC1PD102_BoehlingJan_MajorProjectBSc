using UnityEngine;

public class AssignmentController : MonoSingleton<AssignmentController>
{
    [SerializeField] private Transform _assignmentUIContainer;

    public void Enable(GameObject assignmentUIPrefab)
    {
        _assignmentUIContainer.parent.gameObject.SetActive(true);
        LoadAssignmentUI(assignmentUIPrefab);
    }

    public void LoadAssignmentUI(GameObject assignmentUIPrefab)
    {
        var _assignmentUI = Instantiate(assignmentUIPrefab, _assignmentUIContainer);
    }
}
