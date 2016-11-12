using UnityEngine;
using System.Collections;
using System;

public class EnemySightController : MonoBehaviour
{
    public GameObject Eyes; 

    private GameObject mPlayer;
    private bool mIsPlayerSeen;

    public Action<Vector3> PlayerSightLost;
    public Action playerSigntGained;
    
	private void Awake()
    {
        mPlayer = FindObjectOfType<CharacterController>().gameObject;
	}
    
    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 playerRayDirection = (mPlayer.transform.position - Eyes.transform.position).normalized * 10;
        playerRayDirection.y = Eyes.transform.forward.y;

        Debug.DrawRay(Eyes.transform.position, playerRayDirection,Color.red);
        Debug.DrawRay(Eyes.transform.position, Eyes.transform.forward, Color.red);

        if (Vector3.Angle(playerRayDirection, Eyes.transform.forward)<45  &&  Physics.Raycast(Eyes.transform.position, playerRayDirection ,out hit))
        {
            return hit.collider.gameObject == mPlayer;
        }

        return false;
	}
    
    private void Update()
    {
        bool isPlayerSeen = CanSeePlayer();
        
        /*If the player was just seen by the enemy.*/
        if(isPlayerSeen && !mIsPlayerSeen)
        {
            if (playerSigntGained != null) playerSigntGained();

        }
        else if(!isPlayerSeen && mIsPlayerSeen)
        {
            StartCoroutine(PlayerLostCoroutine());
             
        }

        mIsPlayerSeen = isPlayerSeen;
    }
    
    private IEnumerator PlayerLostCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (PlayerSightLost != null) PlayerSightLost(mPlayer.transform.position);
          
    }
}
