using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    //public GameObject target;
    public Transform target;
    public float minDistance;
    public float maxDistance;
    public float camHeight;
    public float XrotateSpeed;
    public float YrotateSpeed;
    Vector3 offset;
    private float x;
    private float y;

    Vector3 targetDir;
    public GameObject lockMarker;
    Vector3 hiddenMarker;

    GameObject lockTarget;
    string currentLockTargetName;
    bool lockOn = false;

    private Transform myTransform;

    void Start()
    {
        if (target == null)
            Debug.LogWarning("no target selected");
        myTransform = transform;
        CameraSetup();
        lockTarget = findClosest();
        currentLockTargetName = "none";
        hiddenMarker = new Vector3(0, -200, 0);
    }

    //using late update so camera moves after player moves in update
    void LateUpdate()
    {
        if(Input.GetAxis("rightTrigger") > 0 && !lockOn && enemiesExist())
        {
            //Debug.Log("MADEITTTTTTGFKUYTYFIYSFUYKDGUYKDFKUDY");
            lockOnToTarget();
        }else if((Input.GetAxis("rightTrigger") == 0 && lockOn) || GameObject.Find(currentLockTargetName) == null && lockOn)
        {
            endLockOn();
            Debug.Log("stopped locking on");
        }
        if (!lockOn)
        {
            x += Input.GetAxis("RStick X") * XrotateSpeed;
            y -= Input.GetAxis("RStick Y") * YrotateSpeed;
            
            var rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -minDistance) + target.position;

            myTransform.rotation = rotation;
            myTransform.position = position;
        }else
        {
            targetDir = lockTarget.transform.position - transform.position;
            // The step size is equal to speed times frame time.
            float step = 40 * Time.deltaTime;
            var targetRotation = Quaternion.LookRotation(lockTarget.transform.position - transform.position);
            Vector3 position = targetRotation * new Vector3(0.0f, 0.0f, -minDistance) + target.position;
            myTransform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, step);
            myTransform.position = position;
            Vector3 markerPos = new Vector3(lockTarget.transform.position.x, lockTarget.transform.position.y + 2, lockTarget.transform.position.z);
            lockMarker.transform.position = markerPos;
        }
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
