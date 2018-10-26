using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Character : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] protected string Name;

    protected UI_EquipSlot[] EquipamentSlots;
    protected UI_Inventory InventoryComponent;

	protected void Awake ()
    {
        EquipamentSlots = GetComponentsInChildren<UI_EquipSlot>();
        if (EquipamentSlots.Length <= 0)
            Debug.LogError("UI_Character: Equipament slots invalid or not detected in childs objects");

        InventoryComponent = GetComponentInChildren<UI_Inventory>();
        if (!InventoryComponent)
            Debug.LogError("UI_Character: Inventory component are null or invalid on childs objects");
	}
    

    public bool TryToAddItem(Item NewItem)
    {
        Vector2Int Index = UI_Inventory.InvalidIndex;
        UI_Item Cache = new UI_Item(NewItem.Id, NewItem.Icon, NewItem.Size);
        Index = InventoryComponent.AddItem(Cache);
        return Index != UI_Inventory.InvalidIndex;
    }

    public bool TryToEquipItem(Item NewItem, UI_EquipSlot Slot)
    {
        if(NewItem && Slot)
        {
            return NewItem.SlotType == Slot.Type || NewItem.SlotType == UI_EquipSlot.SlotType.All;
        }
        return false;
    }
}
