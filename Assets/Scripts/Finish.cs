using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public GroundPlatform myPlatform;
    private void OnTriggerEnter(Collider other)
    {
        if (myPlatform.isFinish)
        {
            if (other.tag == "Enemy")
            {
                other.GetComponent<Enemy>().Finish();
            }

            if (other.tag == "Player")
            {
                other.GetComponent<Player>().Finish();
            }
        }
    }
}
