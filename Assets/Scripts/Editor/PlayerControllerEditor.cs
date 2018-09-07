using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    protected PlayerController Target;
    bool CTab;

	protected virtual void OnEnable ()
    {
        Target = (PlayerController)target;
	}

	public override void OnInspectorGUI ()
    {
        GUILayout.Space(10);
        GUI.skin.label.alignment = TextAnchor.LowerLeft;
        GUI.skin.label.normal.textColor = Color.black;
        GUI.skin.label.fontSize = 12;
        GUILayout.Label("Player Controller System");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(5);
        CTab = EditorGUILayout.Foldout(CTab, "Controller", true);
        EditorGUILayout.EndHorizontal();
        if (CTab) ControllerTab();
        

    }

    protected virtual void ControllerTab()
    {
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.skin.label.normal.textColor = Color.blue;
        EditorGUILayout.BeginVertical("Box");
        GUILayout.Label("General:");
        Target.Enabled = EditorGUILayout.Toggle("Enabled", Target.Enabled);
        Target.Type = (InputType)EditorGUILayout.EnumPopup("Type", Target.Type);
        Target.Mode = (InputMode)EditorGUILayout.EnumPopup("Mode", Target.Mode);
        Target.Name = EditorGUILayout.TextField("Name", Target.Name);
        Target.Score = EditorGUILayout.IntField("Score", Target.Score);
        //Target.Character = (GameObject)EditorGUILayout.ObjectField("Character",Target.Character, typeof(GameObject), true);

        GUILayout.Space(5);
        GUILayout.Label("Cursor:");
        Target.CursorEnabled = EditorGUILayout.Toggle("Visible",Target.CursorEnabled);
        Target.CursorLockMode = (CursorLockMode)EditorGUILayout.EnumPopup("LockMode", Target.CursorLockMode);
        EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "CursorMode"));
        EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "CursorOverride"));
        EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "CursorDefault"));
        EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "CursorHotspot"));


        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndVertical();
        

    }
}

