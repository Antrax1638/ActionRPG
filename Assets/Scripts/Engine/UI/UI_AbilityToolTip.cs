using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AbilityToolTip : MonoBehaviour
{
    public string Name { set { NameComponent.text = value; } get { return NameComponent.text; } }
    public string Description { set { DescriptionComponent.text = value; } get { return DescriptionComponent.text; } }
    public Sprite Icon { set { IconComponent.sprite = value; } get { return IconComponent.sprite; } }
    public string Type { set { TypeComponent.text = value; } get { return TypeComponent.text; } }

	[Header("Components")]
    [SerializeField] private Image IconComponent = null;
    [SerializeField] private Text NameComponent = null;
    [SerializeField] private Text DescriptionComponent = null;
    [SerializeField] private Text TypeComponent = null;

}
