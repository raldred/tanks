using UnityEngine;
using System.Collections;

public class UITest : MonoBehaviour
{
    public Tank movingTank;
    
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 100), "Move"))
        {
            movingTank.MoveToWorldPos(new Vector3(35f, 0f, 35f));
        }
        
    }
}
