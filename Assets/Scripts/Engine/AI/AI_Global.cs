using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Global : MonoBehaviour
{
    public enum AI_State
    {
        Idle,
        Patrol,
        Combat,
        None
    }

    [Header("Global Parameters")]
    public AI_State State = AI_State.Idle;
    public Transform Target;

    [HideInInspector] public Vector3 InitialPosition { get { return initialPosition; } }
    [HideInInspector] public Quaternion InitialRotation { get { return initialRotation; } }
    [HideInInspector] public GameObject CurrentState { get { return currentState; } }

    private Transform RootComponent;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private GameObject currentState;
    private FSMManager ManagerComponent;

    private void Awake()
    {
        ManagerComponent = GetComponent<FSMManager>();
        if (!ManagerComponent)
            Debug.LogError("AI_Global: FSM Manager component is null or invalid.");

        RootComponent = transform.root;
        initialPosition = RootComponent.position;
        initialRotation = RootComponent.rotation;
    }

    private void Update()
    {
        if (ManagerComponent)
            currentState = ManagerComponent.Default.gameObject;
    }
}