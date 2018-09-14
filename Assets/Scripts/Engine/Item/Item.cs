using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int Id;
    public EItemType Type = EItemType.None;
    public Stat Stats;

    public static bool operator ==(Item a,Item b)
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
        && a.Stats.VisionRadius == b.Stats.VisionRadius );
    }

    public static bool operator !=(Item a, Item b)
    {
        return !(a == b);
    }

    public override bool Equals(object other)
    {
        return (this == (Item)other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
