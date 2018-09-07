using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

[System.Serializable]
public enum EGender
{
	Male = -1,
	Female = 1,
	Other = 0,
}

public enum ESheath
{
	Hips = 1,
	Back = 0,
}

/*switch (Mode)
{
case 1: AnimatorComponent.SetTrigger ("RollForwardTrigger");	break;
case 2: AnimatorComponent.SetTrigger("RollRightTrigger");		break;
case 3: AnimatorComponent.SetTrigger ("RollBackwardTrigger");	break;
case 4: AnimatorComponent.SetTrigger ("RollLeftTrigger");		break;
default: break;
}*/

public enum ERoll
{
	RollFoward = 1,
	RollBackward = 3,
	RollLeft = 4,
	RollRight = 2
}

public enum EWeapon
{
	LeftRightAll = -2,
	Relax = -1,
	Unarmed = 0,
	TwoHandSword = 1,
	TwoHandSpear = 2,
	TwoHandAxe = 3,
	TwoHandBow = 4,
	TwoHandCrossBow = 5,
	TwoHandStaff = 6,
	Armed = 7,
	LeftSword = 8,
	RightSword = 9,
	LeftMace = 10,
	RightMace = 11,
	LeftDagger = 12,
	RightDagger = 13,
	LeftItem = 14,
	RightItem = 15,
	LeftPistol = 16,
	RightPistol = 17,
	Rifle = 18,
	RightSpear = 19,
	TwoHandClub = 20,
}

public enum EInteract
{
	Idle,
	PickUp,
	Activation,
}

public enum ECast
{
	Summon,
	Buff,
	Aoe,
}

public enum ECastMode
{
	Default,
	Channeling,
	Toggle,
	Instant,
	Strike
}

//weaponNumber -1 = Relax
//weaponNumber 0 = Unarmed
//weaponNumber 1 = 2H Sword
//weaponNumber 2 = 2H Spear
//weaponNumber 3 = 2H Axe
//weaponNumber 4 = 2H Bow
//weaponNumber 5 = 2H Crowwbow
//weaponNumber 6 = 2H Staff
//weaponNumber 7 = Shield
//weaponNumber 8 = L Sword
//weaponNumber 9 = R Sword
//weaponNumber 10 = L Mace
//weaponNumber 11 = R Mace
//weaponNumber 12 = L Dagger
//weaponNumber 13 = R Dagger
//weaponNumber 14 = L Item
//weaponNumber 15 = R Item
//weaponNumber 16 = L Pistol
//weaponNumber 17 = R Pistol
//weaponNumber 18 = Rifle
//weaponNumber 19 == Right Spear
//weaponNumber 20 == 2H Club

public class AnimationState : MonoBehaviour
{
	public int AttackValue;
	public EWeapon WeaponType;
	public ESheath SheathType;
	public EInteract InteractType;
	public ERoll RollType;
	public int LeftRight;
	public int DebugIdle = 0;

	public ECast CastType;
	public ECastMode DebugCastMode;

	//Valores Publicos Finales:
	[Header("Animation State Values:")]
	public EGender Gender;
	public float Charge = 0.0f;
	public int State = 0;
	public float SpeedMultiplier = 1.0f;
	public int Talking = 0;
	public bool Injured = false;
	public int CastAdd = 8;
	public int CastCap = 100;
	public float CastCapTime = 0.05f;
    public GameObject Controller = null;
    public Vector3 Velocity;


    //Animator:
    float BowPull,AimVertical,AimHorizontal;
	float CastDeltaTime = 0.0f,CastTime = 0.0f;
	ECastMode CastMode;
	int CastMeter = 0;
	bool Blocking = false,Shield = false,Crouch = false,Moving = false;

	//Components:
	private Animator AnimatorComponent;
	private AnimationEvents AnimationEventComponent;
	private AnimatorController AnimatorControllerComponent;
    private PlayerController PlayerControllerComponent;
    private MovementController MovementControllerComponent;

	//General:
	EWeapon WeaponState;
	Vector3 VelocityNormalized;
	bool IsDead,IsRoll,IsLock,IsCast;
	int AttackState = 1;
	float AttackTime = 0.0f;

