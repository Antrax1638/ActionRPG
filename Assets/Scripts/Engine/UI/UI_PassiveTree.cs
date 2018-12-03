using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PassiveTree : MonoBehaviour
{
    [Header("Points Left")]
    public int PointsLeft;
    public string PointsLeftText = "Points Left";
    public Color AvailablePoints = Color.white;
    public Color UnAvailablePoints = Color.red;

    [Header("Components")]
    [SerializeField] private Text Value;
    [SerializeField] private Text Text;

	void Update ()
    {
        if (Value && Text) {
            Text.text = PointsLeftText;
            Value.text = PointsLeft.ToString();
            Value.color = (PointsLeft>0) ? AvailablePoints : UnAvailablePoints;
        }
	}

    public void OnActivate(bool Value)
    {
        PointsLeft = (Value) ? PointsLeft - 1 : PointsLeft + 1;
    }
}
