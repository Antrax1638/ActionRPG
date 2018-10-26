using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Window : UI_Base, IPointerClickHandler, IDragHandler , IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private bool AdjustToContent = false;
	
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

	[HideInInspector] public bool Activated { get{return IsActivated;} }

	protected Vector2 InitialPivotPoint = Vector2.zero;
	protected Vector2 LocalPivotPoint = Vector2.zero;
	protected RectTransform TransformComponent;
	protected bool Dragged,IsActivated;
    private bool CanDrag = true;
    private bool[] ActiveChilds;


	protected virtual void Awake ()
	{
		TransformComponent = GetComponent<RectTransform> ();
		if (!TransformComponent)
			Debug.LogError ("UI_Window: transform component is null");

		TransformComponents = GetComponentsInChildren<RectTransform> ();
		if (TransformComponents == null)
			Debug.Log("UI_Window: transform components are null");

        ActiveChilds = new bool[TransformComponents.Length];
        

		InitialPivotPoint = TransformComponent.pivot;
		IsActivated = gameObject.activeInHierarchy;
	}

    protected virtual void Start()
    {
        if (AdjustToContent) {
            //WIP
        }
    }
		
	protected virtual void Update()
	{
        if (Toggleable) {
            if (Input.GetButtonDown(ToggleAction) && UI_Manager.Instance.InputKeyModifier(ToggleModifier))
            {
                ToggleWindow();
            }
        }

		if (Input.GetKeyDown (CloseKey))
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
		
	public virtual void OnDrag (PointerEventData eventData)
	{
		if (!IsActivated || Visible == Visibility.Hidden)
			return;
		
		if (Draggable && UI_Manager.Instance.InputKeyModifier(DragModifier) && CanDrag)
		{
            Vector2 Position = Input.mousePosition;
			TransformComponent.anchoredPosition = Position + DragOffset;
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
                LocalPivotPoint = Rect.PointToNormalized(TransformComponent.rect, LocalPivotPoint);
                TransformComponent.pivot = LocalPivotPoint;
                Dragged = true;
            }
			
		}	
	}

	public virtual void OnEndDrag (PointerEventData eventData)
	{
		Dragged = false;
        CanDrag = true;
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
