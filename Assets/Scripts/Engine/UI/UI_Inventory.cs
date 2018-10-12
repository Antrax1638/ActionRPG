using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Inventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static Vector2Int InvalidIndex = new Vector2Int(-1, -1);
    public const int Null = 0;

    [Header("General Properties:")]
    public bool Safe;
    public GameObject Slot;
    public bool SlotSize;
    public Vector2Int Size;

    [Header("Highlight Properties:")]
    public bool Highlight;
    public GameObject HighLightObject;
    public Color HighlightValid = Color.green;
    public Color HighlightInvalid = Color.red;

    [Header("Debug Properties:")]
    public bool DebugMode = false;
    public Sprite DebugIcon;
    public int DebugId;
    public Vector2Int DebugSize;
    public Vector2Int DebugPos;

    [HideInInspector] public int Length { get { return Size.x * Size.y; } }
    [HideInInspector] public static UI_InventorySlot HoveredSlot;
    [HideInInspector] public int State { get { return LastState; } }

    GameObject GridPanel, SlotTemp;
    private RectTransform TransformComponent;
    private RectTransform GridPanelTransformComponent;
    private GridLayoutGroup GridPanelComponent;
    //private UnSymmetricalGridLayoutGroup UnSymGridPanelComponent;
    private bool MouseOver = false;

    protected GameObject[,] GridCells;
    protected int LastState = 1;
    Queue<Vector2Int> PendingSlots = new Queue<Vector2Int>();

    int Width, Height;
    UI_Item Temp = UI_Item.invalid;

    protected void Awake()
    {
        TransformComponent = GetComponent<RectTransform>();
        if (!TransformComponent)
            Debug.LogError("UI_Inventory: Transform component is null.");


        GridPanelComponent = GetComponentInChildren<GridLayoutGroup>();
        if (GridPanelComponent)
        {
            RectTransform SlotTransform = Slot.GetComponent<RectTransform>();
            GridPanelComponent.cellSize = (SlotSize) ? SlotTransform.sizeDelta : GridPanelComponent.cellSize;

            GridCells = new GameObject[Size.x, Size.y];
            Width = (int)GridPanelComponent.cellSize.x * Size.x;
            Height = (int)GridPanelComponent.cellSize.y * Size.y;
        }

        /*UnSymGridPanelComponent = GetComponentInChildren<UnSymmetricalGridLayoutGroup>();
        if (UnSymGridPanelComponent)
        { 
            RectTransform SlotTransform = Slot.GetComponent<RectTransform>();
            UnSymGridPanelComponent.CellSize = (SlotSize) ? SlotTransform.sizeDelta : UnSymGridPanelComponent.CellSize;

            GridCells = new GameObject[Size.x, Size.y];
            Width = (int)UnSymGridPanelComponent.CellSize.x * Size.x;
            Height = (int)UnSymGridPanelComponent.CellSize.y * Size.y;

            UnSymGridPanelComponent.MaxFillWidth = Size.x;
            UnSymGridPanelComponent.MaxFillHeight = Size.y;
            UnSymGridPanelComponent.LockCellSize = false;
        }*/

        if (!GridPanelComponent)
            Debug.LogError("UI_Inventory: Grid panel component is null.");
        else
        {
            GridPanel = GridPanelComponent.gameObject;
            GridPanelTransformComponent = (RectTransform)GridPanel.transform;
            if (!GridPanelTransformComponent)
                Debug.LogError("UI_Inventory: grid panel transform component is null.");

            Vector2 DeltaSpace = Vector2.zero;
            DeltaSpace.x = TransformComponent.rect.width - GridPanelTransformComponent.rect.width;
            DeltaSpace.y = TransformComponent.rect.height - GridPanelTransformComponent.rect.height;

            TransformComponent.sizeDelta = new Vector2(Width + DeltaSpace.x, Height + DeltaSpace.y);
            if (GridPanelTransformComponent.anchorMin != Vector2.zero && GridPanelTransformComponent.anchorMax != Vector2.one)
            {
                GridPanelTransformComponent.sizeDelta = new Vector2(Width + DeltaSpace.x, Height + DeltaSpace.y);
            }
        }

        //Create Grid:
        UI_InventorySlot SlotComponent;
        for (int y = 0; y < GridCells.GetLength(1); y++)
        {
            for (int x = 0; x < GridCells.GetLength(0); x++)
            {
                GridCells[x, y] = Instantiate(Slot, GridPanel.transform);
                GridCells[x, y].name = "Slot [" + x + "-" + y + "]";
                SlotComponent = GridCells[x, y].GetComponent<UI_InventorySlot>();
                if (SlotComponent) {
                    SlotComponent.Position = new Vector2Int(x, y);
                    SlotComponent.Item = new UI_Item();
                } else {
                    Debug.LogError("UI_Inventory: " + GridCells[x, y].name + " Inventory slot component is null.");
                }

            }
        }
    }

    protected void Start()
    {
        GameObject HighLightCache = HighLightObject;
        HighLightObject = Instantiate(HighLightCache, transform);
        HighLightObject.transform.position = Vector3.zero;
        HighLightObject.SetActive(false);
    }

    protected void Update()
    {
        if (HighLightObject && HighLightObject.activeInHierarchy && !UI_Slot.DragObject)
            ExitHighLight(true);

        //HighLightUpdate();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Temp = new UI_Item(DebugId, DebugIcon, DebugSize);
            AddItem(Temp);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //GetDataMap ();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            print(RemoveItem(DebugPos));
        }
    }

    protected void DebugLog(string info)
    {
        if (DebugMode)
            Debug.Log(info);
    }

    protected virtual bool CompareItem(UI_Item a, UI_Item b)
    {
        return a.Id == b.Id && a.Size == b.Size;
    }

    //Publicas:
    public bool ValidateItemFormat(UI_Item Item)
    {
        bool Validate = true;
        Validate &= Item.Size.x > 0;
        Validate &= Item.Size.y > 0;
        Validate &= Item.Icon != null;
        Validate &= Item.Id > int.MinValue;
        Validate &= Item.Id <= int.MaxValue;
        DebugLog("Item " + Validate + " Format.");
        return Validate;
    }

    public void EnterHighLight()
    {
        if (Highlight && HoveredSlot != null && UI_Slot.DragObject != null)
        {
            HighLightObject.SetActive(true);
            bool RangeValid = true;
            RectTransform HLTransform = HighLightObject.transform as RectTransform;
            RectTransform HLTransformHover = HoveredSlot.transform as RectTransform;
            UI_Drag DragObject = UI_Slot.DragObject.GetComponent<UI_Drag>();

            HLTransform.anchoredPosition = HLTransformHover.anchoredPosition;
            HLTransform.sizeDelta = new Vector2(
                DragObject.DragSize.x * GridPanelComponent.cellSize.x, 
                DragObject.DragSize.y * GridPanelComponent.cellSize.y
                );

            RangeValid &= (HoveredSlot.Position.x >= 0);
            RangeValid &= (HoveredSlot.Position.y >= 0);
            RangeValid &= (HoveredSlot.Position.x + DragObject.DragSize.x < GridCells.GetLength(0));
            RangeValid &= (HoveredSlot.Position.y + DragObject.DragSize.y < GridCells.GetLength(1));

            if (RangeValid)
            {
                for (int x = HoveredSlot.Position.x; x < HoveredSlot.Position.x + DragObject.DragSize.x; x++)
                {
                    for (int y = HoveredSlot.Position.y; y < HoveredSlot.Position.y + DragObject.DragSize.y; y++)
                    {

                    }
                }
            }

            /*bool SlotValid = true;
            bool RangeValid = true;

            UI_InventorySlot Slot;
            UI_Drag DragObject = UI_Slot.DragObject.GetComponent<UI_Drag>();
            for (int x = HoveredSlot.Position.x; x < HoveredSlot.Position.x + DragObject.DragSize.x; x++)
            {
                for (int y = HoveredSlot.Position.y; y < HoveredSlot.Position.y + DragObject.DragSize.y; y++)
                {
                    RangeValid &= (HoveredSlot.Position.x >= 0);
                    RangeValid &= (HoveredSlot.Position.y >= 0);
                    RangeValid &= (HoveredSlot.Position.x + DragObject.DragSize.x < GridCells.GetLength(0));
                    RangeValid &= (HoveredSlot.Position.y + DragObject.DragSize.y < GridCells.GetLength(1));

                    if (RangeValid)
                    {
                        Slot = GridCells[x, y].GetComponent<UI_InventorySlot>();
                        SlotValid &= (Slot && Slot.Item == UI_Item.invalid);
                        Slot.GetImage("Frame").color = (SlotValid) ? HighlightValid : HighlightInvalid;
                        Slot.GetImage("Background").color = (SlotValid) ? HighlightValid : HighlightInvalid;
                    }
                    else
                    {
                        int NewX = Mathf.Clamp(x, 0, GridCells.GetLength(0) - 1);
                        int NewY = Mathf.Clamp(y, 0, GridCells.GetLength(1) - 1);

                        Slot = GridCells[NewX, NewY].GetComponent<UI_InventorySlot>();
                        Slot.GetImage("Frame").color = HighlightValid;
                        Slot.GetImage("Background").color = HighlightValid;

                    }

                    PendingSlots.Enqueue(Slot.Position);
                }
            }*/
        }
    }

    public void ExitHighLight(bool Check)
    {
        HighLightObject.SetActive(false);

        /*if (Highlight && UI_Slot.DragObject != null || (Highlight && Check))
        {
            for (int Index = 0; Index < PendingSlots.Count; Index++)
            {
                Vector2Int CurrentPosition = PendingSlots.Dequeue();
                UI_InventorySlot HSlot = GridCells[CurrentPosition.x, CurrentPosition.y].GetComponent<UI_InventorySlot>();
                HSlot.GetImage("Frame").color = Color.white;
                HSlot.GetImage("Background").color = Color.white;
            }
        }*/
    }

    public Vector2Int AddItem(UI_Item NewItem)
    {
        Vector2Int Position = UI_Inventory.InvalidIndex;
        Vector2Int CurrentPosition = UI_Inventory.InvalidIndex;

        List<Vector2Int> ValidPositions = new List<Vector2Int>();

        UI_InventorySlot CurrentSlot;
        bool ValidSpace = true;
        for(int x = 0; x < GridCells.GetLength(0); x++)
        {
            for (int y = 0; y < GridCells.GetLength(1); y++)
            {
                CurrentSlot = GridCells[x, y].GetComponent<UI_InventorySlot>();
                if (CurrentSlot && !ValidateItemFormat(CurrentSlot.Item)) {
                    ValidPositions.Add(CurrentSlot.Position);
                }
            }
        }

        for(int i = 0; i < ValidPositions.Count; i++)
        {
            Position = ValidPositions[i];
            for (int x = Position.x; x < Position.x + NewItem.Size.x; x++)
            {
                for (int y = Position.y; y < Position.y + NewItem.Size.y; y++)
                {
                    CurrentPosition.x = x;
                    CurrentPosition.y = y;
                    ValidSpace &= ValidPositions.Contains(CurrentPosition);
                }
            }

            if (ValidSpace) break;
        }

        Position = AddItem(NewItem, Position);
        return Position;
    }

    public Vector2Int AddItem(UI_Item NewItem, Vector2Int Position)
    {
        if (!ValidateItemFormat(NewItem)) {
            LastState = -1;
            return UI_Inventory.InvalidIndex;
        }
		
		bool ValidIndex = false, ValidSlot = true;
		Vector2Int Index = UI_Inventory.InvalidIndex;

		int ItemWidth = Position.x + NewItem.Size.x;
		int ItemHeight = Position.y + NewItem.Size.y;

        ValidIndex = (Position.x >= 0 && Position.y >= 0) && (ItemWidth - 1 < Size.x) && (ItemHeight - 1 < Size.y);
        if (!ValidIndex) { DebugLog("Invalid Initial Position"); return Position; }

		List<int> HierarchyIndex = new List<int> ();
		List<GameObject> DeactivatedSlots = new List<GameObject> ();

        string InvalidSlots = "Invalid [";
		bool PointerValid;
		UI_InventorySlot Slot;
		for(int y = Position.y; y < ItemHeight; y++)
		{
			for (int x = Position.x; x < ItemWidth; x++) 
			{
				PointerValid = (x >= 0 && y >= 0 && x < GridCells.GetLength(0) && y < GridCells.GetLength(1));
                if (PointerValid)
				{
                    Slot = GridCells [x, y].GetComponent<UI_InventorySlot> ();
					if (Slot && !ValidateItemFormat(Slot.Item))
                    {
                        ValidSlot &= true;
						Slot.SetVisibility (Visibility.Hidden);
						Slot.Item = NewItem;
						DeactivatedSlots.Add (Slot.gameObject);
						HierarchyIndex.Add (Slot.transform.GetSiblingIndex ());
					}
                    else
                    {
                        InvalidSlots += "-" + Slot.Position;
						ValidSlot &= false;
					}
				} 
				/*else
					ValidSlot &= false;*/
			}
		}
        InvalidSlots += "]";

        if (ValidIndex && ValidSlot)
		{
			DebugLog ("Item Added: to [" + Position.x + "-" + Position.y + "]");
            Slot = GridCells[Position.x, Position.y].GetComponent<UI_InventorySlot>();
            Slot.SetVisibility(Visibility.Visible);
            Slot.SetScale(NewItem.Size);//Problema con la escala
            Slot.SetIcon(NewItem.Icon);
            Slot.Item = NewItem;

            /*if (UnSymGridPanelComponent)
            {
                DebugLog("Unsym Grid Panel Subsystem");
                int MaxIndex = Mathf.Max(HierarchyIndex.ToArray());
                Slot.transform.SetSiblingIndex(MaxIndex);
                HierarchyIndex.Clear();
                LastState = 1;
            }*/

            if (GridPanelComponent)
            {
                DebugLog("Grid Panel Subsystem");
                UI_InventorySlot DeactivatedSlot;
                for (int i = 0; i < DeactivatedSlots.Count; i++)
                {
                    DeactivatedSlot = DeactivatedSlots[i].GetComponent<UI_InventorySlot>();
                    if (DeactivatedSlot && DeactivatedSlot.Position != Position)
                    {
                        DeactivatedSlot.RaycastMode = UI_Slot.RaycastFilter.Inactive;
                    }
                }
                LastState = 1;
                DeactivatedSlots.Clear();
            }
			
			Index = Position;
		}
		else 
		{
            LastState = 0;
            DebugLog("Inventory space unavailable:");
            DebugLog("Slots Deactivated: " + DeactivatedSlots.Count);
            DebugLog("Index: [" + ValidIndex + "]");
            DebugLog("Slot: [" + ValidSlot + "]");
            DebugLog(InvalidSlots);
        
			for (int i = 0; i < DeactivatedSlots.Count; i++)
			{
				Slot = DeactivatedSlots [i].GetComponent<UI_InventorySlot> ();
				if (Slot && Slot.Item == NewItem || Slot.Item == UI_Item.invalid) 
				{
					Slot.SetVisibility (Visibility.Visible);
                    Slot.Item = new UI_Item();
                    Slot.SetIcon (null);
					Slot.SetScale (new Vector2 (1, 1));
				}
			}
			DeactivatedSlots.Clear ();
		}
		return Index;
	}

    public Vector2Int AddItem(UI_Item NewItem,GameObject Slot)
    {
        if (!ValidateItemFormat(NewItem))
            return InvalidIndex;

        Vector2Int Index = InvalidIndex;
        UI_EquipSlot SlotComponent = Slot.GetComponent<UI_EquipSlot>();
        if(SlotComponent)
        {
            DebugLog("Item added to Equipament Slot: [" + SlotComponent.Id + "]");
            SlotComponent.SetVisibility(Visibility.Visible);
            SlotComponent.SetIcon(NewItem.Icon);
            SlotComponent.SetScale(NewItem.Size);
            SlotComponent.Item = NewItem;
            Index = new Vector2Int(SlotComponent.Id, SlotComponent.Id);
        }

        return Index;
    }

	public bool RemoveItem(Vector2Int Position)
	{
        UI_InventorySlot Slot = GridCells [Position.x, Position.y].GetComponent<UI_InventorySlot> ();
		if (Slot.Item == UI_Item.invalid)
			return false;

		bool Success = false;
		int OffsetWidth = Position.x + Slot.Item.Size.x;
		int OffsetHeight = Position.y + Slot.Item.Size.y;

		Slot = GridCells [Position.x,Position.y].GetComponent<UI_InventorySlot> ();
		if (Slot && Slot.Item != UI_Item.invalid) 
		{
			Slot.SetIcon (null);
			Slot.SetScale (new Vector2Int (1, 1));
            Slot.Item = new UI_Item();
            Slot.SetVisibility (Visibility.Visible);
			Success = true;
		}

		for (int x = Position.x; x < OffsetWidth; x++)
		{
			for(int y = Position.y; y < OffsetHeight; y++)
			{
				Slot = GridCells [x, y].GetComponent<UI_InventorySlot> ();
				if (Slot) 
				{
					Slot.SetIcon (null);
					Slot.SetScale (new Vector2Int (1, 1));
                    Slot.Item = new UI_Item();
                    Slot.SetVisibility (Visibility.Visible);
                    Slot.RaycastMode = UI_Slot.RaycastFilter.Active;
                    Success &= true;
				} 
			}
		}

        if(Success)
            DebugLog("Item Removed Successfully");

		return Success;
	}

    public bool RemoveItem(GameObject Slot)
    {
        bool Success = false;

        UI_EquipSlot SlotComponent = Slot.GetComponent<UI_EquipSlot>();
        if (SlotComponent)
        {
            SlotComponent.SetVisibility(Visibility.Visible);
            SlotComponent.SetScale(new Vector2Int(1, 1));
            SlotComponent.SetIcon(null);
            SlotComponent.Item = new UI_Item();
            Success = true;
            DebugLog("Item Removed Successfully: " + SlotComponent.Id);
        }

        return Success;
        
    }

	public bool RemoveItem(UI_Item Item)
	{
		return false;
		/*bool Valid = false;
		UI_Slot Slot;
		for (int y = 0; y < GridCells.GetLength (1); y++)
		{
			for(int x = 0; x < GridCells.GetLength(0); x++)
			{
				Slot = GridCells [x, y].GetComponent<UI_Slot> ();
				if(GridCellData [GridCells [x, y].GetInstanceID ()] == Item.Id)
				{
					GridCellData [GridCells [x, y].GetInstanceID ()] = 0;
					Slot.SetVisibility (Visiblility.Visible);
				}

				if(Slot.GetIcon() == Item.Icon)
				{
					GridCellData [GridCells [x, y].GetInstanceID ()] = 0;
					Slot.AdjustSize = true;
					Slot.SetIcon (null);
					Slot.SetScale (new Vector2 (1, 1));
					Valid = true;
				}
			}
		}

		return Valid;*/
	}

	public void SwitchItem(Vector2Int OldPosition,Vector2Int NewPosition)
	{
        UI_InventorySlot OldSlot, NewSlot;
        OldSlot = GridCells[OldPosition.x, OldPosition.y].GetComponent<UI_InventorySlot>();
        NewSlot = GridCells[NewPosition.x, NewPosition.y].GetComponent<UI_InventorySlot>();

        if (Safe && OldSlot && NewSlot)
        {
            print(OldSlot.name);
            print(NewSlot.name);
        }
	}

	public void ClearItems()
	{
		UI_InventorySlot Slot;
		for (int y = 0; y < GridCells.GetLength (1); y++)
		{
			for(int x = 0; x < GridCells.GetLength(0); x++)
			{
				Slot = GridCells [x, y].GetComponent<UI_InventorySlot> ();
				Slot.SetVisibility (Visibility.Visible);
				Slot.AdjustSize = true;
				Slot.SetIcon (null);
				Slot.SetScale (new Vector2 (1, 1));
				Slot.Item = UI_Item.invalid;
			}
		}
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOver = false;
    }
}
