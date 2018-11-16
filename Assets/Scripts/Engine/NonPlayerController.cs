using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerController : MonoBehaviour {

    [Header("NPC:")]
    [SerializeField] string Name;
    [SerializeField] KeyCode InteractionKey = KeyCode.F;
    [SerializeField] GameObject QuestMark;

    [SerializeField] List<Quest> QuestList = new List<Quest>();

    private Queue<Quest> QuestQueue;

    protected void Awake()
    {
        QuestQueue = new Queue<Quest>(QuestList);
    }

    protected void Update()
    {
        QuestMark.SetActive(QuestList.Count > 0);
    }

    private void OnTriggerStay(Collider other)
    {
        UI_QuestLog Log;
        if(other.tag == "Player")
        {
            if (Input.GetKeyDown(InteractionKey) && QuestList.Count > 0)
            {
                Log = UI_Hud.main.QuestLogReference;
                Log.CurrentQuest = QuestQueue.Dequeue();
                Log.GetComponent<UI_Window>().OpenWindow();
                Log.NewQuest();
                QuestList.Remove(Log.CurrentQuest);
            }
        }
    }

}
