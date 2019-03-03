﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    CharacterController controller;
    public CheckpointManager checkpoint;
    // Set health variables
    public float maxHealth;
    public float currentHealth;
    // State bools
    public string state;
    public bool isAttacking = false;
    // Create a list of attack hitboxes
    public Collider[] punchHitboxes;
    public Collider[] kickHitBoxes;
    public Collider[] miscHitBoxes;
    public GameObject canvas;
    // Reference to the health bar
    public Slider healthbar;

    private string attackType;
    // Create single element input queue
    private string[] inputQueue;
    // Queue check 
    private List<string> animQueueStateNames;
    private bool isBlocking;
    private float currentInputTimer;
    private float inputStartTime;
    private float shield;
    private int currentHitNumber;
    private float[,] punchTimes;

    // Create an animator variable and animation overrider for outfit switching
    Animator anim;
    AnimatorOverrideController animatorOverrideController;
    
    // Initialize animator, current health and healthbar value
    void Start()
    {
        state = "idle";
        punchTimes = new float[,]
        {
            {.2f, .2f, 2f},
            {.3f, .1f, 2f},
            {.2f, .4f, 2f},
            {.1f, .3f, 2f}
        };
        controller = GetComponent<CharacterController>();
        checkpoint.updateCheckpoint(transform.position);
        //transform.localScale = new Vector3(0.35F, 0.35f, 0.35f);
        attackType = "";
        shield = 100;
        isBlocking = false;
        inputQueue = new string[1] { "" };
        animQueueStateNames = new List<string>() { "checkQueueState1", "checkQueueState2", "checkQueueState3" };
        currentHitNumber = 0;
        currentInputTimer = 0;
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthbar.value = currentHealth / maxHealth;

    }
    // Get user inputs
    void Update()
    {
        // Buffer inputs if the player is not blocking, continue block otherwise.
        if (state != "blocking")
        {
            if (Input.GetButtonDown("XButton"))
            {
                inputQueue[0] = "punch";
            }
            else if (Input.GetButtonDown("YButton"))
            {
                inputQueue[0] = "kick";
            }
            else if (Input.GetButtonDown("AButton"))
            {
                inputQueue[0] = "misc";
            }
            else if (Input.GetButton("BButton"))
            {
                inputQueue[0] = "block";
            }
        }
        else // Check if block has ended
        {
            if (Input.GetButton("BButton") == false)
            {
                state = "idle";
                gameObject.GetComponent<PlayerMove>().changeBlock();
                anim.SetBool("block", false);
            }
        }

        // Combo counter resets if 4 moves in a row have been done.
        if (currentHitNumber == 4)
        {
            currentHitNumber = 0;
        }

        // When no actions are being performed, tell the movement script and check the queue.
        if (state == "idle" || state == "run")
        {
            gameObject.GetComponent<PlayerMove>().changeAttacking(false);
            CheckQueue();
        }
    }

    // Check the input queue for what attack to use
    public void CheckQueue()
    {
        string input = "";
        if (inputQueue[0] != "")
        {
            input = inputQueue[0];
            inputQueue[0] = "";
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
            else if (input == "block")
            {
                gameObject.GetComponent<PlayerMove>().changeAttacking(false);
                if (state == "attacking")
                {
                    anim.SetTrigger("backtoIdle");
                }
                anim.SetBool("block", true);
                state = "blocking";
                // Disable player motion when isBlocking
                gameObject.GetComponent<PlayerMove>().changeBlock();
            }
        }
        else // No inputs at the moment
        {
            if (state == "attacking")
            {
                gameObject.GetComponent<PlayerMove>().changeAttacking(false);
                state = "idle";
                anim.SetTrigger("backtoIdle");
            }
            Debug.Log("here");
        }
    }

    // Decrease the current health and update health bar
    public void DecreaseHealth(float damage)
    {
        if (!isBlocking)
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
            decreaseShield(damage);
        }
    }

    // Shield damage if isBlocking
    public void decreaseShield(float damage)
    {
        shield -= damage;
    }

    // Kill the player
    private void killPlayer()
    {
        controller.enabled = false;
        controller.transform.position = checkpoint.getCheckpoint();
        controller.enabled = true;
        Debug.Log(checkpoint.getCheckpoint());
        //Destroy(this.gameObject);
        currentHealth = maxHealth;
		healthbar.value = currentHealth / maxHealth;
        //transform.position = checkpoint.getCheckpoint();
        //canvas.SendMessage("PlayerDead", true);
    }

    // Make the attack activate
    IEnumerator launchAttack()
    {
        // Set the collider being used based on current attack type
        bool hit;
        Collider attack = null;
        int timeListIncrement = 0;
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

        // Start lag
        yield return new WaitForSeconds(punchTimes[currentHitNumber, timeListIncrement]);
        timeListIncrement++;

        // Do hitbox calcuation after 0.2 seconds. ADJUST THIS TO MATCH ANIMATION TIME LATER?
        hit = false;
        for (float i = 0f; i < punchTimes[currentHitNumber, timeListIncrement]; i += Time.deltaTime)
        {
            // Create a list of all objects that have collided with the attack hitbox
            Collider[] cols = Physics.OverlapBox(attack.bounds.center, attack.bounds.extents, attack.transform.rotation, LayerMask.GetMask("Hitbox"));
            // Iterate through each collision event if a hit hasn't been landed yet
            if (hit == false)
            {
                foreach (Collider c in cols)
                {
                    // If you collided with an enemy  them
                    if (c.tag == "Enemy")
                    {
                        // Decrease the hit target's health by 10 CHANGE TO ATTACK DAMAGE
                        c.SendMessageUpwards("DecreaseHealth", 10);
                        hit = true;
                    }
                }
            }
            yield return null;
        }
        timeListIncrement++;

        // End lag
        Debug.Log(punchTimes[currentHitNumber, timeListIncrement]);
        yield return new WaitForSeconds(punchTimes[currentHitNumber, timeListIncrement]); 
        // Check the queue for a buffered input. Sets state to "idle" if none are found.
        Debug.Log("checkQueue");
        CheckQueue();
    }

    // Activate punch
    public void pressX()
    {
        Debug.Log("pressed x");
        inputStartTime = currentInputTimer;
        anim.SetTrigger("punch");
        attackType = "punch";
        state = "attacking";
        gameObject.GetComponent<PlayerMove>().changeAttacking(true);
        StartCoroutine("launchAttack");
        currentHitNumber += 1;
    }

    // Activate kick
    public void pressY()
    {
        inputStartTime = currentInputTimer;
        anim.SetTrigger("kick");
        attackType = "kick";
        state = "attacking";
        gameObject.GetComponent<PlayerMove>().changeAttacking(true);
        StartCoroutine("launchAttack");
        currentHitNumber += 1;
    }

    // Activate misc attack
    public void pressA()
    {
        inputStartTime = currentInputTimer;
        anim.SetTrigger("miscAttack");
        attackType = "misc";
        state = "attacking";
        gameObject.GetComponent<PlayerMove>().changeAttacking(true);
        StartCoroutine("launchAttack");
        currentHitNumber += 1;
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
