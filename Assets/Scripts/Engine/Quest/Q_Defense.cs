using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Q_Defense : Q_Base
{
    [Header("Defense")]
    public GameObject Target;
    public float TargetHealth;
    public float TargetMaxHealth;
    public float Time;
    public AnimationCurve LevelFactor;

    private float ElapsedTime;
    private float TimeLeft;

    protected override void Awake ()
    {
        base.Awake();
        ElapsedTime = 0.0f;
	}

	void Update ()
    {
        if (!Completed)
        {
            ElapsedTime += UnityEngine.Time.deltaTime;
            TimeLeft = Time - ElapsedTime;
            TargetHealth = Mathf.Clamp(TargetHealth, -1.0f, TargetMaxHealth);

            if (TargetHealth <= 0)
            {
                Completed = true;
                Status = EQuestStatus.Falied;
            }

            if (TimeLeft <= 0 && TargetHealth > 0.0f)
            {
                Completed = true;
                Status = EQuestStatus.Finished;
            }
            
        }
	}
}
