using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnergyType
{
    None,
    Mana,
    Fury,
    Soul,
    Ki,
    Wrath,
    Spirit
}

public class Character : MonoBehaviour
{
    [Header("Character")]
    public string Name;
    public int Level;
    public EEnergyType EnergyType;
    public Stat Stats;

    [Header("Drop")]
    [SerializeField] float DropForce = 5;
    [SerializeField] float DropAngularForce = 5;

    public EquipamentManager Equipament;
    public InventoryManager Inventory;

    protected void Awake()
    {

    }

    private void Update()
    {

    }

    public void PickUp(GameObject Object)
    {
        int Index = -1;
        Item PickedItem = Object.GetComponent<Item>();
        if (PickedItem && Inventory != null) {
            if (UI_Hud.main.CharacterReference.TryToAddItem(PickedItem))
            {
                PickedItem.OnPickUp();
                Index = Inventory.AddItem(PickedItem.gameObject);
            }
        }

        if (Index < 0)
            Debug.Log("Character "+Name+" Item cannot be added to inventory.");
    }

    public void Drop(int Id)
    {
        if (Inventory != null)
        {
            Item Current;
            int Index = Inventory.FindIndex(Id);
            if (Inventory.IsValidIndex(Index))
            {
                Current = Inventory.RemoveAt(Index).GetComponent<Item>();
                Current.OnDrop();
                Current.transform.position = transform.position;
                Current.GetComponent<Rigidbody>().AddForce(transform.forward * DropForce, ForceMode.Impulse);
                Current.GetComponent<Rigidbody>().AddTorque(Current.transform.forward * DropAngularForce, ForceMode.Impulse);
            }
        }
    }

    public bool Equip(int Id, UI_EquipSlot Slot)
    {
        bool Success = false;
        Item Temp = Inventory.Find(Id).GetComponent<Item>();
        if (UI_Hud.main.CharacterReference.TryToEquipItem(Temp, Slot))
        {
            Stats = Stats + Temp.Stats;
            Success = true;
        }
        return Success;
    }

    public void UnEquip(int Id, UI_EquipSlot Slot)
    {
        Item Temp = Inventory.Find(Id).GetComponent<Item>();
        Stats = Stats - Temp.Stats;
    }
}
