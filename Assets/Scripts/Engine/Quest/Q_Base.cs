using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(QuestFlag))]
public class Q_Base : MonoBehaviour
{
    public enum EQuestStatus { None, Finished, UnFinished, Falied };

    public int Level;
    public bool Completed;
    public EQuestStatus Status;
    protected QuestFlag FlagComponent;

	protected virtual void Awake ()
    {
        FlagComponent = GetComponent<QuestFlag>();
        if (!FlagComponent) Debug.LogError("Q_Base [" + name + "]: Flag component is null or invalid.");

        Completed = false;
        Status = EQuestStatus.UnFinished;
	}

}
