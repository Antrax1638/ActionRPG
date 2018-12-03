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
    [Tooltip("Slots object tooltip preset")][SerializeField] private GameObject SlotToolTip = null;
    [Tooltip("Slots object overlay preset")][SerializeField] private GameObject SlotOverlay = null;
    [Tooltip("Ability drag & drop preset")][SerializeField] private GameObject AbilityDrag = null;
    [Tooltip("Ability drag & drop preset")][SerializeField] private GameObject AbilityToolTip = null;

    protected List<UI_Window> Windows = new List<UI_Window>();
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
		UI_Window[] WindowGameObjects = GameObject.FindObjectsOfType<UI_Window> ();
		for (int i = 0; i < WindowGameObjects.Length; i++) 
		{
            if (WindowGameObjects[i].gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                Windows.Add(WindowGameObjects[i]);
                WindowOpen &= WindowGameObjects[i].Activated;
            }
		}
		WindowGameObjects = new UI_Window[0];

        UI_Slot.InitializeToolTip(SlotToolTip, GameObject.FindGameObjectWithTag("MainCanvas").transform);
        UI_Slot.InitializeOverlay(SlotOverlay, GameObject.FindGameObjectWithTag("MainCanvas").transform);
        UI_Ability.InitializeDrag(AbilityDrag, GameObject.FindGameObjectWithTag("MainCanvas").transform);
        UI_Ability.InitializeToolTip(AbilityToolTip, GameObject.FindGameObjectWithTag("MainCanvas").transform);
    }

    protected void Update()
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

    public void SetInputMode(int Mode)
    {
        if (ControllerComponent)
            ControllerComponent.Mode = (InputMode)Mode;
    }

    public void SetInputMode(InputMode Mode)
    {
        if (ControllerComponent)
            ControllerComponent.Mode = Mode;

        for (int i = 0; i < Windows.Count; i++)
        {
            if (Windows[i].Activated) {
                WindowOpen = true;
                return;
            }
        }
        WindowOpen = false;
    }

    
}
