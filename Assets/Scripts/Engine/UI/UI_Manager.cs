using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Manager : MonoBehaviour
{
	[HideInInspector] public static UI_Manager Instance {get{return ManagerInstance;}}
	[HideInInspector] public GameObject Focus;

	[Header("UI Manager Properties:")]
	public GameObject Controller;
    public bool WindowOpen;

    [Header("Prefabs:")]
    [Tooltip("Slots object tooltip preset")][SerializeField] private GameObject ToolTip = null;
    [Tooltip("Slots object overlay preset")][SerializeField] private GameObject Overlay = null;

	protected List<GameObject> Windows = new List<GameObject>();
	private static UI_Manager ManagerInstance;
	private GameObject DragOperation;
    protected PlayerController ControllerComponent;

	void Awake()
	{
		if (ManagerInstance == null)
		{
			ManagerInstance = this;
			DontDestroyOnLoad (gameObject);
		} 
		else
		{
			Destroy (gameObject);
		}
	}

    void Start()
	{
        ControllerComponent = Controller.GetComponent<PlayerController>();
        if (!ControllerComponent) Debug.LogError("UI_Manager: Controller component is null");

        WindowOpen = true;
		RectTransform[] WindowGameObjects = GameObject.FindObjectsOfType<RectTransform> ();
		for (int i = 0; i < WindowGameObjects.Length; i++) 
		{
            if (WindowGameObjects[i].gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                Windows.Add(WindowGameObjects[i].gameObject);
                WindowOpen &= WindowGameObjects[i].gameObject.activeInHierarchy;
            }
		}
		WindowGameObjects = new RectTransform[0];

        UI_Slot.InitializeToolTip(ToolTip, GameObject.FindGameObjectWithTag("MainCanvas").transform);
        UI_Slot.InitializeOverlay(Overlay, GameObject.FindGameObjectWithTag("MainCanvas").transform);
    }

    private void Update()
    {
        if (WindowOpen && ControllerComponent)
            ControllerComponent.Mode = InputMode.InterfaceOnly;
        
    }

    public bool InputKeyModifier(KeyModifier Key)
	{
		bool Success = false;
		switch(Key)
		{
		//Control:
		case KeyModifier.Ctrl:
			Success = Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl);
			break;
		//Alt:
		case KeyModifier.Alt:
			Success = Input.GetKey (KeyCode.LeftAlt) || Input.GetKey (KeyCode.RightAlt) || Input.GetKey (KeyCode.AltGr);
			break;
		//Shift:
		case KeyModifier.Shift:
			Success = Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);
			break;
		case KeyModifier.None: 
			Success = true;
			break;
		case KeyModifier.Cmd:
			Success = Input.GetKey (KeyCode.LeftCommand) || Input.GetKey (KeyCode.RightCommand);
			break;
		}
		return Success;
	}

	public int ToLength(int X,int Y,int Width)
	{
		return (Y * Width) + X;
	}

	public int ToLength(Vector2Int Coords,int Width)
	{
		return (Coords.y * Width) + Coords.x;
	}

	public Vector2Int ToCoord(int Index,int Width)
	{
		return new Vector2Int (Index%Width,Index/Width);
	}

    public void SetInputMode(InputMode Mode)
    {
        if (ControllerComponent)
            ControllerComponent.Mode = Mode;

        UI_Window Current = null;
        for (int i = 0; i < Windows.Count; i++)
        {
            Current = Windows[i].GetComponent <UI_Window>();
            if (Current && Current.Activated) {
                WindowOpen = true;
                return;
            }
        }

        WindowOpen = false;
    }
}
