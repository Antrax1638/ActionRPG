using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat 
{
    //Basic Stats:
    [Header("Basic")]
	public int Strength;
	public int Agility;
    public int Dexterity;
    public int Inteligence;
    public int Endurance;
	public int Luck;

    //Generic Stats:
    [Header("Generic")]
	public float Health;
    public float MaxHealth;
    public float HealthRegeneration;
    public float Energy;
    public float MaxEnergy;
    public float EnergyRegeneration;
    public float Shield;
    public float ShieldRegeneration;

    //Damage Stats:
    [Header("Damage")]
    public float Damage;
    public float DamageMultiplier;
    public float AttackSpeed;
    public int Range;
	public float CriticalChance;
	public float CriticalMultiplier;
	public float CriticalOverflow;
	
    //Defence Stats:
    [Header("Defence")]
    public int Armor;
    public int ElementalArmor;
    public float DamageReduction;
    public float Block;
    public float BlockChance;
    public float Evasion;
    public float EvasionChance;

    //Misc Stats:
    [Header("Misc")]
	public float ColdownReduction;
	public float Speed;
    public float VisionRadius;


    //Basic Operations:
    public static Stat operator+(Stat a, Stat b)
    {
        Stat c = new Stat();
        c.Strength = a.Strength + b.Strength;
        c.Agility = a.Agility + b.Agility;
        c.Dexterity = a.Dexterity + b.Dexterity;
        c.Inteligence = a.Inteligence + b.Inteligence;
        c.Endurance = a.Endurance + b.Endurance;
        c.Luck = a.Luck + b.Luck;

        c.Health = a.Health + b.Health;
        c.MaxHealth = a.MaxHealth + b.MaxHealth;
        c.HealthRegeneration = a.HealthRegeneration + b.HealthRegeneration;
        c.Energy = a.Energy + b.Energy;
        c.MaxEnergy = a.MaxEnergy + b.MaxEnergy;
        c.EnergyRegeneration = a.EnergyRegeneration + b.EnergyRegeneration;
        c.Shield = a.Shield + b.Shield;

        c.Damage = a.Damage + b.Damage;
        c.DamageMultiplier = a.DamageMultiplier + b.DamageMultiplier;
        c.AttackSpeed = a.AttackSpeed + b.AttackSpeed;
        c.Range = a.Range + b.Range;
        c.CriticalChance = a.CriticalChance + b.CriticalChance;
        c.CriticalMultiplier = a.CriticalMultiplier + b.CriticalMultiplier;
        c.CriticalOverflow = a.CriticalOverflow + b.CriticalOverflow;

        c.Armor = a.Armor + b.Armor;
        c.ElementalArmor = a.ElementalArmor + b.ElementalArmor;
        c.DamageReduction = a.DamageReduction + b.DamageReduction;
        c.Block = a.Block + b.Block;
        c.BlockChance = a.BlockChance + b.BlockChance;
        c.Evasion = a.Evasion + b.Evasion;
        c.EvasionChance = a.EvasionChance + b.EvasionChance;

        c.ColdownReduction = a.ColdownReduction + b.ColdownReduction;
        c.Speed = a.Speed + b.Speed;
        c.VisionRadius = a.VisionRadius + b.VisionRadius;

        return c;
    }

    public static Stat operator-(Stat a, Stat b)
    {
        Stat c = new Stat();
        c.Strength = a.Strength - b.Strength;
        c.Agility = a.Agility - b.Agility;
        c.Dexterity = a.Dexterity - b.Dexterity;
        c.Inteligence = a.Inteligence - b.Inteligence;
        c.Endurance = a.Endurance - b.Endurance;
        c.Luck = a.Luck - b.Luck;

        c.Health = a.Health - b.Health;
        c.MaxHealth = a.MaxHealth - b.MaxHealth;
        c.HealthRegeneration = a.HealthRegeneration - b.HealthRegeneration;
        c.Energy = a.Energy - b.Energy;
        c.MaxEnergy = a.MaxEnergy - b.MaxEnergy;
        c.EnergyRegeneration = a.EnergyRegeneration - b.EnergyRegeneration;
        c.Shield = a.Shield - b.Shield;

        c.Damage = a.Damage - b.Damage;
        c.DamageMultiplier = a.DamageMultiplier - b.DamageMultiplier;
        c.AttackSpeed = a.AttackSpeed - b.AttackSpeed;
        c.Range = a.Range - b.Range;
        c.CriticalChance = a.CriticalChance - b.CriticalChance;
        c.CriticalMultiplier = a.CriticalMultiplier - b.CriticalMultiplier;
        c.CriticalOverflow = a.CriticalOverflow - b.CriticalOverflow;

        c.Armor = a.Armor - b.Armor;
        c.ElementalArmor = a.ElementalArmor - b.ElementalArmor;
        c.DamageReduction = a.DamageReduction - b.DamageReduction;
        c.Block = a.Block - b.Block;
        c.BlockChance = a.BlockChance - b.BlockChance;
        c.Evasion = a.Evasion - b.Evasion;
        c.EvasionChance = a.EvasionChance - b.EvasionChance;

        c.ColdownReduction = a.ColdownReduction - b.ColdownReduction;
        c.Speed = a.Speed - b.Speed;
        c.VisionRadius = a.VisionRadius - b.VisionRadius;

        return c;
    }

    public static Stat operator*(Stat a, float b)
    {
        Stat c = new Stat();
        c.Strength = System.Convert.ToInt32(a.Strength * b);
        c.Agility = System.Convert.ToInt32(a.Agility * b);
        c.Dexterity = System.Convert.ToInt32(a.Dexterity * b);
        c.Inteligence = System.Convert.ToInt32(a.Inteligence * b);
        c.Endurance = System.Convert.ToInt32(a.Endurance * b);
        c.Luck = System.Convert.ToInt32(a.Luck * b);

        c.Health = a.Health * b;
        c.MaxHealth = a.MaxHealth * b;
        c.HealthRegeneration = a.HealthRegeneration * b;
        c.Energy = a.Energy * b;
        c.MaxEnergy = a.MaxEnergy * b;
        c.EnergyRegeneration = a.EnergyRegeneration * b;
        c.Shield = a.Shield * b;

        c.Damage = a.Damage * b;
        c.DamageMultiplier = a.DamageMultiplier * b;
        c.AttackSpeed = a.AttackSpeed * b;
        c.Range = System.Convert.ToInt32(a.Range * b);
        c.CriticalChance = a.CriticalChance * b;
        c.CriticalMultiplier = a.CriticalMultiplier * b;
        c.CriticalOverflow = a.CriticalOverflow * b;

        c.Armor = System.Convert.ToInt32(a.Armor * b);
        c.ElementalArmor = System.Convert.ToInt32(a.ElementalArmor * b);
        c.DamageReduction = a.DamageReduction * b;
        c.Block = a.Block * b;
        c.BlockChance = a.BlockChance * b;
        c.Evasion = a.Evasion * b;
        c.EvasionChance = a.EvasionChance * b;

        c.ColdownReduction = a.ColdownReduction * b;
        c.Speed = a.Speed * b;
        c.VisionRadius = a.VisionRadius * b;

        return c;
    }
}