	void Awake () 
	{
		AnimatorComponent = GetComponentInChildren<Animator>();
		if (!AnimatorComponent)
			Debug.LogError ("Animation State: Animator is  null.");
		else 
		{
			AnimatorControllerComponent = AnimatorComponent.runtimeAnimatorController as AnimatorController;
			if (!AnimatorControllerComponent)
				Debug.LogError ("Animation State: Animator Controller is null");
		}

        AnimationEventComponent = GetComponentInChildren<AnimationEvents>();
        if (!AnimationEventComponent)
            Debug.LogError("Animation State: Animation Events is null");

        PlayerControllerComponent = Controller.GetComponent<PlayerController>();
        if (!PlayerControllerComponent)
            Debug.LogError("Animation State: PlayerControllerBase component is null");

        MovementControllerComponent = Controller.GetComponent<MovementController>();
        if (!MovementControllerComponent)
            Debug.LogError("Animation State: Movement Controller component is null");
    }

	void Update () 
	{
		CastUpdate ();
		AttackTime = (AttackState <= 0) ? AttackTime + Time.deltaTime : AttackTime;

		/*if (IsRoll)
			ControllerComponent.transform.position = Vector3.Lerp (ControllerComponent.transform.position, transform.position, Time.deltaTime); 
            */
		if ( Controller ) 
		{
            switch (PlayerControllerComponent.Type)
            {
                case InputType.Physics:
                    VelocityNormalized = PlayerControllerComponent.GetBodyController().velocity;
                    VelocityNormalized /= MovementControllerComponent.MaxMovementSpeed;
                    VelocityNormalized = PlayerControllerComponent.transform.InverseTransformDirection (VelocityNormalized);

                    Moving = (PlayerControllerComponent.GetBodyController().velocity.magnitude > 0.5f);
                    break;
                case InputType.NoPhysics:
                    VelocityNormalized = PlayerControllerComponent.GetCharacterController().velocity;
                    VelocityNormalized /= MovementControllerComponent.MaxMovementSpeed;
                    VelocityNormalized = PlayerControllerComponent.transform.InverseTransformDirection(VelocityNormalized);

                    Moving = (PlayerControllerComponent.GetCharacterController().velocity.magnitude > 0.5f);
                    break;
                case InputType.Custom:
                    VelocityNormalized = Velocity / MovementControllerComponent.MaxMovementSpeed;
                    Moving = (Velocity.magnitude > 0.5f);
                    break;
            }

            Vector3 FinalVelocity = Vector3.zero;
            FinalVelocity.x = -VelocityNormalized.x;
            FinalVelocity.y = VelocityNormalized.y;
            FinalVelocity.z = (State == 0) ? VelocityNormalized.magnitude : -VelocityNormalized.z;

            AnimatorComponent.SetFloat ("Velocity X", FinalVelocity.x);
            AnimatorComponent.SetFloat ("Velocity Z", FinalVelocity.z);
            AnimatorComponent.SetFloat ("SpeedMultiplier", SpeedMultiplier);
            AnimatorComponent.SetBool ("Crouch", Crouch);
            AnimatorComponent.SetBool ("Moving", Moving);
            AnimatorComponent.SetFloat ("Charge", Charge);
            AnimatorComponent.SetInteger ("Talking", Talking);
            AnimatorComponent.SetBool ("Injured", Injured);
            AnimatorComponent.SetInteger ("Gender", (int)Gender);
        }
    }

	void SheathWeaponValues(int Weapon,int WeaponSwitch,int SheathLocation,int LeftRight)
	{
		if (AnimatorComponent) 
		{
			AnimatorComponent.SetBool ("Moving", Moving);						//[T F]
			AnimatorComponent.SetBool ("Shield", Shield);						//[T F]
			AnimatorComponent.SetInteger ("SheathLocation", SheathLocation);	//[0 1]
			AnimatorComponent.SetInteger ("LeftRight", LeftRight);				//[1 2 3]
			AnimatorComponent.SetInteger ("Weapon", Weapon);					//[-1]
			AnimatorComponent.SetInteger ("WeaponSwitch", WeaponSwitch); 		//[-1 7]
		}
	}

	void InstantSheathWeaponSwitch(EWeapon Weapon)
	{
		if (AnimatorComponent)
		{
			int WeaponNumber = (int)Weapon;
			AnimatorComponent.SetTrigger ("InstantSwitchTrigger");
			AnimatorComponent.SetInteger ("Weapon", WeaponNumber);
		}
	}

