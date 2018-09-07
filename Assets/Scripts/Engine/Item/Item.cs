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


}
