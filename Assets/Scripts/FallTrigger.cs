using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTrigger : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      if (other.tag == "Enemy")
      {
         other.GetComponent<Enemy>().ResetPositionOnCheckPoint();
      }

      if (other.tag == "Player")
      {
         if (other.GetComponent<Player>() != null)
         {
            other.GetComponent<Player>().ResetPositionOnCheckPoint();
         }
         else
         {
            other.GetComponent<GravityTest>().ResetPositionOnCheckPoint();
         }
      }
   }
}
