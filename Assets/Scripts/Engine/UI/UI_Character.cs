using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_Character : MonoBehaviour
{
    [Header("General Components:")]
    [SerializeField] private Text Title = null;
    [SerializeField] private Text NameTxt = null;
    [SerializeField] private Text EnergyTxt = null;

    [Header("Details")]
    [SerializeField] private ScrollRect Scroll = null;
    [SerializeField] private GameObject ListLabel = null;
    [SerializeField] private GameObject ListSeparator = null;
    [SerializeField] private GameObject ListSpace = null;

    [Header("Extras")]
    public int Spaces = 4;
    public int Separators = 5;
    public Color ListStatColor;
    [SerializeField] private Image HealthBar = null;
    [SerializeField] private Image EnergyBar = null;
    [SerializeField] private Image ExperienceBar = null;
    [SerializeField] private Text LevelTxt = null;

    [Header("Gender")]
    public EGender Gender;
    public Color GenderColor = Color.white;
    [SerializeField] private Image GenderIcon = null;
    [SerializeField] private Text GenderTitle = null;
    [SerializeField] private Text GenderDescription = null;
    [SerializeField] private Sprite FemaleIcon;
    [SerializeField] private Sprite MaleIcon;
    [SerializeField] private Sprite OtherIcon;

    [Header("Currency")]
    public int Currency;
    [SerializeField] private Text CurrencyValue;

    [Header("Base Attributes:")]
    [SerializeField] private Text PointLeftText;
    [SerializeField] public Button Strength;
    [SerializeField] public Button Endurance;
    [SerializeField] public Button Dexterity;
    [SerializeField] public Button Agility;
    [SerializeField] public Button Inteligence;
    [SerializeField] public Button Luck;

    [HideInInspector] public int LeftPoints;
    [HideInInspector] public int Level;
    [HideInInspector] public float Health, MaxHealth = 1;
    [HideInInspector] public float Energy, MaxEnergy = 1;
    [HideInInspector] public float Experience, MaxExperience = 1;

    public string Name { set { Title.text = value; } get { return Title.text; } }
    protected UI_EquipSlot[] EquipamentSlots;
    protected UI_Inventory InventoryComponent;

    private Dictionary<string, GameObject> Stats = new Dictionary<string, GameObject>();
    private List<GameObject> Separator = new List<GameObject>();
    private List<GameObject> Space = new List<GameObject>();

	protected void Awake ()
    {
        EquipamentSlots = GetComponentsInChildren<UI_EquipSlot>();
        if (EquipamentSlots.Length <= 0)
            Debug.LogError("UI_Character: Equipament slots invalid or not detected in childs objects");

        InventoryComponent = GetComponentInChildren<UI_Inventory>();
        if (!InventoryComponent)
            Debug.LogError("UI_Character: Inventory component are null or invalid on childs objects");

        bool ButtonValidation = true;
        ButtonValidation &= Strength;
        ButtonValidation &= Agility;
        ButtonValidation &= Dexterity;
        ButtonValidation &= Endurance;
        ButtonValidation &= Inteligence;
        ButtonValidation &= Luck;

        if(ButtonValidation) Debug.Log("UI_Character: All Buttons Valid");

        for (int i = 0; i < Spaces; i++)
        { Space.Add(Instantiate(ListSpace, Scroll.content)); }
        
        for(int i = 0; i < Separators; i++)
        { Separator.Add(Instantiate(ListSeparator, Scroll.content)); }

        if(GenderIcon && GenderTitle)
        {
            GenderTitle.color = GenderColor;
            switch (Gender)
            {
                //Other:
                case EGender.Other:
                    GenderIcon.sprite = OtherIcon;
                    GenderTitle.text = "Gender: Other";
                    break;
                //Male:
                case EGender.Male:
                    GenderIcon.sprite = MaleIcon;
                    GenderTitle.text = "Gender: Male";
                    break;
                //Female:
                case EGender.Female:
                    GenderIcon.sprite = FemaleIcon;
                    GenderTitle.text = "Gender: Female";
                    break;
            }
        }
        
    }

    protected void Start()
    {
        if (NameTxt) NameTxt.text = GameManager.Instance.Character.Name;
    }

    protected void Update()
    {
        HealthBar.fillAmount = Mathf.Clamp01(Health / MaxHealth);
        EnergyBar.fillAmount = Mathf.Clamp01(Energy / MaxEnergy);
        ExperienceBar.fillAmount = Mathf.Clamp01(Experience / MaxExperience);
        LevelTxt.text = Level.ToString();
        PointLeftText.text = LeftPoints.ToString();
        CurrencyValue.text = Currency.ToString();
        EnergyTxt.text = GameManager.Instance.Character.EnergyType.ToString();
        

        if(LeftPoints <= 0)
        {
            Strength.interactable = false;
            Agility.interactable = false;
            Dexterity.interactable = false;
            Endurance.interactable = false;
            Inteligence.interactable = false;
            Luck.interactable = false;
        }
    }

    public bool TryToAddItem(Item NewItem)
    {
        Vector2Int Index = UI_Inventory.InvalidIndex;
        UI_Item Cache = new UI_Item(NewItem.Id, NewItem.Icon, NewItem.Size);
        Index = InventoryComponent.AddItem(Cache);
        return Index != UI_Inventory.InvalidIndex;
    }

    public bool TryToEquipItem(Item NewItem, UI_EquipSlot Slot)
    {
        if(NewItem && Slot)
        {
            return NewItem.SlotType == Slot.Type || NewItem.SlotType == UI_EquipSlot.SlotType.All;
        }
        return false;
    }

    public void SetProperties()
    {
        Stat Stats = GameManager.Instance.Character.Stats;

        //Base:
        SetSeparator("Base",0,0);
        SetStat<int>("Strength", Stats.Strength, ListStatColor);
        SetStat<int>("Agility", Stats.Agility, ListStatColor);
        SetStat<int>("Dexterity", Stats.Dexterity, ListStatColor);
        SetStat<int>("Inteligence", Stats.Inteligence, ListStatColor);
        SetStat<int>("Endurance", Stats.Endurance, ListStatColor);
        SetStat<int>("Luck", Stats.Luck);
        SetSpace(0, 8);
        //Generic
        SetSeparator("Generic:",1,8);
        SetStat<float>("Health", Stats.Health, ListStatColor);
        SetStat<float>("MaxHealth", Stats.MaxHealth, ListStatColor);
        SetStat<float>("HealthRegeneration", Stats.HealthRegeneration, ListStatColor);
        SetStat<float>("Energy", Stats.Energy, ListStatColor);
        SetStat<float>("MaxEnergy", Stats.MaxEnergy, ListStatColor);
        SetStat<float>("EnergyRegeneration", Stats.EnergyRegeneration, ListStatColor);
        SetStat<float>("Shield", Stats.Shield, ListStatColor);
        SetStat<float>("ShieldRegeneration", Stats.ShieldRegeneration, ListStatColor);
        SetSpace(1, 18);
        //Offence
        SetSeparator("Offensive",2,18);
        SetStat<float>("Damage", Stats.Damage, ListStatColor);
        SetStat<float>("DamageMultiplier", Stats.DamageMultiplier, ListStatColor);
        SetStat<float>("AttackSpeed", Stats.AttackSpeed, ListStatColor);
        SetStat<int>("Range", Stats.Range, ListStatColor);
        SetStat<float>("CriticalChance", Stats.CriticalChance, ListStatColor);
        SetStat<float>("CriticalMultiplier", Stats.CriticalMultiplier, ListStatColor);
        SetStat<float>("CriticalOverflow", Stats.CriticalOverflow, ListStatColor);
        SetSpace(2, 27);
        //Defence
        SetSeparator("Defensive",3,27);
        SetStat<int>("Armor", Stats.Armor, ListStatColor);
        SetStat<int>("ElementalArmor", Stats.ElementalArmor, ListStatColor);
        SetStat<float>("DamageReduction", Stats.DamageReduction, ListStatColor);
        SetStat<float>("Block", Stats.Block, ListStatColor);
        SetStat<float>("BlockChance", Stats.BlockChance, ListStatColor);
        SetStat<float>("Evasion", Stats.Evasion, ListStatColor);
        SetStat<float>("EvasionChance", Stats.EvasionChance, ListStatColor);
        SetSpace(3, 36);
        //Misc
        SetSeparator("Miselanius",4,36);
        SetStat<float>("ColdownReduction", Stats.ColdownReduction, ListStatColor);
        SetStat<float>("Speed", Stats.Speed, ListStatColor);
        SetStat<float>("VisionRadius", Stats.VisionRadius, ListStatColor);
        
    }

    void SetStat<T>(string Name,T Stat)
    {
        SetStat<T>(Name, Stat, Color.white);
    }

    void SetStat<T>(string Name,T Stat,Color Color,string Unit = "")
    {
        if (Stats.ContainsKey(Name))
        {
            UI_Stat StatComponent = Stats[Name].GetComponent<UI_Stat>();
            StatComponent.Value = Stat + Unit;
            StatComponent.Description = Name;
            StatComponent.DescriptionColor = Color;
        }
        else
        {
            Stats.Add(Name, Instantiate(ListLabel, Scroll.content));
            UI_Stat StatComponent = Stats[Name].GetComponent<UI_Stat>();
            StatComponent.Value = Stat + Unit;
            StatComponent.Description = Name;
            StatComponent.DescriptionColor = Color;
        }
    }

    void SetSeparator(string Text,int Index,int Position)
    {
        if (Index < 0 || Index >= Separator.Count) return;

        GameObject Object = Separator[Index];
        Object.GetComponentInChildren<Text>().text = Text;
        Object.transform.SetSiblingIndex(Position);
        Object.name = Text;
    }

    void SetSpace(int Index,int PositionIndex)
    {
        if (Index < 0 || Index >= Space.Count) return;

        Space[Index].transform.SetSiblingIndex(PositionIndex);
    }
}
