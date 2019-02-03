using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    public float speed;
    float initialY;
    private Transform myTransform;
    public int health;
    public float followDistance;
    public Transform transformObject;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        initialY = myTransform.position.y;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(getDistance() > followDistance)
        {
            //float step = speed * Time.deltaTime;
            //transformObject.position = Vector3.MoveTowards(transformObject.position, player.transform.position, step);
            //myTransform.position = new Vector3(transformObject.position.x, initialY, transformObject.position.z);
            //myTransform.LookAt(player.transform);
            anim.Play("run");
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
