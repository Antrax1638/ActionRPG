using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UI_ToolTip : MonoBehaviour
{
    [Header("General Properties:")]
    public bool Fade = true;
    public float FadeSmooth = 0.01f;
    [Range(0.0f, 1.0f)] public float FadeMin = 0.0f;
    [Range(0.0f, 1.0f)] public float FadeMax = 1.0f;
    public Color TitleColor;
    public Color StatColor;

    public EItemType Type = EItemType.None;
    public string Rarity { set { RarityTxt.text = value; } }
    public Color RarityColor { set { RarityTxt.color = value; } }
    public string TypeText { set { TypeTxt.text = value; } }
    public string MainStatText { set { StatText.text = value; } }
    public float MainStatValue { set { StatValue.text = value.ToString(); } }
    public int Value { set { SellValue.text = value.ToString(); } }

    [Header("Components:")]
    [SerializeField] private Text Title = null;
    [SerializeField] private Image Icon = null;
    [SerializeField] private Text RarityTxt = null;
    [SerializeField] private Text TypeTxt = null;
    [SerializeField] private Text StatText = null;
    [SerializeField] private Text StatValue = null;
    [SerializeField] private Text SellValue = null;

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

    public void SetStat<T>(string Name, T Stat, Color Color, string Unit = "")
    {
        if (Stats.ContainsKey(Name))
        {
            if (System.Convert.ToSingle(Stat) > 0.0f)
            {
                char Operator = (System.Convert.ToSingle(Stat) > 0.0f) ? '+' : '-';
                UI_Stat Temp = Stats[Name].GetComponent<UI_Stat>();
                Temp.Value = "" + Operator + Stat + Unit;
                Temp.ValueColor = Color.white;
                Temp.Description = Name;
                Temp.DescriptionColor = Color;
                

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
                UI_Stat Temp = Stats[Name].GetComponent<UI_Stat>();
                Temp.Value = "" + Operator + Stat + Unit;
                Temp.ValueColor = Color.white;
                Temp.Description = Name;
                Temp.DescriptionColor = Color;
            }
        }

    }

    public void SetProperties(string Name,Sprite Icon,Stat Stats)
    {
        this.Title.text = Name;
        this.Title.color = TitleColor;
        this.Icon.sprite = Icon;

        switch (Type)
        {
            default: MainStatText = "None"; MainStatValue = 0; break;
            case EItemType.Weapon: MainStatText = "Damage Per Second"; MainStatValue = Stats.Damage; break;
            case EItemType.Consumible: MainStatText = "Health/Energy"; MainStatValue = Stats.Health; break;
            case EItemType.Armor: MainStatText = "Armor"; MainStatValue = Stats.Armor; break;
            case EItemType.Material: MainStatText = "Vision Radius"; MainStatValue = Stats.VisionRadius; break;
        }

        //Basic:
        SetStat<int>("Strength", Stats.Strength, StatColor);
        SetStat<int>("Agility", Stats.Agility, StatColor);
        SetStat<int>("Dexterity", Stats.Dexterity, StatColor);
        SetStat<int>("Inteligence", Stats.Inteligence, StatColor);
        SetStat<int>("Endurance", Stats.Endurance, StatColor);
        SetStat<int>("Luck", Stats.Luck, StatColor);
        //Generic
        //SetStat<float>("Health", Stats.Health,StatColor);
        SetStat<float>("MaxHealth", Stats.MaxHealth, StatColor);
        SetStat<float>("HealthRegeneration", Stats.HealthRegeneration, StatColor);
        SetStat<float>("Energy", Stats.Energy, StatColor);
        SetStat<float>("MaxEnergy", Stats.MaxEnergy, StatColor);
        SetStat<float>("EnergyRegeneration", Stats.EnergyRegeneration, StatColor);
        SetStat<float>("Shield", Stats.Shield, StatColor);
        SetStat<float>("ShieldRegeneration", Stats.ShieldRegeneration, StatColor);
        //Offence
        //SetStat<float>("Damage", Stats.Damage,StatColor);
        SetStat<float>("DamageMultiplier", Stats.DamageMultiplier, StatColor);
        SetStat<float>("AttackSpeed", Stats.AttackSpeed, StatColor);
        SetStat<int>("Range", Stats.Range, StatColor);
        SetStat<float>("CriticalChance", Stats.CriticalChance, StatColor);
        SetStat<float>("CriticalMultiplier", Stats.CriticalMultiplier, StatColor);
        SetStat<float>("CriticalOverflow", Stats.CriticalOverflow, StatColor);
        //Defence
        //SetStat<int>("Armor", Stats.Armor,StatColor);
        SetStat<int>("ElementalArmor", Stats.ElementalArmor, StatColor);
        SetStat<float>("DamageReduction", Stats.DamageReduction, StatColor);
        SetStat<float>("Block", Stats.Block, StatColor);
        SetStat<float>("BlockChance", Stats.BlockChance, StatColor);
        SetStat<float>("Evasion", Stats.Evasion, StatColor);
        SetStat<float>("EvasionChance", Stats.EvasionChance, StatColor);
        //Misc
        SetStat<float>("ColdownReduction", Stats.ColdownReduction, StatColor);
        SetStat<float>("Speed", Stats.Speed, StatColor);
        SetStat<float>("VisionRadius", Stats.VisionRadius, StatColor);
    }

}
