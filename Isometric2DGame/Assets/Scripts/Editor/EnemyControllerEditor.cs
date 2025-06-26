using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class EnemyControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();

        // Reference to the target script
        EnemyController enemy = (EnemyController)target;
        GUILayout.Space(10);
        // Add button
        if (GUILayout.Button("Change Waypoints"))
        {
            enemy.ChangeWaypoints();
        }
    }
}