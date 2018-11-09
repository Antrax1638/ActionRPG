using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestFlag : MonoBehaviour
{
    public enum EFlagType
    {
        None,
        Capture,
        Kill,
        Interact,
        Protect,
        Collect,
        Defend,
        Escort,
    }

    public EFlagType Type;
    [HideInInspector] public bool Completed = false;
}
