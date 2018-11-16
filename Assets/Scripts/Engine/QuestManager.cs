using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get { return instance; } }
    public static QuestManager instance;

    public List<Quest> Quests = new List<Quest>();

	void Awake ()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
	}

	void Update ()
    {
        for (int i = 0; i < Quests.Count; i++)
        {
            Quests[i].Update();
        }
            
	}
}
