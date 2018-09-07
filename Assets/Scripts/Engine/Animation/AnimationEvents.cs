using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour 
{
	public bool IkEnabled;

	private Animator AnimatorComponent;
	Vector3 LeftFootPosition,RightFootPosition;
	float LeftFootBlend,RightFootBlend;
	AnimationState CurrentAnimation;

	void Awake()
	{
		AnimatorComponent = GetComponent<Animator> ();
		if (!AnimatorComponent)
			Debug.LogError ("Animation Events: Animator is null.");
	}
		
	void Update()
	{
		
	}

	//Animations Events:
	void Land()
	{
		
	}

	void WeaponSwitch()
	{
	
	}

	void FootR()
	{
		Transform RightFoot = AnimatorComponent.GetBoneTransform (HumanBodyBones.RightFoot);
		RaycastHit Hit;
		Physics.Raycast (RightFoot.position + (Vector3.up * 0.3f), Vector3.down,out Hit, 0.5f);
		RightFootPosition = Hit.point;

	}

	void FootL()
	{
		Transform LeftFoot = AnimatorComponent.GetBoneTransform (HumanBodyBones.LeftFoot);
		RaycastHit Hit;
		Physics.Raycast (LeftFoot.position + (Vector3.up * 0.3f), Vector3.down,out Hit, 0.5f);
		LeftFootPosition = Hit.point;

	}

	void Hit()
	{
		
	}

	void Shoot()
	{
		
	}
}
