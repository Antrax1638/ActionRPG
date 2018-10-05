using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipamentManager
{
    [HideInInspector] public Character Owner;

    protected Item[] EquipamentSlots;

    public EquipamentManager(int Size)
    {
        EquipamentSlots = new Item[Size];
    }

    public void AddItem(Item Item,int Index)
    {
        bool Valid = (Index >= 0 && Index < EquipamentSlots.Length);
        Valid &= Item != null;
        if(Valid)
        {
            EquipamentSlots[Index] = Item;
        }
    }

    public void RemoveItem(Item Item)
    {
        bool Valid = Item != null;
        if (Valid)
        {
            for (int i = 0; i < EquipamentSlots.Length; i++) {
                if(EquipamentSlots[i] == Item) EquipamentSlots[i] = null;
            }
        }
    }

    public void RemoveItem(int Index)
    {
        bool Valid = (Index >= 0 && Index < EquipamentSlots.Length);
        if (Valid)
        {
            EquipamentSlots[Index] = null;
        }
    }

    public void Empty()
    {
        int OldSize = EquipamentSlots.Length;
        EquipamentSlots = new Item[OldSize];
    }

    public void Resize(int Size)
    {
        List<Item> Temp = new List<Item>(EquipamentSlots);
        EquipamentSlots = new Item[Size];
        for (int i = 0; i < Temp.Count; i++)
        {
            if (i < Temp.Count && i < EquipamentSlots.Length) {
                EquipamentSlots[i] = Temp[i];
            }
        }
        Temp.Clear();
    }

    public Item GetAt(int Index)
    {
        if (Index >= 0 && Index < EquipamentSlots.Length) {
            return EquipamentSlots[Index];
        }

        return null;
    }
}