	void SheathWeaponSwitch(EWeapon From,EWeapon To,int LR,bool Dual,bool Safe)
	{
		int SheathLocation = Mathf.Clamp ((int)SheathType, 0, 1);
		int LeftRight = (Dual) ? 3 : LR;
		int WeaponNumber = (int)From;
		int WeaponSwitch = (int)To;
		if (Safe) 
		{
			switch (To) 
			{
			//Relax:
			case EWeapon.Relax:
				WeaponSwitch = (int)EWeapon.Relax;
				break;
			//Unarmed:
			case EWeapon.Unarmed: 
				SheathLocation = -1; 
				WeaponNumber = -1; 
				WeaponSwitch = -1;
				break;
			//AllRightLeft:
			case EWeapon.LeftRightAll:
				LeftRight = Mathf.Clamp (LeftRight, 1, 2);
			break;
			//Two Handed:
			case EWeapon.TwoHandAxe:
			case EWeapon.TwoHandClub:
			case EWeapon.TwoHandSpear:
			case EWeapon.TwoHandStaff:
			case EWeapon.TwoHandSword:
				WeaponSwitch = (int)EWeapon.Armed;
				LeftRight = Mathf.Clamp (LeftRight, 1, 3);
				break;
			//Left Handed:
			case EWeapon.LeftItem:
			case EWeapon.LeftDagger:
			case EWeapon.LeftMace:
			case EWeapon.LeftPistol:
			case EWeapon.LeftSword:
				WeaponSwitch = (int)EWeapon.Armed;
				LeftRight = 1;
				break;
			//Right Handed:
			case EWeapon.RightItem:
			case EWeapon.RightDagger:
			case EWeapon.RightMace:
			case EWeapon.RightPistol:
			case EWeapon.RightSword:
				WeaponSwitch = (int)EWeapon.Armed;
				LeftRight = 2;
				break;

			default: break;
			}
		}
		WeaponState = (EWeapon)WeaponSwitch;
		SheathWeaponValues (WeaponNumber, WeaponSwitch, SheathLocation, LeftRight);
	}

	public void SheathWeapon(EWeapon To,int LeftRight,bool Dual = false)
	{
        //Envainar arma
        PlayerControllerComponent.Mode = InputMode.None;
		AnimatorComponent.SetTrigger ("WeaponSheathTrigger");
		SheathWeaponSwitch (WeaponState, To, LeftRight, Dual, true);
        PlayerControllerComponent.Mode = InputMode.GameOnly;
        State = 0;
	}

	public void UnSheathWeapon(EWeapon Type,int LeftRight,bool Dual = false)
	{
        //Desenvainar arma
        PlayerControllerComponent.Mode = InputMode.None;
        AnimatorComponent.SetTrigger ("WeaponUnsheathTrigger");
		SheathWeaponSwitch (EWeapon.Relax, Type, LeftRight, Dual,true);
        PlayerControllerComponent.Mode = InputMode.GameOnly;
        State = 1;
 	}

	void AttackValues(int Weapon,int AttackSide,int LeftWeapon,int RightWeapon)
	{
		if (AnimatorComponent) 
		{
			AnimatorComponent.SetInteger ("Weapon", Weapon);
			AnimatorComponent.SetInteger ("AttackSide", AttackSide);
			AnimatorComponent.SetInteger ("LeftWeapon", LeftWeapon);
			AnimatorComponent.SetInteger ("RightWeapon", RightWeapon);
			AnimatorComponent.SetBool ("Moving", Moving);
			AnimatorComponent.SetBool ("Shield", Shield);
		}
	}

