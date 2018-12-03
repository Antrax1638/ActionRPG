using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_Window : UI_Base, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Drag Properties:")]
    public bool Draggable;
    public KeyModifier DragModifier;
    public Vector2 DragOffset = Vector2.zero;
    public RectTransform[] DragFilter;

    [Header("Toggle Properties:")]
    public bool Toggleable;
    public KeyModifier ToggleModifier;
    public string ToggleAction;
    public KeyCode CloseKey = KeyCode.Escape;

    [Header("Focus Properties:")]
    public bool Focusable;

    [HideInInspector] public bool Activated { get { return IsActivated; } }

    [System.Serializable] public class OnWindowOpen : UnityEvent { }
    [System.Serializable] public class OnWindowClose : UnityEvent { }

    [Header("Events Properties:")]
    [SerializeField] private OnWindowOpen WindowOpen = new OnWindowOpen();
    [SerializeField] private OnWindowClose WindowClose = new OnWindowClose();

    protected Vector2 InitialPosition;
    protected Vector2 InitialPivotPoint = Vector2.zero;
    protected Vector2 LocalPivotPoint = Vector2.zero;
    protected RectTransform TransformComponent;
    protected bool Dragged, IsActivated;
    private bool CanDrag = true;
    private bool[] ActiveChilds;

    protected virtual void Awake()
    {
        TransformComponent = GetComponent<RectTransform>();
        if (!TransformComponent)
            Debug.LogError("UI_Window: transform component is null");

        TransformComponents = GetComponentsInChildren<RectTransform>();
        if (TransformComponents == null)
            Debug.Log("UI_Window: transform components are null");

        ActiveChilds = new bool[TransformComponents.Length];
        for (int i = 0; i < ActiveChilds.Length; i++)
            ActiveChilds[i] = TransformComponents[i].gameObject.activeInHierarchy;


        InitialPivotPoint = TransformComponent.pivot;
        InitialPosition = TransformComponent.anchoredPosition;
        IsActivated = Visible == Visibility.Visible;

    }

    protected virtual void Start()
    {
        switch (Visible)
        {
            case Visibility.None: CloseWindow(); break;
            case Visibility.Hidden: CloseWindow(); break;
            case Visibility.Visible: OpenWindow(); break;
        }
    }

	protected virtual void Update()
	{
        if (Toggleable && !UI_Slot.DragObject) {
            if (Input.GetButtonDown(ToggleAction) && UI_Manager.Instance.InputKeyModifier(ToggleModifier))
            {
                ToggleWindow();
            }
        }
        
		if (Input.GetKeyDown (CloseKey) && IsActivated && !UI_Slot.DragObject)
			CloseWindow ();
	}

	public virtual void OnPointerClick (PointerEventData eventData)
	{
		if (!IsActivated || Visible == Visibility.Hidden)
			return;
		
		if (Focusable)
		{
			TransformComponent.SetAsLastSibling ();
			UI_Manager.Instance.Focus = gameObject;
		}
	}
		
	public virtual void OnBeginDrag (PointerEventData eventData)
	{
		if (!IsActivated || Visible == Visibility.Hidden)
			return;

		if (Focusable)
        {
			TransformComponent.SetAsLastSibling ();
			UI_Manager.Instance.Focus = gameObject;
		}

		if (Draggable && UI_Manager.Instance.InputKeyModifier(DragModifier))
        {
            if(DragFilter != null && DragFilter.Length > 0)
            {
                CanDrag = false;
                for (int i = 0; i < DragFilter.Length; i++)
                {
                    if (eventData.hovered.Contains(DragFilter[i].gameObject))
                    {
                        CanDrag = true;
                    }
                }
            }

            if (CanDrag)
            {
                Canvas ParentCanvas = GetComponentInParent<Canvas>();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(TransformComponent, eventData.position, ParentCanvas.worldCamera, out LocalPivotPoint);
                TransformComponent.pivot = Rect.PointToNormalized(TransformComponent.rect, LocalPivotPoint);
                Dragged = true;
            }
			
		}	
	}

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!IsActivated || Visible == Visibility.Hidden)
            return;

        if (Draggable && UI_Manager.Instance.InputKeyModifier(DragModifier) && CanDrag)
        {
            TransformComponent.anchoredPosition = eventData.position + DragOffset;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        bool Inside = true;
        Dragged = false;
        CanDrag = true;

        Vector2[] Position = new Vector2[2];
        Position[0] = TransformComponent.offsetMax;
        Position[1] = TransformComponent.offsetMin;
        Rect ScreenRect = new Rect(0, 0, Screen.width, Screen.height);
        
        Inside &= ScreenRect.Contains(Position[0]);
        //Inside &= ScreenRect.Contains(Position[1]);
        print(TransformComponent.offsetMin);
        print(Inside);
    }
		
	public void CloseWindow()
	{
        for (int i = 0; i < ActiveChilds.Length; i++)
            ActiveChilds[i] = TransformComponents[i].gameObject.activeInHierarchy;

        if (TransformComponents != null) {
			for (int i = 0; i < TransformComponents.Length; i++) {
				TransformComponents [i].gameObject.SetActive (false);
			}
		}
		IsActivated = false;
        gameObject.SetActive(true);

        WindowClose.Invoke();

        if(UI_Slot.ToolTipComponent)
            UI_Slot.ToolTipComponent.SetActive(false);
        if(UI_Slot.OverlayComponent)
            UI_Slot.OverlayComponent.SetActive(false);

        Destroy(UI_Slot.DragComponent);
        UI_Manager.Instance.SetInputMode(InputMode.GameOnly);
	}

	public void OpenWindow()
	{
        if (TransformComponents != null) {
			for (int i = 0; i < TransformComponents.Length; i++) {
				TransformComponents [i].gameObject.SetActive (ActiveChilds[i]);
			}
		}
		IsActivated = true;
        gameObject.SetActive(true);

        if (UI_Slot.ToolTipComponent)
            UI_Slot.ToolTipComponent.SetActive(false);
        if (UI_Slot.OverlayComponent)
            UI_Slot.OverlayComponent.SetActive(false);

        WindowOpen.Invoke();

        if (UI_Slot.LastActivedObject)
        {
            UI_Slot.LastActivedObject.GetComponent<UI_Slot>().RestoreColor();
        }

        UI_Manager.Instance.SetInputMode(InputMode.InterfaceOnly);
    }

	public void ToggleWindow()
	{
		if (IsActivated)
			CloseWindow ();
		else
			OpenWindow ();
	}
	
}
