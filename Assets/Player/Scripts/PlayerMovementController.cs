using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    public static event Action DistractionStarted;
    public float MovementSpeed = 1;
    public float RotationSpeed = 1;
    private CharacterController mController;

    public KeyCode DistractionKey = KeyCode.Space; //key to distract enemy


    public bool isMoving
    {
        get
        {
            return Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        }
    }

    void Awake ()
    {
        mController = GetComponent<CharacterController>();
	}
	
	void Update ()
    {
        if(Input.GetKeyUp(DistractionKey) && DistractionStarted!=null)
        {
            DistractionStarted();
        }

        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical"));
        mController.SimpleMove(direction * MovementSpeed);

        if (direction == Vector3.zero)
            return;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * RotationSpeed);
    }

}
