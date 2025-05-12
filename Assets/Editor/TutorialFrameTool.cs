using UnityEngine;
using UnityEditor;

public class TutorialFrameTool : EditorWindow
{
    private GameObject prefabToSpawn;
    private GameObject ContentObject;
    private bool isSpawning = false;

    private string spawnParentName = "--- TutorialTriggerVolumes ---";
    private string TutorialContentObjectName = "TutorialContent";

    private const string PrefabGUIDKey = "TutorialFrameVolumeSpawner_LastPrefabGUID";
    private const string ContentGUIDKey = "TutorialContentObject_LastObjectGUID";

    [MenuItem("Tools/TutorialFrameVolume Spawner")]
    public static void ShowWindow()
    {
        var window = GetWindow<TutorialFrameTool>("TutorialFrameVolume Spawner");
        window.LoadLastPrefab();
        window.LoadLastContentObject();
    }

    private void OnGUI()
    {
        GUILayout.Label("Spawn Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        
        prefabToSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab to Spawn", prefabToSpawn, typeof(GameObject), false);
        EditorGUILayout.HelpBox("The above prefab is the individial TutorialVolume to spawn.", MessageType.None);
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        TutorialContentObjectName = EditorGUILayout.TextField("Tutorial Content Name", TutorialContentObjectName);
        EditorGUILayout.HelpBox("Spawned objects will be parented under this GameObject in the scene.", MessageType.None);
        ContentObject = (GameObject)EditorGUILayout.ObjectField("Tutorial Content Object", ContentObject, typeof(GameObject), false);
        EditorGUILayout.HelpBox("The above Object reference is where the parent for ALL the tutorial graphics.", MessageType.Info);

        EditorGUILayout.Space();
        spawnParentName = EditorGUILayout.TextField("Parent Object Name", spawnParentName);
        EditorGUILayout.HelpBox("Spawned objects will be parented under this GameObject in the scene.", MessageType.None);

        if (EditorGUI.EndChangeCheck())
        {
            SavePrefabReference();
            SaveContentObjectReference();
        }

        if (!isSpawning && prefabToSpawn != null)
        {
            if (GUILayout.Button("Ready to Spawn"))
            {
                isSpawning = true;
                SceneView.duringSceneGui += OnSceneGUI;
            }
        }

        if (isSpawning)
        {
            EditorGUILayout.HelpBox("Click anywhere in Scene to spawn. Press ESC to cancel.", MessageType.Warning);

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("EXIT SPAWNING"))
            {
                CancelSpawning();
            }
            GUI.backgroundColor = Color.white;
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            CancelSpawning();
            e.Use();
        }

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                SpawnPrefab(hit.point);
            }
            else
            {
                // fallback to flat ground (Y = 0)
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                if (plane.Raycast(ray, out float enter))
                {
                    Vector3 point = ray.GetPoint(enter);
                    SpawnPrefab(point);
                }
            }

            e.Use();
        }
    }

    private void SpawnPrefab(Vector3 position)
    {
        if (prefabToSpawn != null)
        {
            GameObject spawned = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn);
            TutorialFrameTriggerVolume varChildComp = spawned.GetComponentInChildren<TutorialFrameTriggerVolume>();
            if (varChildComp != null)
            {
                varChildComp.contentObject = ContentObject;
            }

            Undo.RegisterCreatedObjectUndo(spawned, "Spawn Prefab");
            spawned.transform.position = position;

            if (!string.IsNullOrEmpty(spawnParentName))
            {
                GameObject parent = GameObject.Find(spawnParentName);
                if (parent == null)
                {
                    parent = new GameObject(spawnParentName);
                    Undo.RegisterCreatedObjectUndo(parent, "Create Spawn Parent");
                }

                spawned.transform.SetParent(parent.transform);
            }
        }
    }

    private void CancelSpawning()
    {
        isSpawning = false;
        SceneView.duringSceneGui -= OnSceneGUI;
        Repaint(); // update the UI
    }

    private void OnDisable()
    {
        CancelSpawning();
    }

    private void SavePrefabReference()
    {
        if (prefabToSpawn == null)
        {
            EditorPrefs.DeleteKey(PrefabGUIDKey);
            return;
        }

        string path = AssetDatabase.GetAssetPath(prefabToSpawn);
        string guid = AssetDatabase.AssetPathToGUID(path);
        EditorPrefs.SetString(PrefabGUIDKey, guid);
    }
    private void SaveContentObjectReference()
    {
        GameObject found = GameObject.Find(TutorialContentObjectName);

        if (found == null)
        {
            EditorPrefs.DeleteKey(ContentGUIDKey);
            ContentObject = null;
            return;
        }
        else
        {
            ContentObject = found;
        }

        EditorPrefs.SetString(ContentGUIDKey, ContentObject.name);
    }

    private void LoadLastPrefab()
    {
        if (EditorPrefs.HasKey(PrefabGUIDKey))
        {
            string guid = EditorPrefs.GetString(PrefabGUIDKey);
            string path = AssetDatabase.GUIDToAssetPath(guid);
            prefabToSpawn = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        else
        {
            // Load default prefab
            string defaultPath = "Assets/Prefabs/UI/TutorialCameraFrame/TutorialVolume.prefab";
            prefabToSpawn = AssetDatabase.LoadAssetAtPath<GameObject>(defaultPath);

            if (prefabToSpawn != null)
            {
                // Save the default prefab as the current selection
                string guid = AssetDatabase.AssetPathToGUID(defaultPath);
                EditorPrefs.SetString(PrefabGUIDKey, guid);
            }
            else
            {
                Debug.LogWarning($"Default prefab not found at path: {defaultPath}");
            }
        }
    }
    private void LoadLastContentObject()
    {
        if (EditorPrefs.HasKey(ContentGUIDKey))
        {
            string savedPath = EditorPrefs.GetString(ContentGUIDKey);
            GameObject savedObj = GameObject.Find(savedPath);

            if (savedObj != null)
            {
                ContentObject = savedObj;
                return;
            }
        }

        GameObject found = GameObject.Find(TutorialContentObjectName);

        if (found != null)
        {
            ContentObject = found;
            EditorPrefs.SetString(ContentGUIDKey, found.name); // store name only
        }
        else
        {
            Debug.LogWarning("No GameObject named 'ContentObject' found in the scene.");
        }
    }
}
