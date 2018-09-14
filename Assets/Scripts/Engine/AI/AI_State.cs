using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_State : MonoBehaviour
{
    [Header("State Properties:")]
    public bool Visible = true;
    public float Smooth;
    public Vector3 MinSize;
    public Vector3 MaxSize;

    public List<Material> MaterialList = new List<Material>();
    public List<string> StateList = new List<string>();

    private MeshRenderer RendererComponent;
    private MeshFilter FilterComponent;
    private FSMManager FSMComponent;

    Vector3 ObjetiveScale;
    bool Size;
    float Value = 0;

	void Awake ()
    {
        RendererComponent = GetComponent<MeshRenderer>();
        if (!RendererComponent)
            Debug.LogError("AI_State: Mesh renderer component is null");

        FilterComponent = GetComponent<MeshFilter>();
        if (!FilterComponent)
            Debug.LogError("AI_State: Mesh filter component is null");

        FSMComponent = GetComponentInParent<FSMManager>();
        if (!FSMComponent)
            Debug.LogError("AI_State: FSM component is null");

        ObjetiveScale = new Vector3(3,3,3);
        Size = true;

	}

    void Update()
    {
        gameObject.SetActive(Visible);

        if (MaterialList.Count == StateList.Count)
            RendererComponent.material = MaterialList[StateList.IndexOf(FSMComponent.Default.Name)];

        if (Size)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, MinSize, Smooth * Time.deltaTime);
            Value += Time.deltaTime;
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, MaxSize, Smooth * Time.deltaTime);
            Value -= Time.deltaTime;
        }

        if (transform.localScale == MinSize)
            Size = false;
        else if (transform.localScale == MaxSize)
            Size = true;
    }
}
