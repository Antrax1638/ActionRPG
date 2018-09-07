using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Component")]
    public GameObject Controller;

    protected CharacterController CharacterControllerComponent = null;

	protected virtual void Awake ()
    {
        CharacterControllerComponent = Controller.GetComponent<CharacterController>();
        if (!CharacterControllerComponent) CharacterControllerComponent = Controller.GetComponentInChildren<CharacterController>();
        if (!CharacterControllerComponent) CharacterControllerComponent = Controller.GetComponentInParent<CharacterController>();
        if (!CharacterControllerComponent) Debug.LogError("Character: Character Controller is null or invalid");
    }
	
	
}
