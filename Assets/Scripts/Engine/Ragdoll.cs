using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [Header("Ragdoll")]
    public bool AutoActivate = false;

    protected Rigidbody[] BodyComponent;
    protected Collider[] CollisionComponent;

    private void Awake()
    {
        BodyComponent = GetComponentsInChildren<Rigidbody>();
        if (BodyComponent.Length <= 0)
            Debug.LogError("Ragdoll: Body components are null or invalid");

        CollisionComponent = GetComponentsInChildren<Collider>();
        if(CollisionComponent.Length <= 0)
            Debug.LogError("Ragdoll: Collision components are null or invalid");

    }

    private void Start ()
    {
        SetActive(AutoActivate);
	}

    public bool SetActive(bool NewActive)
    {
        bool Success = true;
        for (int i = 0; i < BodyComponent.Length; i++)
        {
            BodyComponent[i].isKinematic = !NewActive;
            Success &= BodyComponent[i].isKinematic;
        }
        return Success;
    }

}
