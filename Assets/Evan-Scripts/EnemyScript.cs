using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    public float speed;
   // float initialY;
    private Transform myTransform;
    public int health;
    public float followDistance;
    public Collider[] attackHitboxes;
    //public Transform transformObject;
    float timer;
    bool canLaunchAttack;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
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

        if (getDistance() > followDistance)
        {
            float step = speed * Time.deltaTime;
            //transformObject.position = Vector3.MoveTowards(transformObject.position, player.transform.position, step);
            //Debug.Log("enemy out of range");
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
            //myTransform.position = new Vector3(transformObject.position.x, initialY, transformObject.position.z);
            anim.SetBool("isIdle", false);
            //anim.Play("run");
        }else
        {
            //Debug.Log(timer);
            anim.SetBool("isIdle", true);
            if ((int)timer % 20 == 0 && canLaunchAttack)
            {
                anim.SetTrigger("punch");
                launchAttack(attackHitboxes[0]);
                canLaunchAttack = false;
            }else if((int)timer % 3 == 0 && !canLaunchAttack)
            {
                canLaunchAttack = true;
            }
            //Debug.Log("enemy in range");
            
            //anim.Play("idle");

        }
        myTransform.LookAt(2 * myTransform.position - player.transform.position);
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
                c.SendMessageUpwards("decreaseHealth", 10);
            }
        }
    }

    public float getDistance()
    {
        float distance = Vector3.Distance(myTransform.position, player.transform.position);
        return distance;
    }

    public void decreaseHealth(int damage)
    {
        
        health -= damage;
        if(health <= 0)
        {
            die();
        }
        Debug.Log("enemy current Health is " + health);
    }

    private void die()
    {
        Destroy(this.gameObject);
    }
}
