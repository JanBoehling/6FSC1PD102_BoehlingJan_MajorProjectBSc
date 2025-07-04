using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(DBTester))]
public class DBTesterEditor : Editor
{
    private string select;
    private string from;
    private string where;
    private string predicate;

    private string username;
    private string password;
    private int streak;
    private int xp;

    private string query;

    private System.Action<string[]> _testCallback = new((response) =>
    {
        foreach (var item in response)
        {
            Debug.Log(item);
        }
    });

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Test MySQL Connection")) TestConnection();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        select = EditorGUILayout.TextField("Select", select);
        from = EditorGUILayout.TextField("From", from);
        where = EditorGUILayout.TextField("Where", where);
        predicate = EditorGUILayout.TextField("Predicate", predicate);
        if (GUILayout.Button("SELECT")) TestGetUserData();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        username = EditorGUILayout.TextField("Username", username);
        password = EditorGUILayout.TextField("Password", password);
        streak = EditorGUILayout.IntField("Streak", streak);
        xp = EditorGUILayout.IntField("XP", xp);
        if (GUILayout.Button("INSERT")) TestInsertUserData();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        query = EditorGUILayout.TextField("Query", query);
        if (GUILayout.Button("QUERY")) TestQuery();
    }

    private void TestConnection()
    {
        Cls();

        DB.Instance.TestConnection(_testCallback);
    }

    private void TestGetUserData()
    {
        Cls();

        if (string.IsNullOrEmpty(where) || string.IsNullOrEmpty(predicate)) DB.Instance.Select(_testCallback, select: select, from: from);
        else DB.Instance.Select(_testCallback, select: select, from: from, where: where, predicate: predicate);
    }

    private void TestInsertUserData()
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || streak < 0 || xp < 0) return;

        Cls();

        DB.Instance.Insert(_testCallback, username, password, (uint)streak, (uint)xp);
    }

    private void TestQuery()
    {
        if (string.IsNullOrEmpty(query)) return;

        Cls();
        
        DB.Instance.Query(_testCallback, query);
    }

    /// <summary>
    /// Clears every log from the development console<br/>
    /// <see href="https://discussions.unity.com/t/clear-console-through-code-in-development-build/87213"></see>
    /// </summary>
    public static void Cls()
    {
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var logEntries = assembly.GetType("UnityEditor.LogEntries");
        var clearConsoleMethod = logEntries.GetMethod("Clear");
        clearConsoleMethod.Invoke(new object(), null);
    }
}
