using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CEditor : Editor 
{
	public static void ProgressBar(float Value,string Label)
	{
		Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
		EditorGUI.ProgressBar (rect, Value, Label);
		EditorGUILayout.Space();
	}

	public static void ProgressBar(float Value,float MaxValue,string Label)
	{
		ProgressBar (Value / MaxValue, Label);
	}
		
	public static void ArrayProperty(SerializedObject Object,string Name)
	{
		SerializedProperty Property = Object.FindProperty (Name);
		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.PropertyField (Property,true);
		if (EditorGUI.EndChangeCheck ())
			Object.ApplyModifiedProperties ();
	}

	public static GUIContent ToolTip(string name,string tooltip = "")
	{
		return new GUIContent(name,tooltip);
	}

	public static Quaternion Vector4ToQuaternion(Vector4 Value)
	{
		return new Quaternion (Value.x, Value.y, Value.z, Value.w);
	}

	public static Vector4 QuaternionToVector4(Quaternion Rot)
	{
		return new Vector4(Rot.x,Rot.y,Rot.z,Rot.w);
	}

	public static string[] GetLayerMasks()
	{
		List<string> Temp = new List<string> ();
		for (int i = 0; i < 32; i++) 
		{
			if (LayerMask.LayerToName (i) != "")
				Temp.Add (LayerMask.LayerToName (i));
		}
		return Temp.ToArray ();
	}

    public static SerializedProperty GetSerializedProperty(SerializedObject Object, string Name,bool AutoUpdate = false)
    {
        SerializedProperty Property = Object.FindProperty(Name);
        if (AutoUpdate) Object.ApplyModifiedProperties();
        return Property;
    }

    public static T GetProperty<T>(SerializedObject Object, string Name) where T : IConvertible
    {
        SerializedProperty NewProperty = Object.FindProperty(Name);
        switch (NewProperty.type)
        {
            case "Integer": return (T)Convert.ChangeType(NewProperty.intValue,typeof(int));
            case "Boolean": return (T)Convert.ChangeType(NewProperty.boolValue, typeof(bool));
            case "Float": return (T)Convert.ChangeType(NewProperty.floatValue, typeof(float));
            case "String": return (T)Convert.ChangeType(NewProperty.stringValue, typeof(string));
            case "Color": return (T)Convert.ChangeType(NewProperty.colorValue, typeof(Color));
            case "ObjectReferense": return (T)Convert.ChangeType(NewProperty.objectReferenceValue, typeof(object));
            case "LayerMask": return (T)Convert.ChangeType(NewProperty.longValue, typeof(long));
            case "Enum": return (T)Convert.ChangeType(NewProperty.enumValueIndex, typeof(Enum));
            case "Vector2": return (T)Convert.ChangeType(NewProperty.vector2Value, typeof(Vector2));
            case "Vector3": return (T)Convert.ChangeType(NewProperty.vector3Value, typeof(Vector3));
            case "Vector4": return (T)Convert.ChangeType(NewProperty.vector4Value, typeof(Vector4));
            case "Rect": return (T)Convert.ChangeType(NewProperty.rectValue, typeof(Rect));
            case "Character": return (T)Convert.ChangeType(NewProperty.intValue, typeof(Char));
            case "AnimationCurve": return (T)Convert.ChangeType(NewProperty.animationCurveValue, typeof(AnimationCurve));
            case "Bounds": return (T)Convert.ChangeType(NewProperty.boundsValue, typeof(Bounds));
            case "Gradient": return (T)Convert.ChangeType(NewProperty.doubleValue, typeof(Gradient));
            case "Quaternion": return (T)Convert.ChangeType(NewProperty.quaternionValue, typeof(Quaternion));
            case "Vector2Int": return (T)Convert.ChangeType(NewProperty.vector2IntValue, typeof(Vector2Int));
            case "Vector3Int": return (T)Convert.ChangeType(NewProperty.vector3IntValue, typeof(Vector3Int));
            case "BoundsInt": return (T)Convert.ChangeType(NewProperty.boundsIntValue, typeof(BoundsInt));
            case "RectInt": return (T)Convert.ChangeType(NewProperty.rectIntValue, typeof(RectInt));
            default: return (T)Convert.ChangeType(null, typeof(int));
        }
    }
}
