using UnityEngine;
using UnityEditor;

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
    }

    private async void TestConnection()
    {
        Debug.Log(await DB.TestConnection());
    }

    private async void TestGetUserData()
    {
        if (string.IsNullOrEmpty(where) || string.IsNullOrEmpty(predicate)) Debug.Log(await DB.Select(select: select, from: from));
        else Debug.Log(await DB.Select(select: select, from: from, where: where, predicate: predicate));
    }

    private async void TestInsertUserData()
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || streak < 0 || xp < 0) return;
        Debug.Log(await DB.Insert(username, password, (uint)streak, (uint)xp));
    }
}
