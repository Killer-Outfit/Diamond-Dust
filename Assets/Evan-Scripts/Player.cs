using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public Collider[] attackHitboxes;
    private GameObject enemyHit;

    // External variables
    public Slider healthbar;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthbar.value = currentHealth / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("XButton"))
        {
            Debug.Log("PUNCH!!!!!!!!!");
            launchAttack(attackHitboxes[0]);
        }
    }

    public void decreaseHealth(float damage)
    {
        currentHealth -= damage;
        healthbar.value = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            killPlayer();
        }
    }

    private void killPlayer()
    {
        Destroy(this.gameObject);
    }

    //ADD COLLER TO THE HAND BONE AND FOOT BONES
    public void launchAttack(Collider attack)
    {
        //overlapSphere is best if applicable
        Collider[] cols = Physics.OverlapBox(attack.bounds.center, attack.bounds.extents, attack.transform.rotation, LayerMask.GetMask("Hitbox"));
       
        foreach(Collider c in cols)
        {
            Debug.Log(c.name);
            //if the collision is with the own player body
            if(c.transform == transform)
            {
                //skips the rest of code in loop and keeps checking 
                Debug.Log("ignoring self hit");
                continue;
            }else
            {
                Debug.Log("hit the " + c.name);
                c.SendMessageUpwards("decreaseHealth", 10);
            }
        }
    }
}
