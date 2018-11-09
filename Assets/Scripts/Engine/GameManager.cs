using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get { return instance; } }
    private static GameManager instance = null;

	void Awake ()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
	}
	
	public static GameObject FindInstanceID(int InstanceId)
    {
        return (GameObject)UnityEditor.EditorUtility.InstanceIDToObject(InstanceId);
    }

}
