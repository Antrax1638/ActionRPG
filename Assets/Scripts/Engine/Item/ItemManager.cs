using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class ItemManager : MonoBehaviour
{
	public static ItemManager Instance { get{ return _Instance; } }
	private static ItemManager _Instance;

	[Header("Item Manager")]
    public GameObject Prefab;
    public string Path = "";
    public string Extension = ".json";

    [Header("User Iterface")]
    [SerializeField] private string Tag = "Item";
    [SerializeField] private GameObject Preview;
    [SerializeField] private string Canvas = "MainCanvas";

    [Header("Static")]
    [Tooltip("Player controller reference")] public PlayerController Controller;
    [Tooltip("Player character reference")] public Character Character;

    [Header("Actions")]
    public string LeftClickAction = "Fire1";

    [HideInInspector] public GameObject SelectedItem { get { return Object; } }

    private Text PreviewTextComponent;
    private Item SelectedItemComponent;
    private RectTransform PreviewTransform;
    private GameObject Object;
    RaycastHit Hit;
    bool HitPending;
    static int GeneratedIndex = int.MinValue; 

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GameObject MainCanvas = GameObject.FindGameObjectWithTag(Canvas);
        GameObject ButtonPrefab = Preview;
        Preview = Instantiate(ButtonPrefab, MainCanvas.transform);
        Preview.name = "UI_ItemPreview";
        PreviewTransform = Preview.transform as RectTransform;
        PreviewTextComponent = Preview.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        if (Preview && Controller)
        {
            Vector3 Mouse = new Vector3(Input.mousePosition.x,Input.mousePosition.y, Camera.main.nearClipPlane);
            Vector3 Direction = Camera.main.ScreenToWorldPoint(Mouse) - Camera.main.transform.position;
            if (Physics.SphereCast(Camera.main.transform.position, 0.3f,Direction.normalized, out Hit, 999999))
            {
                if (Hit.collider.tag == Tag)
                {
                    Controller.Mode = InputMode.InterfaceOnly;
                    HitPending = true;

                    Preview.SetActive(true);
                    PreviewTransform.anchoredPosition = Input.mousePosition;
                    SelectedItemComponent = Hit.collider.gameObject.GetComponent<Item>();
                    if (SelectedItemComponent)
                    {
                        PreviewTextComponent.text = SelectedItemComponent.Name;
                        PreviewTextComponent.color = SelectedItemComponent.Rarity;
                    }

                    if (Input.GetButtonDown(LeftClickAction))
                    {
                        Object = Hit.collider.gameObject;
                        Character.PickUp(Object);
                    }
                }
                else if(HitPending)
                {
                    Preview.SetActive(false);
                    Controller.Mode = InputMode.GameOnly;
                    HitPending = false;
                }
            }
        }

        
    }

    public void SaveItem(string Name,GameObject Prefab)
    {
        string Data = JsonUtility.ToJson(Prefab.GetComponent<Item>(), true);
        File.WriteAllText(Path + Name + Extension, Data);
    }

    public JsonData LoadItem(string Name)
    {
        string Data = File.ReadAllText(Path + Name + Extension);
        JsonReader Reader = new JsonReader(Data);
        JsonData StructData = JsonMapper.ToObject(Reader);
        return StructData;
    }

    public GameObject SpawnItem(string Name,Vector3 Position, Quaternion Rotation)
    {
        GameObject Object = GameObject.Instantiate(Prefab, Position, Rotation);
        Item ObjectData = Object.GetComponent<Item>();
        JsonData ItemData = LoadItem(Name);

        if (ObjectData && ItemData.IsObject)
        {
            ObjectData.Id = (int)ItemData["Id"];
            ObjectData.Name = (string)ItemData["Name"];
            ObjectData.Type = (EItemType)(int)ItemData["Type"];
            ObjectData.Description = (string)ItemData["Description"];
            //Basic Stats
            ObjectData.Stats.Strength = (int)ItemData["Stats"]["Strength"];
            ObjectData.Stats.Agility = (int)ItemData["Stats"]["Agility"];
            ObjectData.Stats.Dexterity = (int)ItemData["Stats"]["Dexterity"];
            ObjectData.Stats.Inteligence = (int)ItemData["Stats"]["Inteligence"];
            ObjectData.Stats.Endurance = (int)ItemData["Stats"]["Endurance"];
            ObjectData.Stats.Luck = (int)ItemData["Stats"]["Luck"];
            
            //Generic Stats:
            ObjectData.Stats.Health = Convert.ToSingle((double)ItemData["Stats"]["Health"]);
            ObjectData.Stats.MaxHealth = Convert.ToSingle((double)ItemData["Stats"]["MaxHealth"]);
            ObjectData.Stats.HealthRegeneration = Convert.ToSingle((double)ItemData["Stats"]["HealthRegeneration"]);
            ObjectData.Stats.Energy = Convert.ToSingle((double)ItemData["Stats"]["Energy"]);
            ObjectData.Stats.MaxEnergy = Convert.ToSingle((double)ItemData["Stats"]["MaxEnergy"]);
            ObjectData.Stats.EnergyRegeneration = Convert.ToSingle((double)ItemData["Stats"]["EnergyRegeneration"]);
            ObjectData.Stats.Shield = Convert.ToSingle((double)ItemData["Stats"]["Shield"]);
            ObjectData.Stats.ShieldRegeneration = Convert.ToSingle((double)ItemData["Stats"]["ShieldRegeneration"]);

            //Damage
            ObjectData.Stats.Damage = Convert.ToSingle((double)ItemData["Stats"]["Damage"]);
            ObjectData.Stats.DamageMultiplier = Convert.ToSingle((double)ItemData["Stats"]["DamageMultiplier"]);
            ObjectData.Stats.AttackSpeed = Convert.ToSingle((double)ItemData["Stats"]["AttackSpeed"]);
            ObjectData.Stats.Range = (int)ItemData["Stats"]["Range"];
            ObjectData.Stats.CriticalChance = Convert.ToSingle((double)ItemData["Stats"]["CriticalChance"]);
            ObjectData.Stats.CriticalMultiplier = Convert.ToSingle((double)ItemData["Stats"]["CriticalMultiplier"]);
            ObjectData.Stats.CriticalOverflow = Convert.ToSingle((double)ItemData["Stats"]["CriticalOverflow"]);

            //Defence Stats:
            ObjectData.Stats.Armor = (int)ItemData["Stats"]["Armor"];
            ObjectData.Stats.ElementalArmor = (int)ItemData["Stats"]["ElementalArmor"];
            ObjectData.Stats.DamageReduction = Convert.ToSingle((double)ItemData["Stats"]["DamageReduction"]);
            ObjectData.Stats.Block = Convert.ToSingle((double)ItemData["Stats"]["Block"]);
            ObjectData.Stats.BlockChance = Convert.ToSingle((double)ItemData["Stats"]["BlockChance"]);
            ObjectData.Stats.Evasion = Convert.ToSingle((double)ItemData["Stats"]["Evasion"]);
            ObjectData.Stats.EvasionChance = Convert.ToSingle((double)ItemData["Stats"]["EvasionChance"]);

            //Misc Stats:
            ObjectData.Stats.ColdownReduction = Convert.ToSingle((double)ItemData["Stats"]["ColdownReduction"]);
            ObjectData.Stats.Speed = Convert.ToSingle((double)ItemData["Stats"]["Speed"]);
            ObjectData.Stats.VisionRadius = Convert.ToSingle((double)ItemData["Stats"]["VisionRadius"]);
        }

        return Object;
    }

    public GameObject Search(string Name)
    {
        return GameObject.Find(Name);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Hit.point, 0.2f);
    }

    public int GenerateId()
    {
        return Mathf.Clamp(GeneratedIndex++, int.MinValue + 1, int.MinValue);
    }
}
