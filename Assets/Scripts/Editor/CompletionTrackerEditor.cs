using UnityEditor;

[CustomEditor(typeof(UnitAndAssignmentManager))]
public class UnitAndAssignmentManagerEditor : MyCustomEditor<UnitAndAssignmentManager>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();

        // Draws inspectors of units in list
        DrawFoldout("Units", () => DrawFoldoutEditor(targetScript.Units));

        EditorGUILayout.Separator();

        // Draws inspectors of assignments in list
        DrawFoldout("Assignments", () => DrawFoldoutEditor(targetScript.Assignments));
    }
}
