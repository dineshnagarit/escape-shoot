using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Animator))]
[RequireComponent(typeof(PlayerMovementController))]
public class PlayerAnimator : MonoBehaviour
{

    private Animator mAnimator;

    private PlayerAnimationType mCurrentAnimation;
    private PlayerMovementController mMovementController;

    void Awake ()
    {
        mAnimator = GetComponent<Animator>();
        mMovementController = GetComponent<PlayerMovementController>();
    }
	
	  
	void LateUpdate ()
    {
        if(mMovementController.isMoving)
            ChageState(PlayerAnimationType.Run);
        else
            ChageState(PlayerAnimationType.Idle);

    }

    private void ChageState(PlayerAnimationType newState)
    {
        if (mCurrentAnimation == newState)
            return;

        mCurrentAnimation = newState;
        mAnimator.SetInteger("State", (int) mCurrentAnimation);
    
    }
}
