using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Zoom : MonoBehaviour, IScrollHandler
{
    [Header("Zoom")]
    public float ZoomSpeed;
    public Vector2 ZoomLimits;
    public KeyCode ResetKey = KeyCode.Space;


    private Vector3 InitialLocalScale;

    protected void Awake()
    {
        InitialLocalScale = transform.localScale;
    }

    protected void Update()
    {
        if (Input.GetKeyDown(ResetKey))
            transform.localScale = InitialLocalScale;
    }

    public void OnScroll(PointerEventData eventData)
    {
        Vector3 Delta = Vector3.one * (eventData.scrollDelta.y* ZoomSpeed);
        Vector3 TargetScale = transform.localScale + Delta;

        TargetScale = Vector3.Max(InitialLocalScale * ZoomLimits.x, TargetScale);
        TargetScale = Vector3.Min(InitialLocalScale * ZoomLimits.y, TargetScale);

        transform.localScale = TargetScale;
    }
}
