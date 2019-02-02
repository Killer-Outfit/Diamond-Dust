using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    public float movementSpeed;
    public float turningSpeed;
    float lockSpeed;
    Quaternion targetRotation;
    //public GameObject child; //used with child relative motion

    //use with cam relative motion 2
    public Transform camPivot;
    public Transform cam;
    float heading;
    //

    bool isLock;

    Animator anim;

    // Use this for initialization
    void Start () {
        isLock = false;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        //get stick inputs
        float horizontal = Input.GetAxis("LStick X") * movementSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("LStick Y") * movementSpeed * Time.deltaTime;

        Debug.Log("horiz = " + horizontal + " vert = " + vertical);

        //cam2 movement
        heading += horizontal;
        camPivot.rotation = Quaternion.Euler(0, heading, 0);
        Vector2 inputs = new Vector2(horizontal, vertical);
        inputs = Vector2.ClampMagnitude(inputs, 1);
        Vector3 camF = cam.forward;
        Vector3 camR = cam.right;
        camF.y = 0f;
        camR.y = 0f;
        camF = camF.normalized;
        camR = camR.normalized;


        if (Input.GetAxis("rightTrigger") > 0 && !isLock)
        {
            //isLock = true;
        }
        else if (Input.GetAxis("rightTrigger") == 0 && isLock)
        {
            isLock = false;
        }

        if (!isLock)
        {
            transform.position += (camF * inputs.y + camR * inputs.x) * Time.deltaTime * movementSpeed;

            //setting character rotation
            if (inputs.x != 0 || inputs.y != 0)
            {
                var rotation = Quaternion.LookRotation(((camF * inputs.y + camR * inputs.x) * Time.deltaTime * movementSpeed));
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turningSpeed);
            }
            
        }
        else
        {
            //
            if (System.Math.Abs(horizontal) < vertical)
            {
                lockSpeed = movementSpeed;
            }else
            {
                lockSpeed = 10;
            }
            //Debug.Log("lockspeed = " + lockSpeed);
            transform.rotation = Quaternion.LookRotation(camF);
            transform.position += (camF * inputs.y + camR * inputs.x) * Time.deltaTime * lockSpeed;
            
        }
            //
        //

        //play run animation when the player is moving
        if (vertical != 0 || horizontal != 0)
        {
            anim.Play("HumanoidRun");
        }
        else
        {
            anim.SetTrigger("stopRun");
        }
        //


        /*
        //Code to establish camera relative movement
        var camera = Camera.main;
        var camFor = camera.transform.forward;
        var camRight = camera.transform.right;

        camFor.y = 0f;
        camRight.y = 0f;

        camFor.Normalize();
        camRight.Normalize();

        var relativeDirection = camFor * vertical + camRight * horizontal;
        */
        /*
        //check to see if user in pushing a stick
        var input = new Vector3(horizontal, 0, vertical);
        if (input != Vector3.zero)
        {
            //set rotation vectors
            transform.forward = input;
            targetRotation = Quaternion.LookRotation(input);

            //trigger the run animation
            anim.SetTrigger("run");
            
        }
        else {
            //end the run animation
            anim.SetTrigger("stopRun");
        }

        //set rotation
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, movementSpeed * Time.deltaTime);

        //child relative motion
        //transform.position = child.transform.position;

        //world relative motion
        //transform.Translate(horizontal, 0, vertical, Space.World);

        //camera relative motion
        //transform.Translate(relativeDirection * movementSpeed * Time.deltaTime);

        horizontal = 0;
        vertical = 0;
        */
    }
}
