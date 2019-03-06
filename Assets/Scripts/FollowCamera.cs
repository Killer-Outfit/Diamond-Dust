using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Camera mainCam;
    public GameObject gameManager;
    // Set target to player, creating reference to camera transform
    public Transform target;
    // Variables for camera speed and location
    public float maxRotation;
    public float minRotation;
    public float minDistance;
    public float maxDistance;
    public float camHeight;
    public float XrotateSpeed;
    public float YrotateSpeed;
    public bool isLockedOn;
    // Objects to hold the lock Marker and target
    public GameObject lockMarker;
    public GameObject lockTarget;

    private float x;
    private float y;
    private Transform myTransform;

    // Bool to check if the player has locked on and already inputted a target change
    bool canChangeTarget;
    float lockTargetHeight;
    // Name of the current lock target, used to check if the object exists after it is destroyed
    string currentLockTargetName;
    // Location where maker is when hidden
    Vector3 hiddenMarker;


    void Start()
    {
        // Instantiate the transform for better performance
        myTransform = transform;
        // Initial cam setup
        CameraSetup();
        // Sets the closest enemy to the lock target
        lockTarget = findClosest();
        currentLockTargetName = "none";
        // Position of marker when not locked on
        hiddenMarker = new Vector3(0, -200, 0);
        canChangeTarget = true;
        isLockedOn = false;
    }

    // Using late update so camera moves after player moves in update for smoother looking motion
    void LateUpdate()
    {
        if (!gameManager.GetComponent<PauseScript>().checkPause())
        {
            // Only lock on when the right trigger or are pressed, when its not already locked on, and when enemies exist
            if ((Input.GetAxis("rightTrigger") > 0 || Input.GetKey(KeyCode.LeftShift)) && !isLockedOn && enemiesExist())
            {
                lockOnToTarget();
            }
            else if (((Input.GetAxis("rightTrigger") == 0 && !Input.GetKey(KeyCode.LeftShift)) && isLockedOn) || GameObject.Find(currentLockTargetName) == null && isLockedOn)
            {
                endisLockedOn();
            }
            // Different updates depending on the camera lock state
            if (!isLockedOn)
            {
                UnlockUpdate();
            }
            else
            {
                LockUpdate();
            }
        }
    }
    // Update to occur when it is not locked
    public void UnlockUpdate()
    {
        // Get right stick input
        x += Input.GetAxis("RStick X") * XrotateSpeed;
        y -= Input.GetAxis("RStick Y") * YrotateSpeed;
        // Get mouse input
        if (Input.GetAxis("MouseX") != 0)
        {
            x += Input.GetAxis("MouseX") * XrotateSpeed;
        }
        if (Input.GetAxis("MouseY") != 0)
        {
            y += Input.GetAxis("MouseY") * YrotateSpeed;
        }
        // Prevent camera motion that is too high or too low
        if( y > maxRotation)
        {
            y = maxRotation;
        }
        if( y < minRotation)
        {
            y = minRotation;
        }
        // Create rotation and the position vectors
        var rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -minDistance) + target.position;
        // Rotate and move the camera
        myTransform.rotation = rotation;
        myTransform.position = position;
    }
    // Update when lock is on 
    public void LockUpdate()
    {
        // The step size is equal to rotate speed times frame time
        float step = 40 * Time.deltaTime;
        // Get the rotation toward the enemy
        var targetRotation = Quaternion.LookRotation(lockTarget.transform.position - transform.position);
        //Debug.Log(targetRotation);
        // Move position of camera with player
        Vector3 position = targetRotation * new Vector3(0.0f, 0.0f, -minDistance) + target.position;
        // Rotate camera by 1 step from current rotate to the target rotation
        myTransform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, step);
        myTransform.position = position;
        // Move the lockMarker to slightly above the position of the lockTarget 
        Vector3 markerPos = new Vector3(lockTarget.transform.position.x, lockTarget.transform.position.y + lockTargetHeight / 2.1f, lockTarget.transform.position.z);
        lockMarker.transform.position = markerPos;
        // Reset the condition to change target
        if (Input.GetAxis("RStick X") == 0 && !canChangeTarget)
        {
            canChangeTarget = true;
        }
        // Change target if it is allowed to and the user pushes the right stick
        if (Input.GetAxis("RStick X") != 0 && canChangeTarget)
        {
            canChangeTarget = false;
            // Change the target to the right, else to the left
            if (Input.GetAxis("RStick X") > 0)
            {
                lockTarget = findClosestInDirection(true);
                markerPos = new Vector3(lockTarget.transform.position.x, lockTarget.transform.position.y + lockTargetHeight / 2, lockTarget.transform.position.z);
                lockMarker.transform.position = markerPos;
            }
            else
            {
                lockTarget = findClosestInDirection(false);
                markerPos = new Vector3(lockTarget.transform.position.x, lockTarget.transform.position.y + lockTargetHeight / 2, lockTarget.transform.position.z);
                lockMarker.transform.position = markerPos;
            }
        }
    }
    // Returns true if to the right, returns false if to the left
    public bool CheckRelativeDirection(GameObject potentialTarget)
    {
        // Get direction of the potential target, the vector toward the potential enemy and the right vector of the current target
        var relativeDir = lockTarget.transform.InverseTransformPoint(potentialTarget.transform.position);
        Vector3 right = lockTarget.transform.TransformDirection(Vector3.right);
        Vector3 toOther = potentialTarget.transform.position - lockTarget.transform.position;
        // Find the Dot product of the right vector and the enemy direction vector, if < 0 the enemy is to the left
        /*if (Vector3.Dot(right, toOther) < 0)
        {
            return false;
        }else
        {
            return true;
        }*/
        /*Debug.Log(potentialTarget.transform.position);
        Debug.Log(relativeDir);
        if (relativeDir.x > 0)
        {
            return true;
        }else
        {
            return false;
        }*/
        /*Vector3 heading = potentialTarget.transform.position - lockTarget.transform.position;
        Vector3 perp = Vector3.Cross(lockTarget.transform.forward, heading);
        float dir = Vector3.Dot(perp, lockTarget.transform.up);
        if (dir > 0f)
        {
            return true;
        }
        else
        {
            return false;
        }*/
        /*Debug.Log(lockTarget.transform.right);
        if (Vector3.Dot(lockTarget.transform.right, potentialTarget.transform.position) < 0)
        {
            return false;
        }
        else
        {
            return true;
        }*/

        /*Vector3 fvec = target.forward;
        Vector3 pvec = target.position - potentialTarget.transform.position;
        float angle = Vector3.SignedAngle(new Vector3(fvec.x, 0, fvec.z), new Vector3(pvec.x, 0, pvec.z), Vector3.up);
        if (angle < 0)
        {
            return false;
        }
        else
        {
            return true;
        }*/

        Vector3 curTargetPortPos = mainCam.WorldToViewportPoint(lockTarget.transform.position);
        Vector3 potentialTargetPortPos = mainCam.WorldToViewportPoint(potentialTarget.transform.position);
        Debug.Log(curTargetPortPos);
        if(curTargetPortPos.x - potentialTargetPortPos.x < 0)
        {
            return true;
        }else
        {
            return false;
        }

    }
    // Find the closest enemy in a particular relative direction
    public GameObject findClosestInDirection(bool right)
    {
        // Create a list of all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float closestDist = 0;
        float enemyDist = 0;
        // Iterate through enemy list
        foreach (GameObject enemy in enemies)
        {
            // Check the enemies relative direction
            if (CheckRelativeDirection(enemy) == right)
            {
                // Initialize closest to the first enemy
                if (closest == null)
                {
                    closest = enemy;
                    closestDist = Vector3.Distance(enemy.transform.position, lockTarget.transform.position);
                }
                enemyDist = Vector3.Distance(enemy.transform.position, lockTarget.transform.position); 
                // Check if the enemy is closest
                if (enemyDist < closestDist)
                {
                    closest = enemy;
                    closestDist = enemyDist;
                }
            }
        }
        // Change target if a new target exists
        if (closest != null)
        {
            return closest;
        }
        // Dont change the target if there is no valid new one
        return lockTarget;
    }
    // Finding the closest enemy in any direction
    public GameObject findClosest()
    {
        // Create a list of all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float closestDist = 0;
        float enemyDist = 0;
        // Iterate through enemy list
        foreach (GameObject enemy in enemies)
        {
            if (isEnemyOnScreen(enemy.transform.position))
            {
                // Initialize closest to the first enemy
                if (closest == null)
                {
                    closest = enemy;
                    closestDist = getDistance(enemy);
                }
                enemyDist = getDistance(enemy);
                // Check if the enemy is closest
                if (enemyDist < closestDist)
                {
                    closest = enemy;
                    closestDist = enemyDist;
                }
            }
        }
        return closest;
    }
    public bool isEnemyOnScreen(Vector3 worldPos)
    {
        Vector3 camRelativePos = mainCam.WorldToViewportPoint(worldPos);
        if(camRelativePos.x < 0 || camRelativePos.y < 0 || camRelativePos.z < 0)
        {
            return false;
        }else if (camRelativePos.x > mainCam.pixelWidth || camRelativePos.y > mainCam.pixelHeight)
        {
            return false;
        }else
        {
            return true;
        }
    }
    // Get the distance between an enemy and the player
    public float getDistance(GameObject enemy)
    {
        float distance = Vector3.Distance(enemy.transform.position, target.transform.position);
        return distance;
    }
    // Set initial position and look target to the player
    public void CameraSetup()
    {
        myTransform.position = new Vector3(target.position.x, target.position.y + camHeight, target.position.z - minDistance);
        myTransform.LookAt(target);
    }
    // Check if there are enemies in the scene
    public bool enemiesExist()
    {
        var array = GameObject.FindGameObjectsWithTag("Enemy");
        if(array.Length > 0)
        {
            return true;
        }
        return false;
    }
    // Lock onto target if the target is less than 30 distance away
    public void lockOnToTarget()
    {
        lockTarget = findClosest();
        if (getDistance(lockTarget) < 30)
        {
            currentLockTargetName = lockTarget.name;
            ChangeLockTargetHeight();
            isLockedOn = true;
        }
    }
    // Change the lock target height variable
    public void ChangeLockTargetHeight()
    {
        lockTargetHeight = lockTarget.GetComponent<CapsuleCollider>().height;
    }
    // Move the lockMarker and end isLockedOn
    public void endisLockedOn()
    {
        lockMarker.transform.position = hiddenMarker;
        isLockedOn = false;
    }
}
