using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EItemType
{
	None,
	Weapon,
	Consumible,
	Armor,
	Material,
}

[System.Serializable]
public class Item : MonoBehaviour
{
    [Header("Item Properties:")]
    public string Name;
    public string Description;
    public bool AutoGenerateId = true;
    public int Id;
    public int Stack;
    public int MaxStack;
    public EItemType Type = EItemType.None;
    public UI_EquipSlot.SlotType SlotType = UI_EquipSlot.SlotType.None;
    public Stat Stats;
    public Stat RequieredStats;
    public Color Rarity = Color.white;
    public Sprite Icon;
    public Vector2Int Size;

    

    private RectTransform ButtonTransform;
    private int State = -2;
    private string Seed;

    public static bool operator ==(Item a, Item b)
    {
        return (
        a.Name == b.Name
        && a.Id == b.Id
        && a.Type == b.Type

        && a.Stats.Strength == b.Stats.Strength
        && a.Stats.Agility == b.Stats.Agility
        && a.Stats.Dexterity == b.Stats.Dexterity
        && a.Stats.Inteligence == b.Stats.Inteligence
        && a.Stats.Endurance == b.Stats.Endurance
        && a.Stats.Luck == b.Stats.Luck

        && a.Stats.Health == b.Stats.Health
        && a.Stats.MaxHealth == b.Stats.MaxHealth
        && a.Stats.HealthRegeneration == b.Stats.HealthRegeneration
        && a.Stats.Energy == b.Stats.Energy
        && a.Stats.MaxEnergy == b.Stats.MaxEnergy
        && a.Stats.EnergyRegeneration == b.Stats.EnergyRegeneration
        && a.Stats.Shield == b.Stats.Shield
        && a.Stats.ShieldRegeneration == b.Stats.ShieldRegeneration

        && a.Stats.Damage == b.Stats.Damage
        && a.Stats.DamageMultiplier == b.Stats.DamageMultiplier
        && a.Stats.AttackSpeed == b.Stats.AttackSpeed
        && a.Stats.Range == b.Stats.Range
        && a.Stats.CriticalChance == b.Stats.CriticalChance
        && a.Stats.CriticalMultiplier == b.Stats.CriticalMultiplier
        && a.Stats.CriticalOverflow == b.Stats.CriticalOverflow

        && a.Stats.Armor == b.Stats.Armor
        && a.Stats.ElementalArmor == b.Stats.ElementalArmor
        && a.Stats.DamageReduction == b.Stats.DamageReduction
        && a.Stats.Block == b.Stats.Block
        && a.Stats.BlockChance == b.Stats.BlockChance
        && a.Stats.Evasion == b.Stats.Evasion
        && a.Stats.EvasionChance == b.Stats.EvasionChance

        && a.Stats.ColdownReduction == b.Stats.ColdownReduction
        && a.Stats.Speed == b.Stats.Speed
        && a.Stats.VisionRadius == b.Stats.VisionRadius);
    }

    public static bool operator !=(Item a, Item b)
    {
        return !(a == b);
    }

    public bool Equal(GameObject Other)
    {
        return this == Other.GetComponent<Item>();
    }

    public override bool Equals(object other)
    {
        return (this == (Item)other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    protected virtual void Start()
    {
        State = -1;
        Id = (AutoGenerateId) ? ItemManager.Instance.GenerateId() : Id;
    }

    public virtual void OnPickUp()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        State = 0;
    }

    public virtual void OnDrop()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;
        State = -1;
    }

    public virtual void OnEquip()
    {
        //WIP
        State = 1;
    }

    protected void GenerateSeed()
    {
        Seed = "";
        Seed += "S" + Stats.Strength;
        Seed += "A" + Stats.Agility;
        Seed += "D" + Stats.Dexterity;
        Seed += "I" + Stats.Inteligence;
        Seed += "E" + Stats.Endurance;
        Seed += "L" + Stats.Luck;
        Seed += "MaxHealth" + Stats.MaxHealth;
        Seed += "HRegen" + Stats.HealthRegeneration;
        Seed += "MaxEnergy" + Stats.MaxEnergy;
        Seed += "ERegen" + Stats.EnergyRegeneration;
        Seed += "Shield" + Stats.Shield;
        Seed += "SRegen" + Stats.ShieldRegeneration;
        Seed += "Damage" + Stats.Damage;
        Seed += "DMult" + Stats.DamageMultiplier;
        Seed += "ASpeed" + Stats.AttackSpeed;
        Seed += "Range" + Stats.Range;
        Seed += "CritChance" + Stats.CriticalChance;
        Seed += "CritMult" + Stats.CriticalMultiplier;
        Seed += "CritOver" + Stats.CriticalOverflow;
        Seed += "Armor" + Stats.Armor;
        Seed += "EArmor" + Stats.ElementalArmor;
        Seed += "DamageRed" + Stats.DamageReduction;
        Seed += "Block" + Stats.Block;
        Seed += "BChance" + Stats.BlockChance;
        Seed += "Evasion" + Stats.Evasion;
        Seed += "EChance" + Stats.EvasionChance;
        Seed += "CD" + Stats.ColdownReduction;
        Seed += "Speed" + Stats.Speed;
        Seed += "VRadius" + Stats.VisionRadius;
    }

    static public bool IsValid(int Id)
    {
        return Id > int.MinValue && Id <= int.MaxValue;
    }
}
