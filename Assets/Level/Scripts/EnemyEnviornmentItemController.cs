using UnityEngine;
using System.Collections;
using System;

//Handle enviorment enemies
public class EnemyEnviornmentItemController : MonoBehaviour
{
   public static event Action PlayerSpotted; 

   private void OnTriggerEnter(Collider other)
   {
        if (other.tag != "Player" || PlayerSpotted==null) return;

        PlayerSpotted();
   } 
}
 