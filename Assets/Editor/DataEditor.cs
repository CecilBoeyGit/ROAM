using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataPersistenceManager))]

public class DataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DataPersistenceManager dataPersisManager = (DataPersistenceManager)target;

        if (dataPersisManager == null)
            return;

        if (GUILayout.Button("Forced Clear"))
        {
            Debug.Log("Forced clear pressed! -----------------------------");
            dataPersisManager.ClearData();
        }
    }
}
