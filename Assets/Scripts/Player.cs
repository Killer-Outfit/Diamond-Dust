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
    private bool blocking;
    // Create a list of attack hitboxes
    public Collider[] punchHitboxes;
    public Collider[] kickHitBoxes;
    public Collider[] miscHitBoxes;
    private string attackType;
    // Create single element input queue
    private string[] inputQueue;
    // Queue check 
    private List<string> animQueueStateNames;

    // Create an animator variable and animation overrider for outfit switching
    Animator anim;
    AnimatorOverrideController animatorOverrideController;
    
    // Reference to the health bar
    public Slider healthbar;


    private float currentInputTimer;
    private float inputStartTime;
    private float shield;
    private int currentHitNumber;
    

    // Initialize animator, current health and healthbar value
    void Start()
    {
        //transform.localScale = new Vector3(0.35F, 0.35f, 0.35f);
        attackType = "";
        shield = 100;
        blocking = false;
        inputQueue = new string[1] { "" };
        animQueueStateNames = new List<string>() { "checkQueueState1", "checkQueueState2", "checkQueueState3" };
        currentHitNumber = 0;
        currentInputTimer = 0;
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthbar.value = currentHealth / maxHealth;


        //animatorOverrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        //anim.runtimeAnimatorController = animatorOverrideController;
        //var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

    }
    // Get user inputs
    void Update()
    {
        // Don't call attacks if the player is mid-attack already.
        if (!this.isAttacking) 
        {
            currentInputTimer += Time.deltaTime;
            // Add punch to input queue
            if (Input.GetButtonDown("XButton"))
            {
                inputQueue[0] = "punch";
            }
            // Activate kick when user presses Y
            /*if (Input.GetButtonDown("YButton"))
            {
                if (inputQueue.Count < 3)
                {
                    inputQueue.Add("kick");
                }
            }*/
            // Add misc attack to input queue
            if (Input.GetButtonDown("AButton"))
            {
                inputQueue[0] = "misc";
            }
            // Block initiation
            if (Input.GetButton("BButton"))
            {
                if (!blocking)
                {
                    anim.SetTrigger("block");
                    blocking = true;
                    // Disable player motion when blocking
                    gameObject.GetComponent<PlayerMove>().changeBlock();
                }
            }
            // Enable player motion after blocking is complete  
            else if (blocking)
            {
                Debug.Log("here");
                blocking = false;
                gameObject.GetComponent<PlayerMove>().changeBlock();
                anim.SetTrigger("block");
            }

            // Resets the hit number when the plaer has reached a max of 4 hits or when 6 seconds has past without input
            if (currentHitNumber == 4 || currentInputTimer - inputStartTime > 2)
            {
                currentHitNumber = 0;
            }
            // Occur when player is in idle
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                // Reset the hit#, allow non-attack movement, check the input queue
                gameObject.GetComponent<PlayerMove>().changeAttacking(false);
                currentHitNumber = 0;
                checkQueue();
            }
            // Check if player is in a queue check animation if it is check the queue
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
    // Sheild damage if blocking
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
    IEnumerator launchAttack()
    {
        // Set the collider beig used based on current attack type
        Collider attack = null;
        if(attackType == "punch")
        {
            attack = punchHitboxes[currentHitNumber];
        }
        else if (attackType == "kick")
        {
            attack = kickHitBoxes[currentHitNumber];
        }
        else if (attackType == "misc")
        {
            attack = miscHitBoxes[currentHitNumber];
        }
        
        this.isAttacking = true;
        gameObject.GetComponent<PlayerMove>().changeAttacking(isAttacking);
        // Do hitbox calcuation after 0.2 seconds. ADJUST THIS TO MATCH ANIMATION TIME LATER?
        yield return new WaitForSeconds(0.2f); 
        //overlapSphere is best if applicable
        // Create a list of all objects that have collided with the attack hitbox
        Collider[] cols = Physics.OverlapBox(attack.bounds.center, attack.bounds.extents, attack.transform.rotation, LayerMask.GetMask("Hitbox"));
        // Iterate through each collision event
        foreach(Collider c in cols)
        {
            // If you collided with an enemy damage them
            if (c.tag == "Enemy")
            {
                // Decrease the hit target's health by 10 CHANGE TO ATTACK DAMAGE
                c.SendMessageUpwards("DecreaseHealth", 10);
            }
        }
        // "Cooldown" time
        yield return new WaitForSeconds(0.2f); 
        this.isAttacking = false;
        gameObject.GetComponent<PlayerMove>().changeAttacking(isAttacking);
    }
    // Activate punch
    public void pressX()
    {
        inputStartTime = currentInputTimer;
        anim.SetTrigger("punch");
        attackType = "punch";
        StartCoroutine("launchAttack");
        currentHitNumber += 1;
    }
    // Activate kick
    public void pressY()
    {
        inputStartTime = currentInputTimer;
        anim.SetTrigger("kick");
        attackType = "kick";
        StartCoroutine("launchAttack");
        currentHitNumber += 1;
    }
    // Activate misc attack
    public void pressA()
    {
        inputStartTime = currentInputTimer;
        anim.SetTrigger("miscAttack");
        attackType = "misc";
        StartCoroutine("launchAttack");
        currentHitNumber += 1;
    }
    // Check the input queue for what attack to use
    public void checkQueue()
    {
        string input = "";
        if (inputQueue[0] != "")
        {
            input = inputQueue[0];
            inputQueue[0]= "";
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
    // Change outfit function takes in the new outfit 
    public void changeOutfit(outfit newOutfit)
    {
        // 
        newOutfit.outfitSkinRenderer.sharedMesh = newOutfit.outfitMesh;
        newOutfit.outfitSkinRenderer.material = newOutfit.outfitMaterial;
        // Create new runtime animator override controller
        AnimatorOverrideController aoc = new AnimatorOverrideController(anim.runtimeAnimatorController);
        // Create a list of current animations and their replacements
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        int index = 0;
        // For each animation in the current animation tree
        foreach (var a in aoc.animationClips)
            // If an animation name contains the outfitType(must be the word punch, kick, and misc)
            if (a.name.Contains(newOutfit.outfitType))
            {
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, newOutfit.attacks[index]));
                index += 1;
            }
        // Override all animations in the anims list
        aoc.ApplyOverrides(anims);
        anim.runtimeAnimatorController = aoc;
        // Change the hitboxes for player attacks
        punchHitboxes = newOutfit.attackColliders;
    }
}
