using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(State))]
public class AI_Idle : MonoBehaviour
{
    [Header("Rotation")]
    public bool Enable;
    [Range(0,100)]public float Probability;
    public float Smooth;
    public float Delay;
    public float DeadZone;
    public Vector2 Limits = new Vector2(90, -90);

    private Transform RootComponent;
    private State StateComponent;
    private Vector3 InitialPosition;
    private Quaternion InitialRotation;

    Quaternion Rotation,TargetRotation;
    bool Reach = true;
    
    private void Awake()
    {
        StateComponent = GetComponent<State>();
        if (!StateComponent)
            Debug.LogError("AI_Idle: State component is null or invalid.");

        RootComponent = transform.root;
    }

    private void Start ()
    {
        InitialPosition = RootComponent.transform.position;
        InitialRotation = RootComponent.transform.rotation;

        if(Enable)
            InvokeRepeating("IdleUpdate", 0.0f, Delay);
	}

    private void Update()
    {
        if (Enable && !Reach) {
            Rotation = RootComponent.transform.rotation;
            RootComponent.transform.rotation = Quaternion.Lerp(Rotation, TargetRotation, Smooth * Time.deltaTime);
        }

        Quaternion DeltaRotation = Utils.Substract(TargetRotation, Rotation);
        if (Quaternion.Angle(DeltaRotation, RootComponent.transform.rotation) <= (0 + DeadZone))
            Reach = true;
    }

    void IdleUpdate()
    {
        if (Random.Range(0.0f, 100.0f) <= Probability && Reach)
        {
            float Angle = Random.Range(Limits.x, Limits.y);
            TargetRotation = Quaternion.AngleAxis(Angle, Vector3.up);
            Reach = false;
        }
    }
}