	public void Attack(EWeapon Type,int Value,bool Dual = false)
	{
		if (AttackState >= 1) 
		{
			AttackState = -1;
			string Name = "";
			int WeaponNumber = (int)Type,HalfMaxValue;
			string[] AnimationClipName = new string [2];
			int AttackValue = 0,MaxValue = 0;

			int OldLeft = AnimatorComponent.GetInteger ("LeftWeapon");
			int OldRight = AnimatorComponent.GetInteger ("RightWeapon");
			switch(Type)
			{
			case EWeapon.Relax:
				Name = "";
				MaxValue = 0;
				AttackValue = 1;
				AttackValues ((int)EWeapon.Relax, 0, -1, -1);
				break;

			case EWeapon.Unarmed:
				Name = "Unarmed";
				MaxValue = 6;
				AttackValue = Value;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AnimationClipName [1] = (AttackValue <= 3) ? "-L" + Value : "-R" + Value;
				AnimatorComponent.SetInteger ("Weapon", -1);
				AnimatorComponent.SetInteger ("LeftWeapon", (int)EWeapon.Unarmed);
				AnimatorComponent.SetInteger ("RightWeapon", (int)EWeapon.Unarmed);
				break;

			case EWeapon.LeftDagger: 
				Name = "Dagger";
				MaxValue = 6;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AttackValue = Value;
				AnimationClipName [1] = "-L" + Value;
				AttackValues ((int)EWeapon.Armed, 1, WeaponNumber, OldRight);
				break;
			case EWeapon.LeftItem:
				Name = "Item";
				MaxValue = 6;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AttackValue = Value;
				AnimationClipName [1] = "-L"+Value;
				AttackValues ((int)EWeapon.Armed, 1, WeaponNumber, OldRight);
				break;
			case EWeapon.LeftMace:
				Name = "Mace";
				MaxValue = 6;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AttackValue = Value;
				AnimationClipName [1] = "-L"+Value;
				AttackValues ((int)EWeapon.Armed, 1, WeaponNumber, OldRight);
				break;
			case EWeapon.LeftPistol:
				Name = "Pistol";
				MaxValue = 6;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AttackValue = Value;
				AnimationClipName [1] = "-L"+Value;
				AttackValues ((int)EWeapon.Armed, 1, WeaponNumber, OldRight);
				break;
			case EWeapon.LeftSword:
				Name = "Sword";
				MaxValue = 14;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AttackValue = Value;
				AnimationClipName [1] = "-L"+Value;
				AttackValues ((int)EWeapon.Armed, 1, WeaponNumber, OldRight);
				break;

			case EWeapon.RightDagger:
				Name = "Dagger";
				MaxValue = 6;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AttackValue = Value + HalfMaxValue;
				AnimationClipName [1] = "-R"+Value;
				AttackValues ((int)EWeapon.Armed, 2, OldLeft, WeaponNumber);
				break;
			case EWeapon.RightItem:
				Name = "Item";
				MaxValue = 6;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AttackValue = Value + HalfMaxValue;
				AnimationClipName [1] = "-R"+Value;
				AttackValues ((int)EWeapon.Armed, 2, OldLeft, WeaponNumber);
				break;
			case EWeapon.RightMace:
				Name = "Mace";
				MaxValue = 6;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AttackValue = Value + HalfMaxValue;
				AnimationClipName [1] = "-R"+Value;
				AttackValues ((int)EWeapon.Armed, 2, OldLeft, WeaponNumber);
				break;
			case EWeapon.RightPistol:
				Name = "Pistol";
				MaxValue = 6;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AttackValue = Value + HalfMaxValue;
				AnimationClipName [1] = "-R" + Value;
				AttackValues ((int)EWeapon.Armed, 2, OldLeft, WeaponNumber);
				break;
			case EWeapon.RightSpear:
				Name = "Spear";
				MaxValue = 14;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AttackValue = Value + HalfMaxValue;
				AnimationClipName [1] = "-R" + Value;
				AttackValues ((int)EWeapon.Armed, 2, OldLeft, WeaponNumber);
				break;
			case EWeapon.RightSword:
				Name = "Sword";
				MaxValue = 14;
				HalfMaxValue = Mathf.RoundToInt(MaxValue / 2);
				Value = (Value > HalfMaxValue) ? Value - HalfMaxValue : Value;
				AttackValue = Value + HalfMaxValue;
				AnimationClipName [1] = "-R"+Value;
				AttackValues ((int)EWeapon.Armed, 2, OldLeft, WeaponNumber);
				break;

			case EWeapon.Armed:
				//AnimatorComponent.SetBool ("Shield", Shield);
				break;

			default:
				AttackValue = Value;
				AnimationClipName [1] = System.Convert.ToString(Value);
				AnimatorComponent.SetInteger ("Weapon", WeaponNumber);
				break;
			}

			AnimationClipName [0] = Name + "-Attack";
			string AttackDual = "AttackDual" + AttackValue + "Trigger";
			string AttackSimple = "Attack" + AttackValue + "Trigger";
			string AttackName = (Dual) ? AttackDual : AttackSimple;
			AnimatorComponent.SetTrigger (AttackName);

			float AttackLength = 0.0f;
			AnimationClip Temp = GetAnimationClip (AnimationClipName[0] + AnimationClipName[1]);
			if (Temp != null) {
				AttackLength = Temp.length;
			}
			Invoke ("OnAttack", AttackLength/SpeedMultiplier);
		}
	}

	void OnAttack()
	{
		Debug.Log (AttackTime);
		AttackState = 1;
		AttackTime = 0.0f;
	}

