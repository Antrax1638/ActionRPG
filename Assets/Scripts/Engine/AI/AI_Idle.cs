using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(State))]
public class AI_Idle : MonoBehaviour
{
    public enum EIdleType
    {
        None,
        Random,
        Static
    }


    [Header("Idle")]
    public EIdleType Type;
    public int Index = 0;
    [Range(0.0f,100.0f)] public float Probability;
    [Tooltip("Time in seconds for idle animation change.")]
    public float Rate = 0.0f;
    [Tooltip("List of all idle animations ids for system pick")]
    public List<int> Idles = new List<int>();
    

    [Header("Turn")]
    public bool Turn;
    public float TurnRate;
    public List<int> Turns = new List<int>();
    public List<int> Sequence = new List<int>();
    [SerializeField] bool RootMotion = false;

    private Transform RootComponent;
    private State StateComponent;
    private Animator AnimatorComponent;
    private NavMeshAgent NavMeshAgentComponent;

    int IdleId = 0,TurnId = 0, TurnIndex = 0;
    float IdleDelta = 0.0f, TurnDelta = 0.0f;
    Vector3 InitialPosition;
    Quaternion InitialRotation,Rotation,TargetRotation;
    
    private void Awake()
    {
        RootComponent = transform.root;

        StateComponent = GetComponent<State>();
        if (!StateComponent)
            Debug.LogError("AI_Idle: State component is null or invalid.");

        AnimatorComponent = RootComponent.GetComponentInChildren<Animator>();
        if (!AnimatorComponent)
            Debug.LogError("AI_Idle: Animator component is null or invalid");

        NavMeshAgentComponent = GetComponentInParent<NavMeshAgent>();
        if (!NavMeshAgentComponent)
            Debug.LogError("AI_Idle: NavMeshAgent component is null or invalid");

        NavMeshAgentComponent.enabled = false;
        NavMeshAgentComponent.enabled = true;
    }

    private void Start ()
    {
        InitialPosition = RootComponent.transform.position;
        InitialRotation = RootComponent.transform.rotation;

        if (Type == EIdleType.Static)
        {
            IdleId = Idles[Index];
            AnimatorComponent.SetInteger("Idle", Idles[Index]);
        }

        AnimatorComponent.applyRootMotion = RootMotion;

	}

    private void Update()
    { 
        IdleDelta += Time.deltaTime;
        TurnDelta = (Turn) ? TurnDelta + Time.deltaTime : 0.0f;

        if (IdleDelta >= Rate)
        {
            if (Random.Range(0.0f, 100.0f) < Probability)
            {
                switch (Type)
                {
                    case EIdleType.None: Index++; break;
                    case EIdleType.Random: Index = Random.Range(0, Idles.Count); break;
                    case EIdleType.Static: break;
                }
            }
            Index = (Index >= Idles.Count) ? 0 : Index;
            IdleId = Idles[Index];
            AnimatorComponent.SetInteger("Idle", IdleId);
            IdleDelta = 0.0f;
        }

        if (Turn && TurnDelta >= TurnRate)
        {
            TurnIndex = (TurnIndex >= Sequence.Count) ? 0 : TurnIndex++;
            TurnId = Sequence[TurnIndex];
            AnimatorComponent.SetInteger("TurnType", TurnId);
            AnimatorComponent.SetTrigger("Turn");
            TurnDelta = 0.0f;
        }
    }

    private void LateUpdate()
    {
        //RootComponent.rotation = AnimatorComponent.rootRotation;
    }
}
