using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crowd : MonoBehaviour
{
    public float sensorLength = 5.0f;
    public float speed = 0.001f;
    float rotationSpeed = 1.0f;
    Vector3 averageHeading;
    Vector3 averagePosition;
    float neighborDistance = 3.0f;

    Collider myCollider;

    bool canCurrentlyTurn = false;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = transform.GetComponent<Collider>();
        speed = Random.Range(0.5f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        // Right Sensor
        if(Physics.Raycast(transform.position, transform.right, out hit, (sensorLength + transform.localScale.x)))
        {
            if(hit.collider.tag != "Obstacle" || hit.collider == myCollider)
            {
                return;
            }
            transform.Rotate(0.0f, -0.5f, 0.0f);
            speed = Random.Range(0.5f, 1);
        }
        // Left Sensor
        if (Physics.Raycast(transform.position, -transform.right, out hit, (sensorLength + transform.localScale.x)))
        {
            if (hit.collider.tag != "Obstacle" || hit.collider == myCollider)
            {
                return;
            }
            transform.Rotate(0.0f, 0.5f, 0.0f);
            speed = Random.Range(0.5f, 1);
        }
        // Front Sensor
        if (Physics.Raycast(transform.position, transform.forward, out hit, (sensorLength + transform.localScale.z)))
        {
            if (hit.collider.tag != "Obstacle" || hit.collider == myCollider)
            {
                return;
            }
            speed = 0.0f;
            if (Physics.Raycast(transform.position, -transform.right, out hit, (sensorLength + transform.localScale.x)))
                transform.Rotate(0.0f, 0.5f, 0.0f);
            if (Physics.Raycast(transform.position, transform.right, out hit, (sensorLength + transform.localScale.x)))
                transform.Rotate(0.0f, -0.5f, 0.0f);
            else
                transform.Rotate(0.0f, 0.5f, 0.0f);

        }
        else
        {
            speed = Random.Range(0.5f, 1);
        }

        if (Vector3.Distance(transform.position, Vector3.zero) >= globalCrowd.arenaSize)
        {
            canCurrentlyTurn = true;
        }
        else
        {
            canCurrentlyTurn = false;
        }

        if (canCurrentlyTurn)
        {
            Vector3 direction = Vector3.zero - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  rotationSpeed * Time.deltaTime);
            speed = Random.Range(0.5f, 1);
        }
        else
        {

            if (Random.Range(0, 5) < 1)
                ApplyRules();
        }

        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * (sensorLength + transform.localScale.z));
        Gizmos.DrawRay(transform.position, -transform.right * (sensorLength + transform.localScale.x));
        Gizmos.DrawRay(transform.position, transform.right * (sensorLength + transform.localScale.x));
    }

    void ApplyRules()
    {
        GameObject[] gos;
        gos = globalCrowd.allCrowd;

        Vector3 vcenter = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.1f;

        Vector3 goalPos = globalCrowd.goalPos;

        float dist;

        int groupSize = 0;
        foreach(GameObject go in gos)
        {
            if(go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if(dist <-neighborDistance)
                {
                    vcenter += go.transform.position;
                    groupSize++;

                    if (dist < 1.0f)
                        vavoid = vavoid + (this.transform.position - go.transform.position);

                    crowd anotherCrowd = go.GetComponent<crowd>();
                    gSpeed = gSpeed + anotherCrowd.speed;
                }
            }
        }

        if(groupSize > 0)
        {
            vcenter = vcenter/groupSize + (goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            Vector3 direction = (vcenter + vavoid) - transform.position;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                     Quaternion.LookRotation(direction),
                                     rotationSpeed * Time.deltaTime);
        }
    }
}
