﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(State))]
public class AI_Sences : MonoBehaviour 
{
	public enum Detection
	{
		Relaxed,
		Scared,
		Alerted,
		Detected,
	}

    [SerializeField] private bool DebugMode = false;

	[Header("General Sences")]
	public bool Enable;
    public float Memory = 0.0f;
	public float FieldOfView = 90.0f;
    public Detection State;
    public float Instinct;

    [Header("Filters:")]
    [Tooltip("Currently used player controller-character tag.")]
    public string PlayerTag = "Player";
    [Tooltip("Filter all objects that this system can detect in the inmediate range.")]
    public string[] Tags;

    [Header("Threat:")]
	public float ThreatGainRate;
	public float ThreatLossRate;
    public float ThreatLimit;
    public float ThreatDelay;
	public float ThreatLossMultiplier = 2.0f;
    public float ThreatGainMultiplier = 2.0f;
    public string ThreatEnter;

    //Componentes:
    private NavMeshAgent NavMeshAgentComponent;
    private AI_Global GlobalComponent;
    private SphereCollider SphereComponent;
	private State StateComponent;
    
    //Diccionarios y Listas:
	private Dictionary<string,GameObject> ObjectsSenced = new Dictionary<string, GameObject>();
	private Dictionary<string,float> ObjectsThreat = new Dictionary<string, float> ();
    private Dictionary<string, CharacterController> ObjectsController = new Dictionary<string, CharacterController>();
    private List<string> ObjectInside = new List<string>();
   
    //Variables:
    RaycastHit Hit;
	Vector3 Direction,Velocity;
    //bool ObjectDetected = false;

	void Awake()
	{
		SphereComponent = GetComponent<SphereCollider> ();
		if(!SphereComponent)
			Debug.LogError("AI_Sences: Sphere component is null");

		StateComponent = GetComponent<State> ();
		if(!StateComponent)
			Debug.LogError("AI_Sences: State component is null");

        NavMeshAgentComponent = GetComponentInParent<NavMeshAgent>();
        if (!NavMeshAgentComponent)
            Debug.LogError("AI_Sences: NavMeshAgent component is null");

        GlobalComponent = transform.root.GetComponentInChildren<AI_Global>();
        if (!GlobalComponent)
            Debug.LogError("AI_Sences: Global component is null");

        if (Tags == null || Tags.Length <= 0) {
			Tags = new string[1];
			Tags [0] = PlayerTag;
		}
	}

	void Update()
	{
        if (NavMeshAgentComponent)
            Velocity = NavMeshAgentComponent.velocity;    
	}

	void OnTriggerEnter(Collider Other)
	{
		if (!Enable)
			return;

		for (int i = 0; i < Tags.Length; i++)
        {
			if (Other.tag == Tags [i])
            {
                if(!ObjectsSenced.ContainsKey(Other.name))
                    ObjectsSenced.Add (Other.name, Other.gameObject);

                if(!ObjectsThreat.ContainsKey(Other.name))
                    ObjectsThreat.Add (Other.name, 0.0f);

                if (!ObjectsController.ContainsKey(Other.name))
                    ObjectsController.Add(Other.name,GetObjectController(Other.gameObject));

                if (!ObjectInside.Contains(Other.name))
                    ObjectInside.Add(Other.name);
			}	
		}
    }

