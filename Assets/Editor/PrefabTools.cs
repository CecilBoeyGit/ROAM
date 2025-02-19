#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PrefabTools : EditorWindow
{
    private GameObject selectedPrefabInstance;
    private GameObject prefabAsset;
    private List<GameObject> newGameObjects = new List<GameObject>();
    private List<GameObject> removableObjects = new List<GameObject>();

    [MenuItem("Tools/Prefab Modifier Tool")]
    public static void ShowWindow()
    {
        GetWindow<PrefabTools>("Prefab Modifier");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Prefab Modification Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Refresh Prefab Selection"))
        {
            selectedPrefabInstance = Selection.activeGameObject;
        }

        if (selectedPrefabInstance == null)
        {
            EditorGUILayout.HelpBox("Select a prefab instance in the scene to modify.", MessageType.Info);
            return;
        }

        prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(selectedPrefabInstance);

        if (prefabAsset == null)
        {
            EditorGUILayout.HelpBox("The selected object is not a prefab instance.", MessageType.Warning);
            return;
        }
            
        EditorGUILayout.HelpBox("The selected object is not a prefab instance.", MessageType.Warning);
        EditorGUILayout.LabelField("Selected Prefab:", selectedPrefabInstance.name, EditorStyles.boldLabel);

        DetectNewGameObjects();

        if (newGameObjects.Count > 0)
        {
            EditorGUILayout.HelpBox("The following objects were added to the prefab:", MessageType.Info);

            foreach (GameObject newObj in newGameObjects)
            {
                EditorGUILayout.ObjectField("New Object:", newObj, typeof(GameObject), true);
            }

            EditorGUILayout.Space();
        }

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Apply Added GameObjects To Prefab"))
        {
            ApplyChangesToPrefab();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(20);

        EditorGUILayout.HelpBox("The following objects will be removed from the prefab:", MessageType.Warning);

        if (GUILayout.Button("REFRESH SELECTED FOR DELETION"))
        {
            DetectRemovableObjects();
        }

        if (removableObjects.Count > 0)
        {

            foreach (GameObject obj in removableObjects)
            {
                EditorGUILayout.ObjectField("Remove:", obj, typeof(GameObject), true);
            }

            EditorGUILayout.Space();
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete Selected GameObjects From Prefab"))
        {
            RemoveChangedToPrefab();
        }
        GUI.backgroundColor = Color.white;
    }

    private void DetectNewGameObjects()
    {
        newGameObjects.Clear();

        if (prefabAsset == null) return;

        HashSet<string> prefabChildNames = new HashSet<string>();
        foreach (Transform child in prefabAsset.transform)
        {
            prefabChildNames.Add(child.name);
        }

        foreach (Transform child in selectedPrefabInstance.transform)
        {
            if (!prefabChildNames.Contains(child.name))
            {
                newGameObjects.Add(child.gameObject);
            }
        }
    }

    private void ApplyChangesToPrefab()
    {
        if (newGameObjects.Count == 0)
        {
            Debug.Log("No new objects to apply.");
            return;
        }

        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectedPrefabInstance);

        if (string.IsNullOrEmpty(prefabPath))
        {
            Debug.LogError("Could not find prefab asset path. Ensure it's a valid prefab instance.");
            return;
        }

        foreach (GameObject newObj in newGameObjects)
        {
            PrefabUtility.ApplyAddedGameObject(newObj, prefabPath, InteractionMode.UserAction);
        }

        Debug.Log($"Changes applied to prefab: {selectedPrefabInstance.name}");
        newGameObjects.Clear();
    }

    private void DetectRemovableObjects()
    {
        removableObjects.Clear();

        if (selectedPrefabInstance == null) return;

        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.transform.IsChildOf(selectedPrefabInstance.transform))
            {
                removableObjects.Add(obj);
            }
        }
    }

    private void RemoveChangedToPrefab()
    {
        if (removableObjects.Count == 0)
        {
            Debug.Log("No objects to remove.");
            return;
        }

        GameObject prefabObj = PrefabUtility.GetNearestPrefabInstanceRoot(selectedPrefabInstance);

        foreach (GameObject obj in removableObjects)
        {
            DestroyImmediate(obj);
            PrefabUtility.ApplyRemovedGameObject(obj, prefabObj, InteractionMode.UserAction);
        }

        PrefabUtility.ApplyPrefabInstance(prefabObj, InteractionMode.UserAction);
        Debug.Log($"Removed objects from prefab: {selectedPrefabInstance.name}");
        removableObjects.Clear();
    }
}
#endif
