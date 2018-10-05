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
    [SerializeField] private float UpdateRate = 5;

    [Header("Attack")]
    public List<int> AttackList = new List<int>();
    public float AttackSpeed;
    private bool AttackState = false;
    private int CurrentAttackId = 0;
    private float CurrentAttackDelta = 0.0f;

    [Header("Fear")]
    public bool Fear = false;
    public float FearRate;
    public Vector2 AngleLimits = new Vector2(0,360);
    public Vector2 RangeLimits = new Vector2(1.0f, 8.0f);

    [Header("States Transitions")]
    public string ReturnState;
    

    private Vector3 InitialPosition,TargetPosition,TargetDirection;
    private Quaternion InitialRotation;
    private float TargetDistance,InitialDistance,FearDeltaTime;

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
        FearDeltaTime = 0.0f;

        InvokeRepeating("UpdateDamageTaken", 0.0f, UpdateRate);
    }

	void Update ()
    {
        float NormalizedSpeed = Mathf.Clamp01(NavMeshAgentComponent.velocity.magnitude / NavMeshAgentComponent.speed);
        AnimatorComponent.SetFloat("Speed", NormalizedSpeed);
        AnimatorComponent.SetFloat("AttackSpeed", AttackSpeed);

        if (Fear)
        {
            FearDeltaTime += Time.deltaTime;
            if (FearDeltaTime >= FearRate)
            {
                float Angle = UnityEngine.Random.Range(AngleLimits.x, AngleLimits.y);
                float Range = UnityEngine.Random.Range(RangeLimits.x, RangeLimits.y);
                Vector3 NewPosition = Quaternion.AngleAxis(Angle, Vector3.up) * (transform.root.forward * Range);
                NavMeshAgentComponent.SetDestination(NewPosition);
                FearDeltaTime = 0.0f;
            }

            return;
        }

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

        if (NavMeshAgentComponent.remainingDistance <= 0.0f && NavMeshAgentComponent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            if (Target)
            {
                Quaternion NewRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
                transform.root.rotation = Quaternion.Lerp(transform.root.rotation, NewRotation, AngularSmooth * Time.deltaTime);
                
                RaycastHit AttackHit;
                if(Physics.Raycast(transform.root.position, TargetDirection, out AttackHit, Range))
                {
                    if (!AttackState)
                    {
                        AttackState = true;
                        CurrentAttackId = (CurrentAttackId >= AttackList.Count) ? 0 : CurrentAttackId;
                        AnimatorComponent.SetTrigger("Attack");
                        AnimatorComponent.SetInteger("AttackType", AttackList[CurrentAttackId]);
                        CurrentAttackDelta = 0.0f;
                        CurrentAttackId++;
                    }
                    else
                    {
                        CurrentAttackDelta += Time.deltaTime;
                        AttackState = (CurrentAttackDelta <= AnimatorComponent.GetCurrentAnimatorStateInfo(0).length);
                    }
                }
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

        GameObject ObjectFound = GameObject.Find(MaxValueKey);
        if(ObjectFound)
            Target = GameObject.Find(MaxValueKey);

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
