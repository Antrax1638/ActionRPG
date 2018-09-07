using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CameraMode
{
    Locked,
    Free,
    Confined,
    None
}

[System.Serializable]
public enum CameraAxis
{
    UpAxis,
    RightAxis,
    ForwardAxis,
    None
}

public class CursorBase : _Cursor
{
    public Vector3 WorldPosition;
    public Vector3 WorldHitPosition;
    public Vector3 WorldDirection;
    public LayerMask RayMask;
}

[RequireComponent(typeof(PlayerController))]
public class CameraController : MonoBehaviour
{
    [Header("General")]
    public bool Enabled = true;
    public bool DeltaTime = false;
    public GameObject Camera;
    public Vector3 Offset;
    public GameObject Target;
    public float TransitionSmooth;
    [SerializeField] private CameraMode Mode;
    [SerializeField] private bool DebugMode = false;

    [Header("Camera Culling Mask")]
    [SerializeField] private bool Culling;
    [SerializeField] private float CullingRadius;
    [SerializeField] private float CullingOffset;
    [SerializeField] private LayerMask CullingMaskFilter;
    private float DefaultNearClipPlane;
    private Vector3 CullingHitPosition;

    [Header("Zoom")]
    [SerializeField][Range(0,1)] protected float Zoom;
    [SerializeField] protected float ZoomSpeed = 0.1f;
    [SerializeField] protected float ZoomSmooth = 1.0f;
    [SerializeField] protected Vector2 ZoomLimits;

    [Header("Rotation")]
    public CameraAxis Axis = CameraAxis.None;
    public float Speed;
    [Range(0.0f,1.0f)] public float RotationSmooth;
    
    [Header("Cursor")]
    public bool RayCast = true;
    public float RayLength = 10.0f;
    public LayerMask RayMask;
    [Range(0,100)] public float DeadZoneX;
    [Range(0,100)] public float DeadZoneY;
    public float DeadZoneSpeed;

    [Header("Input Action")]
    [SerializeField] private string TurnLeftAction = "";
    [SerializeField] private string TurnRightAction = "";
    [SerializeField] private string BlockViewAction = "";
    [SerializeField] private string ZoomAction = "";

    protected Vector2 MousePosition;
    protected Vector3 MouseWorldPosition;
    protected Camera CameraComponent;
    protected PlayerController PlayerControllerComponent;

    //Variables Privadas
    private RaycastHit CursorHitData;
    private bool CursorHit;
    private Vector3 InitialPositionOffset, DeltaOffset;
    CursorBase CursorData;
    Vector3 DeltaZoom;
    

    void Awake ()
    {
        CameraComponent = Camera.GetComponent<Camera>();
        if (!CameraComponent)
            Debug.LogError("Camera Controller: Camera component is null");

        PlayerControllerComponent = GetComponent<PlayerController>();
        if (!PlayerControllerComponent)
            Debug.LogError("Camera Controller: Player controller component is null");

        InitialPositionOffset = Offset;
        DeltaOffset = Vector3.zero;

        CursorData = new CursorBase();
        CursorData.RayMask = RayMask;
        DefaultNearClipPlane = CameraComponent.nearClipPlane;
	}

    private void Start()
    {
        if (CameraComponent)
        {
            CameraComponent.transform.position = Target.transform.position + Offset;
            CameraComponent.transform.LookAt(Target.transform.position);
            
            float Current = Mathf.Lerp(ZoomLimits.x, ZoomLimits.y, Zoom);
            Offset = Utils.ClampMagnitude(Offset, Current , Current);
        }


    }

