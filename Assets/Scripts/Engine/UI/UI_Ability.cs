using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_Ability : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum AbilityType
    {
        None,
        Default,
        Toggle,
        Held,
    }

    [Header("Properties:")]
    public int Id;
    public string Filter;
    public bool Interactable;
    public AbilityType Type;
    public string Action;
    public int Level;
    public int MaxLevel;
    public float ColdDown;
    public GameObject Prefab;
    public string Name,Description;
    public float Cost;
 
    public Sprite Icon { set { IconComponent.sprite = value; } get { return IconComponent.sprite; } }
   
    private static GameObject DragCompoenent,ToolTipComponent;
    private RectTransform TransformComponent;
    private bool Validate { get { return (Id > -1 && Icon); } }
    private bool OnColdDown;
    private int TotalLevel;
    private float ElapsedTime,RemainingTime;

    [Header("Components")]
    [SerializeField] private Image IconComponent = null;
    [SerializeField] private Image FrameComponent = null;
    [SerializeField] private Image FillComponent = null;
    [SerializeField] private Text LevelComponent = null;
    [SerializeField] private Text ColdDownComponent = null;

    void Awake()
    {
        TransformComponent = transform as RectTransform;
        if (!IconComponent) Debug.LogError("UI_Ablility: Icon component is null");
        if (!FrameComponent) Debug.LogError("UI_Ability: Frame component is null");
        if (!FillComponent) Debug.LogError("UI_Ability: Fill component is null");
        if (!LevelComponent) Debug.LogError("UI_Ability: Level component is null");
        if (!ColdDownComponent) Debug.LogError("UI_Ability: ColdDown component is null");

        FillComponent.enabled = false;
        ColdDownComponent.enabled = false;
    }

    void Update()
    {
        TotalLevel = Mathf.Clamp(Level, 0, MaxLevel);
        LevelComponent.text = TotalLevel + "/" + MaxLevel;

        if (OnColdDown)
        {
            ElapsedTime += Time.deltaTime;
            RemainingTime = ColdDown - ElapsedTime;
            FillComponent.fillAmount = Mathf.Clamp01(RemainingTime / ColdDown);
            ColdDownComponent.text = System.Convert.ToInt32(RemainingTime).ToString();

            if (ElapsedTime > ColdDown)
            {
                ElapsedTime = 0.0f;
                RemainingTime = ColdDown;
                FillComponent.enabled = false;
                ColdDownComponent.enabled = false;

                OnColdDown = false;
            }
        }

        switch (Type)
        {
            default: break;
            case AbilityType.Default:
                if (Input.GetButtonDown(Action) && Interactable)
                {
                    Cast();
                }
                break;
            case AbilityType.Toggle: break;
            case AbilityType.Held: break;
        }

        IconComponent.enabled = (IconComponent.sprite);
    }
    //Funciones de Abilidad:
    public void Cast()
    {
        if(!OnColdDown)
        {
            OnColdDown = true;
            FillComponent.enabled = true;
            ColdDownComponent.enabled = true;

            print("Ability " + Id + " used");
        }
        else
        {
            print("Ability On ColdDown");
        }
    }

    //Funciones de Sistema:
    static public void InitializeDrag(GameObject Prefab,Transform Root)
    {
        DragCompoenent = Instantiate(Prefab, Root);
        DragCompoenent.name = "Drag";
        DragCompoenent.SetActive(false);
    }

    static public void InitializeToolTip(GameObject Prefab, Transform Root)
    {
        ToolTipComponent = Instantiate(Prefab, Root);
        ToolTipComponent.name = "ToolTip";
        ToolTipComponent.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (DragCompoenent && Validate)
        {
            DragCompoenent.SetActive(true);
            DragCompoenent.GetComponent<Image>().sprite = Icon;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DragCompoenent && Validate)
        {
            DragCompoenent.transform.position = Input.mousePosition;
            UI_Manager.Instance.SetInputMode((int)InputMode.InterfaceOnly);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DragCompoenent)
        {
            List<RaycastResult> Objects = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, Objects);
            UI_Ability DropComponent = null;
            for (int i = 0; i < Objects.Count; i++)
            {
                GameObject Current = Objects[i].gameObject;
                GameObject Parent = Current.transform.parent.gameObject;
                if(Parent.tag == Filter)
                {
                    DropComponent = Parent.GetComponent<UI_Ability>();
                    break;
                }
            }

            if (DropComponent)
            {
                if (DropComponent.Validate && DropComponent.Interactable && Interactable)
                {
                    Sprite OldIcon = DropComponent.Icon;
                    int OldId = DropComponent.Id;
                    int OldLevel = DropComponent.Level;
                    int OldMaxLevel = DropComponent.MaxLevel;
                    float OldColdDown = DropComponent.ColdDown;
                    GameObject OldPrefab = DropComponent.Prefab;
                    string OldName = DropComponent.Name;
                    string OldDescription = DropComponent.Description;
                    float OldCost = DropComponent.Cost;


                    DropComponent.Icon = Icon;
                    DropComponent.Id = Id;
                    DropComponent.Level = Level;
                    DropComponent.MaxLevel = MaxLevel;
                    DropComponent.ColdDown = ColdDown;
                    DropComponent.Prefab = Prefab;
                    DropComponent.Name = Name;
                    DropComponent.Description = Description;
                    DropComponent.Cost = Cost;

                    Icon = OldIcon;
                    Id = OldId;
                    Level = OldLevel;
                    MaxLevel = OldMaxLevel;
                    ColdDown = OldColdDown;
                    Prefab = OldPrefab;
                    Name = OldName;
                    Description = OldDescription;
                    Cost = OldCost;
                }
                else if(DropComponent.Interactable)
                {
                    DropComponent.Icon = Icon;
                    DropComponent.Id = Id;
                    DropComponent.Level = Level;
                    DropComponent.MaxLevel = MaxLevel;
                    DropComponent.ColdDown = ColdDown;
                    DropComponent.Prefab = Prefab;
                    DropComponent.Name = Name;
                    DropComponent.Description = Description;
                    DropComponent.Cost = Cost;
                    //print("Ability Unchanged");
                }
            }
            else if(Interactable)
            {
                Icon = null;
                Id = -1;
                Level = 0;
                MaxLevel = 0;
                ColdDown = 0.0f;
                Prefab = null;
                Name = "";
                Description = "";
                Cost = 0.0f;
            }

            DragCompoenent.SetActive(false);
        }
        UI_Manager.Instance.SetInputMode(InputMode.GameOnly);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ToolTipComponent && Validate)
        {
            ToolTipComponent.SetActive(true);
            ToolTipComponent.transform.position = eventData.position;
            UI_AbilityToolTip ToolTip = ToolTipComponent.GetComponent<UI_AbilityToolTip>();
            ToolTip.Name = Name;
            ToolTip.Description = Description;
            ToolTip.Icon = Icon;
            ToolTip.Type = "CD: "+ColdDown+" Cost: "+System.Convert.ToInt32(Cost)+" ["+Type+"]"; 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ToolTipComponent)
        {
            ToolTipComponent.SetActive(false);
        }
    }
}
