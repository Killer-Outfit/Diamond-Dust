using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime;
    public int damage;
    public bool hasHit;

    private float currentTime;
    

    // Start is called before the first frame update
    void Start()
    {
        hasHit = false;
        currentTime = 0.0f;
    }
    void Update()
    {
        if(currentTime < lifeTime)
        {
            currentTime += Time.deltaTime;
        }else
        {
            Destroy(this.gameObject);
        }
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            hasHit = true;
            collision.gameObject.GetComponent<EnemyScript>().DecreaseHealth(damage);
        }
    }
}