	void OnTriggerStay(Collider Other)
	{
		if (!ObjectsSenced.ContainsKey (Other.name))
			return;
        
        bool OtherMovement = (ObjectsController.ContainsKey(Other.name) && ObjectsController[Other.name].velocity.magnitude > 0.0f);


        bool TargetHit;
        Vector3 Distance;
		Distance = ObjectsSenced [Other.name].transform.position - transform.position;
		Direction = Distance.normalized;
		float Angle = Vector3.Angle (Direction, transform.forward);
        if (Angle < (FieldOfView/2.0f))
		{
            State = Detection.Alerted;
            TargetHit = Physics.Raycast(transform.position, Direction, out Hit, Distance.magnitude * 2);
            TargetHit = TargetHit && (Hit.collider.transform.root.name == Other.name);
            if (TargetHit && ObjectsThreat.ContainsKey(Other.name))
            {
                ObjectsThreat[Other.name] += ((ThreatGainRate * Time.deltaTime) * ThreatGainMultiplier);
            }
            else
            {
                if (OtherMovement && ObjectsThreat.ContainsKey(Other.name)) {
                    ObjectsThreat[Other.name] += (ThreatGainRate * Time.deltaTime);
                }
                else if(ObjectsThreat.ContainsKey(Other.name))
                {
                    float VelocityFactor = (Velocity.magnitude > 0.0f) ? ThreatLossMultiplier : 1.0f;
                    ObjectsThreat[Other.name] -= (ThreatLossRate * Time.deltaTime) * VelocityFactor;
                    ObjectsThreat[Other.name] += Instinct * Time.deltaTime;
                }
            }
        }
		else
		{
            State = Detection.Alerted;
            if (OtherMovement && ObjectsThreat.ContainsKey(Other.name))
            {
                ObjectsThreat[Other.name] += ((ThreatGainRate * Time.deltaTime));
            }
            else if (ObjectsThreat.ContainsKey(Other.name))
            {
                float VelocityFactor = (Velocity.magnitude > 0.0f) ? ThreatLossMultiplier : 1.0f;
                ObjectsThreat[Other.name] -= (ThreatLossRate * Time.deltaTime) * VelocityFactor;
                ObjectsThreat[Other.name] += Instinct * Time.deltaTime;
            }
        }
        ObjectsThreat[Other.name] = Mathf.Clamp(ObjectsThreat[Other.name], 0.0f, ThreatLimit);

        if (ObjectsThreat[Other.name] >= ThreatLimit) 
        {
            StartCoroutine(ThreatAdd(ThreatDelay, Other.name));
        }
	}

	void OnTriggerExit(Collider Other)
	{
        ObjectInside.Remove(Other.name);

        StartCoroutine(MemoryRemove(Memory, Other.name));

        if (ObjectsSenced.Count <= 0)
        {
            ObjectsSenced.Clear();
            ObjectsThreat.Clear();
            ObjectsController.Clear();
            State = Detection.Relaxed;
        }
	}

    IEnumerator MemoryRemove(float Delay, string Name)
    {
        yield return new WaitForSeconds(Delay);
        if (!ObjectInside.Contains(Name))
        {
            ObjectsSenced.Remove(Name);
            ObjectsThreat.Remove(Name);
            ObjectsController.Remove(Name);
        }
    }

    IEnumerator ThreatAdd(float Delay,string Name)
    {
        yield return new WaitForSeconds(Delay);
        State = Detection.Detected;

        if (ObjectsSenced.ContainsKey(Name))
        {
            GlobalComponent.Target = ObjectsSenced[Name];
        }
        else
        {
            Debug.LogWarning("Key Invalid");
        }

        if (StateComponent.Contains(ThreatEnter))
            StateComponent.GetTransitionByName(ThreatEnter).Enter = true;
    }

    protected CharacterController GetObjectController(GameObject Object)
    {
        if (Object) {
            CharacterController Temp = null;

            Temp = Object.GetComponent<CharacterController>();
            Temp = (Temp) ? Temp : Object.GetComponentInChildren<CharacterController>();
            Temp = (Temp) ? Temp : Object.GetComponentInParent<CharacterController>();
            return Temp;
        }
        return null;
    }

    

    void OnDrawGizmos()
	{
        if (DebugMode)
        {
            Gizmos.DrawRay(transform.position, Direction);
            Gizmos.DrawRay(transform.position, transform.forward);
            Gizmos.DrawWireSphere(transform.position + transform.forward, 0.1f);

            if (ObjectsSenced.Count >= 0 && ObjectsThreat.Count >= 0) {
                foreach (KeyValuePair<string, GameObject> entry in ObjectsSenced)
                {
                    Gizmos.DrawLine(transform.position, entry.Value.transform.position);

                    Gizmos.color = Color.Lerp(Color.green, Color.red, ObjectsThreat[entry.Key] / ThreatLimit);
                    Vector3 ObjectPositon = entry.Value.transform.position;
                    ObjectPositon.y = 0.0f;
                    Gizmos.DrawSphere(ObjectPositon, 0.15f);
                }
                Gizmos.color = Color.grey;
            }
            
        }

        
	}
}
