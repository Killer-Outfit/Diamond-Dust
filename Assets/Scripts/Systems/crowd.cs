using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class crowd : MonoBehaviour
{
    public float sensorLength;
    public float speed;

    float rotationSpeed;
    Vector3 averageHeading;
    Vector3 averagePosition;
    float neighborDistance;

    Collider myCollider;
    NavMeshAgent agent;
    Animator anim;

    bool canCurrentlyTurn = false;

    // Start is called before the first frame update
    void Start()
    {
        sensorLength = 5.0f;
        rotationSpeed = 1.0f;
        neighborDistance = 3.0f;
        myCollider = transform.GetComponent<Collider>();
        speed = Random.Range(1f, 5);
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
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
            anim.SetFloat("speed", speed);
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
            anim.SetFloat("speed", speed);
        }
        // Front Sensor
        if (Physics.Raycast(transform.position, transform.forward, out hit, (sensorLength + transform.localScale.z)))
        {
            if (hit.collider.tag != "Obstacle" || hit.collider == myCollider)
            {
                return;
            }
            speed = 0.0f;
            anim.SetFloat("speed", speed);
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
            anim.SetFloat("speed", speed);
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
            anim.SetFloat("speed", speed);

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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetComponent<Rigidbody>().isKinematic = true;
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            GetComponent<Rigidbody>().isKinematic = false;
    }
}
