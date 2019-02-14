﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    // Set health variables
    public float maxHealth;
    public float currentHealth;
    // State bools
    public bool isAttacking = false;
    // Create a list of attack hitboxes
    public Collider[] attackHitboxes;

    private GameObject enemyHit;
    // Create an animator variable
    Animator anim;
    // Reference to the health bar
    public Slider healthbar;
    // Initialize animator, current health and healthbar value
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthbar.value = currentHealth / maxHealth;
    }
    // Get user inputs
    void Update()
    {
        if (!this.isAttacking) //Don't call attacks if the player is mid-attack already.
        {
            // Activate punch when the user presses x
            if (Input.GetButtonDown("XButton"))
            {
                anim.SetTrigger("punch");
                StartCoroutine(launchAttack(attackHitboxes[0]));
            }

            // Activate kick when user presses Y
            if (Input.GetButtonDown("YButton"))
            {
                anim.SetTrigger("kick");
                StartCoroutine(launchAttack(attackHitboxes[1]));
            }
        }
    }
    // Decrease the current health and update health bar
    public void DecreaseHealth(float damage)
    {
        currentHealth -= damage;
        healthbar.value = currentHealth / maxHealth;
        // If health drops to or bellow 0 then the player dies
        if (currentHealth <= 0)
        {
            killPlayer();
        }
    }
    // Kill the player
    private void killPlayer()
    {
        //Destroy(this.gameObject);
        currentHealth = maxHealth;
    }
    // Make the attack activate
    IEnumerator launchAttack(Collider attack)
    {
        this.isAttacking = true;
        yield return new WaitForSeconds(0.2f); // Do hitbox calcuation after 0.2 seconds. Adjust this to match the animation later?
        //overlapSphere is best if applicable
        // Create a list of all objects that have collided with the attack hitbox
        Collider[] cols = Physics.OverlapBox(attack.bounds.center, attack.bounds.extents, attack.transform.rotation, LayerMask.GetMask("Hitbox"));
        // Iterate through each collision event
        foreach(Collider c in cols)
        {
            //Debug.Log(c.name);
            // If the collision is with the player's own body
            if (c.transform == transform)
            {
                // Skips the rest of code in loop and keeps checking 
                continue;
            }
            // Check if collision event is not with itself
            else if (c.name == attack.name)
            {
                continue;
            }
            else
            {
                //Debug.Log("hit the " + c.name);
                // Decrease the hit target's health by 10
                c.SendMessageUpwards("DecreaseHealth", 10);
            }
        }
        yield return new WaitForSeconds(0.2f); //"Cooldown" time
        this.isAttacking = false;
    }
}
