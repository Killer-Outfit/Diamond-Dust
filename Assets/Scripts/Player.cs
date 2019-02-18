using System.Collections;
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
    public Collider[] punchHitboxes;
    public Collider[] kickHitBoxes;
    public Collider[] miscHitBoxes;

    private string[] punchAnims;
    private string[] kickAnims;
    private string[] miscAnims;

    private List<string> inputQueue;
    private List<string> animQueueStateNames;

    private GameObject enemyHit;
    // Create an animator variable
    Animator anim;
    // Reference to the health bar
    public Slider healthbar;


    private float currentInputTimer;
    private float inputStartTime;
    private float shield;
    private int currentHitNumber;
    private bool blocking;

    // Initialize animator, current health and healthbar value
    void Start()
    {
        //transform.localScale = new Vector3(0.35F, 0.35f, 0.35f);
        shield = 100;
        blocking = false;
        animQueueStateNames = new List<string>() { "checkQueueState1", "checkQueueState2", "checkQueueState3" };
        inputQueue = new List<string>();
        punchAnims = new string[] { "punch1", "punch2", "punch3", "punch4" };
        kickAnims = new string[] { "kick1", "kick2", "kick3", "kick4" };
        miscAnims = new string[] { "misc1", "misc2", "misc3", "misc4" };
        currentHitNumber = 0;
        currentInputTimer = 0;
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthbar.value = currentHealth / maxHealth;
    }
    // Get user inputs
    void Update()
    {
        if (!this.isAttacking) //Don't call attacks if the player is mid-attack already.
        {
            currentInputTimer += Time.deltaTime;
            // Activate punch when the user presses x
            if (Input.GetButtonDown("XButton"))
            {
                if (inputQueue.Count < 3)
                {
                    inputQueue.Add("punch");
                }
            }
            // Activate kick when user presses Y
            if (Input.GetButtonDown("YButton"))
            {
                if (inputQueue.Count < 3)
                {
                    inputQueue.Add("kick");
                }
            }

            if (Input.GetButtonDown("AButton"))
            {
                if (inputQueue.Count < 3)
                {
                    inputQueue.Add("misc");
                }
            }


            if (Input.GetButton("BButton"))
            {
                Debug.Log("pressing B");
                if (!blocking)
                {
                    anim.SetTrigger("block");
                    blocking = true;
                    gameObject.GetComponent<PlayerMove>().changeBlock();
                }
            }
            else if (blocking)
            {
                Debug.Log("here");
                blocking = false;
                gameObject.GetComponent<PlayerMove>().changeBlock();
                anim.SetTrigger("block");
            }

            // resets the hit number when the plaer has reached a max of 4 hits or when 6 seconds has past without input
            if (currentHitNumber == 4 || currentInputTimer - inputStartTime > 2)
            {
                currentHitNumber = 0;
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                // Avoid any reload.
                //Debug.Log("hello I am idle");
                gameObject.GetComponent<PlayerMove>().changeAttacking(false);
                currentHitNumber = 0;
                checkQueue();
            }
            foreach (var stateName in animQueueStateNames)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
                {
                    checkQueue();
                }
            }

        }
    }
    // Decrease the current health and update health bar
    public void DecreaseHealth(float damage)
    {
        if (!blocking)
        {
            currentHealth -= damage;
            healthbar.value = currentHealth / maxHealth;
            // If health drops to or bellow 0 then the player dies
            if (currentHealth <= 0)
            {
                killPlayer();
            }
        }
        else
        {
            decreaseSheild(damage);
        }
    }
    public void decreaseSheild(float damage)
    {
        shield -= damage;
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
        gameObject.GetComponent<PlayerMove>().changeAttacking(true);
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
    public void pressX()
    {
        inputStartTime = currentInputTimer;
        anim.SetTrigger("punch");
        launchAttack(punchHitboxes[currentHitNumber]);
        currentHitNumber += 1;
    }

    public void pressY()
    {
        Debug.Log("I am pressing Y");
        inputStartTime = currentInputTimer;
        anim.SetTrigger("kick");
        launchAttack(kickHitBoxes[currentHitNumber]);
        currentHitNumber += 1;
    }
    public void pressA()
    {
        Debug.Log("I am pressing A");
        inputStartTime = currentInputTimer;
        anim.SetTrigger("miscAttack");
        launchAttack(miscHitBoxes[currentHitNumber]);
        currentHitNumber += 1;
    }

    public void checkQueue()
    {
        string input = "";
        if (inputQueue.Count > 0)
        {
            input = inputQueue[0];
            inputQueue.RemoveAt(0);
            if (input == "punch")
            {
                pressX();
            }
            else if (input == "kick")
            {
                pressY();
            }
            else if (input == "misc")
            {
                pressA();
            }
        }
    }
}
