using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    GameObject player;
    public float speed;
    //float initialY;
    private Transform myTransform;
    public int health;
    public float followDistance;
    public Collider[] attackHitboxes;
    //public Transform transformObject;
    float timer;
    bool canLaunchAttack;
    public bool canBlock = false;
    public bool isBlocking = false;
    Animator anim;
    NavMeshAgent agent;
    
    //Vector3 destination;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        //destination = agent.destination;
        canLaunchAttack = true;
        timer = 0;
        myTransform = transform;
        //initialY = myTransform.position.y;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        int seconds = ((int)timer % 60);

        if (player)
        {
            if (Vector3.Distance(agent.transform.position, player.transform.position) > followDistance)
            {
                agent.destination = player.transform.position;
                agent.isStopped = false;
                //Debug.Log("Distance to player is " + Vector3.Distance(agent.transform.position, player.transform.position));
                //destination = player.transform.position;
                agent.speed = speed;
                //Debug.Log("Destination is " + agent.destination);
                //float step = speed * Time.deltaTime;
                //transformObject.position = Vector3.MoveTowards(transformObject.position, player.transform.position, step);
                //Debug.Log("enemy out of range");
                //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
                //myTransform.position = new Vector3(transformObject.position.x, initialY, transformObject.position.z);
                anim.SetBool("isIdle", false);
                //anim.Play("run");
            }

            else
            {
                //Debug.Log(timer);
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                anim.SetBool("isIdle", true);
                if (!isBlocking) // Only try to attack if not blocking. Always true on enemies that can't block.
                {
                    if ((int) timer % 20 == 0 && canLaunchAttack)
                    {
                        anim.SetTrigger("punch");
                        launchAttack(attackHitboxes[0]);
                        canLaunchAttack = false;
                    }
                    else if ((int) timer % 3 == 0 && !canLaunchAttack)
                    {
                        canLaunchAttack = true;
                    }
                }
                //Debug.Log("enemy in range");

                //anim.Play("idle");

            }

            // Try to block if the player is close enough and attacking, and if the enemy can block. Sets the block script's timer to 1, so it refreshes if the player attacks repeatedly.
            if (Vector3.Distance(agent.transform.position, player.transform.position) < 5 && canBlock && player.gameObject.GetComponent<Player>().isAttacking)
            {
                SendMessage("Block");
            }
            myTransform.LookAt(2 * myTransform.position - player.transform.position);
        }
    }

    public void launchAttack(Collider attack)
    {
        //overlapSphere is best if applicable
        Collider[] cols = Physics.OverlapBox(attack.bounds.center, attack.bounds.extents, attack.transform.rotation, LayerMask.GetMask("Hitbox"));

        foreach (Collider c in cols)
        {
            //Debug.Log(c.name);
            //if the collision is with the own player body
            if (c.transform == transform)
            {
                //skips the rest of code in loop and keeps checking 
                //Debug.Log("ignoring self hit");
                continue;
            }
            else if (c.name == attack.name)
            {
                //Debug.Log("stopped self hit");
                continue;
            }
            else if (c.name == "Player")
            {
                Debug.Log("hit the " + c.name);
                c.SendMessageUpwards("DecreaseHealth", 10);
            }
        }
    }

    public float GetDistance()
    {
        float distance = Vector3.Distance(myTransform.position, player.transform.position);
        return distance;
    }

    public void DecreaseHealth(int damage)
    {
        if (isBlocking) //Take no damage if blocking.
        {
            Debug.Log("get blocked idiot");
        }
        else
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }
        //Debug.Log("enemy current Health is " + health);
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }

    protected void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        Debug.Log("Collided with player");
    //        GetComponent<Rigidbody>().isKinematic = false;
    //    }
            
    //}

    //void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //        GetComponent<Rigidbody>().isKinematic = true;
    //}
}
