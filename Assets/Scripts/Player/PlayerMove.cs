using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    public float movementSpeed;
    public float turningSpeed;
    public float dashSpeed;
    public float maxDashTime;
    // Use with cam relative motion 2
    public Transform camPivot;
    public Transform cam;

    private float currenDashTime;
    private float dashTimeIncriment;
    private float inputIntervalMax = 10f;
    private float inputTime;
    private float inputTimeIncriment;
    // States for blocking, attacking, dashing and locking
    public bool isBlocking;
    private bool isAttacking;
    private bool hasDashed;
    private bool isDashing;
    private bool isLock;
    private bool hasStickPushed;
    private bool hasBPressed;
    private float horizontalDash;
    private float verticalDash;

    Quaternion targetRotation;    
    float heading;
    // Set up character controller for motion and vector 
    CharacterController controller;
    Vector3 movementVector;
    float vVelocity;
    // Main camera and its script
    GameObject mainCamera;
    FollowCamera mainCameraScript;
    float lockSpeed;
    // Animator
    Animator anim;

    // Use this for initialization
    void Start () {
        inputTime = inputIntervalMax;
        currenDashTime = maxDashTime;
        dashTimeIncriment = 0.1f;
        horizontalDash = 0f;
        verticalDash = 0f;
        hasDashed = false;
        isDashing = false;
        isBlocking = false;
        isAttacking = false;
        hasStickPushed = false;
        hasBPressed = false;
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
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("run"))
        {
            isAttacking = false;
        }
        else if(!anim.GetCurrentAnimatorStateInfo(0).IsName("block"))
        {
            isAttacking = true;
        }
        if (!isBlocking && !isAttacking && !isDashing)
        {
             normalMovement();
        }
        else if (isAttacking)
        {
             stationaryRotate();
        }
        if (Input.GetAxis("LStick X") != 0 && !hasStickPushed)
        {
             horizontalDash = Input.GetAxis("LStick X") * dashSpeed * Time.deltaTime;
             verticalDash = Input.GetAxis("LStick Y") * movementSpeed * Time.deltaTime;
             hasStickPushed = true;
             StartCoroutine("inputChecker");
            //dash(horizontalDash);
        }
    }
    IEnumerator inputChecker()
    {
        for(float i = 0f; i < inputIntervalMax; i += Time.deltaTime)
        {
            if (Input.GetButtonDown("BButton"))
            {
                hasBPressed = true;
            }
            if(Input.GetAxis("LStick X") == 0)
            {
                hasStickPushed = true;
            }
            else
            {
                hasStickPushed = false;
            }
            if (hasStickPushed && hasBPressed)
            {
                StartCoroutine("dash");
                break;
            }
            yield return null;
        }
        
    }
    IEnumerator dash()
    {
        Debug.Log("dash");
        isDashing = true;
        //cam2 movement
        heading += horizontalDash;
        camPivot.rotation = Quaternion.Euler(0, heading, 0);
        Vector2 inputs = new Vector2(horizontalDash, verticalDash);
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
            var rotation = Quaternion.LookRotation(((-1 * camF * inputs.y - camR * inputs.x) * Time.deltaTime * dashSpeed));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turningSpeed);
        }
        hasDashed = true;
        Debug.Log("start waiting");
        yield return new WaitForSeconds(2f);
        Debug.Log("done waiting");
        isDashing = false;
        hasDashed = false;
    }
    
    private void stationaryRotate()
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

        if (inputs.x != 0 || inputs.y != 0)
        {
            //remove "-1 *" change -  to plus to invert rotation
            var rotation = Quaternion.LookRotation(((-1 * camF * inputs.y - camR * inputs.x) * Time.deltaTime * movementSpeed));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turningSpeed);
        }
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
            if (mainCameraScript.isLockedOn)
            {
                isLock = true;
            }
            
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
            if (System.Math.Abs(horizontal) < vertical)
            {
                lockSpeed = movementSpeed;
            }
            else
            {
                lockSpeed = 50;
            }
            //Debug.Log("lockspeed = " + lockSpeed);
            transform.rotation = Quaternion.LookRotation(-camF);
            movementVector = (camF * inputs.y + camR * inputs.x);
            vVelocity += Physics.gravity.y * Time.deltaTime;
            movementVector.y = vVelocity;
            controller.Move(movementVector * Time.deltaTime * lockSpeed);
            
        }
        // play run animation when the player is moving
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
