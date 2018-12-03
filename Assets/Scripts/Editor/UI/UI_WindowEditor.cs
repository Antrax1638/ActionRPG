using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UI_Window))]
public class UI_WindowEditor : Editor
{
    UI_Window Target;
    bool Win = true;
    List<RectTransform> DragFilter;

	protected void OnEnable ()
    {
        Target = target as UI_Window;
        DragFilter = new List<RectTransform>();
	}

    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal("Box");
        GUILayout.Space(15);
        EditorGUILayout.BeginVertical();
        Win = EditorGUILayout.Foldout(Win, "Window", true);
        if (Win) {
            GUILayout.Label("General Properties:");
            Target.Visible = (Visibility)EditorGUILayout.EnumPopup("Visibility", Target.Visible);
            Target.CloseKey = (KeyCode)EditorGUILayout.EnumPopup("Close Key", Target.CloseKey);

            GUILayout.Space(10);
            GUILayout.Label("Drag Properties:");
            Target.Draggable = EditorGUILayout.Toggle("Draggable", Target.Draggable);
            Target.DragModifier = (KeyModifier)EditorGUILayout.EnumPopup("Drag Modifier", Target.DragModifier);
            Target.DragOffset = EditorGUILayout.Vector2Field("Drag Offset", Target.DragOffset);

            if (DragFilter != null)
            {
                EditorGUILayout.BeginVertical("Box");
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("Drag Filters [" + DragFilter.Count + "]");
                GUI.skin.label.alignment = TextAnchor.LowerLeft;
                for (int i = 0; i < DragFilter.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    DragFilter[i] = (RectTransform)EditorGUILayout.ObjectField(DragFilter[i], typeof(RectTransform), true);
                    GUI.color = Color.red;
                    if (GUILayout.Button("[-]")) DragFilter.RemoveAt(i);
                    GUI.color = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                Target.DragFilter = DragFilter.ToArray();

                GUILayout.Space(10);
                if (GUILayout.Button("[+]"))
                    DragFilter.Add(new RectTransform());
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(10);
            GUILayout.Label("Toggle Properties:");
            Target.Toggleable = EditorGUILayout.Toggle("Toggle", Target.Toggleable);
            Target.ToggleAction = EditorGUILayout.TextField("Toggle Action", Target.ToggleAction);
            Target.ToggleModifier = (KeyModifier)EditorGUILayout.EnumPopup("Toggle Modifier", Target.ToggleModifier);

            GUILayout.Space(10);
            GUILayout.Label("Focus Properties:");
            Target.Focusable = EditorGUILayout.Toggle("Focusable", Target.Focusable);
            GUILayout.Space(10);

            //GUILayout.Label("Events Properties:");
            EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "WindowOpen"));
            EditorGUILayout.PropertyField(CEditor.GetSerializedProperty(serializedObject, "WindowClose"));
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(Target);
    }
}