	public IEnumerator Roll(int Mode,float Time)
	{
		IsRoll = true;
		//Vector3 OldRelativePosition = PlayerObject.transform.localPosition;
		//float OldCameraSmooth = ControllerComponent.CameraSmooth;
		//ControllerComponent.CameraSmooth = 0.075f;
		//ControllerComponent.CameraTarget = PlayerObject;
		AnimatorComponent.applyRootMotion = true;
		Mode = Mathf.Clamp (Mode, 1, 4);
		switch (Mode)
		{
			case 1: AnimatorComponent.SetTrigger ("RollForwardTrigger");	break;
			case 2: AnimatorComponent.SetTrigger("RollRightTrigger");		break;
			case 3: AnimatorComponent.SetTrigger ("RollBackwardTrigger");	break;
			case 4: AnimatorComponent.SetTrigger ("RollLeftTrigger");		break;
			default: break;
		}
		yield return new WaitForSeconds (Time);
		//PlayerObject.transform.parent = null;
		//transform.position = PlayerObject.transform.position;
		//ControllerComponent.transform.position = PlayerObject.transform.position;
		//ControllerComponent.CameraTarget = ControllerComponent.gameObject;
		//PlayerObject.transform.SetParent (transform);
		//StartCoroutine (CorutineRoll (OldRelativePosition, 0.0f/*OldCameraSmooth*/));
		AnimatorComponent.applyRootMotion = false;
		IsRoll = false;
	}

	IEnumerator CorutineRoll(Vector3 Vec,float Val)
	{
		//PlayerObject.transform.localPosition = Vec;
		yield return new WaitForSeconds (0.15f);
		//ControllerComponent.CameraSmooth = Val;
	}

	public void Death(bool Physics = false)
	{
		AnimatorComponent.SetTrigger ("Death1Trigger");
		IsDead = true;
		State = -1;
	}

	public void Revive()
	{
		AnimatorComponent.SetTrigger("Revive1Trigger");
		IsDead = false;
		State = 0;
	}

	public void Interact(EInteract Type,int Value)
	{
		if (AnimatorComponent)
		{	
			switch (Type) 
			{
			case EInteract.Activation:
				AnimatorComponent.SetTrigger ("ActivateTrigger");
				break;

			case EInteract.Idle:
				AnimatorComponent.SetTrigger ("IdleTrigger");
				AnimatorComponent.SetInteger ("Idle", Value);
				break;

			case EInteract.PickUp:
				AnimatorComponent.SetTrigger ("PickupTrigger");
				break;
			}
		}
	}

	public void Lock(bool Value)
	{
		Moving = !Value;
		VelocityNormalized = (Value) ? Vector3.zero : VelocityNormalized;
		IsLock = Value;

		AnimatorComponent.SetBool ("Moving", Moving);
	}

	public void Block(bool NewBlock,EWeapon Weapon,int LR)
	{
		Blocking = NewBlock;
		if (Weapon == EWeapon.Relax) {
			State = (State == 2) ? 0 : State;
			return;
		}

		if (AnimatorComponent) 
		{
			bool ShieldState = AnimatorComponent.GetBool("Shield");
			int LeftRight = AnimatorComponent.GetInteger ("LeftRight");
			int WeaponNumber = AnimatorComponent.GetInteger ("Weapon");

			AnimatorComponent.SetTrigger ("BlockTrigger");
			State = Mathf.Clamp (State, -1, int.MaxValue);
			LeftRight = LR;
			ShieldState = (LR <= 0) ? true : ShieldState;

			switch (Weapon) 
			{
			case EWeapon.Relax:
				LeftRight = 0;
				ShieldState = false;
				WeaponNumber = (int)EWeapon.Relax;
				Blocking = false;
				break;
			case EWeapon.Unarmed:
				break;
			case EWeapon.LeftDagger:
			case EWeapon.LeftItem:
			case EWeapon.LeftMace:
			case EWeapon.LeftSword:
				WeaponNumber = (int)EWeapon.Armed;
				LeftRight = 1;
				ShieldState = false;
				break;

			case EWeapon.RightDagger:
			case EWeapon.RightItem:
			case EWeapon.RightMace:
			case EWeapon.RightSword:
			case EWeapon.RightSpear:
				WeaponNumber = (int)EWeapon.Armed;
				LeftRight = 2;
				ShieldState = false;
				break;

			default:
				WeaponNumber = (int)Weapon;
				break;
			}
				
			AnimatorComponent.SetBool ("Shield", ShieldState);
			AnimatorComponent.SetInteger ("Weapon", WeaponNumber);
			AnimatorComponent.SetInteger ("LeftRight", LeftRight);
			AnimatorComponent.SetBool ("Blocking", Blocking);
			State = (Blocking) ? 2 : State;
		}
	}

