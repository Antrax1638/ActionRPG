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

    public EquipamentManager Equipament;
    public InventoryManager Inventory;

    private void Update()
    {
        print(Inventory.Length);
    }

}
