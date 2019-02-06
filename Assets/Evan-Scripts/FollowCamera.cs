using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // set target to player, creating reference to camera transform
    public Transform target;
    private Transform myTransform;

    // variables for camera speed and location
    public float minDistance;
    public float maxDistance;
    public float camHeight;
    public float XrotateSpeed;
    public float YrotateSpeed;
    private float x;
    private float y;

    // location where maker is when hidden
    //Vector3 targetDir;
    Vector3 hiddenMarker;

    public GameObject lockMarker;
    public GameObject lockTarget;

    string currentLockTargetName;

    bool lockOn = false;

    void Start()
    {
        myTransform = transform;
        CameraSetup();
        lockTarget = findClosest();
        currentLockTargetName = "none";
        hiddenMarker = new Vector3(0, -200, 0);
    }

    //using late update so camera moves after player moves in update
    void LateUpdate()
    {
        if((Input.GetAxis("rightTrigger") > 0 || Input.GetButtonDown("L")) && !lockOn && enemiesExist())
        {
            //Debug.Log("MADEITTTTTTGFKUYTYFIYSFUYKDGUYKDFKUDY");
            lockOnToTarget();
        }else if(((Input.GetAxis("rightTrigger") == 0 || Input.GetButtonDown("L"))&& lockOn) || GameObject.Find(currentLockTargetName) == null && lockOn)
        {
            endLockOn();
            Debug.Log("stopped locking on");
        }
        if (!lockOn)
        {

            x += Input.GetAxis("RStick X") * XrotateSpeed;
            y -= Input.GetAxis("RStick Y") * YrotateSpeed;

            if(Input.GetAxis("MouseX") != 0)
            {
                x += Input.GetAxis("MouseX") * XrotateSpeed;
            }
            if (Input.GetAxis("MouseY") != 0)
            {
                y += Input.GetAxis("MouseY") * YrotateSpeed;
            }

            var rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -minDistance) + target.position;

            myTransform.rotation = rotation;
            myTransform.position = position;
        }else
        {
            //targetDir = lockTarget.transform.position - transform.position;
            // The step size is equal to speed times frame time.
            float step = 40 * Time.deltaTime;
            var targetRotation = Quaternion.LookRotation(lockTarget.transform.position - transform.position);
            Vector3 position = targetRotation * new Vector3(0.0f, 0.0f, -minDistance) + target.position;
            myTransform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, step);
            myTransform.position = position;
            Vector3 markerPos = new Vector3(lockTarget.transform.position.x, lockTarget.transform.position.y + 2, lockTarget.transform.position.z);
            lockMarker.transform.position = markerPos;

            if (Input.GetAxis("RStick X") != 0)
            {
                Debug.Log("x = " + Input.GetAxis("RStick X") + "y = " + Input.GetAxis("RStick Y"));
                Debug.Log(lockTarget.transform.forward);
                if(Input.GetAxis("RStick X") > 0)
                {
                    lockTarget = findClosestInDirection(true);
                }else
                {
                    lockTarget = findClosestInDirection(false);
                }
            }
        }
    }



    //returns true if to the right, returns false if to the left
    public bool CheckRelativeDirection(GameObject potentialTarget)
    {
        var relativeDir = lockTarget.transform.InverseTransformPoint(potentialTarget.transform.position);
        Vector3 right = lockTarget.transform.TransformDirection(Vector3.right);
        Vector3 toOther = potentialTarget.transform.position - lockTarget.transform.position;
        if (Vector3.Dot(right, toOther) < 0)
        {
            Debug.Log("move to left");
            return false;
        }else
        {
            return true;
        }
    }

    public GameObject findClosestInDirection(bool right)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float closestDist = 0;
        float enemyDist = 0;
        foreach (GameObject enemy in enemies)
        {
            if (CheckRelativeDirection(enemy) == right)
            {
                if (closest == null)
                {
                    closest = enemy;
                    closestDist = getDistance(enemy);
                }
                enemyDist = getDistance(enemy);
                //Debug.Log("Enemy = " + enemy.name + " distance = " + enemyDist);
                if (enemyDist < closestDist)
                {
                    closest = enemy;
                    closestDist = enemyDist;
                }
            }
        }
        if (closest != null)
        {
            return closest;
        }
        return lockTarget;
    }

    public GameObject findClosest()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float closestDist = 0;
        float enemyDist = 0;
        foreach (GameObject enemy in enemies)
        {
            if(closest == null)
            {
                closest = enemy;
                closestDist = getDistance(enemy);
            }
            enemyDist = getDistance(enemy);
            //Debug.Log("Enemy = " + enemy.name + " distance = " + enemyDist);
            if(enemyDist < closestDist)
            {
                closest = enemy;
                closestDist = enemyDist;
            }
        }
        
        return closest;
    }

    public float getDistance(GameObject enemy)
    {
        float distance = Vector3.Distance(enemy.transform.position, target.transform.position);
        return distance;
    }

    public void CameraSetup()
    {
        myTransform.position = new Vector3(target.position.x, target.position.y + camHeight, target.position.z - minDistance);
        myTransform.LookAt(target);
    }

    public bool enemiesExist()
    {
        var array = GameObject.FindGameObjectsWithTag("Enemy");
        if(array.Length > 0)
        {
            return true;
        }
        return false;
    }

    public void lockOnToTarget()
    {
        lockTarget = findClosest();
        if (getDistance(lockTarget) < 30)
        {
            currentLockTargetName = lockTarget.name;
            lockOn = true;
        }
    }

    public void endLockOn()
    {
        lockMarker.transform.position = hiddenMarker;
        lockOn = false;
        //x = myTransform.rotation.x;
        //y = myTransform.rotation.y;
    }
}
