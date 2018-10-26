using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    Item Target;
    bool StatTab,RStatTab;
    Vector2 Scroll;

	protected void OnEnable ()
    {
        Target = target as Item;
	}
	
	public override void OnInspectorGUI ()
    {
        GUILayout.Space(10);
        Target.Name = EditorGUILayout.TextField("Name", Target.Name);

        GUI.skin.label.fontStyle = FontStyle.Normal;
        GUILayout.Label("Description");
        Scroll = EditorGUILayout.BeginScrollView(Scroll);
        Target.Description = EditorGUILayout.TextArea(Target.Description);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        Target.AutoGenerateId = EditorGUILayout.Toggle(new GUIContent("Id", "Auto generate an Id for the current item"),Target.AutoGenerateId);
        if(!Target.AutoGenerateId) Target.Id = EditorGUILayout.IntField(new GUIContent("","Manual id of the item"),Target.Id);
        EditorGUILayout.EndHorizontal();
        Target.Stack = EditorGUILayout.IntField("Stack",Target.Stack);
        Target.MaxStack = EditorGUILayout.IntField("Max",Target.MaxStack);
        Target.Type = (EItemType)EditorGUILayout.EnumPopup("Type", Target.Type);
        Target.SlotType = (UI_EquipSlot.SlotType)EditorGUILayout.EnumPopup("Slot Type", Target.SlotType);
        Target.Rarity = EditorGUILayout.ColorField("Rarity", Target.Rarity);
        Target.Icon = (Sprite)EditorGUILayout.ObjectField("Icon", Target.Icon, typeof(Sprite), true);
        Target.Size = EditorGUILayout.Vector2IntField(new GUIContent("Size","Icon size in inventory space grid"), Target.Size);

        EditorGUILayout.BeginHorizontal("Box");
        GUILayout.Space(15);
        EditorGUILayout.BeginVertical();
        GUI.skin.label.fontStyle = FontStyle.Bold;
        StatTab = EditorGUILayout.Foldout(StatTab, new GUIContent("Stats", "current item stats"), true);
        if (StatTab)
        {
            Stats();
        }
        RStatTab = EditorGUILayout.Foldout(RStatTab, new GUIContent("Requiered Stats", "requiered item stats to be equiped"), true);
        if (RStatTab)
        {
            RStats();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    void Stats()
    {
        GUI.color = Color.white;
        GUILayout.Space(5);
        GUILayout.Label("Base Stats:");
        Target.Stats.Strength = EditorGUILayout.IntField("Strength", Target.Stats.Strength);
        Target.Stats.Agility = EditorGUILayout.IntField("Agility", Target.Stats.Agility);
        Target.Stats.Dexterity = EditorGUILayout.IntField("Dexterity", Target.Stats.Dexterity);
        Target.Stats.Inteligence = EditorGUILayout.IntField("Inteligence", Target.Stats.Inteligence);
        Target.Stats.Endurance = EditorGUILayout.IntField("Endurance", Target.Stats.Endurance);
        Target.Stats.Luck = EditorGUILayout.IntField("Luck", Target.Stats.Luck);

        GUILayout.Space(10);
        GUI.color = Color.yellow;
        GUILayout.Label("Generic Stats:");
        Target.Stats.Health = EditorGUILayout.FloatField("Health", Target.Stats.Health);
        Target.Stats.MaxHealth = EditorGUILayout.FloatField("MaxHealth", Target.Stats.MaxHealth);
        Target.Stats.HealthRegeneration = EditorGUILayout.FloatField("HealthRegeneration", Target.Stats.HealthRegeneration);
        Target.Stats.Energy = EditorGUILayout.FloatField("Energy", Target.Stats.Energy);
        Target.Stats.MaxEnergy = EditorGUILayout.FloatField("MaxEnergy", Target.Stats.MaxEnergy);
        Target.Stats.EnergyRegeneration = EditorGUILayout.FloatField("EnergyRegeneration", Target.Stats.EnergyRegeneration);
        Target.Stats.Shield = EditorGUILayout.FloatField("Shield", Target.Stats.Shield);
        Target.Stats.ShieldRegeneration = EditorGUILayout.FloatField("ShieldRegeneration", Target.Stats.ShieldRegeneration);

        GUILayout.Space(10);
        GUI.color = Color.red;
        GUILayout.Label("Damage Stats:");
        Target.Stats.Damage = EditorGUILayout.FloatField("Damage", Target.Stats.Damage);
        Target.Stats.DamageMultiplier = EditorGUILayout.FloatField("DamageMultiplier", Target.Stats.DamageMultiplier);
        Target.Stats.AttackSpeed = EditorGUILayout.FloatField("AttackSpeed", Target.Stats.AttackSpeed);
        Target.Stats.Range = EditorGUILayout.IntField("Range", Target.Stats.Range);
        Target.Stats.CriticalChance = EditorGUILayout.FloatField("CriticalChance", Target.Stats.CriticalChance);
        Target.Stats.CriticalMultiplier = EditorGUILayout.FloatField("CriticalMultiplier", Target.Stats.CriticalMultiplier);
        Target.Stats.CriticalOverflow = EditorGUILayout.FloatField("CriticalOverflow", Target.Stats.CriticalOverflow);

        GUILayout.Space(10);
        GUI.color = Color.green;
        GUILayout.Label("Defense Stats:");
        Target.Stats.Armor = EditorGUILayout.IntField("Armor", Target.Stats.Armor);
        Target.Stats.ElementalArmor = EditorGUILayout.IntField("ElementalArmor", Target.Stats.ElementalArmor);
        Target.Stats.DamageReduction = EditorGUILayout.FloatField("DamageReduction", Target.Stats.DamageReduction);
        Target.Stats.Block = EditorGUILayout.FloatField("Block", Target.Stats.Block);
        Target.Stats.BlockChance = EditorGUILayout.FloatField("BlockChance", Target.Stats.BlockChance);
        Target.Stats.Evasion = EditorGUILayout.FloatField("Evasion", Target.Stats.Evasion);
        Target.Stats.EvasionChance = EditorGUILayout.FloatField("EvasionChance", Target.Stats.EvasionChance);

        GUILayout.Space(10);
        GUI.color = Color.cyan;
        GUILayout.Label("Misc Stats:");
        Target.Stats.ColdownReduction = EditorGUILayout.FloatField("ColdownReduction", Target.Stats.ColdownReduction);
        Target.Stats.Speed = EditorGUILayout.FloatField("Speed", Target.Stats.Speed);
        Target.Stats.VisionRadius = EditorGUILayout.FloatField("VisionRadius", Target.Stats.VisionRadius);

    }

    void RStats()
    {
        GUI.color = Color.white;
        GUILayout.Space(5);
        GUILayout.Label("Base RequieredStats:");
        Target.RequieredStats.Strength = EditorGUILayout.IntField("Strength", Target.RequieredStats.Strength);
        Target.RequieredStats.Agility = EditorGUILayout.IntField("Agility", Target.RequieredStats.Agility);
        Target.RequieredStats.Dexterity = EditorGUILayout.IntField("Dexterity", Target.RequieredStats.Dexterity);
        Target.RequieredStats.Inteligence = EditorGUILayout.IntField("Inteligence", Target.RequieredStats.Inteligence);
        Target.RequieredStats.Endurance = EditorGUILayout.IntField("Endurance", Target.RequieredStats.Endurance);
        Target.RequieredStats.Luck = EditorGUILayout.IntField("Luck", Target.RequieredStats.Luck);

        GUILayout.Space(10);
        GUI.color = Color.yellow;
        GUILayout.Label("Generic RequieredStats:");
        Target.RequieredStats.Health = EditorGUILayout.FloatField("Health", Target.RequieredStats.Health);
        Target.RequieredStats.MaxHealth = EditorGUILayout.FloatField("MaxHealth", Target.RequieredStats.MaxHealth);
        Target.RequieredStats.HealthRegeneration = EditorGUILayout.FloatField("HealthRegeneration", Target.RequieredStats.HealthRegeneration);
        Target.RequieredStats.Energy = EditorGUILayout.FloatField("Energy", Target.RequieredStats.Energy);
        Target.RequieredStats.MaxEnergy = EditorGUILayout.FloatField("MaxEnergy", Target.RequieredStats.MaxEnergy);
        Target.RequieredStats.EnergyRegeneration = EditorGUILayout.FloatField("EnergyRegeneration", Target.RequieredStats.EnergyRegeneration);
        Target.RequieredStats.Shield = EditorGUILayout.FloatField("Shield", Target.RequieredStats.Shield);
        Target.RequieredStats.ShieldRegeneration = EditorGUILayout.FloatField("ShieldRegeneration", Target.RequieredStats.ShieldRegeneration);

        GUILayout.Space(10);
        GUI.color = Color.red;
        GUILayout.Label("Damage RequieredStats:");
        Target.RequieredStats.Damage = EditorGUILayout.FloatField("Damage", Target.RequieredStats.Damage);
        Target.RequieredStats.DamageMultiplier = EditorGUILayout.FloatField("DamageMultiplier", Target.RequieredStats.DamageMultiplier);
        Target.RequieredStats.AttackSpeed = EditorGUILayout.FloatField("AttackSpeed", Target.RequieredStats.AttackSpeed);
        Target.RequieredStats.Range = EditorGUILayout.IntField("Range", Target.RequieredStats.Range);
        Target.RequieredStats.CriticalChance = EditorGUILayout.FloatField("CriticalChance", Target.RequieredStats.CriticalChance);
        Target.RequieredStats.CriticalMultiplier = EditorGUILayout.FloatField("CriticalMultiplier", Target.RequieredStats.CriticalMultiplier);
        Target.RequieredStats.CriticalOverflow = EditorGUILayout.FloatField("CriticalOverflow", Target.RequieredStats.CriticalOverflow);

        GUILayout.Space(10);
        GUI.color = Color.green;
        GUILayout.Label("Defense RequieredStats:");
        Target.RequieredStats.Armor = EditorGUILayout.IntField("Armor", Target.RequieredStats.Armor);
        Target.RequieredStats.ElementalArmor = EditorGUILayout.IntField("ElementalArmor", Target.RequieredStats.ElementalArmor);
        Target.RequieredStats.DamageReduction = EditorGUILayout.FloatField("DamageReduction", Target.RequieredStats.DamageReduction);
        Target.RequieredStats.Block = EditorGUILayout.FloatField("Block", Target.RequieredStats.Block);
        Target.RequieredStats.BlockChance = EditorGUILayout.FloatField("BlockChance", Target.RequieredStats.BlockChance);
        Target.RequieredStats.Evasion = EditorGUILayout.FloatField("Evasion", Target.RequieredStats.Evasion);
        Target.RequieredStats.EvasionChance = EditorGUILayout.FloatField("EvasionChance", Target.RequieredStats.EvasionChance);

        GUILayout.Space(10);
        GUI.color = Color.cyan;
        GUILayout.Label("Misc RequieredStats:");
        Target.RequieredStats.ColdownReduction = EditorGUILayout.FloatField("ColdownReduction", Target.RequieredStats.ColdownReduction);
        Target.RequieredStats.Speed = EditorGUILayout.FloatField("Speed", Target.RequieredStats.Speed);
        Target.RequieredStats.VisionRadius = EditorGUILayout.FloatField("VisionRadius", Target.RequieredStats.VisionRadius);
    }
}
