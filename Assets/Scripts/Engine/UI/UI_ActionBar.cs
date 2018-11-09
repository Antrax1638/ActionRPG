using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ActionBar : UI_Base
{
    [Header("Action Bar:")]
    [SerializeField] private Image LifeBar;
    [SerializeField] private Image EnergyBar;

    [HideInInspector] public float HealthDifferencial;
    [HideInInspector] public float EnergyDifferencial;

    private void Update()
    {
        LifeBar.fillAmount = HealthDifferencial;
        EnergyBar.fillAmount = EnergyDifferencial;
    }

}
