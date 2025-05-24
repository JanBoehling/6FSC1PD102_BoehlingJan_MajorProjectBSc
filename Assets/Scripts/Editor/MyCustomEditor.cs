using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

// Script from my 4-2 editor assignment. Used in this project to draw inspectors of list items
public class MyCustomEditor<T> : Editor where T : Object
{
    protected T targetScript;

    private readonly List<Object> foldouts = new();
    private readonly List<bool> foldoutStates = new();
    private readonly List<Editor> foldoutEditors = new();

    private readonly Dictionary<string, bool> simpleFoldoutStates = new();

    protected virtual void OnEnable()
    {
        targetScript = target as T;
    }

    /// <summary>
    /// Draws the editor of a serialized object without the script property
    /// </summary>
    /// <param name="editor">The serialized object whose editor should be drawn</param>
    protected void DrawEditor(SerializedObject editor) => DrawEditorExcludingProperties(editor, "m_Script");

    /// <summary>
    /// Draws the editor of a serialized object
    /// </summary>
    /// <param name="editor">The serialized object whose editor should be drawn</param>
    /// <param name="propertyIDs">The property IDs that should not be drawn</param>
    protected void DrawEditorExcludingProperties(SerializedObject editor, params string[] propertyIDs)
    {
        editor.Update();
        DrawPropertiesExcluding(editor, propertyIDs);
        editor.ApplyModifiedProperties();
    }

    /// <summary>
    /// Draws a foldout with an inspector titlebar of the given object
    /// </summary>
    /// <param name="editorToDraw">The object whose editor should be drawn</param>
    /// <param name="excludeProperties">The property IDs that should not be drawn</param>
    protected void DrawFoldoutEditor(Object editorToDraw, params string[] excludeProperties) => DrawFoldoutEditor(editorToDraw, null, excludeProperties);

    /// <summary>
    /// Draws a foldout with an inspector titlebar of the given object
    /// </summary>
    /// <param name="editorToDraw">The object whose editor should be drawn</param>
    /// <param name="onPropertiesChanged">The action that should be performed when a property has been changed</param>
    /// <param name="excludeProperties">The property IDs that should not be drawn</param>
    protected void DrawFoldoutEditor(Object editorToDraw, System.Action onPropertiesChanged = null, params string[] excludeProperties)
    {
        int index;

        if (foldouts.Contains(editorToDraw))
        {
            index = foldouts.IndexOf(editorToDraw);
        }
        else
        {
            foldouts.Add(editorToDraw);
            foldoutStates.Add(true);
            foldoutEditors.Add(CreateEditor(editorToDraw));

            index = foldouts.Count - 1;
        }

        if (!editorToDraw) return;

        foldoutStates[index] = EditorGUILayout.InspectorTitlebar(foldoutStates[index], editorToDraw);

        using var check = new EditorGUI.ChangeCheckScope();

        if (!foldoutStates[index]) return;

        var editorSerializedObject = foldoutEditors[index].serializedObject;

        editorSerializedObject.Update();

        excludeProperties = new string[] { "m_Script" }.Concat(excludeProperties).ToArray();
        Editor.DrawPropertiesExcluding(editorSerializedObject, excludeProperties);

        editorSerializedObject.ApplyModifiedProperties();

        if (check.changed && onPropertiesChanged is not null) onPropertiesChanged();
    }

    /// <summary>
    /// Draws foldouts with an inspector titlebar for each given object
    /// </summary>
    /// <param name="editorsToDraw">The objects whose editors should be drawn</param>
    /// <param name="excludeProperties">The property IDs that should not be drawn</param>
    protected void DrawFoldoutEditor(Object[] editorsToDraw, params string[] excludeProperties) => DrawFoldoutEditor(editorsToDraw, null, excludeProperties);

    /// <summary>
    /// Draws foldouts with an inspector titlebar for each given object
    /// </summary>
    /// <param name="editorsToDraw">The objects whose editors should be drawn</param>
    /// <param name="onPropertiesChanged">The action that should be performed when a property has been changed</param>
    /// <param name="excludeProperties">The property IDs that should not be drawn</param>
    protected void DrawFoldoutEditor(Object[] editorsToDraw, System.Action onPropertiesChanged = null, params string[] excludeProperties)
    {
        foreach (var editorToDraw in editorsToDraw)
        {
            int index;

            if (foldouts.Contains(editorToDraw))
            {
                index = foldouts.IndexOf(editorToDraw);
            }
            else
            {
                foldouts.Add(editorToDraw);
                foldoutStates.Add(true);
                foldoutEditors.Add(CreateEditor(editorToDraw));

                index = foldouts.Count - 1;
            }

            if (!editorToDraw) return;

            foldoutStates[index] = EditorGUILayout.InspectorTitlebar(foldoutStates[index], editorToDraw);

            using var check = new EditorGUI.ChangeCheckScope();

            if (!foldoutStates[index]) continue;

            var editorSerializedObject = foldoutEditors[index].serializedObject;

            editorSerializedObject.Update();

            excludeProperties = new string[] { "m_Script" }.Concat(excludeProperties).ToArray();
            Editor.DrawPropertiesExcluding(editorSerializedObject, excludeProperties);

            editorSerializedObject.ApplyModifiedProperties();

            if (check.changed && onPropertiesChanged is not null) onPropertiesChanged();
        }
    }

    /// <summary>
    /// Draws a foldout that can hide properties
    /// </summary>
    /// <param name="title">The title text of the foldout</param>
    /// <param name="onFoldoutOpened">The action that should be performed when the foldout is opened</param>
    /// <param name="onPropertiesChanged">The action that should be performed when a property has been changed</param>
    protected void DrawFoldout(string title, System.Action onFoldoutOpened, System.Action onPropertiesChanged = null)
    {
        if (!simpleFoldoutStates.TryGetValue(title, out bool foldout))
        {
            foldout = true;
            simpleFoldoutStates.Add(title, foldout);
        }
        
        using var check = new EditorGUI.ChangeCheckScope();

        foldout = EditorGUILayout.Foldout(foldout, title, true);
        if (foldout) onFoldoutOpened();

        if (check.changed && onPropertiesChanged is not null)
        {
            onPropertiesChanged();
        }

        simpleFoldoutStates[title] = foldout;
    }

    /// <summary>
    /// Draws a button
    /// </summary>
    /// <param name="label">The text tha should be on the button</param>
    /// <param name="action">The action that should be performed when the button has been pressed</param>
    protected void DrawButton(string label, System.Action action)
    {
        if (GUILayout.Button(label)) action();
    }

    /// <summary>
    /// Draws some vertical space on the inspector
    /// </summary>
    /// <param name="space">The amount of space that should be drawn. Default is single line height</param>
    protected void DrawSpace(float space = -1f)
    {
        if (space == -1f) space = EditorGUIUtility.singleLineHeight;
        EditorGUILayout.Space(space);
    }

    /// <summary>
    /// Loops through all child objects of the given object and destroys them. If given an index, only that on planet will be deleted
    /// </summary>
    /// <param name="parent">The parent transform whose children should be deleted</param>
    /// <param name="index">The index of the child that should be deleted</param>
    public void DestroyAllChildren(Transform parent, int index = -1)
    {
#if UNITY_EDITOR
        if (index >= 0)
        {
            DestroyImmediate(parent.GetChild(index).gameObject);
            return;
        }

        int childCount = parent.childCount;

        var children = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            children[i] = parent.GetChild(i).gameObject;
        }

        foreach (var child in children)
        {
            DestroyImmediate(child);
        }
#endif
    }
}