	bool CastValues(ECast Type,ECastMode Mode,int Value,float CastTime)
	{
		if (AnimatorComponent)
		{
			this.CastTime = CastTime;
			CastMode = Mode;
			string Trigger = "Cast";
			switch (Type) 
			{
			case ECast.Aoe: 	Trigger += ("AOE" + Value); break;
			case ECast.Buff:	Trigger += ("Buff" + Value); break;
			case ECast.Summon:	Trigger += ("Summon" + Value); break;
			default: break;
			}
			Trigger += "Trigger";
			AnimatorComponent.SetTrigger (Trigger);
			AnimatorComponent.SetInteger ("Weapon", (int)EWeapon.Armed);
			AnimatorComponent.SetInteger ("LeftWeapon", 0);
			AnimatorComponent.SetInteger ("RightWeapon", 0);
			return true;
		}
		return false;
	}

	public void Cast (string InputName, ECast Type, ECastMode Mode, int Value, float CastTime)
	{
		switch (CastMode) 
		{
		case  ECastMode.Default:
			if (Input.GetButtonDown (InputName)) {
				IsCast = true;
				CastValues (Type, Mode, Value, CastTime);
			}
			break;
		case ECastMode.Channeling:
			if (Input.GetButtonDown (InputName))
				IsCast = CastValues (Type, Mode, Value, CastTime);
			
			if (Input.GetButtonUp (InputName))
			{
				AnimatorComponent.SetTrigger ("CastEndTrigger");
				IsCast = false;
			}
			break;
		case ECastMode.Toggle:
			this.CastTime = -1;
			if (IsCast) 
			{
				if (Input.GetButtonDown (InputName)) 
				{
					IsCast = false;
					AnimatorComponent.SetTrigger ("CastEndTrigger");
				}

			} 
			else 
			{
				if (Input.GetButtonDown (InputName))
				{
					IsCast = true;
					CastValues (Type, Mode, Value, CastTime);
				}
			}
			break;
		case ECastMode.Instant:
			if (Input.GetButtonDown (InputName)){
				IsCast = true;
				CastValues (Type, Mode, Value, CastTime);
			}
			break;
		case ECastMode.Strike:
			if (Input.GetButtonDown (InputName))
				CastMeter = Mathf.Clamp (CastMeter + CastAdd, 0, CastCap);

			if (CastMeter >= CastCap) {
				IsCast = true;
				CastMeter = 0;
				CastValues (Type, Mode, Value, CastTime);
				Invoke ("OnCastInvoke", CastCapTime);
			}
			break;
		}
	}

	void CastUpdate()
	{
		if (AnimatorComponent) 
		{
			switch (CastMode)
			{
			case ECastMode.Default:
				if (IsCast)
				{
					CastDeltaTime += Time.deltaTime;
					IsCast = (CastDeltaTime <= CastTime);
					if (!IsCast) {
						CastDeltaTime = 0.0f;
						AnimatorComponent.SetTrigger ("CastEndTrigger");
					}
				}
				break;

			case ECastMode.Channeling:
				CastDeltaTime += Time.deltaTime;
				CastDeltaTime = (!IsCast) ? 0.0f : CastDeltaTime;	
				break;

			case ECastMode.Toggle:
				CastDeltaTime += Time.deltaTime;
				CastDeltaTime = (!IsCast) ? 0.0f : CastDeltaTime;	
				break;

			case ECastMode.Instant:
				AnimatorComponent.SetTrigger ("CastEndTrigger");
				IsCast = false;
				break;

			}
		}
	}

	void OnCastInvoke()
	{
		IsCast = false;
	}

	//Animation State Helpers:
	AnimationClip GetAnimationClip(string name)
	{
		for (int i = 0; i < AnimatorControllerComponent.animationClips.Length; i++) 
		{
			if (AnimatorControllerComponent.animationClips [i].name == name)
				return AnimatorControllerComponent.animationClips [i];
		}
		return null;
	}

	AnimationClip GetCurrentAnimationClip(int Layer,int Index)
	{
		AnimatorClipInfo[] AnimationList;
		AnimationList = AnimatorComponent.GetCurrentAnimatorClipInfo (Layer);
		return AnimationList [Index].clip;
	}
}
