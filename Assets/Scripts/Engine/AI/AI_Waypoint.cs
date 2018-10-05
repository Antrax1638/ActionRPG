using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AI_Waypoint : MonoBehaviour {

    [Header("Waypoint")]
    [SerializeField] float Radius = 0.25f;
    [SerializeField] Color NodeColor = Color.blue;
    [SerializeField] Color LineColor = Color.white;

    private void OnDrawGizmos()
    { 
        for(int i = 0; i < transform.childCount; i++)
        {
            int ChildPlus = Mathf.Clamp(i + 1, 0, transform.childCount-1);

            Gizmos.color = LineColor;
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(ChildPlus).position);
            Gizmos.color = NodeColor;
            Gizmos.DrawWireSphere(transform.GetChild(i).position, Radius);
            
        }
    }
}
