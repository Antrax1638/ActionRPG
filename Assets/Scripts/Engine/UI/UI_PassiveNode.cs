using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Toggle))]
public class UI_PassiveNode : MonoBehaviour
{
    [Header("Node")]
    public bool First = false;
    public bool Skill = false;
    public GameObject ParentNode;

    private Toggle ParentToggle;
    private Toggle ThisToggle;
    private UI_PassiveTree PassiveTree;
    private UI_Ability ThisAbility;

    protected void Awake()
    {
        if (!First)
        {
            ParentToggle = ParentNode.GetComponent<Toggle>();
            if (!ParentToggle) Debug.LogError("UI_PassiveNode: Parent node invalid or null");
        }

        ThisToggle = GetComponent<Toggle>();
        if (!ThisToggle) Debug.LogError("UI_PassiveNode: Toggle component is invalid or null");

        PassiveTree = GetComponentInParent<UI_PassiveTree>();
        if (!PassiveTree) Debug.LogError("UI_PassiveNode: Passive tree in parent is invalid or null");

        if (Skill) {
            ThisAbility = GetComponent<UI_Ability>();
            if (!ThisAbility) Debug.LogError("UI_PassiveNode: Skill node is active and invalid or null ability object");
        }
    }

    protected void Update()
    {
        ThisToggle.enabled = (ParentToggle && !First) ? ParentToggle.isOn : true;
        ThisToggle.enabled &= (PassiveTree.PointsLeft > 0) || ThisToggle.isOn;

        if(ThisAbility)
            ThisAbility.enabled = ThisToggle.isOn;

        if (ParentToggle && !ParentToggle.isOn)
        {
            ThisToggle.isOn = false;
        }
    }

    
}
