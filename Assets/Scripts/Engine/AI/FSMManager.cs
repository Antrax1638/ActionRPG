using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMManager : MonoBehaviour 
{
    [Header("States:")]
    [Tooltip("Default state when the object is instantiated in the scene")]
    public State Default;

	private GameObject DefaultObject;

	void Start () 
	{

        if (Default) {
            DefaultObject = Instantiate(Default.gameObject, transform);
            Default = DefaultObject.GetComponent<State>();
            Default.transform.localPosition = Vector3.zero;
            Default.transform.localRotation = Quaternion.identity;
        }
        else {
            Debug.LogError("FSM Manager: Default State Object is null or not found.");
        }
        
	}

	void Update()
	{
		MakeTransition ();	
	}

	void MakeTransition()
	{
		if(Default && Default.TransitionPass)
		{
			GameObject OldObject = DefaultObject;
			State OldState = Default;
			if (OldState.TransitionState != null) 
			{
				DefaultObject = Instantiate (OldState.TransitionState.gameObject,transform);
				DefaultObject.transform.localPosition = Vector3.zero;
				Default = DefaultObject.GetComponent<State> ();
				Destroy (OldObject);
			}
		}
	}
}
