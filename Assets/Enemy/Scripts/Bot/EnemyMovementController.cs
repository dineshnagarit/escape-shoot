using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemySightController))]
[RequireComponent(typeof(EnemyEnviornmentItemController))]
[RequireComponent(typeof(PlayerMovementController))]
public class EnemyMovementController : MonoBehaviour
{
    public GameObject NavPath;

    public float NavPathDelay = 1;

    public float FollowSpeed = 2;

    private NavMeshAgent mAgent;

    private GameObject mPlayer;

    private EnemyNavPoint[] mNavPoints;

    private int mCurrentNavPointindex = 0;

    private bool mIsWaitingForNextPatrolPoint = false;

    private Vector3 mPrevAgentPosition;

    private Animator mAnimator;

    private EnemySightController mSightController;

    private float mPatrolSpeed;

    private bool mHasPlayerBeenLost = false;

    private Vector3 mLastKnownPosition = Vector3.zero;

    private EnemyAIState mCurrentState = EnemyAIState.Patrol;

    private EnemyNavPoint mCurrentNavPoint
    {
        get
        {
            return mNavPoints[mCurrentNavPointindex];
        }
    }
    
    private void Awake()
    {
        mAgent = GetComponent<NavMeshAgent>();
        mNavPoints = NavPath.GetComponentsInChildren<EnemyNavPoint>();

        if (mNavPoints.Length >= 2)
        {
            transform.LookAt(mNavPoints[1].transform.position);
        }

        mAnimator = GetComponent<Animator>();
        mSightController = GetComponent < EnemySightController>();

        mPatrolSpeed = mAgent.speed;

        mSightController.playerSigntGained = OnPlayerSpotted;
        mSightController.PlayerSightLost =  OnPlayerLost;

        EnemyEnviornmentItemController.PlayerSpotted += OnEnviornmentSpottedPlayer;
        PlayerMovementController.DistractionStarted += OnDistractionStarted;
        mPlayer = FindObjectOfType<CharacterController>().gameObject;

    }

    /// <summary>
    /// Called when the object has been destroyed.
    /// </summary>
    private void OnDestroy()
    {
        EnemyEnviornmentItemController.PlayerSpotted -= OnEnviornmentSpottedPlayer;
        PlayerMovementController.DistractionStarted += OnDistractionStarted;
    }

    private void Update()
    {
        switch (mCurrentState)
        {
            case EnemyAIState.Patrol:
                UpdatePatrol();
                break;

            case EnemyAIState.Follow:
                UpdateFollow();
                break;
            case EnemyAIState.Distracted:
                UpdateDistracted();
                break;
        }

        mPrevAgentPosition = transform.position;
    }

    private void UpdateAnimationSpeed()
    {
        Vector3 difference = transform.position - mPrevAgentPosition;
        float currentSpeed = difference.magnitude / Time.deltaTime;
        mAnimator.SetFloat("Speed", currentSpeed);
    }


#region Patrol
    private void UpdatePatrol()
    {
        if (!HasAgentReachedDestination() || mIsWaitingForNextPatrolPoint) return;

        StartCoroutine("NextPatrolPoint");
        mIsWaitingForNextPatrolPoint = true;
    } 

    private IEnumerator NextPatrolPoint()
    {
        yield return new WaitForSeconds (NavPathDelay);
        mCurrentNavPointindex++;

        if (mCurrentNavPointindex >= mNavPoints.Length)
        {
            mCurrentNavPointindex = 0;
        }

        mIsWaitingForNextPatrolPoint = false;
        mAgent.SetDestination(mCurrentNavPoint.transform.position);
    }
#endregion

    private void UpdateFollow()
    {
        if (mHasPlayerBeenLost)
        {
            mAgent.SetDestination(mLastKnownPosition);

            if (HasAgentReachedDestination())
                ReturnToPatrol();

            return;
        }

        mAgent.SetDestination(mPlayer.transform.position);
    }

    private void OnPlayerSpotted()
    {
        mCurrentState = EnemyAIState.Follow;
        StopCoroutine("NextPatrolPoint");
        mIsWaitingForNextPatrolPoint = false;
        mAgent.speed = FollowSpeed;
        mHasPlayerBeenLost = false;
    }   

    private void OnPlayerLost(Vector3 lastKnownPostition)
    {
        if (mCurrentState != EnemyAIState.Follow)
            return;

        mHasPlayerBeenLost = true;
        mLastKnownPosition = lastKnownPostition; 
        //mCurrentState = EnemyAIState.Follow;
    }

    private void ReturnToPatrol()
    {
        mHasPlayerBeenLost = false;
        mCurrentState = EnemyAIState.Patrol;
        mAgent.speed = mPatrolSpeed;
    }

    private void OnEnviornmentSpottedPlayer()
    {
        OnPlayerSpotted();
        mAgent.SetDestination(mPlayer.transform.position);
        mCurrentState = EnemyAIState.Distracted;
    }

    private void OnDistractionStarted()
    {
        if(Vector3.Distance(transform.position,mPlayer.transform.position)<10)
        {
            OnEnviornmentSpottedPlayer();
        }
        
    }


    private void UpdateDistracted()
    {

        if (!HasAgentReachedDestination())
            return;

         ReturnToPatrol();

    }


    private bool HasAgentReachedDestination()
    {
        return (mAgent.pathStatus == NavMeshPathStatus.PathComplete && mAgent.remainingDistance == 0) || mAgent.pathStatus == NavMeshPathStatus.PathPartial;
    }

}
