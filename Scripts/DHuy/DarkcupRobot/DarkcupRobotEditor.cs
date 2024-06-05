#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DarkcupRobot))]
public class DarkcupRobotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        DarkcupRobot robot = (DarkcupRobot)target;
        if (GUILayout.Button("PREVIOUS", GUILayout.Width(150), GUILayout.Height(50)))
        {
            Debug.Log("this is pikachu");
            robot.Previous();
        }
        if (GUILayout.Button("NEXT", GUILayout.Width(150), GUILayout.Height(50)))
        {
            Debug.Log("this is pikachu");
            robot.Next();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
#endif