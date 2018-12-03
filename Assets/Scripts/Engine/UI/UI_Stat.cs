using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stat : MonoBehaviour
{
    public string Value { set { ValueComponent.text = value; } get { return ValueComponent.text; } }
    public Color ValueColor { set { ValueComponent.color = value; } get { return ValueComponent.color; } }

    public string Description { set { DescriptionComponent.text = value; } get { return DescriptionComponent.text; } }
    public Color DescriptionColor { set { DescriptionComponent.color = value; } get { return DescriptionComponent.color; } }
    
    [Header("Components:")]
    [SerializeField] private Text ValueComponent;
    [SerializeField] private Text DescriptionComponent;
}
