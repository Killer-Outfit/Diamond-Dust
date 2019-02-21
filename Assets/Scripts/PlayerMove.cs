using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    public float movementSpeed;
    public float turningSpeed;
    public float dashSpeed;
    float lockSpeed;

    private float horizontalDash;
    Quaternion targetRotation;

    // Use with cam relative motion 2
    public Transform camPivot;
    public Transform cam;
    float heading;
    // Set up character controller for motion and vector 
    CharacterController controller;
    Vector3 movementVector;
    float vVelocity;
    // Main camera and its script
    GameObject mainCamera;
    FollowCamera mainCameraScript;
    // States for blocking, attacking, dashing and locking
    private bool isBlocking;
    private bool isAttacking;
    private bool dashed;
    private bool isLock;
    // Animator
    Animator anim;

    // Use this for initialization
    void Start () {
        horizontalDash = 0f;
        dashed = false;
        isBlocking = false;
        isAttacking = false;
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
        if (!isBlocking && !isAttacking)
        {
            normalMovement();
        }else if(isBlocking)
        {
            if(Input.GetAxis("LStick X") != 0 && !dashed)
            {
                horizontalDash = Input.GetAxis("LStick X") * dashSpeed * Time.deltaTime;
                dashed = true;
                dash(horizontalDash);
            }else if(Input.GetAxis("LStick X") == 0 && dashed)
            {
                dash(horizontalDash);
                dashed = false;
            }
        }
       
    }

    private void dash(float horizontal)
    {
        heading += horizontal;
        camPivot.rotation = Quaternion.Euler(0, heading, 0);
        Vector2 inputs = new Vector2(horizontal, 0);
        inputs = Vector2.ClampMagnitude(inputs, 1); // CLAMP?
        Vector3 camF = cam.forward;
        Vector3 camR = cam.right;
        camF.y = 0f;
        camR.y = 0f;
        camF = camF.normalized;
        camR = camR.normalized;

        movementVector = (camF * inputs.y + camR * inputs.x);
        vVelocity += Physics.gravity.y * Time.deltaTime;
        movementVector.y = vVelocity;
        controller.Move(movementVector * Time.deltaTime * dashSpeed);
        if (horizontal > 0)
        {
            controller.Move(Vector3.left * Time.deltaTime * dashSpeed);
        }else
        {
            controller.Move(Vector3.right * Time.deltaTime * dashSpeed);
        }
    }
    private void stationaryRotate()
    {
        
    }

    private void normalMovement()
    {
        //get stick inputs
        float horizontal = Input.GetAxis("LStick X") * movementSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("LStick Y") * movementSpeed * Time.deltaTime;

        if (Input.GetAxis("Horizontal") != 0)
        {
            horizontal = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        }

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
                //remove "-1 *" change -  to plus to invert rotation
                var rotation = Quaternion.LookRotation(((-1 * camF * inputs.y - camR * inputs.x) * Time.deltaTime * movementSpeed));
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turningSpeed);
            }
        }
        else
        {
            //
            if (System.Math.Abs(horizontal) < vertical)
            {
                lockSpeed = movementSpeed;
            }
            else
            {
                lockSpeed = 10;
            }
            //Debug.Log("lockspeed = " + lockSpeed);
            transform.rotation = Quaternion.LookRotation(camF);
            transform.position += (camF * inputs.y + camR * inputs.x) * Time.deltaTime * lockSpeed;
        }
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
    }
    // Block to change state
    public void changeBlock()
    {
        if (isBlocking)
        {
            isBlocking = false;
        }else
        {
            isBlocking = true;
        }
    }
    // Set attacking state
    public void changeAttacking(bool action)
    {
        isAttacking = action;
    }
}
