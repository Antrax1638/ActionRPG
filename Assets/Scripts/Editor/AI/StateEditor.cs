using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(State))]
public class StateEditor : Editor 
{
	State Target;
	bool StatesExtend;

	void OnEnable()
	{
		Target = target as State;
	}

	public override void OnInspectorGUI()
	{
		GUILayout.Space (10);
		EditorGUILayout.BeginVertical ("Box");
		EditorGUILayout.BeginHorizontal ();
			string StatesTitle = (!StatesExtend) ? "Expand" : "Contract";
			GUILayout.Space (10);
			StatesExtend = EditorGUILayout.Foldout (StatesExtend,(StatesTitle +" Transitions"),true);
			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			if(Target.Transitions != null)
				GUILayout.Label ("Length: ["+Target.Transitions.Length+"]");
		EditorGUILayout.EndHorizontal ();
		if (StatesExtend && Target.Transitions != null)
		{
			for (int i = 0; i < Target.Transitions.Length; i++) 
			{
				EditorGUILayout.BeginHorizontal ();
				Target.Transitions [i].Enter = EditorGUILayout.ToggleLeft("Enter",Target.Transitions [i].Enter,GUILayout.MaxWidth(80));
				Target.Transitions [i].State = (State)EditorGUILayout.ObjectField (Target.Transitions [i].State, typeof(State), true);
				if (GUILayout.Button ("-"))
					Target.RemoveAt (i);
				EditorGUILayout.EndHorizontal ();
			}
		}
		EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Transition"))
				Target.AddTransition (new Transition ());
			if (GUILayout.Button ("Empty Transitions"))
				Target.Empty ();
		EditorGUILayout.EndHorizontal ();
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		EditorGUILayout.EndVertical ();

		GUILayout.Space (15);
		GUILayout.Label ("State Properties:");
		Target.Name = EditorGUILayout.TextField ("Name", Target.Name);
		Target.DebugMode = EditorGUILayout.Toggle ("Debug Mode", Target.DebugMode);
	}
}
