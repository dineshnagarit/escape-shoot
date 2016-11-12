using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public  PlayerMovementController Player;
    private Vector3 mPreviousPosition;

	void Awake ()
    {
        mPreviousPosition = Player.transform.position;
    }

    private void LateUpdate()
    {
        if (!Player.isMoving)
            return;

        Vector3 difference = Player.transform.position - mPreviousPosition;

        transform.position += difference;
        mPreviousPosition = Player.transform.position; 

    }

}
