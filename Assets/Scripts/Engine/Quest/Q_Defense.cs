using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Q_Defense : Q_Base
{
    [Header("Defense")]
    public GameObject Target;
    public float TargetHealth;
    public float TargetMaxHealth;
    public float Time;
    public AnimationCurve LevelFactor;
    public GameObject TextPrefab;

    private Text TimeLeftText;
    private float ElapsedTime;
    private float TimeLeft;

    protected override void Awake ()
    {
        base.Awake();
        ElapsedTime = 0.0f;
        gameObject.SetActive(false);
	}

    protected void Start()
    {
        TextPrefab = Instantiate(TextPrefab, UI_Hud.main.transform);
        TimeLeftText = TextPrefab.GetComponent<Text>();
        TimeLeftText.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(transform.position);
    }

    void Update ()
    {
        if (!Completed)
        {
            TimeLeft = Mathf.Clamp(TimeLeft, 0.0f, Time);
            TimeLeftText.text = TimeLeft.ToString();
            TimeLeftText.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(Target.transform.position);

            ElapsedTime += UnityEngine.Time.deltaTime;
            TimeLeft = Time - ElapsedTime;
            TargetHealth = Mathf.Clamp(TargetHealth, -1.0f, TargetMaxHealth);

            if (TargetHealth <= 0)
            {
                Completed = true;
                Status = EQuestStatus.Falied;
                Debug.LogWarning("Mision Fallada!");
            }

            if (TimeLeft <= 0 && TargetHealth > 0.0f)
            {
                Completed = true;
                Status = EQuestStatus.Finished;
                Debug.LogWarning("Mision Completada!");
            }
            
        }
	}
}
