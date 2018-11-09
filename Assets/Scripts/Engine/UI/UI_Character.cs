using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Text Title = null;
    [SerializeField] private ScrollRect Scroll = null;
    [SerializeField] private GameObject Label = null;
    [SerializeField] private Image HealthBar = null;
    [SerializeField] private Image EnergyBar = null;
    [SerializeField] private Image ExperienceBar = null;
    [SerializeField] private Text LevelTxt = null;

    [Header("Base Attributes:")]
    [SerializeField] private Text Strength;
    [SerializeField] private Text Endurance;
    [SerializeField] private Text Agility;
    [SerializeField] private Text Inteligence;

    [HideInInspector] public int Level;
    [HideInInspector] public float Health, MaxHealth = 1;
    [HideInInspector] public float Energy, MaxEnergy = 1;
    [HideInInspector] public float Experience, MaxExperience = 1;

    public string Name { set { Title.text = value; } get { return Title.text; } }
    protected UI_EquipSlot[] EquipamentSlots;
    protected UI_Inventory InventoryComponent;

    private Dictionary<string, GameObject> Stats = new Dictionary<string, GameObject>();

	protected void Awake ()
    {
        EquipamentSlots = GetComponentsInChildren<UI_EquipSlot>();
        if (EquipamentSlots.Length <= 0)
            Debug.LogError("UI_Character: Equipament slots invalid or not detected in childs objects");

        InventoryComponent = GetComponentInChildren<UI_Inventory>();
        if (!InventoryComponent)
            Debug.LogError("UI_Character: Inventory component are null or invalid on childs objects");

    }

    protected void Update()
    {
        HealthBar.fillAmount = Mathf.Clamp01(Health / MaxHealth);
        EnergyBar.fillAmount = Mathf.Clamp01(Energy / MaxEnergy);
        ExperienceBar.fillAmount = Mathf.Clamp01(Experience / MaxExperience);
        LevelTxt.text = Level.ToString();
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
        Stat Stats = ItemManager.Instance.Character.Stats;

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

    void SetStat<T>(string Name,T Stat)
    {
        SetStat<T>(Name, Stat, Color.white, "", 1);
    }

    void SetStat<T>(string Name,T Stat,Color Color,string Unit,int Space = 1)
    {
        string StatText = "";
        string Spaces = "";
        char Operator = (System.Convert.ToSingle(Stat) >= 0.0f) ? '+' : '-';
        for (int i = 0; i < Space; i++) Spaces += " ";

        StatText = Name + Spaces + Operator + Stat + Unit;

        if (Stats.ContainsKey(Name))
        {
            Stats[Name].GetComponent<Text>().text = StatText;
            Stats[Name].GetComponent<Text>().color = Color;
        }
        else
        {
            Stats.Add(Name, Instantiate(Label, Scroll.content));
            Stats[Name].GetComponent<Text>().text = StatText;
            Stats[Name].GetComponent<Text>().color = Color;
        }
    }
}
