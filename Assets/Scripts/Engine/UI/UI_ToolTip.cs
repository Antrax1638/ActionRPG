using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UI_ToolTip : MonoBehaviour
{
    [Header("General Properties:")]
    public bool Fade = true;
    public float FadeSmooth = 0.01f;
    [Range(0.0f, 1.0f)] public float FadeMin = 0.0f;
    [Range(0.0f, 1.0f)] public float FadeMax = 1.0f;

    [Header("Components:")]
    [SerializeField] private Text Title = null;
    [SerializeField] private Image Icon = null;
   
    [Header("Stat Properties")]
    public GameObject StatPrefab;

    private float FadeRatio;
    private RectTransform TransformComponent;
    private CanvasGroup CanvasGroupComponent;
    private ScrollRect ScrollRectComponent;
    private VerticalLayoutGroup VerticalGroupComponent;
    private Dictionary<string, GameObject> Stats = new Dictionary<string, GameObject>();

    void Awake()
    {
        TransformComponent = GetComponent<RectTransform>();
        if (!TransformComponent)
            Debug.LogError("UI_ToolTip: Transform component is null");

        ScrollRectComponent = GetComponentInChildren<ScrollRect>();
        if (!ScrollRectComponent)
            Debug.LogError("UI_ToolTip: Scroll rect component is null in child objects");

        VerticalGroupComponent = GetComponentInChildren<VerticalLayoutGroup>();
        if (!VerticalGroupComponent) Debug.LogError("UI_ToolTip: Vertical layout group component is null in child objects");

        if (Fade)
        {
            CanvasGroupComponent = GetComponent<CanvasGroup>();
            if (!CanvasGroupComponent)
            {
                Debug.LogWarning("UI_ToolTip: Canvas group component is null");
                Debug.LogWarning("UI_ToolTip: Adding component...");

                CanvasGroupComponent = gameObject.AddComponent<CanvasGroup>();
                CanvasGroupComponent.blocksRaycasts = false;
                CanvasGroupComponent.interactable = false;
            }
        }
    }

    void Start()
    {
        if (Fade && CanvasGroupComponent)
            CanvasGroupComponent.alpha = 0.0f;
    }

    void Update()
    {
        if (Fade && FadeRatio < 0.99 && CanvasGroupComponent)
        {
            FadeRatio = Mathf.Clamp(FadeRatio, FadeMin, FadeMax);
            FadeRatio = Mathf.Lerp(FadeRatio, 1.0f, FadeSmooth);

            CanvasGroupComponent.alpha = FadeRatio;
        }
    }
    
    public void SetStat<T>(string Name, T Stat)
    {
        SetStat<T>(Name, Stat, Color.white, "");
    }

    public void SetStat<T>(string Name, T Stat, Color Color, string Unit, int Space = 1)
    {
        string Spaces = "";
        for (int i = 0; i < Space; i++)
            Spaces += " ";

        if (Stats.ContainsKey(Name))
        {
            if (System.Convert.ToSingle(Stat) > 0.0f)
            {
                char Operator = (System.Convert.ToSingle(Stat) > 0.0f) ? '+' : '-';
                string StatText = Name + Spaces + Operator + Stat + Unit;
                Stats[Name].GetComponentInChildren<Text>().text = StatText;
                Stats[Name].GetComponentInChildren<Text>().color = Color;

            }
            else
            {
                Destroy(Stats[Name]);
                Stats.Remove(Name);
            }
        }
        else
        {
            if (System.Convert.ToSingle(Stat) > 0.0f)
            {
                Stats.Add(Name, Instantiate(StatPrefab, ScrollRectComponent.content));
                char Operator = (System.Convert.ToSingle(Stat) > 0.0f) ? '+' : '-';
                string StatText = Name + Spaces + Operator + Stat + Unit;
                Stats[Name].GetComponentInChildren<Text>().text = StatText;
                Stats[Name].GetComponentInChildren<Text>().color = Color;
            }
        }

    }

    public void SetProperties(string Name,Sprite Icon,Stat Stats)
    {
        this.Title.text = Name;
        this.Icon.sprite = Icon;

        //Basic:
        SetStat<int>("Strength", Stats.Strength);
        SetStat<int>("Agility", Stats.Agility);
        SetStat<int>("Dexterity", Stats.Dexterity);
        SetStat<int>("Inteligence", Stats.Inteligence);
        SetStat<int>("Endurance", Stats.Endurance);
        SetStat<int>("Luck", Stats.Luck);
        //Generic
        SetStat<float>("Health", Stats.Health);
        SetStat<float>("MaxHealth", Stats.MaxHealth);
        SetStat<float>("HealthRegeneration", Stats.HealthRegeneration);
        SetStat<float>("Energy", Stats.Energy);
        SetStat<float>("MaxEnergy", Stats.MaxEnergy);
        SetStat<float>("EnergyRegeneration", Stats.EnergyRegeneration);
        SetStat<float>("Shield", Stats.Shield);
        SetStat<float>("ShieldRegeneration", Stats.ShieldRegeneration);
        //Offence
        SetStat<float>("Damage", Stats.Damage);
        SetStat<float>("DamageMultiplier", Stats.DamageMultiplier);
        SetStat<float>("AttackSpeed", Stats.AttackSpeed);
        SetStat<int>("Range", Stats.Range);
        SetStat<float>("CriticalChance", Stats.CriticalChance);
        SetStat<float>("CriticalMultiplier", Stats.CriticalMultiplier);
        SetStat<float>("CriticalOverflow", Stats.CriticalOverflow);
        //Defence
        SetStat<int>("Armor", Stats.Armor);
        SetStat<int>("ElementalArmor", Stats.ElementalArmor);
        SetStat<float>("DamageReduction", Stats.DamageReduction);
        SetStat<float>("Block", Stats.Block);
        SetStat<float>("BlockChance", Stats.BlockChance);
        SetStat<float>("Evasion", Stats.Evasion);
        SetStat<float>("EvasionChance", Stats.EvasionChance);
        //Misc
        SetStat<float>("ColdownReduction", Stats.ColdownReduction);
        SetStat<float>("Speed", Stats.Speed);
        SetStat<float>("VisionRadius", Stats.VisionRadius);
    }

}
