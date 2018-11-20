using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_InventorySlot : UI_Slot
{
    public enum ERemoveType
    {
        RemoveOnDrag,
        RemoveOnDrop
    }

    public enum ERemoveMode
    {
        None,
        DropOnVoid,
        DeleteOnVoid,
        RestoreOnVoid
    }

    [Header("Inventory Slot Properties:")]
    public bool Inventory;
    public UI_Item Item = new UI_Item();
    public bool RemoveEvent = false;
    public ERemoveType Remove = ERemoveType.RemoveOnDrop;
    public ERemoveMode RemoveMode = ERemoveMode.DropOnVoid;

    private UI_Inventory ParentInventory;
    private GameObject IconReference;

    protected override void Awake()
    {
        base.Awake();
        if (Inventory)
        {
            ParentInventory = GetComponentInParent<UI_Inventory>();
            if (!ParentInventory)
                Debug.LogError("UI_InventorySlot: inventory class component is null");
        }
    }

    protected override void Start()
    {
        base.Start();
        IconReference = GetImage("Icon").gameObject;

        SetVisibility(Visible);
    }

    protected override void Update()
    {
        base.Update();

        if (IconReference.activeInHierarchy && Item == UI_Item.invalid)
        {
            IconReference.SetActive(false);
            Debug.Log("Active and Null");
        }
    }

    //Operations:
    public override void OnBeginDrag(PointerEventData Data)
    {
        base.OnBeginDrag(Data);
        if (Data.button != DragKey) return;

        if (DragComponent)
        {
            UI_Drag DragObject = DragComponent.GetComponent<UI_Drag>();
            if (DragObject && Item != UI_Item.invalid)
            {
                DragObject.DragItem = Item;
                DragObject.Source = gameObject;
            }
        }
        
        if (Remove == ERemoveType.RemoveOnDrag && RemoveEvent)
        {
            ParentInventory.RemoveItem(Position);
        }
    }

    public override void OnDrag(PointerEventData Data)
    {
        base.OnDrag(Data);
    }

    public override void OnEndDrag(PointerEventData Data)
    {
        base.OnEndDrag(Data);
        
    }

    public override void OnPointerEnter(PointerEventData Data)
    {
        ToolTip = (Item != UI_Item.invalid);
        base.OnPointerEnter(Data);
        UI_Inventory.HoveredSlot = this;
        ParentInventory.EnterHighLight();
    }

    protected override void ToolTipEnter()
    {
        base.ToolTipEnter();
        if (ToolTip && ToolTipComponent)
        {
            UI_ToolTip ToolTipObj = ToolTipComponent.GetComponent<UI_ToolTip>();
            Item ToolTipItem = ItemManager.Instance.Character.Inventory.Find(Item.Id).GetComponent<Item>();

            ToolTipObj.SetProperties(ToolTipItem.Name, Item.Icon, ToolTipItem.Stats);
        }
    }

    protected override void ToolTipExit()
    {
        base.ToolTipExit();
    }

    public override void OnPointerExit(PointerEventData Data)
    {
        UI_Inventory.HoveredSlot = null;
        HoverObject = null;

        base.OnPointerExit(Data);
        ParentInventory.ExitHighLight();
    }

    public override void OnDrop(GameObject Slot, UI_Item Item)
    {
        if (Visible == Visibility.Hidden)
            return;

        Debug.LogWarning("Drop Id:" + Item.Id);
        if (Slot)
        {
            UI_InventorySlot SlotComponent = Slot.GetComponent<UI_InventorySlot>();
            if (Inventory && SlotComponent)
            {
                Vector2Int NewPosition = SlotComponent.ParentInventory.AddItem(Item, SlotComponent.Position);

                if (NewPosition != UI_Inventory.InvalidIndex)
                {
                    if (Remove == ERemoveType.RemoveOnDrop && RemoveEvent)
                    {
                        ParentInventory.RemoveItem(Position);
                    }
                }
                else
                {
                    if (Remove == ERemoveType.RemoveOnDrag)
                    {
                        ParentInventory.AddItem(Item, Position);
                    }
                }
            }

            UI_EquipSlot EquipComponent = Slot.GetComponent<UI_EquipSlot>();
            if (EquipComponent && EquipComponent.Inventory)
            {
                if (ItemManager.Instance.Character.Equip(Item.Id, EquipComponent))
                {
                    int NewId = EquipComponent.Inventory.AddItem(Item, Slot);
                    if (NewId >= 0)
                    {
                        if (Remove == ERemoveType.RemoveOnDrop && RemoveEvent)
                        {
                            ParentInventory.RemoveItem(Position);
                        }
                    }
                    else
                    {
                        if (Remove == ERemoveType.RemoveOnDrag)
                            ParentInventory.AddItem(Item, Position);
                    }
                }
                else
                {
                    switch (Remove) {
                        case ERemoveType.RemoveOnDrag: ParentInventory.AddItem(Item, Position); break;
                        case ERemoveType.RemoveOnDrop: break;
                    }
                }
                Debug.Log("Cast To Equip slot");
            }
        }
        else
        {
            //Debug.Log("Void Drop WIP");
            switch (RemoveMode)
            {
                //Restore
                case ERemoveMode.RestoreOnVoid:
                    if (Remove == ERemoveType.RemoveOnDrag)
                        ParentInventory.AddItem(Item);
                    break;
                //Drop
                case ERemoveMode.DropOnVoid:
                    ItemManager.Instance.Character.Drop(Item.Id);
                    if(Remove == ERemoveType.RemoveOnDrop) ParentInventory.RemoveItem(Item);
                    break;
                //Delete
                case ERemoveMode.DeleteOnVoid:
                    if (Remove == ERemoveType.RemoveOnDrop) ParentInventory.RemoveItem(Item);
                    break;
            }
        }
	}
}
