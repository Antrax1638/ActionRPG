using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EFlagType
{
    Capture,
    Kill,
    Interact,
    Protect,
    Collect,
    Defend,
    Escort,
}

public struct EQuestFlag
{
    public bool Completed;
//    public 

}

[System.Serializable]
public class Quest : MonoBehaviour
{
    [Header("Quest Settings")]
    public string Name;
    public Item[] Reward;
    public int RewardCount;
	
	void Awake ()
    {
		
	}

	void Update ()
    {
		
	}
}
