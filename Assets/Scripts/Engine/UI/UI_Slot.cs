using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_Slot : UI_Base, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, ICanvasRaycastFilter
{
    public enum RaycastFilter
    {
        Active,
        Inactive
    }

	public bool IsOver{get{ return MouseOver; }}
    public static GameObject DragObject { get { return DragComponent; } }

	[Header("Slot Properties:")]
	public Color OverColor = Color.white;
	[Tooltip("Only use if achors are point type.")] public bool AdjustSize;
	[SerializeField] private string SlotTag = "Slot";
	public Vector2Int Position;
    public RaycastFilter RaycastMode = RaycastFilter.Active;

	[Header("Overlay Properties:")]
	public bool Overlay = false;
	public Sprite HoverOverlay = null;
	public Sprite PressOverlay = null;
	public GameObject OverlayPrefab = null;

	[Header("Drag&Drop Properties:")]
	public bool DragEnabled;
	public GameObject DragPrefab;
	public KeyModifier DragKeyModifier = KeyModifier.None;

	[Header("ToolTip Properties:")]
	public bool ToolTip;
	public GameObject ToolTipPrefab;
	public float ToolTipTime = 1.0f;
	public Vector2 ToolTipOffset = Vector2.zero;

	//Events: 
	[System.Serializable] public class OnLeftClickEvent : UnityEvent<UI_Slot> { }
	[System.Serializable] public class OnRightClickEvent : UnityEvent<UI_Slot> { }

	[SerializeField] public OnLeftClickEvent LeftClick = new OnLeftClickEvent ();
	[SerializeField] public OnLeftClickEvent RightClick = new OnLeftClickEvent ();

	private bool MouseOver;
	private Color[] DefaultColor;
	private RectTransform TransformComponent;

	protected static GameObject DragComponent,ToolTipComponent;
	private List<RaycastResult> RayCastResults = new List<RaycastResult>();
	private GameObject[] HoverObjects;
	private GameObject HoverObject;
	private bool DragKey;
	private GameObject OverlayObject;

	protected virtual void Awake () 
	{
        CanvasGroupComponent = GetComponent<CanvasGroup>();
        if (!CanvasGroupComponent)
            Debug.LogError("UI_Slot: Canvas Group component is null");

        ImageComponents = GetComponentsInChildren<Image> ();
		if (ImageComponents.Length <= 0)
			Debug.LogError ("UI_Slot: Image components are null");

		TransformComponents = GetComponentsInChildren<RectTransform> ();
        if (TransformComponents == null || TransformComponents.Length <= 0)
            Debug.LogError("UI_Slot: Transform components are null");
        else
        {
            RectTransform CastTransform = transform as RectTransform;
            for (int i = 0; i < TransformComponents.Length && AdjustSize; i++)
            {
                if(TransformComponents[i].anchorMin != Vector2.zero && TransformComponents[i].anchorMax != Vector2.one)
                    TransformComponents[i].sizeDelta = CastTransform.sizeDelta;
            }
        }

		DefaultColor = new Color[ImageComponents.Length];
		for (int i = 0; i < ImageComponents.Length; i++) {
			DefaultColor[i] = ImageComponents [i].color;
		}

		TransformComponent = GetComponent<RectTransform> ();
		if (!TransformComponent)
			Debug.Log ("UI_Slot: rect transform component is null");
       
		DragComponent = null;
		ToolTipComponent = null;

        
    }
		
	protected virtual void Start()
	{
		SetIcon (null);
    }

	protected virtual void Update () 
	{
        if (this == null)
            return;

        if (MouseOver) 
		{
			OnMouseOver ();

			if (Input.GetMouseButtonDown (0))
				LeftClick.Invoke (this);

            if (Input.GetMouseButtonDown (1))
				RightClick.Invoke (this);
		}

        if (UI_Manager.Instance)
            DragKey = UI_Manager.Instance.InputKeyModifier(DragKeyModifier);
        else
            Debug.LogWarning("UI_Slot: UI Manager component is null");
	}

	protected void InstantiateOverlay()
	{
		if (!OverlayObject && Overlay) 
		{
			OverlayObject = Instantiate (OverlayPrefab,transform);
			OverlayObject.name = "Overlay";
			Image OverlayImage = OverlayObject.GetComponent<Image> ();
			RectTransform OverlayTransform = OverlayObject.GetComponent<RectTransform> ();
			if(AdjustSize)
				OverlayTransform.sizeDelta = TransformComponent.sizeDelta;
			OverlayImage.sprite = HoverOverlay;
		}
	}

	protected void DestroyOverlay()
	{
		Destroy (OverlayObject);
	}

	protected void ToolTipEnter()
	{
		if (ToolTip && MouseOver) 
		{
			ToolTipComponent = Instantiate (ToolTipPrefab, TransformComponent.root);
			ToolTipComponent.name = "ToolTip";
			Vector2 Position = (Vector2)Input.mousePosition + ToolTipOffset;
			ToolTipComponent.GetComponent<RectTransform> ().anchoredPosition = Position;
		}
	}

	protected void ToolTipExit()
	{
		Destroy (ToolTipComponent);
	}

	protected void UpdateDrag(PointerEventData Data)
	{
		Data.position = Input.mousePosition;
		EventSystem.current.RaycastAll (Data, RayCastResults);
		HoverObjects = new GameObject[RayCastResults.Count];
		for (int i = 0; i < RayCastResults.Count; i++) 
		{
			if (RayCastResults [i].isValid) 
			{
				GameObject Parent = RayCastResults [i].gameObject.transform.parent.gameObject;
				bool ParentValid = (Parent != null && Parent.tag == SlotTag);
				HoverObjects [i] = (ParentValid) ? Parent : RayCastResults [i].gameObject;

				if (HoverObjects [i].tag == SlotTag)
					HoverObject = HoverObjects [i];
			}
		}
	}

    //Functions publicas:
    public Sprite GetIcon()
    {
        bool Valid;
        Valid = (transform.childCount > 0);
        Valid &= (GetComponentsInChildren<Image>().Length > 0);
        Image IconImage = GetImage("Icon");

        if (Valid && IconImage)
            return IconImage.sprite;
        else
        {
            Image Component = GetComponent<Image>();
            return (Component) ? Component.sprite : null;
        }
    }

    public void SetIcon(Sprite Icon)
    {
        Image TempImage = GetImage("Icon");
        if (TempImage)
        {
            TempImage.gameObject.SetActive(Icon);
            TempImage.sprite = Icon;
        }
        else
        {
            Debug.LogWarning("UI_Slot: Icon image object is invalid or null");
        }
    }

    public void SetScale(Vector2 Scale)
    {
        TransformComponent.localScale = new Vector3(Scale.x, Scale.y, 1);
        if (AdjustSize)
        {
            for (int i = 0; i < TransformComponents.Length; i++)
            {
                if(TransformComponents[i].anchorMin != Vector2.zero && TransformComponents[i].anchorMax != Vector2.one)
                    TransformComponents[i].sizeDelta = TransformComponent.sizeDelta;
            }
        }
    }

    public Vector2 GetScale()
    {
        return TransformComponent.localScale;
    }

    public virtual void OnBeginDrag(PointerEventData Data)
	{
		if (Visible == Visibility.Hidden)
			return;

		if(GetIcon () == null)
			return;

		if (DragPrefab && DragEnabled && DragKey)
		{
			DragComponent = Instantiate (DragPrefab, TransformComponent.root);
			UI_Drag Temp = DragComponent.GetComponent<UI_Drag> ();
			Temp.name = name;
			Temp.DragIcon = GetIcon ();
            Temp.DragPosition = Position;
			Temp.OnBeginDrag ();
		}
	}

	public virtual void OnDrag(PointerEventData Data)
	{
		if (Visible == Visibility.Hidden)
			return;

		if (DragComponent && DragEnabled) 
		{
			UI_Drag Temp = DragComponent.GetComponent<UI_Drag> ();
			Temp.OnDrag (Input.mousePosition);
			UpdateDrag (Data);
		}
	}

	public virtual void OnEndDrag(PointerEventData Data)
	{
		if (Visible == Visibility.Hidden)
			return;

		if (DragComponent && DragEnabled) 
		{
			UI_Drag Temp = DragComponent.GetComponent<UI_Drag> ();
			GetImage("Icon").color = Temp.DropColor;
			Temp.OnEndDrag ();
            DragComponent = null;
		}

		if (HoverObject && HoverObject.layer == LayerMask.NameToLayer("UI"))
		{
			OnDrop (HoverObject);
		}
	}

	public virtual void OnPointerEnter(PointerEventData Data)
	{
		if (Visible == Visibility.Hidden)
			return;
		
		MouseOver = true;
		for (int i = 0; i < ImageComponents.Length; i++) 
		{
			ImageComponents [i].color = OverColor;
		}
		InstantiateOverlay ();
		Invoke ("ToolTipEnter", ToolTipTime);
	}

	public virtual void OnPointerExit(PointerEventData Data)
	{
		if (Visible == Visibility.Hidden)
			return;

		MouseOver = false;
		for (int i = 0; i < ImageComponents.Length; i++)
		{
			ImageComponents [i].color = DefaultColor [i];
		}
		DestroyOverlay ();
		if (IsInvoking ("ToolTipEnter"))
			CancelInvoke ("ToolTipEnter");
		else
			ToolTipExit ();
	}

	public virtual void OnMouseOver()
	{
		
	}

	public virtual void OnDrop(GameObject Slot)
	{
		
	}

    public virtual bool IsRaycastLocationValid(Vector2 ScreenPoint, Camera EventCamera)
    {
        bool Success = false;
        switch (RaycastMode)
        {
            case RaycastFilter.Active: Success = true; break;
            case RaycastFilter.Inactive: Success = false;  break;
        }
        return Success;
    }
}
