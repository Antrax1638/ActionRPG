using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GorePart
{
    public string Name;
    public Transform Transform;
    public Rigidbody Rigidbody;
    [Range(0,1)]public float Blend;
    public GameObject Part;

    private GameObject SpawnedPart;

    public void SpawnPart()
    {
        if (Part) {
            SpawnedPart = GameObject.Instantiate(Part, Transform);
            SpawnedPart.transform.parent = null;
        }
    }
}

public class Gore : MonoBehaviour
{
    [Header("Gore")]
    public List<GorePart> GoreParts = new List<GorePart>();

    private void Update()
    {
        UpdateGore();
    }

    public void UpdateGore()
    {
        Vector3 CurrentScale = Vector3.zero;
        for (int i = 0; i < GoreParts.Count; i++)
        {
            CurrentScale = Vector3.one * GoreParts[i].Blend;
            GoreParts[i].Transform.localScale = CurrentScale;
        }
    }

    public void SpawnGore()
    {
        for (int i = 0; i < GoreParts.Count; i++)
        {
            GoreParts[i].SpawnPart();
        }
    }
}
