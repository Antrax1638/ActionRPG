using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestLog : MonoBehaviour
{
    [Header("Quest Components")]
    [SerializeField] Text Dialog;
    [SerializeField] Text Title;
    [SerializeField] GameObject AcceptFrame;
    [SerializeField] GameObject QuestsFrame;


    [SerializeField] GameObject QuestPrefab;
    [SerializeField] Color UnFinishedQuest = Color.white;
    [SerializeField] Color CompletedQuest = Color.green;
    [SerializeField] Color FaliedQuest = Color.red;

    [HideInInspector] public List<GameObject> ActiveQuests = new List<GameObject>();

    [HideInInspector] public Quest CurrentQuest;
    //[HideInInspector] public Quest ActiveQuest;

    private void Awake()
    {
        if(AcceptFrame)
            AcceptFrame.SetActive(false);
    }

    protected void Update()
    {
        if (CurrentQuest != null && Title && Dialog)
        {
            Title.text = CurrentQuest.Name;
            if(CurrentQuest.Dialog != null && CurrentQuest.Dialog.Length > 0) {
                int DialogID = Mathf.Clamp(CurrentQuest.State, 0, CurrentQuest.Dialog.Length-1);
                Dialog.text = CurrentQuest.Dialog[DialogID];
            }
        }
    }

    public void NewQuest()
    {
        AcceptFrame.SetActive(true);
        QuestsFrame.SetActive(false);
    }

    public void NextStep()
    {
        CurrentQuest.State++;
        if(CurrentQuest.State >= CurrentQuest.Dialog.Length)
        {
            AcceptFrame.SetActive(false);
            QuestsFrame.SetActive(true);
            
            ActiveQuests.Add(Instantiate(QuestPrefab, QuestsFrame.transform));
            ActiveQuests[ActiveQuests.Count - 1].GetComponentInChildren<Text>().text = CurrentQuest.Name;
            ActiveQuests[ActiveQuests.Count - 1].name = CurrentQuest.Name;
            for (int i = 0; i < CurrentQuest.Flags.Length; i++)
                CurrentQuest.Flags[i].gameObject.SetActive(true);
            QuestManager.Instance.Quests.Add(CurrentQuest);
            CurrentQuest = null;
        }
    }

    public void PrevStep()
    {
        CurrentQuest.State--;
    }
}
