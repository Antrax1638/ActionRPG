using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Transition
{
	public State State;
	public bool Enter;
}

//[CreateAssetMenu(fileName = "State",menuName = "IA/Action", order = 1)]
public class State : MonoBehaviour
{
	[HideInInspector()]
	public Transition[] Transitions;

	[Header("State Properties:")]
	public string Name;
	public bool DebugMode;

	[HideInInspector]
	public bool TransitionPass = false;
	[HideInInspector]
	public State TransitionState;

	void Update()
	{
		TransitionPass = false;
		for (int i = 0; i < Transitions.Length; i++) {
			if (Transitions [i].Enter)
			{
				TransitionPass = Transitions [i].Enter;
				TransitionState = Transitions [i].State;
			}
		}
			
		if (!TransitionPass) 
		{
			UpdateState ();
		}
	}

	protected virtual void UpdateState()
	{
		if (DebugMode)
			Debug.Log (Name);
	}

	public int AddTransition(Transition NewTransition)
	{
		if (Transitions == null)
		{
			Transitions = new Transition[1];
			Transitions [0] = NewTransition;
			return 0;
		}

		for(int i = 0; i < Transitions.Length; i++)
		{
			if (Transitions [i] == null)
				Transitions [i] = NewTransition;
		}
		int OldLength = Transitions.Length;
		Resize (Transitions.Length + 1);
		Transitions[OldLength] = NewTransition;
		return OldLength;
	}

	public Transition AddTransitionAt(Transition NewTransition,int Index)
	{
		if(NewTransition != null && Index >= 0 && Index < Transitions.Length)
		{
			Transition Temp = (Transitions [Index] != null) ? Transitions [Index] : null;
			Transitions [Index] = NewTransition;
			return Temp;
		}
		return null;
	}
		
	public void Remove(Transition Transition)
	{
		List<Transition> Temp = new List<Transition> (Transitions.Length);
		for(int i = 0; i < Transitions.Length; i++)
			Temp [i] = Transitions [i];
		Temp.Remove (Transition);
		Transitions = Temp.ToArray ();
	}

	public void RemoveAt(int Index)
	{
		List<Transition> Temp = new List<Transition> ();
		for(int i = 0; i < Transitions.Length; i++)
			Temp.Add(Transitions [i]);
		Temp.RemoveAt (Index);
		Transitions = Temp.ToArray ();
		Temp.Clear ();
	}

	public Transition GetTransitionAt(int Index)
	{
		if (Index >= 0 && Index < Transitions.Length)
			return Transitions [Index];
		else
			return null;
	}

	public Transition GetTransitionByName(string Name)
	{
		for (int i = 0; i < Transitions.Length; i++) {
			if (Transitions [i].State.Name == Name) {
				return Transitions [i];
			}
		}
		return null;
	}
		
	public void ClearTransitions()
	{
		Transitions = new Transition[Transitions.Length];
	}

	public void Empty()
	{
		Transitions = new Transition[0];
	}

	public void Resize(int newSize)
	{
		Transition[] Temp = Transitions;
		for (int i = 0; i < Transitions.Length; i++) 
			Temp [i] = Transitions [i];
		Transitions = new Transition[newSize];
		for (int i = 0; i < Transitions.Length; i++) 
		{
			if(i >= 0 && i < Transitions.Length && i < Temp.Length)
				Transitions [i] = Temp [i];
		}
		Temp = new Transition[0];
	}

    public bool Contains(string Name)
    {
        for (int i = 0; i < Transitions.Length; i++)
        {
            if (Transitions[i].State.Name == Name)
                return true;
        }
        return false;
    }

    public bool Contains(Transition Transition)
	{
		for (int i = 0; i < Transitions.Length; i++) {
			if (Transitions [i].State == Transition.State)
				return true;
		}
		return false;
	}

}
