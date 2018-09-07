using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyModifier
{
    Crtl,
    Shift,
    Alt,
    Cmd
}

public class Utils
{
	public static bool GetButtonDown(string name,KeyModifier key)
    {
        switch (key)
        {
            case KeyModifier.Crtl: 
                if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    return Input.GetButtonDown(name);
                break;
            case KeyModifier.Shift:
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    return Input.GetButtonDown(name);
                break;
            case KeyModifier.Alt:
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                    return Input.GetButtonDown(name);
                break;
            case KeyModifier.Cmd:
                if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
                    return Input.GetButtonDown(name);
                break;
        }
        return false;
    }

    public static bool GetButtonUp(string name, KeyModifier key)
    {
        switch (key)
        {
            case KeyModifier.Crtl:
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    return Input.GetButtonUp(name);
                break;
            case KeyModifier.Shift:
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    return Input.GetButtonUp(name);
                break;
            case KeyModifier.Alt:
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                    return Input.GetButtonUp(name);
                break;
            case KeyModifier.Cmd:
                if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
                    return Input.GetButtonUp(name);
                break;
        }
        return false;
    }

    public static Vector3 ClampMagnitude(Vector3 target, float min, float max)
    {
        double sm = target.sqrMagnitude;
        if (sm > (double)max * (double)max) return target.normalized * max;
        else if (sm < (double)min * (double)min) return target.normalized * min;
        return target;
    }

    public static Quaternion Add(Quaternion a, Quaternion b)
    {
        return a * b;
    }

    public static Quaternion Substract(Quaternion a, Quaternion b)
    {
        return a * (b * Quaternion.Inverse(b));
    }
}
