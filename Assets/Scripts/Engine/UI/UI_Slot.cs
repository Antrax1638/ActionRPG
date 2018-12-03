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

	[Header("Drag&Drop Properties:")]
	public bool DragEnabled;
	public GameObject DragPrefab;
    public PointerEventData.InputButton DragKey = PointerEventData.InputButton.Left;
    public KeyModifier DragKeyModifier = KeyModifier.None;

	[Header("ToolTip Properties:")]
	public bool ToolTip;
	public float ToolTipTime = 1.0f;
	public Vector2 ToolTipOffset = Vector2.zero;

    //Events: 
    [Header("Events:")]
    public bool Events = true;
	[System.Serializable] public class OnLeftClickEvent : UnityEvent<int> { }
	[System.Serializable] public class OnRightClickEvent : UnityEvent<int> { }

	[SerializeField] public OnLeftClickEvent LeftClick = new OnLeftClickEvent ();
	[SerializeField] public OnLeftClickEvent RightClick = new OnLeftClickEvent ();

	private bool MouseOver;
	private Color[] DefaultColor;
	private RectTransform TransformComponent;

	[HideInInspector] public static GameObject DragComponent,ToolTipComponent,OverlayComponent;
    [HideInInspector] public static GameObject LastActivedObject;
	private List<RaycastResult> RayCastResults = new List<RaycastResult>();
	private GameObject[] HoverObjects;
	protected static GameObject HoverObject;
	private bool DragKeyMod;
    private float DefaultAlpha = 0.0f;

	protected virtual void Awake () 
	{
        CanvasGroupComponent = GetComponent<CanvasGroup>();
        if (!CanvasGroupComponent)
            Debug.LogError("UI_Slot: Canvas Group component is null");
        else
        {
            DefaultAlpha = CanvasGroupComponent.alpha;
        }

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
        OverlayComponent = null;
        
    }
		
	protected virtual void Start()
	{
		SetIcon (null);
    }

	protected virtual void Update () 
	{
        if (MouseOver) 
		{
			OnMouseOver ();

			if (Input.GetMouseButtonDown (0) && Events)
				LeftClick.Invoke (GetInstanceID());

            if (Input.GetMouseButtonDown(1) && Events)
                RightClick.Invoke(GetInstanceID());

            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && Overlay && OverlayComponent && PressOverlay) {
                OverlayComponent.GetComponent<Image>().sprite = PressOverlay;
            }

            if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && Overlay && OverlayComponent && HoverOverlay)
            {
                OverlayComponent.GetComponent<Image>().sprite = HoverOverlay;
            }
        }

        if (UI_Manager.Instance)
            DragKeyMod = UI_Manager.Instance.InputKeyModifier(DragKeyModifier);
        else
            Debug.LogWarning("UI_Slot: UI Manager component is null");
	}

    public static void InitializeOverlay(GameObject Prefab, Transform Root)
    {
        if (Prefab && Root)
        {
            OverlayComponent = Instantiate(Prefab, Root);
            OverlayComponent.name = "Overlay";
            OverlayComponent.SetActive(false);
            OverlayComponent.GetComponent<Image>().raycastTarget = false;
        }
    }

	protected void OverlayEnter()
	{
		if (Overlay && OverlayComponent) 
		{
            OverlayComponent.SetActive(true);
            Image OImage = OverlayComponent.GetComponent<Image> ();
            RectTransform OTransform = OverlayComponent.GetComponent<RectTransform>();

            OTransform.SetParent(transform);
            OTransform.anchoredPosition = Vector2Int.zero;
            OTransform.localScale = Vector3.one;
            OTransform.sizeDelta = (AdjustSize) ? TransformComponent.sizeDelta : OTransform.sizeDelta;
			OImage.sprite = HoverOverlay;
		}
	}

	protected void OverlayExit()
	{
		if(Overlay && OverlayComponent)
        {
            OverlayComponent.SetActive(false);
        }
	}

    public static void InitializeToolTip(GameObject Prefab, Transform Root)
    {
        if (!ToolTipComponent)
        {
            ToolTipComponent = Instantiate(Prefab, Root);
            ToolTipComponent.name = "ToolTip";
            ToolTipComponent.SetActive(false);
        }
    }

	protected virtual void ToolTipEnter()
	{
        if (ToolTip && MouseOver && ToolTipComponent)
        {
            ToolTipComponent.SetActive(true);
            Vector2 Position = (Vector2)Input.mousePosition + ToolTipOffset;
            ToolTipComponent.GetComponent<RectTransform>().anchoredPosition = Position;
            
        }
	}

	protected virtual void ToolTipExit()
	{
        if (ToolTipComponent)
        {
            ToolTipComponent.SetActive(false);
        }
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
		if (Visible == Visibility.Hidden || Data.button != DragKey)
			return;
		if(GetIcon () == null)
			return;
		if (DragPrefab && DragEnabled && DragKeyMod)
		{
			DragComponent = Instantiate (DragPrefab, TransformComponent.root);
			UI_Drag Temp = DragComponent.GetComponent<UI_Drag> ();
			Temp.name = name;
			Temp.DragItem.Icon = GetIcon ();
            Temp.DragPosition = Position;
			Temp.OnBeginDrag ();
		}
	}

	public virtual void OnDrag(PointerEventData Data)
	{
		if (Visible == Visibility.Hidden)
			return;

        if (Data.button != DragKey) return;

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
        if (Data.button != DragKey) return;

        UI_Item DragItem = UI_Item.invalid;
        if (DragComponent && DragEnabled) 
		{
			UI_Drag Temp = DragComponent.GetComponent<UI_Drag> ();
			GetImage("Icon").color = Temp.DropColor;
            DragItem = Temp.DragItem;
			Temp.OnEndDrag ();
            DragComponent = null;

            if (HoverObject)
            {
                switch (HoverObject.layer)
                {
                    case 5: OnDrop(HoverObject, DragItem); break;
                    default: break;
                }
            }
            else
            {
                OnDrop(null, DragItem);
            }
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
        OverlayEnter();
		Invoke ("ToolTipEnter", ToolTipTime);
        LastActivedObject = gameObject;
	}

	public virtual void OnPointerExit(PointerEventData Data)
	{
		if (Visible == Visibility.Hidden)
			return;

		MouseOver = false;
        HoverObject = null;
        RestoreColor();
        OverlayExit();
		if (IsInvoking ("ToolTipEnter"))
			CancelInvoke ("ToolTipEnter");
		else
			ToolTipExit ();
	}

	public virtual void OnMouseOver()
	{
		
	}

	public virtual void OnDrop(GameObject Slot,UI_Item Item)
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

    public virtual void RestoreOpacity() {
        if (CanvasGroupComponent) {
            CanvasGroupComponent.alpha = DefaultAlpha;
        }
    }

    public virtual void RestoreColor()
    {
        for (int i = 0; i < ImageComponents.Length; i++)
        {
            ImageComponents[i].color = DefaultColor[i];
        }
    }
}
