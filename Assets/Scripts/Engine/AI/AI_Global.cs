using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Global : MonoBehaviour
{


    [SerializeField] private bool DebugMode = false;

    [Header("Global Parameters")]
    public GameObject Target;
    public string State;

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
            ManagerComponent = GetComponentInParent<FSMManager>();
        if (!ManagerComponent)
            ManagerComponent = GetComponentInChildren<FSMManager>();
        if (!ManagerComponent)
            Debug.LogError("AI_Global: FSM Manager component is null or invalid.");

        RootComponent = transform.root;
        initialPosition = RootComponent.position;
        initialRotation = RootComponent.rotation;
    }

    private void Update()
    {
        if (ManagerComponent)
        {
            State = ManagerComponent.Default.name;
            currentState = ManagerComponent.Default.gameObject;
            
        }
    }
    
    private void OnDrawGizmos()
    {
        if (DebugMode)
        {
            Gizmos.DrawLine(transform.position, initialPosition);
            Vector3 TargetPosition = (Target) ? Target.transform.position : transform.position;
            Gizmos.DrawLine(transform.position, TargetPosition);

            if (Target) {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(TargetPosition, new Vector3(1, 1, 1));
            }
        }
    }
}