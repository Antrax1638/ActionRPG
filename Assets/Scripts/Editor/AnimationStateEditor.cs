using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[CustomEditor(typeof(AnimationState))]
public class AnimationStateEditor : Editor
{
    AnimationState Target;

    void OnEnable () {
        Target = (AnimationState)target;
	}

    public override void OnInspectorGUI ()
    {
        GUILayout.Space(10);
        DetectComponents();
    }

    protected virtual void DetectComponents()
    {
        GUILayout.Label("Requiered Components: (In Children)");
        AnimatorController Temp;

        Target.Controller = (GameObject)EditorGUILayout.ObjectField("Controller", Target.Controller, typeof(GameObject), true);


        GUI.backgroundColor = Color.gray;
        EditorGUILayout.BeginVertical("Box");
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        if (Target.GetComponentInChildren<Animator>())
        {
            GUI.skin.label.normal.textColor = Color.green;
            GUILayout.Label("Detected Animator");
            Temp = Target.GetComponentInChildren<Animator>().runtimeAnimatorController as AnimatorController;
            if (Temp)
            {
                GUI.skin.label.normal.textColor = Color.green;
                GUILayout.Label("Detected Animator Controller");
            }
            else
            {
                GUI.skin.label.normal.textColor = Color.red;
                GUILayout.Label("UnDetected Animator Controller");
            }
        }
        else
        {
            GUI.skin.label.normal.textColor = Color.red;
            GUILayout.Label("UnDetected Animator");
        }
        if (Target.GetComponentInChildren<AnimationEvents>())
        {
            GUI.skin.label.normal.textColor = Color.green;
            GUILayout.Label("Detected Animation Events");
        }
        else
        {
            GUI.skin.label.normal.textColor = Color.red;
            GUILayout.Label("UnDetected Animation Events");
        }
        if(Target.Controller)
        {
            if(Target && Target.GetComponent<PlayerController>())
            {
                GUI.skin.label.normal.textColor = Color.green;
                GUILayout.Label("Detected Controller Reference");
            }
            else
            {
                GUI.skin.label.normal.textColor = Color.red;
                GUILayout.Label("UnDetected Controller Reference");
            }
        }
        else
        {
            GUI.skin.label.normal.textColor = Color.red;
            GUILayout.Label("UnDetected Controller Reference");
        }
        GUI.skin.label.normal.textColor = Color.black;
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = Color.white;

        }

}
