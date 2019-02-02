using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float initialY;
    private Transform myTransform;
    public int health;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(getDistance() > 5)
        {
            float step = speed * Time.deltaTime;
            myTransform.position = Vector3.MoveTowards(myTransform.position, player.transform.position, step);
            myTransform.position = new Vector3(myTransform.position.x, initialY, myTransform.position.z);
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
