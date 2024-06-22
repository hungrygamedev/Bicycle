using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersPositionUI : MonoBehaviour
{
    public Transform startPoint, endPoint;
    public List<Transform> uiPoints = new List<Transform>();
    public List<Transform> worldPoints = new List<Transform>();

    public Vector3 startPointW, endPointW;

    public bool canMove;
    
    public void SetVectors(Vector3 v1, Vector3 v2)
    {
        startPointW = v1;
        endPointW = v2;
        canMove = true;
    }
    void LateUpdate()
    {
        if (canMove)
        {
            for (int i = 0; i < uiPoints.Count; i++)
            {
                float percent = Remap(worldPoints[i].position.z, startPointW.z, endPointW.z,0f,100f);
                Vector3 pos = uiPoints[i].transform.position;
                uiPoints[i].transform.position =
                    new Vector3(Remap(percent, 0f, 100f, startPoint.position.x, endPoint.position.x), pos.y, pos.z);
            }
            
        }
    }
    
    public  float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
