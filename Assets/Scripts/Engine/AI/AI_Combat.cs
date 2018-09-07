using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Components")]
    [SerializeField]private CharacterController Controller;

    [Header("Combat")]
    public bool Enable = false;
    public GameObject Target;
    public Vector3 TargetPosition;
    public AI_Dificulty Dificulty = AI_Dificulty.Low;
    public float MinDistance;
    public float MaxDistance;

    //[Header("Threat")]
    //public float 

    private State StateComponent;
    private Dictionary<int, float> DamageSource = new Dictionary<int, float>();
    private Vector3 InitialCombatPosition;

    private float TargetDistance,InitialDistance;

	void Awake () {
        StateComponent = GetComponent<State>();
        if (!StateComponent)
            Debug.LogError("AI_Combat: State component is null or invalid");

        if (!Controller)
            Debug.LogError("AI_Combat: Controller component is null or invalid");

        if (!Target)
            Debug.LogWarning("AI_Combat: Target object is null or invalid");

        gameObject.SetActive(Enable);
        InitialCombatPosition = transform.root.position;
	}

	void Update ()
    {
        InitialDistance = Vector3.Distance(transform.root.position, InitialCombatPosition);
        if (Target)
        {
            TargetDistance = Vector3.Distance(transform.root.position, Target.transform.position);
        }
        

        if(InitialDistance <= MinDistance)
        {
            Debug.Log("Stay Idle");
        }

        if (Controller && Controller.isGrounded)
        {
            
        }
	}

    public void AddTarget(int id)
    {
        if (!DamageSource.ContainsKey(id))
        {
            DamageSource.Add(id, 0.0f);
        }
    }

    public bool RemoveTarget(int id)
    {
        return DamageSource.Remove(id);
    }

    public void AddDamage(int id, float Damage)
    {
        if (DamageSource.ContainsKey(id))
        {
            DamageSource[id] += Damage;
        }
    }

    public void RemoveDamage(int id, float Damage)
    {
        if (DamageSource.ContainsKey(id))
        {
            DamageSource[id] -= Damage;
        }
    }

}
