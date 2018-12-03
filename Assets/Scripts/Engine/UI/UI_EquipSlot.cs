using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    [System.Serializable] class OnEquipEvent : UnityEvent { }
    [System.Serializable] class OnUnEquipEvent : UnityEvent { }

    [SerializeField] private OnEquipEvent OnEquip = new OnEquipEvent();
    [SerializeField] private OnUnEquipEvent OnUnEquip = new OnUnEquipEvent();

    private bool _Trigger = true,_Empty = true;

	protected override void Start ()
    {
        base.Start();
	}
	
	protected override void Update ()
    {
        base.Update();

        _Empty = (Item == UI_Item.invalid);
        if (!_Empty && _Trigger)
        {
            Debug.Log("E");
            OnEquip.Invoke();
            _Trigger = false;
        }
        else if(_Empty && !_Trigger)
        {
            Debug.Log("U");
            OnUnEquip.Invoke();
            _Trigger = true;
        }
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
                DragObject.DragItem = Item;
            }
        }

        if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrag && Inventory)
        {
            ItemManager.Instance.Character.UnEquip(Item.Id, this);
            Inventory.RemoveItem(gameObject);
        }
    }

    public override void OnDrag(PointerEventData Data)
    {
        base.OnDrag(Data);
        if (Data.button != DragKey) return;
    }

    public override void OnDrop(GameObject Slot, UI_Item Item)
    {
        if (Visible == Visibility.Hidden)
            return;

        if (Slot)
        {
            UI_EquipSlot EquipComponent = Slot.GetComponent<UI_EquipSlot>();
            if (EquipComponent && EquipComponent.Item == UI_Item.invalid && Inventory)
            {
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
            switch (RemoveMode)
            {
                case UI_InventorySlot.ERemoveMode.RestoreOnVoid:
                    if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrag)
                    {
                        Inventory.AddItem(Item, Position);
                    }
                    break;
                case UI_InventorySlot.ERemoveMode.DropOnVoid:
                    ItemManager.Instance.Character.Drop(Item.Id);
                    if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrop)
                    {
                        Inventory.RemoveItem(gameObject);
                    }
                    break;
                case UI_InventorySlot.ERemoveMode.DeleteOnVoid:
                    Destroy(ItemManager.Instance.Character.Inventory.Find(Item.Id));
                    if (Remove == UI_InventorySlot.ERemoveType.RemoveOnDrop)
                    {
                        Inventory.RemoveItem(gameObject);
                    }
                    break;
            }
            
        }
    }

}
