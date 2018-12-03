using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnergyType
{
    None,
    Mana,
    Fury,
    Soul,
    Ki,
    Wrath,
    Spirit
}

public class Character : MonoBehaviour
{
    [Header("Character")]
    public string Name;
    public EGender Gender;
    public int Level;
    public float Experience;
    public float MaxExperience;
    public float ExperienceBase;
    public int StatsPerLevel;
    public AnimationCurve ExperienceCurve;
    
    public Stat Stats;

    [Header("Energy")]
    public EEnergyType EnergyType;
    public Color[] EnergyColors;

    [Header("Drop")]
    [SerializeField] Vector3 DropOffset;
    [SerializeField] float DropForce = 5;
    [SerializeField] float DropAngularForce = 5;

    public EquipamentManager Equipament;
    public InventoryManager Inventory;

    [Header("Sockets")]
    [SerializeField] private Transform Head = null;
    [SerializeField] private Transform LeftHand = null;
    [SerializeField] private Transform RightHand = null;

    private int StatRemaining;
    private int StatUsed;

    private void Start()
    {
        UI_Hud.main.CharacterReference.Name = Name;
        UI_Hud.main.CharacterReference.SetProperties();
        StatRemaining = StatsPerLevel;
        StatUsed = 0;
    }

    private void Update()
    {
        MaxExperience = ExperienceBase * ExperienceCurve.Evaluate(Level + 1);
        Experience = Mathf.Clamp(Experience, 0.0f, MaxExperience);
        if (Experience >= MaxExperience) {
            OnLevelUp();
        }

        switch (EnergyType) {
            default:break;
            case EEnergyType.Fury: break;
        }

        //Character
        UI_Hud.main.CharacterReference.Health = Stats.Health;
        UI_Hud.main.CharacterReference.MaxHealth = Stats.MaxHealth;
        UI_Hud.main.CharacterReference.Energy = Stats.Energy;
        UI_Hud.main.CharacterReference.MaxEnergy = Stats.MaxEnergy;
        UI_Hud.main.CharacterReference.Experience = Experience;
        UI_Hud.main.CharacterReference.MaxExperience = MaxExperience;
        UI_Hud.main.CharacterReference.Level = Level;
        //Action Bar
        UI_Hud.main.ActionBarReference.Health = Stats.Health;
        UI_Hud.main.ActionBarReference.MaxHealth = Stats.MaxHealth;
        UI_Hud.main.ActionBarReference.Energy = Stats.Energy;
        UI_Hud.main.ActionBarReference.MaxEnergy = Stats.MaxEnergy;
        UI_Hud.main.ActionBarReference.Experience = Experience;
        UI_Hud.main.ActionBarReference.MaxExperience = MaxExperience;
        int EnergyIndex = Mathf.Clamp((int)EnergyType, 0, EnergyColors.Length - 1);
        UI_Hud.main.ActionBarReference.EnergyColor = EnergyColors[EnergyIndex];
    }

    public void PickUp(GameObject Object)
    {
        int Index = -1;
        Item PickedItem = Object.GetComponent<Item>();
        if (PickedItem && Inventory != null) {
            if (UI_Hud.main.CharacterReference.TryToAddItem(PickedItem))
            {
                PickedItem.OnPickUp();
                Index = Inventory.AddItem(PickedItem.gameObject);
            }
        }

        if (Index < 0)
            Debug.Log("Character "+Name+" Item cannot be added to inventory.");
    }

    public void Drop(int Id)
    {
        if (Inventory != null)
        {
            Item Current;
            int Index = Inventory.FindIndex(Id);
            if (Inventory.IsValidIndex(Index))
            {
                Current = Inventory.RemoveAt(Index).GetComponent<Item>();
                Current.OnDrop();
                Current.transform.position = transform.position + DropOffset;
                Current.GetComponent<Rigidbody>().AddForce(transform.forward * DropForce, ForceMode.Impulse);
                Current.GetComponent<Rigidbody>().AddTorque(Current.transform.forward * DropAngularForce, ForceMode.Impulse);
            }
        }
    }

    public bool Equip(int Id, UI_EquipSlot Slot)
    {
        bool Success = false;

        Item Temp = Inventory.Find(Id).GetComponent<Item>();
        if (UI_Hud.main.CharacterReference.TryToEquipItem(Temp, Slot))
        {
            UI_EquipSlot.SlotType NewType;
            Stats = Stats + Temp.Stats;
            NewType = (Temp.SlotType != UI_EquipSlot.SlotType.All) ? Temp.SlotType : Slot.Type;
            switch (NewType)
            {
                //WIP
                case UI_EquipSlot.SlotType.Head: Temp.OnEquip(null); break;
                case UI_EquipSlot.SlotType.Body: break;
                case UI_EquipSlot.SlotType.Pants: break;
                case UI_EquipSlot.SlotType.Arms: break;
                case UI_EquipSlot.SlotType.Foot: break;
                case UI_EquipSlot.SlotType.LeftWeapon: Temp.OnEquip(LeftHand); break;
                case UI_EquipSlot.SlotType.RightWeapon: Temp.OnEquip(RightHand); break;
                case UI_EquipSlot.SlotType.LeftRing: break;
                case UI_EquipSlot.SlotType.RightRing: break;
                case UI_EquipSlot.SlotType.Trinket: break;
                default: break;
            }
            Temp.transform.localPosition = Vector3.zero;
            Temp.transform.localRotation = Quaternion.identity;

            Success = true;
        }
        return Success;
    }

    public void UnEquip(int Id, UI_EquipSlot Slot)
    {
        GameObject Object = Inventory.Find(Id);
        if (Object)
        {
            Item ObjectItem = Object.GetComponent<Item>();
            Stats = Stats - ObjectItem.Stats;
            ObjectItem.OnUnEquip();
        }
    }

    public void SetEnergyType(int Value)
    {
        EnergyType = (EEnergyType)Value;
    }

    protected virtual void OnLevelUp()
    {
        Level++;
        Experience = 0.0f;
        StatRemaining += StatsPerLevel;
        UI_Hud.main.PassiveTreeReference.PointsLeft++;

        if (UI_Hud.main.CharacterReference) {
            UI_Hud.main.CharacterReference.LeftPoints = StatRemaining;
            UI_Hud.main.CharacterReference.Strength.interactable = true;
            UI_Hud.main.CharacterReference.Agility.interactable = true;
            UI_Hud.main.CharacterReference.Dexterity.interactable = true;
            UI_Hud.main.CharacterReference.Endurance.interactable = true;
            UI_Hud.main.CharacterReference.Inteligence.interactable = true;
            UI_Hud.main.CharacterReference.Luck.interactable = true;
        }
    }

    //Base Stat
    public void AddStrength(int Value)
    {
        Stats.Strength += Value;
    }

    public void AddAgility(int Value)
    {
        Stats.Agility += Value;
    }

    public void AddDexterity(int Value)
    {
        Stats.Dexterity += Value;
    }

    public void AddEndurance(int Value)
    {
        Stats.Endurance += Value;
    }

    public void AddInteligence(int Value)
    {
        Stats.Inteligence += Value;
    }

    public void AddLuck(int Value)
    {
        Stats.Luck += Value;
    }
    //Base Stat
}
