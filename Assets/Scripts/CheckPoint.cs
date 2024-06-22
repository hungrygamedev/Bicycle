using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GroundPlatform myPlatform;
    public Transform checkPoint;
    private bool isMoneyAdded;
    public bool isStartCheckPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Enemy>().SetCheckPoint(checkPoint,myPlatform);
        }
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().checkPoint = checkPoint;
            if (!isStartCheckPoint)
            {
                if (!isMoneyAdded)
                {
                    GameManager.instance.AddPlayerMoney();
                    isMoneyAdded = true;
                }
            }
        }
    }
}
