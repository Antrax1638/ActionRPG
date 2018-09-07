using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class MovementController : MonoBehaviour
{
    [SerializeField] private bool DebugMode = false;

    [Header("Components")]
    [SerializeField] protected GameObject Character;
    [SerializeField] protected AnimationState Animation;

    [Header("Movement")]
    public float MaxMovementSpeed = 0.0f;
    public float MovementSpeedSmooth = 0.0f;
    [SerializeField] private bool MouseLook = true;
    [SerializeField] private float MouseRotationRate = 0.0f;
    
    [Header("Action Mapping")]
    [SerializeField] string ForwardAction;
    [SerializeField] string BackwardAction;
    [SerializeField] string LeftAction;
    [SerializeField] string RightAction;
    [SerializeField] string PrimaryMouseAction;
    [SerializeField] string SecondaryMouseAction;
    [SerializeField] string MiddleMouseAction;
    [SerializeField] string StayAction;

    public float MovementSpeed { get { return DeltaSpeed; } }

    private PlayerController PlayerControllerComponent;
    private CharacterController CharacterControllerComponent;
    private CameraController CameraControllerComponent;

    private float DeltaSpeed = 0.0f;
    protected Vector3 MouseDirection, MovementDirection;

	void Awake ()
    {
        PlayerControllerComponent = GetComponent<PlayerController>();
        if (!PlayerControllerComponent)
        {
            Debug.LogError("MovementController: Player Controller Component is null");
        }

        CameraControllerComponent = GetComponent<CameraController>();
        if (!CameraControllerComponent)
            Debug.LogError("MovementController: Camera Controller component is null");

	}

    private void Start()
    {
        CharacterControllerComponent = PlayerControllerComponent.GetCharacterController();
        if (!CharacterControllerComponent)
            Debug.LogError("MovementController: Character controller component is null");

        if (!Character)
            Debug.LogError("MovementController: Character object not found");
    }

    void Update ()
    {
        InputUpdate();
        AnimationUpdate();
    }

    void InputUpdate()
    {
        if (Character && Input.GetButton(PrimaryMouseAction) && MouseLook)
        {
            MouseDirection = CameraControllerComponent.GetCursor().WorldHitPosition - Character.transform.position;
            MouseDirection = Vector3.RotateTowards(Character.transform.forward, MouseDirection, MouseRotationRate * Time.deltaTime, 0.0f);
            MouseDirection = MouseDirection.normalized;
            Quaternion Rotation = Quaternion.LookRotation(MouseDirection);
            Rotation = new Quaternion(0.0f, Rotation.y, 0.0f, Rotation.w);
            Character.transform.rotation = Quaternion.Lerp(Character.transform.rotation, Rotation, 1.0f);
        }

        
        MovementDirection = Vector3.zero;
        if (PlayerControllerComponent.CanInput)
        {
            if (CharacterControllerComponent.isGrounded && !Input.GetButton(StayAction))
            {
                DeltaSpeed = Mathf.Lerp(DeltaSpeed, MaxMovementSpeed, MovementSpeedSmooth);
                
                MovementDirection = new Vector3(MouseDirection.x, transform.position.y, MouseDirection.z);
                MovementDirection = MovementDirection.normalized;
                MovementDirection *= System.Convert.ToInt16(Input.GetButton(PrimaryMouseAction));
                MovementDirection *= Mathf.Clamp(DeltaSpeed, 0.0f, MaxMovementSpeed);
                /*
                AxisDirection = new Vector3(Input.GetAxis(LeftAction) + Input.GetAxis(RightAction), 0.0f, Input.GetAxis(ForwardAction) + Input.GetAxis(BackwardAction));
                AxisDirection = CameraControllerComponent.Camera.transform.TransformDirection(AxisDirection);
                AxisDirection *= MaxSpeed;
                */
            }


            if (Input.GetButtonUp(PrimaryMouseAction) || Input.GetButtonDown(StayAction))
                DeltaSpeed = 0.0f;
        }

        

        MovementDirection.y = Physics.gravity.y;
        CharacterControllerComponent.Move(MovementDirection * Time.deltaTime);
    }

    void AnimationUpdate()
    {
        Animation.Velocity = MovementDirection;
    }

    private void OnDrawGizmos()
    {
        if (DebugMode)
        {
            Gizmos.DrawLine(transform.position,transform.position + transform.forward);
            Gizmos.DrawWireSphere(transform.position + transform.forward, 0.1f);

            Gizmos.DrawLine(Character.transform.position, Character.transform.position + Character.transform.forward);
            Gizmos.DrawWireSphere(Character.transform.position + Character.transform.forward, 0.1f);
        }
    }
}
