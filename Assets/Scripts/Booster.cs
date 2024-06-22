using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    public float boostSpeed, boostJump;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (boostSpeed > 0)
            {
                other.GetComponent<Enemy>().Boost(boostSpeed);
            }
        }

        if (other.tag == "Player")
        {
            if (boostSpeed > 0)
            {
                other.GetComponent<Player>().Boost(boostSpeed);
            }
            if (boostJump > 0)
            {
                other.GetComponent<Player>().Jump(boostJump);
            }
        }
    }
}
