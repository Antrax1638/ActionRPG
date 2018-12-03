using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_ActionBar : UI_Base, IPointerEnterHandler,IPointerExitHandler
{
    [Header("Action Bar:")]
    [SerializeField] private Image LifeBar;
    public Color EnergyColor;
    [SerializeField] private Image EnergyBar;
    [SerializeField] private Image ExperienceBar;

    private Text LifeValue;
    private Text EnergyValue;
    
    [HideInInspector] public float Health,MaxHealth;
    [HideInInspector] public float Energy,MaxEnergy;
    [HideInInspector] public float Experience,MaxExperience;

    protected void Awake()
    {
        LifeValue = LifeBar.GetComponentInChildren<Text>();
        EnergyValue = EnergyBar.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        //Life
        LifeBar.fillAmount = Health / MaxHealth;
        LifeValue.text = Health + "/" + MaxHealth;
        //Energy
        EnergyBar.fillAmount = Energy / MaxEnergy;
        EnergyValue.text = Energy + "/" + MaxEnergy;
        EnergyBar.color = EnergyColor;

        ExperienceBar.fillAmount = Experience / MaxExperience;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UI_Manager.Instance.SetInputMode(InputMode.InterfaceOnly);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI_Manager.Instance.SetInputMode(InputMode.GameOnly);
    }
}
