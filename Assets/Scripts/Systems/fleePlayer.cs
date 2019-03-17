using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fleePlayer : MonoBehaviour
{
    public float multiplier;
    public float runRange;

    Animator anim;

    private Transform player;
    private UnityEngine.AI.NavMeshAgent myNMagent;
    private float nextTurnTime;
    private Transform startTransform;

    // Use this for initialization
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        myNMagent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        if ((player.position - transform.position).sqrMagnitude < Mathf.Pow(runRange, 3))
        {
            //Debug.Log("Too close to player on -> " + Mathf.Sqrt((player.position - transform.position).sqrMagnitude));
            RunFrom();
        }
            

        //if (Mathf.Abs(player.position.z - transform.position.z) < runRange)
        //{
        //    Debug.Log("Too close to player on z plane -> " + Mathf.Abs(player.position.z - transform.position.z));
        //    RunFrom();
        //}
            
    }

    public void RunFrom()
    {

       
        // temporarily point the object to look away from the player
        transform.rotation = Quaternion.LookRotation(transform.position - player.position);

        // Then we'll get the position on that rotation that's multiplier down the path (you could set a Random.range
        // for this if you want variable results) and store it in a new Vector3 called runTo
        Vector3 runTo = transform.position + transform.forward * multiplier;
        anim.SetFloat("speed", multiplier);
        // Debug.Log("runTo = " + runTo);

        // So now we've got a Vector3 to run to and we can transfer that to a location on the NavMesh with samplePosition.

        UnityEngine.AI.NavMeshHit hit;    // stores the output in a variable called hit

        // 5 is the distance to check, assumes you use default for the NavMesh Layer name
        UnityEngine.AI.NavMesh.SamplePosition(runTo, out hit, 5, 1 << UnityEngine.AI.NavMesh.GetAreaFromName("Walkable"));
        //Debug.Log("hit = " + hit + " hit.position = " + hit.position);

        // And get it to head towards the found NavMesh position
        myNMagent.SetDestination(hit.position);
    }
}
