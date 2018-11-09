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
    private UI_Item TempDrag = new UI_Item();
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

        //Posible bug
        if (DragComponent)
        {
            UI_Drag DragObject = DragComponent.GetComponent<UI_Drag>();
            if (DragObject && Item != UI_Item.invalid)
            {
                DragObject.DragSize = Item.Size;
                DragObject.Source = gameObject;
            }
        }

        if (Inventory && Remove == ERemoveType.RemoveOnDrag && RemoveEvent)
        {
            TempDrag = Item;
            ParentInventory.RemoveItem(Position);
        }

    }

    public override void OnDrag(PointerEventData Data)
    {
        base.OnDrag(Data);
        if (Data.button != DragKey) return;
    }

    public override void OnEndDrag(PointerEventData Data)
    {
        base.OnEndDrag(Data);
        if (Data.button != DragKey) return;
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
        if (Inventory)
            UI_Inventory.HoveredSlot = null;

        base.OnPointerExit(Data);
        ParentInventory.ExitHighLight();
    }

    public override void OnDrop(GameObject Slot)
    {
        if (Visible == Visibility.Hidden)
            return;

        if (Slot)
        {
            UI_InventorySlot SlotComponent = Slot.GetComponent<UI_InventorySlot>();
            if (Inventory && SlotComponent)
            {
                UI_Item Item = (Remove == ERemoveType.RemoveOnDrag) ? TempDrag : this.Item;
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
                UI_Item Item = (Remove == ERemoveType.RemoveOnDrag) ? TempDrag : this.Item;
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
            Debug.Log("Void Drop WIP");

            Item = (Remove == ERemoveType.RemoveOnDrag) ? TempDrag : Item;
            ItemManager.Instance.Character.Drop(Item.Id);
            if (Remove == ERemoveType.RemoveOnDrag && RemoveMode == ERemoveMode.RestoreOnVoid)
            {
                ParentInventory.AddItem(Item, Position);
            }

            if(Remove == ERemoveType.RemoveOnDrop && RemoveMode != ERemoveMode.RestoreOnVoid)
            {
                ParentInventory.RemoveItem(Position);
            }

            Item = UI_Item.invalid;
        }
	}
}
