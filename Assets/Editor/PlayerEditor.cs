using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerController playerController = (PlayerController)target;

        if (playerController == null)
            return;

        if (GUILayout.Button("Forced Death"))
        {
            playerController.HealthNullAction(true);
        }
    }

}
