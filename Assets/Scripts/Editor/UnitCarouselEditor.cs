using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitCarousel))]
public class UnitCarouselEditor : Editor
{
    private UnitCarousel _unitCarousel;

    private void OnEnable() => _unitCarousel = (UnitCarousel)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField($"Current Unit: {_unitCarousel.UnitPosition}");

        if (!Application.isPlaying) return;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("<"))
        {
            _unitCarousel.SwipeCarousel(-1);
        }

        else if (GUILayout.Button(">"))
        {
            _unitCarousel.SwipeCarousel(1);
        }

        EditorGUILayout.EndHorizontal();
    }
    public override bool RequiresConstantRepaint() => true;
}
