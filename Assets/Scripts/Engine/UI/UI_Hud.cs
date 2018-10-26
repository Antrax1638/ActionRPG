using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hud : UI_Base
{
    [HideInInspector] public static UI_Hud main { get { return _Instance; } }

    [Header("Hud Reference Properties:")]
    public UI_Character CharacterReference;

    [Header("Blur Control:")]
    public bool Blur;
    [Range(0,128)] public float Disortion = 10;
    [Range(0,20)] public float Size = 1;

    private Image BlurComponent;
    private static UI_Hud _Instance;

	protected virtual void Awake ()
    {
        if (!main) {
            _Instance = GameObject.FindGameObjectWithTag("MainHud").GetComponent<UI_Hud>();
        }

        BlurComponent = GetComponent<Image>();
        if (!BlurComponent)
            Debug.LogError("UI_Hud: Blur component is null or invalid.");
    }

    protected virtual void Update()
    {
        if (BlurComponent)
        {
            BlurComponent.enabled = (UI_Manager.Instance.WindowOpen && Blur);
            BlurComponent.material.SetFloat("Disortion", Mathf.Clamp(Disortion,0.0f,128.0f));
            BlurComponent.material.SetFloat("Size", Mathf.Clamp(Size,0.0f,20.0f));
        }
    }

}
