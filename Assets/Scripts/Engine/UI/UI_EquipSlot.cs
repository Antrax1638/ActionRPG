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
    }

    [Header("Equipament Properties:")]
    public UI_Inventory Inventory;
    public int Id = 1;
    public UI_InventorySlot.RemoveType Remove = UI_InventorySlot.RemoveType.RemoveOnDrop;
    public UI_Item Item;
    public SlotType Type;

    protected UI_Item TempDrag = UI_Item.invalid;

	protected override void Start ()
    {
        base.Start();
	}
	
	protected override void Update ()
    {
        base.Update();
	}

    public override void OnBeginDrag(PointerEventData Data)
    {
        base.OnBeginDrag(Data);
        
        UI_Drag DragObject = DragComponent.GetComponent<UI_Drag>();
        if (DragObject && Item != UI_Item.invalid)
        {
            DragObject.DragSize = Item.Size;
        }

        if (Remove == UI_InventorySlot.RemoveType.RemoveOnDrag && Inventory)
        {
            TempDrag = Item;
            Inventory.RemoveItem(gameObject);
        }
    }

    public override void OnDrag(PointerEventData Data)
    {
        base.OnDrag(Data);
    }

    public override void OnDrop(GameObject Slot)
    {
        if (Visible == Visibility.Hidden)
            return;

        UI_EquipSlot EquipComponent = Slot.GetComponent<UI_EquipSlot>();
        if (EquipComponent && Inventory)
        {
            print("Drop Cast Equip");
        }

        UI_InventorySlot InventoryComponent = Slot.GetComponent<UI_InventorySlot>();
        if (InventoryComponent && Inventory)
        {
            UI_Item Item = (Remove == UI_InventorySlot.RemoveType.RemoveOnDrag) ? TempDrag : this.Item;
            Vector2Int Index = Inventory.AddItem(Item, InventoryComponent.Position);
            if (Index != UI_Inventory.InvalidIndex)
            {
                if (Remove == UI_InventorySlot.RemoveType.RemoveOnDrop)
                    Inventory.RemoveItem(gameObject);
            }
            else
            {
                if (Remove == UI_InventorySlot.RemoveType.RemoveOnDrag)
                    Inventory.AddItem(Item, gameObject);
            }
        }

        
    }

}
