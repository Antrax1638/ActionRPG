using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class UI_Drag : MonoBehaviour, ICanvasRaycastFilter
{
    [Header("Drag Properties")]
    public bool DragIgnore = true;
	public Color DragColor = Color.white;
	public Color DropColor = Color.white;
	public Vector2 DragOffset = Vector2.zero;


    [Header("Content Properties:")]
    public Sprite DragIcon;
    public Vector2Int DragPosition;
    public Vector2Int DragSize = Vector2Int.one;

    public bool IsDrag { get { return MouseDrag; } }
    [HideInInspector] public GameObject Source;

    private bool MouseDrag;
	private Image ImageComponent;
	private RectTransform TransformComponent;
	private Color DefaultColor;

	void Awake ()
	{
		ImageComponent = GetComponent<Image> ();
		if (!ImageComponent)
			Debug.LogError ("UI_Drag: image component is null");

		TransformComponent = GetComponent<RectTransform> ();
		if (!TransformComponent)
			Debug.LogError ("UI_Drag: transform component is null.");

		DefaultColor = ImageComponent.color;
	}

	void Update ()
	{
		
	}

	public void OnBeginDrag()
	{
		MouseDrag = true;
		gameObject.SetActive (true);
		ImageComponent.sprite = DragIcon;
		ImageComponent.color = DragColor;
        TransformComponent.localScale = new Vector3(DragSize.x,DragSize.y,1);
	}

	public void OnDrag(Vector2 Position)
	{
		TransformComponent.anchoredPosition = Position + DragOffset;
	}

	public void OnEndDrag()
	{
        TransformComponent.localScale = Vector3.one;
        MouseDrag = false;
		Destroy (gameObject);
	}

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (DragIgnore)
            return false;
        else
            return true;
    }
}
