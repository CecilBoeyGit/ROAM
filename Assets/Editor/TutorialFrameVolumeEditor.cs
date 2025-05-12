using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TutorialFrameTriggerVolume))]
public class TutorialFrameVolumeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TutorialFrameTriggerVolume TutorialVolume = (TutorialFrameTriggerVolume)target;

        if (TutorialVolume != null)
        {       
            var options = TutorialVolume.GetChildNames();
            if(options.Count > 0)
            {
                TutorialVolume.selectedChildIndex = EditorGUILayout.Popup("Select Frame Child", TutorialVolume.selectedChildIndex, options.ToArray());
            }
            else
            {
                EditorGUILayout.HelpBox("No children found under TutorialFrame.", MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a TutorialFrame with children to expose options.", MessageType.Info);
        }

        if (GUILayout.Button("Refresh Child List"))
        {
            TutorialVolume.UpdateChildList();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(TutorialVolume);
        }
    }
}
