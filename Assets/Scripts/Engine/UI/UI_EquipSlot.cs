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
    public UI_InventorySlot.ERemoveType Remove = UI_InventorySlot.ERemoveType.RemoveOnDrop;
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
            //Funcionalidad desactivada
            switch (Remove)
            {
                case UI_InventorySlot.ERemoveType.RemoveOnDrag: Inventory.AddItem(Item, gameObject); break;
                case UI_InventorySlot.ERemoveType.RemoveOnDrop: break;
            }
        }

        UI_InventorySlot InventoryComponent = Slot.GetComponent<UI_InventorySlot>();
        if (InventoryComponent && Inventory)
        {
            UI_Item Item = (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrag) ? TempDrag : this.Item;
            Vector2Int Index = Inventory.AddItem(Item, InventoryComponent.Position);
            if (Index != UI_Inventory.InvalidIndex)
            {
                ItemManager.Instance.Character.UnEquip(Item.Id, this);
                if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrop)
                    Inventory.RemoveItem(gameObject);
            }
            else
            {
                if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrag)
                    Inventory.AddItem(Item, gameObject);
            }
        }

        
    }

}
