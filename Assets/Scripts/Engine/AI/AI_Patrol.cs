using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Patrol : MonoBehaviour
{
    public enum EPatrolType
    {
        Loop,
        ReverseLoop,
        None
    }

    [Header("Patrol")]
    public string Waypoints;
    public float Rate;
    public EPatrolType Type = EPatrolType.Loop;
    public int LoopTimes = 0;

    [Header("State Transition")]
    public string ExitState;

    private Animator AnimatorComponent;
    private NavMeshAgent NavMeshAgentComponent;
    private State StateComponent;
    private AI_Global GlobalComponent;

    Transform[] WaypointList;
    int CurrentWaypoint = 0,LoopCount = 0;
    float DeltaWaypointTime = 0.0f;
    bool Reverse;

	void Awake ()
    {
        AnimatorComponent = GetComponentInParent<Animator>();
        if (!AnimatorComponent) Debug.LogError("AI_Patrol: Animator component is null");

        NavMeshAgentComponent = GetComponentInParent<NavMeshAgent>();
        if (!NavMeshAgentComponent) Debug.LogError("AI_Patrol: NavMeshAgent component is null");

        StateComponent = GetComponent<State>();
        if (!StateComponent) Debug.LogError("AI_Patrol: State component is null");

        GlobalComponent = transform.root.GetComponentInChildren<AI_Global>();
        if (!GlobalComponent) Debug.LogError("AI_Patrol: Global component is null");

        Transform WayPoint = GameObject.Find(Waypoints).transform;
        WaypointList = new Transform[WayPoint.childCount];
        for(int i = 0; i < WaypointList.Length; i++)
        {
            WaypointList[i] = WayPoint.GetChild(i);
        }
	}

	void Update()
    {
        DeltaWaypointTime += Time.deltaTime;

        switch (Type)
        {
            case EPatrolType.Loop:
                CurrentWaypoint = (CurrentWaypoint >= WaypointList.Length) ? 0 : CurrentWaypoint;
                break;
            case EPatrolType.ReverseLoop:
                if (CurrentWaypoint >= WaypointList.Length)
                {
                    Reverse = true;
                    CurrentWaypoint = WaypointList.Length - 1;
                }
                else if(CurrentWaypoint <= 0)
                {
                    Reverse = false;
                }

                //CurrentWaypoint = (CurrentWaypoint >= WaypointList.Length) ? WaypointList.Length - 1 : CurrentWaypoint;
                //Reverse = (CurrentWaypoint >= WaypointList.Length);
                break;
            case EPatrolType.None:
                if (StateComponent.Contains(ExitState) && CurrentWaypoint >= WaypointList.Length)
                    StateComponent.GetTransitionByName(ExitState).Enter = true;
                break;
        }
        
        float NormalizedSpeed = NavMeshAgentComponent.velocity.magnitude / NavMeshAgentComponent.speed;
        AnimatorComponent.SetFloat("Speed", Mathf.Clamp01(NormalizedSpeed));

        bool Valid = DeltaWaypointTime >= Rate && NavMeshAgentComponent.pathStatus == NavMeshPathStatus.PathComplete;
        if (Valid && !GlobalComponent.Target)
        {
            LoopCount = (CurrentWaypoint >= WaypointList.Length - 1) ? LoopCount + 1 : LoopCount;

            int NextWaypoint = CurrentWaypoint + 1;
            NextWaypoint = (NextWaypoint >= WaypointList.Length) ? 0 : NextWaypoint;

            CurrentWaypoint = Mathf.Clamp(CurrentWaypoint, 0, WaypointList.Length);

            Vector3 Direction = transform.root.position - WaypointList[CurrentWaypoint].position;
            Vector3 NextDirection = transform.root.position - WaypointList[NextWaypoint].position;

            NavMeshAgentComponent.SetDestination(WaypointList[CurrentWaypoint].position);
            DeltaWaypointTime = 0.0f;

            switch (Type)
            {
                case EPatrolType.Loop: CurrentWaypoint++; break;
                case EPatrolType.ReverseLoop: CurrentWaypoint = (Reverse) ? CurrentWaypoint - 1 : CurrentWaypoint + 1;  break;
                case EPatrolType.None: CurrentWaypoint++; break;
            }
        }

        if(LoopCount >= LoopTimes && StateComponent.Contains(ExitState) && LoopTimes > 0)
        {
            StateComponent.GetTransitionByName(ExitState).Enter = true;
        }
	}
}
