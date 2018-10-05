using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemManager))]
public class ItemManagerEditor : Editor
{
    private ItemManager Target;
    private int Tab;
    private string Name;

    private void OnEnable()
    {
        Target = target as ItemManager;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);

        GUILayout.Label("Path:");
        EditorGUILayout.BeginHorizontal("Box");
        Target.Path = EditorGUILayout.TextField(Target.Path);
        Target.Extension = EditorGUILayout.TextField(Target.Extension);
        EditorGUILayout.EndHorizontal();

        Tab = GUILayout.Toolbar(Tab, new string[] { "Create", "Spawn", "Options" });
        switch (Tab)
        {
            case 0:
                Name = EditorGUILayout.TextField("Name", Name);
                Target.Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", Target.Prefab, typeof(GameObject), true);
                
                if (GUILayout.Button("Create"))
                    Target.SaveItem(Name, Target.Prefab);
                break;

            case 1:
                Name = EditorGUILayout.TextField("File Name", Name);

                if(GUILayout.Button("Spawn"))
                    Target.SpawnItem(Name, Target.transform.position, Target.transform.rotation);

                
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.contentColor = Color.yellow;
                GUILayout.Label("Item spawns at manager position and rotation");
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                break;

            case 2:
                EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "Tag"));
                EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "Preview"));
                EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "Canvas"));
                EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "Controller"));
                EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "Character"));
                EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "LeftClickAction"));
                break;
        }
        GUILayout.Space(5);
        

        serializedObject.ApplyModifiedProperties();
    }
}