    void Update ()
    {
        CursorData.Visible = Cursor.visible;
        CursorData.LockState = Cursor.lockState;
        CameraCullingMask();

        if (CameraComponent)
        {
            MousePosition = Input.mousePosition;
            Vector3 MouseNormalized = new Vector3(Input.mousePosition.x, Input.mousePosition.y, CameraComponent.nearClipPlane);
            MouseWorldPosition = CameraComponent.ScreenToWorldPoint(MouseNormalized);
            Vector3 MouseDirection = MouseWorldPosition - CameraComponent.transform.position;
            CursorData.WorldDirection = MouseDirection;
            CursorData.WorldPosition = MouseWorldPosition;
            MouseDirection *= RayLength;
            if (RayCast)
            {
                CursorHit = Physics.Raycast(CameraComponent.transform.position, MouseDirection, out CursorHitData, RayLength, RayMask);
            }
            CursorData.WorldHitPosition = CursorHitData.point;
        }


        if (PlayerControllerComponent.CanInput)
        {
            if (Input.GetButton(TurnLeftAction))
            {
                switch (Axis)
                {
                    case CameraAxis.ForwardAxis: Offset = Quaternion.AngleAxis(-Speed, Vector3.forward) * Offset; break;
                    case CameraAxis.RightAxis: Offset = Quaternion.AngleAxis(-Speed, Vector3.right) * Offset; break;
                    case CameraAxis.UpAxis: Offset = Quaternion.AngleAxis(-Speed, Vector3.up) * Offset; break;
                    default: break;
                }
            }

            if (Input.GetButton(TurnRightAction))
            {
                switch (Axis)
                {
                    case CameraAxis.ForwardAxis: Offset = Quaternion.AngleAxis(Speed, Vector3.forward) * Offset; break;
                    case CameraAxis.RightAxis: Offset = Quaternion.AngleAxis(Speed, Vector3.right) * Offset; break;
                    case CameraAxis.UpAxis: Offset = Quaternion.AngleAxis(Speed, Vector3.up) * Offset; break;
                    default: break;
                }
            }

            //float RotationRate = (DeltaTime) ? RotationSmooth * Time.deltaTime : RotationSmooth;
            float TransitionRate = (DeltaTime) ? TransitionSmooth * Time.deltaTime : TransitionSmooth;
            Vector3 TargetPosition = Target.transform.position + Offset;
            Vector3 CurrentPosition = CameraComponent.transform.position;
            
            //Offset = NewOffset;
            if (Input.GetAxis(ZoomAction) != 0.0f)
            {
                Vector3 Direction = CameraComponent.transform.forward * Input.GetAxis(ZoomAction);
                Direction = Direction.normalized;
                Direction *= ZoomSpeed;

                
 

                Offset = Vector3.Lerp(Offset, Offset + Direction, ZoomSmooth * Time.deltaTime);
                Offset = Utils.ClampMagnitude(Offset, ZoomLimits.x, ZoomLimits.y);
            }

            /*
            Vector3 MinDir = CameraComponent.transform.forward * ZoomLimits.x;
            Vector3 MaxDir = CameraComponent.transform.forward * ZoomLimits.y;
            Vector3 NewOffset = Offset + Vector3.Lerp(MinDir, MaxDir, Zoom);
            Debug.Log(" Dir: " + Offset.magnitude);*/

            switch (Mode)
            {
                //Locked
                case CameraMode.Locked:
                    CameraComponent.transform.position = Vector3.Lerp(CurrentPosition, TargetPosition, TransitionRate);
                    CameraComponent.transform.LookAt(Target.transform.position);
                    break;
                //Free
                case CameraMode.Free:
                    break;
                //Confined
                case CameraMode.Confined:

                    if (Input.GetButton(BlockViewAction))
                    {
                        Vector2 ScreenPercentage = Vector2.zero;
                        ScreenPercentage.x = Screen.width * Mathf.Clamp01(DeadZoneX / 100.0f);
                        ScreenPercentage.y = Screen.height * Mathf.Clamp01(DeadZoneY / 100.0f);

                        if (MousePosition.x < (ScreenPercentage.x)) { DeltaOffset.x += DeadZoneSpeed; }
                        if (MousePosition.x > (Screen.width - ScreenPercentage.x)) { DeltaOffset.x -= DeadZoneSpeed; }
                        if (MousePosition.y < (ScreenPercentage.y)) { DeltaOffset.z += DeadZoneSpeed; }
                        if (MousePosition.y > (Screen.height - ScreenPercentage.y)) { DeltaOffset.z -= DeadZoneSpeed; }
                    }
                    else
                    {
                        CameraComponent.transform.LookAt(Target.transform.position);
                    }

                    CameraComponent.transform.position = Vector3.Lerp(CurrentPosition, TargetPosition + DeltaOffset, TransitionRate);
                    break;
                default: break;
            }

        }
	}

    public void ResetCamera()
    {
        Offset = InitialPositionOffset;
        DeltaOffset = Vector3.zero;
    }

    public CursorBase GetCursor()
    {
        return CursorData;
    }

    protected virtual void CameraCullingMask()
    {
        if (Culling)
        {
            RaycastHit Hit;
            if (Physics.SphereCast(CameraComponent.transform.position, CullingRadius, CameraComponent.transform.forward, out Hit, RayLength, CullingMaskFilter))
            {
                GameObject Obj = Hit.collider.gameObject;
                if (Obj != Target)
                {
                    float Distance = Vector3.Distance(CameraComponent.transform.position, Hit.point);
                    CameraComponent.nearClipPlane = Distance + CullingOffset;
                    CullingHitPosition = Hit.point;
                }
                else
                {
                    CameraComponent.nearClipPlane = DefaultNearClipPlane;
                    CullingHitPosition = Hit.point;
                }
            }
        }
        else {
            CameraComponent.nearClipPlane = DefaultNearClipPlane;
            CullingHitPosition = Target.transform.position;
        }
            
    }

    private void OnDrawGizmos()
    {
        if (DebugMode)
        {
            if (CameraComponent)
            {
                Gizmos.DrawLine(CameraComponent.transform.position, CursorData.WorldHitPosition);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(CameraComponent.transform.position,CameraComponent.transform.forward);
                Gizmos.DrawWireSphere(CameraComponent.transform.position + CameraComponent.transform.forward, 0.05f);
                Gizmos.color = Color.gray;

                Gizmos.DrawLine(CameraComponent.transform.position, CullingHitPosition);
            }

            Gizmos.DrawSphere(CursorHitData.point, 0.15f);
        }
    }
}
