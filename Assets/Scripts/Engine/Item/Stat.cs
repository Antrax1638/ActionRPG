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
	
}
