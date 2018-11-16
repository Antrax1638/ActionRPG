using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_EquipSlot : UI_Slot
{
    public enum SlotType
    {
        None,
        Head,
        Body,
        Pants,
        Arms,
        Foot,
        LeftWeapon,
        RightWeapon,
        LeftRing,
        RightRing,
        Trinket,
        All
    }

    [Header("Equipament Properties:")]
    public UI_Inventory Inventory;
    public int Id = 1;
    public bool RemoveEvent;
    public UI_InventorySlot.ERemoveType Remove = UI_InventorySlot.ERemoveType.RemoveOnDrop;
    public UI_InventorySlot.ERemoveMode RemoveMode = UI_InventorySlot.ERemoveMode.None;
    public UI_Item Item;
    public SlotType Type;
    public bool Empty { get { return _Empty; } }

    private UI_Item TempDrag = UI_Item.invalid;
    private bool _Empty = true;

	protected override void Start ()
    {
        base.Start();
	}
	
	protected override void Update ()
    {
        base.Update();

        _Empty = (Item == UI_Item.invalid);
	}

    public override void OnBeginDrag(PointerEventData Data)
    {
        base.OnBeginDrag(Data);
        if (Data.button != DragKey) return;

        if (DragComponent)
        {
            UI_Drag DragObject = DragComponent.GetComponent<UI_Drag>();
            if (DragObject && Item != UI_Item.invalid)
            {
                DragObject.DragSize = Item.Size;
            }
        }

        if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrag && Inventory)
        {
            TempDrag = Item;
            Inventory.RemoveItem(gameObject);
            ItemManager.Instance.Character.UnEquip(TempDrag.Id, this);
        }
    }

    public override void OnDrag(PointerEventData Data)
    {
        base.OnDrag(Data);
        if (Data.button != DragKey) return;
    }

    public override void OnDrop(GameObject Slot)
    {
        if (Visible == Visibility.Hidden)
            return;

        if (Slot)
        {
            UI_EquipSlot EquipComponent = Slot.GetComponent<UI_EquipSlot>();
            if (EquipComponent && EquipComponent.Item == UI_Item.invalid && Inventory)
            {
                UI_Item Item = (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrag) ? TempDrag : this.Item;
                if (ItemManager.Instance.Character.Equip(Item.Id, EquipComponent))
                {
                    int NewId = EquipComponent.Inventory.AddItem(Item, Slot);
                    if (NewId >= 0)
                    {
                        if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrop && RemoveEvent)
                        {
                            Inventory.RemoveItem(Position);
                        }
                    }
                    else
                    {
                        if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrag)
                            Inventory.AddItem(Item, Position);
                        else
                        {
                            ItemManager.Instance.Character.UnEquip(Item.Id, this);
                        }
                    }
                }
                else
                {
                    switch (Remove)
                    {
                        case UI_InventorySlot.ERemoveType.RemoveOnDrag: Inventory.AddItem(Item, Position); break;
                        case UI_InventorySlot.ERemoveType.RemoveOnDrop: break;
                    }
                }

            }

            UI_InventorySlot InventoryComponent = Slot.GetComponent<UI_InventorySlot>();
            if (InventoryComponent && Inventory)
            {
                UI_Item Item = (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrag) ? TempDrag : this.Item;
                Vector2Int Index = Inventory.AddItem(Item, InventoryComponent.Position);
                if (Index != UI_Inventory.InvalidIndex)
                {
                    if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrop)
                    {
                        ItemManager.Instance.Character.UnEquip(Item.Id, this);
                        Inventory.RemoveItem(gameObject);
                    }
                }
                else
                {
                    if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrag)
                        Inventory.AddItem(Item, gameObject);
                }
            }
        }
        else
        {
            Item = (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrag) ? TempDrag : Item;
            ItemManager.Instance.Character.Drop(Item.Id);
            if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrag && RemoveMode == UI_InventorySlot.ERemoveMode.RestoreOnVoid)
            {
                Inventory.AddItem(Item, Position);
            }

            if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrop && RemoveMode != UI_InventorySlot.ERemoveMode.RestoreOnVoid)
            {
                Inventory.RemoveItem(Position);
            }
            Item = UI_Item.invalid;
        }

        

        
    }

}
