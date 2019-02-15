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
    CharacterController controller;
    Vector3 movementVector;
    float vVelocity;

    GameObject mainCamera;
    FollowCamera mainCameraScript;

    bool isLock;

    Animator anim;

    // Use this for initialization
    void Start () {
        vVelocity = 0;
        isLock = false;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        mainCamera = GameObject.Find("Main Camera");
        mainCameraScript = mainCamera.GetComponent<FollowCamera>();

        //velocity = new Vector3(0, Physics.gravity.y, 0);
    }

    // Update is called once per frame
    void Update () {

        //get stick inputs
        float horizontal = Input.GetAxis("LStick X") * movementSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("LStick Y") * movementSpeed * Time.deltaTime;

        if(Input.GetAxis("Horizontal") != 0)
        {
            horizontal = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        }

        //Debug.Log("horiz = " + horizontal + " vert = " + vertical);

        //cam2 movement
        heading += horizontal;
        camPivot.rotation = Quaternion.Euler(0, heading, 0);
        Vector2 inputs = new Vector2(horizontal, vertical);
        inputs = Vector2.ClampMagnitude(inputs, 1); // CLAMP?
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
            //transform.position += (camF * inputs.y + camR * inputs.x) * Time.deltaTime * movementSpeed;
            movementVector = (camF * inputs.y + camR * inputs.x);
            vVelocity += Physics.gravity.y * Time.deltaTime;
            movementVector.y = vVelocity;
            controller.Move(movementVector * Time.deltaTime * movementSpeed);
            if (controller.isGrounded)
            {
                //Debug.Log("I am on the ground");
                vVelocity = 0;
            }
            //velocity.y += Physics.gravity.y * Time.deltaTime;
            //controller.Move(velocity * Time.deltaTime);

            //setting character rotation
            if (inputs.x != 0 || inputs.y != 0)
            {
                if (!isLock)
                {
                    //remove "-1 *" change -  to plus to invert rotation
                    var rotation = Quaternion.LookRotation(((-1 * camF * inputs.y - camR * inputs.x) * Time.deltaTime * movementSpeed));
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turningSpeed);
                }else
                {
                    //transform.LookAt(mainCameraScript.lockTarget.transform);
                }

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
            //anim.Play("HumanoidRun");
            anim.SetBool("isIdle", false);
        }
        else
        {
            //anim.SetTrigger("stopRun");
            anim.SetBool("isIdle", true);
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
