using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySwarmSpawner))]
public class SwarmEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemySwarmSpawner swarmTarget = (EnemySwarmSpawner)target;

        if (swarmTarget == null)
            return;

        if (GUILayout.Button("Forced Spawn Swarm"))
        {
            swarmTarget.SpawnSwarm();
        }
    }
}
