using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    // Set health variables
    public float maxHealth;
    public float currentHealth;
    // Create a list of attack hitboxes
    public Collider[] punchHitboxes;
    public Collider[] kickHitBoxes; 
    
    //private string[] punchAnimTriggers;
    //private string[] kickAnimTriggers;

    private GameObject enemyHit;
    // Create an animator variable
    Animator anim;
    // Reference to the health bar
    public Slider healthbar;

    private float currentInputTimer;
    private float inputStartTime;
    private int currentHitNumber;

    // Initialize animator, current health and healthbar value
    void Start()
    {
        //punchAnimTriggers = new string[] { "punch", "punch2", "punch3", "punch4" };
        //kickAnimTriggers = new string[] { "kick", "kick2", "kick3", "kick4" };
        currentHitNumber = 0;
        currentInputTimer = 0;
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthbar.value = currentHealth / maxHealth;
    }
    // Get user inputs
    void Update()
    {
        currentInputTimer += Time.deltaTime;
        // Activate punch when the user presses x
        if (Input.GetButtonDown("XButton"))
        {
            inputStartTime = currentInputTimer;
            anim.SetTrigger("punch");
            launchAttack(punchHitboxes[currentHitNumber]);
            currentHitNumber += 1;
        }
        // Activate kick when user presses Y
        if (Input.GetButtonDown("YButton"))
        {
            inputStartTime = currentInputTimer;
            anim.SetTrigger("kick");
            launchAttack(kickHitBoxes[currentHitNumber]);
            currentHitNumber += 1;
        }

        // resets the hit number when the plaer has reached a max of 4 hits or when 6 seconds has past without input
        if( currentHitNumber == 4 || currentInputTimer - inputStartTime > 2)
        {
            currentHitNumber = 0;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            // Avoid any reload.
            Debug.Log("hello I am idle");
            currentHitNumber = 0;
        }
    }
    // Decrease the current health and update health bar
    public void decreaseHealth(float damage)
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
    public void launchAttack(Collider attack)
    {
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
                c.SendMessageUpwards("decreaseHealth", 10);
            }
        }
    }
}
