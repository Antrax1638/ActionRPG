using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType
{
    Physics,
    NoPhysics,
    Custom,
}

public enum InputMode
{
    None,
    GameOnly,
    InterfaceOnly,
    All
};

public class _Cursor
{
    public bool Visible;
    public bool Override;
    public CursorLockMode LockState;
    public Texture2D Icon;
    public Vector2 Offset;
}

public class PlayerController : MonoBehaviour
{
    [Header("Controller")]
    public bool Enabled = true;
    public string Name = "PlayerController";
    public InputMode Mode = InputMode.GameOnly;
    public bool CursorEnabled = true;
    public CursorLockMode CursorLockMode = CursorLockMode.None;
    public InputType Type = InputType.Custom;
    public int Score = 0;
   
    public bool CanInput { get { return (Mode == InputMode.GameOnly || Mode == InputMode.All) && Enabled; } }

    [SerializeField] private bool CursorOverride = false;
    [SerializeField] private UnityEngine.CursorMode CursorMode = CursorMode.Auto;
    [SerializeField] private Texture2D CursorDefault = null;
    [SerializeField] private Vector2 CursorHotspot = Vector2.zero;

    protected CharacterController ControllerComponent = null;
    protected Rigidbody BodyComponent = null;
    private _Cursor CursorData = null;

    protected virtual void Awake()
    {
        switch (Type)
        {
            case InputType.Custom: break;

            case InputType.Physics:
                BodyComponent = GetComponent<Rigidbody>();
                if (!BodyComponent)
                    BodyComponent = GetComponentInChildren<Rigidbody>();
                if (!BodyComponent)
                    BodyComponent = GetComponentInParent<Rigidbody>();
                if (!BodyComponent)
                    Debug.LogError(name + ": Body component not found ");
                break;

            case InputType.NoPhysics:
                ControllerComponent = GetComponent<CharacterController>();
                if (!ControllerComponent)
                    ControllerComponent = GetComponentInChildren<CharacterController>();
                if (!ControllerComponent)
                    ControllerComponent = GetComponentInParent<CharacterController>();
                if (!ControllerComponent)
                    Debug.LogError(name + ": Controller component not found ");
                break;
        }

        name = Name;
        Score = 0;

        CursorData = new _Cursor();
        if (CursorDefault && CursorOverride) { 
            Cursor.SetCursor(CursorDefault, CursorHotspot, CursorMode);
            CursorData.Offset = CursorHotspot;
            CursorData.Icon = CursorDefault;
            CursorData.Override = CursorOverride;
        }

	}

    protected virtual void Update()
    {
        CursorData.Visible = Cursor.visible = CursorEnabled;
        CursorData.LockState = Cursor.lockState = CursorLockMode;
    }

    public CharacterController GetCharacterController() {
        return ControllerComponent;
    }

    public Rigidbody GetBodyController() {
        return BodyComponent;
    }

    public _Cursor GetCursor() {
        return CursorData;
    }
}
