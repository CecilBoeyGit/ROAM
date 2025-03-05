#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class PrefabTools : EditorWindow
{
    private GameObject selectedPrefabInstance;
    private GameObject prefabAsset;
    private List<GameObject> newGameObjects = new List<GameObject>();
    private List<GameObject> removableObjects = new List<GameObject>();

    GameObject selectedObject;

   [MenuItem("Tools/Prefab Modifier Tool")]
    public static void ShowWindow()
    {
        GetWindow<PrefabTools>("Prefab Modifier");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Prefab Modification Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Confirm Take Out Object Selection"))
        {
            selectedObject = Selection.activeGameObject;
        }

        if (selectedObject != null)
        {
            EditorGUILayout.ObjectField("Selected:", selectedObject, typeof(GameObject), true);
            EditorGUILayout.HelpBox("The above object will be unpacked and moved out of its holding Prefab.", MessageType.Warning);

            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Take Out Selected From Prefab"))
            {
                TakeOutSelectionMethod(selectedObject);
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.HelpBox("Select an object to TAKE OUT from Prefab.", MessageType.Info);

        EditorGUILayout.Space(20);

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
            RemoveAt();
            //RemoveChangedToPrefab();
        }
        GUI.backgroundColor = Color.white;
    }

    void TakeOutSelectionMethod(GameObject selectedObject)
    {
        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectedObject);

        // Load the prefab as a GameObject
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
            return;
        }

        // Create an instance of the prefab to modify
        GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (prefabInstance == null)
        {
            Debug.LogError("Failed to instantiate prefab.");
            return;
        }

        Transform childtemp = prefabInstance.transform.Find(selectedObject.name);
        if (childtemp != null)
        {
            GameObject regularInstance = Instantiate(childtemp.gameObject);
            regularInstance.transform.SetParent(null);
            regularInstance.transform.position = selectedObject.transform.position;
            regularInstance.transform.rotation = selectedObject.transform.rotation;
            regularInstance.transform.localScale = selectedObject.transform.localScale;

            Undo.DestroyObjectImmediate(childtemp.gameObject);
        }
        else
        {
            Debug.LogWarning("Child not found: " + selectedObject.name);
        }


        PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
        DestroyImmediate(prefabInstance);

        Debug.Log("Child removed from prefab and saved.");
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
        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectedPrefabInstance);
        //GameObject loadedPath = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        GameObject prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);

        foreach (GameObject obj in removableObjects)
        {
            //PrefabUtility.ApplyRemovedGameObject(obj, prefabContents, InteractionMode.UserAction);
            DestroyImmediate(obj);

            Debug.Log(prefabPath);
        }

        //PrefabUtility.ApplyObjectOverride(prefabObj, prefabPath, InteractionMode.UserAction);
        //PrefabUtility.ApplyPrefabInstance(prefabContents, InteractionMode.UserAction);

        PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabContents);


        Debug.Log($"Removed objects from prefab: {selectedPrefabInstance.name}");
        removableObjects.Clear();
    }

    private void RemoveAt()
    {
        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectedPrefabInstance);

        // Load the prefab as a GameObject
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
            return;
        }

        // Create an instance of the prefab to modify
        GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (prefabInstance == null)
        {
            Debug.LogError("Failed to instantiate prefab.");
            return;
        }

        foreach (GameObject obj in removableObjects)
        {
            Transform childtemp = prefabInstance.transform.Find(obj.name);
            if (childtemp != null)
            {
                Undo.DestroyObjectImmediate(childtemp.gameObject);
            }
            else
            {
                Debug.LogWarning("Child not found: " + obj.name);
            }
        }

        PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
        DestroyImmediate(prefabInstance);

        Debug.Log("Child removed from prefab and saved.");
    }
}
#endif
