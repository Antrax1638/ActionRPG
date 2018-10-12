﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_InventorySlot : UI_Slot
{
	public enum RemoveType
	{
		RemoveOnDrag,
		RemoveOnDrop
	}

    [Header("Inventory Slot Properties:")]
    public bool Inventory;
	public UI_Item Item = new UI_Item();
	public bool RemoveEvent = false;
	public RemoveType Remove = RemoveType.RemoveOnDrop;

	private UI_Inventory ParentInventory;
	private UI_Item TempDrag = new UI_Item();

	protected override void Awake()
	{
		base.Awake ();
        if (Inventory)
        {
            ParentInventory = GetComponentInParent<UI_Inventory>();
            if (!ParentInventory)
                Debug.LogError("UI_InventorySlot: inventory class component is null");
        }
    }

    protected override void Start()
	{
		base.Start ();
        
        SetVisibility(Visible);
	}

	protected override void Update()
	{
		base.Update ();

        if (GetImage("Icon").gameObject.activeInHierarchy && Item == UI_Item.invalid)
            Debug.Log("Active and Null");
	}

	//Operations:
	public override void OnBeginDrag(PointerEventData Data)
	{
		base.OnBeginDrag (Data);

        //Posible bug
        if (DragComponent)
        {
            UI_Drag DragObject = DragComponent.GetComponent<UI_Drag>();
            if (DragObject && Item != UI_Item.invalid)
            {
                DragObject.DragSize = Item.Size;
                DragObject.Source = ParentInventory.gameObject;
            }
        }
        
        if (Inventory && Remove == RemoveType.RemoveOnDrag && RemoveEvent)
        {
            TempDrag = Item;
            ParentInventory.RemoveItem(Position);
        }
		
	}

	public override void OnDrag(PointerEventData Data)
	{
		base.OnDrag (Data);
	}

	public override void OnEndDrag(PointerEventData Data)
	{
		base.OnEndDrag (Data);
	}

	public override void OnPointerEnter(PointerEventData Data)
	{
        ToolTip = (Item != UI_Item.invalid && ToolTipPrefab);
        if (Inventory)
            UI_Inventory.HoveredSlot = this;

        base.OnPointerEnter (Data);
        ParentInventory.EnterHighLight();
    }

	public override void OnPointerExit(PointerEventData Data)
	{
        if (Inventory)
            UI_Inventory.HoveredSlot = null;

        base.OnPointerExit (Data);
        ParentInventory.ExitHighLight(false);
	}

	public override void OnDrop (GameObject Slot)
	{
		if (Visible == Visibility.Hidden)
			return;

		UI_InventorySlot SlotComponent = Slot.GetComponent<UI_InventorySlot> ();
		if (Inventory && SlotComponent) 
		{
            UI_Item Item = (Remove == RemoveType.RemoveOnDrag) ? TempDrag : this.Item;
            Vector2Int NewPosition = SlotComponent.ParentInventory.AddItem (Item, SlotComponent.Position);
            
            if (NewPosition != UI_Inventory.InvalidIndex)
            {
                if (Remove == RemoveType.RemoveOnDrop && RemoveEvent)
                {
                    ParentInventory.RemoveItem(Position);
                }
            }
            else
            {
                if (Remove == RemoveType.RemoveOnDrag)
                    ParentInventory.AddItem(Item, Position);
            }
		}

        UI_EquipSlot EquipComponent = Slot.GetComponent<UI_EquipSlot>();
        if (EquipComponent && EquipComponent.Inventory)
        {
            
            UI_Item Item = (Remove == RemoveType.RemoveOnDrag) ? TempDrag : this.Item;
            Vector2Int NewId = EquipComponent.Inventory.AddItem(Item, Slot);
            if(NewId != UI_Inventory.InvalidIndex)
            {
                if (Remove == RemoveType.RemoveOnDrop && RemoveEvent)
                {
                    ParentInventory.RemoveItem(Position);
                }
            }
            else
            {
                if (Remove == RemoveType.RemoveOnDrag)
                    ParentInventory.AddItem(Item, Position);
            }

            Debug.Log("Cast To Equip slot");
        }
	}
}
