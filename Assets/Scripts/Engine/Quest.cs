using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest : MonoBehaviour
{
    [Header("Quest Settings")]
    public string Name;
    public string Description;
    public int RewardCount;
    public Item[] Reward;
    public QuestFlag[] Flags;
    
    private int CompleteCount = 0;
    private bool Complete = false;

	void Awake ()
    {
        
	}

	void Update ()
    {
		for(int i = 0; i < Flags.Length; i++)
        {
            CompleteCount = (Flags[i].Completed) ? CompleteCount + 1 : CompleteCount;
        }

        if (CompleteCount == Flags.Length && !Complete)
            OnComplete();
	}

    protected virtual void OnComplete() {
        Complete = true;
    }

}
