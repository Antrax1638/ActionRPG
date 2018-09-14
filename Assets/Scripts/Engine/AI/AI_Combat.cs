using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(State))]
public class AI_Combat : MonoBehaviour
{
    public enum AI_Dificulty
    {
        Low,
        Medium,
        High,
        VeryHigh,
        Ultra
    }

    [SerializeField] private bool DebugMode = false;
    
    [Header("Combat")]
    public bool Enable = false;
    public GameObject Target;
    public AI_Dificulty Dificulty = AI_Dificulty.Low;
    public float MinDistance;
    public float MaxDistance;
    public float Range;
    public float AngularSmooth;
    public float DamageRate = 5;

    [Header("States Transitions")]
    public string ReturnState;
    //public string AttackState;

    private Vector3 InitialPosition,TargetPosition,TargetDirection;
    private Quaternion InitialRotation;
    private float TargetDistance,InitialDistance;

    //Damage Factors:
    Dictionary<string, float> DamageTaken = new Dictionary<string, float>();

    //Components:
    private State StateComponent;
    private AI_Global GlobalComponent;
    private NavMeshAgent NavMeshAgentComponent;
    private Animator AnimatorComponent;

    void Awake ()
    {
        StateComponent = GetComponent<State>();
        if (!StateComponent)
            Debug.LogError("AI_Combat: State component is null or invalid");

       
        NavMeshAgentComponent = GetComponentInParent<NavMeshAgent>();
        if (!NavMeshAgentComponent)
            Debug.LogError("AI_Combat: NavMeshAgent component is null or invalid");


        GlobalComponent = transform.root.GetComponentInChildren<AI_Global>();
        if (GlobalComponent)
            Target = GlobalComponent.Target;
        else
            Debug.LogError("AI_Combat: Global component is null or invalid");


        AnimatorComponent = GetComponentInParent<Animator>();
        if (!AnimatorComponent)
            Debug.LogError("AI_Combat: Animator component is null or invalid");

        gameObject.SetActive(Enable);
        InitialPosition = transform.root.position;
        InitialRotation = transform.root.rotation;
        AnimatorComponent.applyRootMotion = false;

        InvokeRepeating("UpdateDamageTaken", 0.0f, DamageRate);
    }

	void Update ()
    {
        InitialDistance = Vector3.Distance(transform.root.position, InitialPosition);
        if (Target)
        {
            TargetDistance = Vector3.Distance(transform.root.position, Target.transform.position);
            TargetDirection = Target.transform.position - transform.root.position;
            TargetPosition = Target.transform.position - (TargetDirection.normalized * Range);
            NavMeshAgentComponent.SetDestination(TargetPosition);
            
            if ((TargetDistance - Range) >= MaxDistance)
            {
                Target = null;
                GlobalComponent.Target = null;
                NavMeshAgentComponent.SetDestination(InitialPosition);
            }
        }

        float NormalizedSpeed = Mathf.Clamp01(NavMeshAgentComponent.velocity.magnitude / NavMeshAgentComponent.speed);
        AnimatorComponent.SetFloat("Speed", NormalizedSpeed);


        if (NavMeshAgentComponent.remainingDistance <= 0.0f && NavMeshAgentComponent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            if (Target)
            {
                
                Quaternion NewRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
                transform.root.rotation = Quaternion.Lerp(transform.root.rotation, NewRotation, AngularSmooth * Time.deltaTime);
                Debug.Log("Attack and Watch");
            }
            else
            {
                transform.rotation = InitialRotation;
                if(StateComponent.Contains(ReturnState))
                    StateComponent.GetTransitionByName(ReturnState).Enter = true;
            }
        }

    }
    
    void UpdateDamageTaken()
    {
        float[] CachedValues = new float[DamageTaken.Values.Count];
        DamageTaken.Values.CopyTo(CachedValues,0);
        float MaxValue = Mathf.Max(CachedValues);
        string MaxValueKey = DamageTaken.FirstOrDefault(x => x.Value == MaxValue).Key;
        Debug.Log("Damage Key:" + MaxValueKey + " Value:" + MaxValue);
        CachedValues = new float[0];
    }

    private void OnDrawGizmos()
    {
        if (DebugMode) {

            if (Target)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(Target.transform.position + Target.transform.up, 0.1f);
            }
                
        }
    }
}
