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
    public bool isAttacking = false;
    // Create a list of attack hitboxes
    public outfit top;
    public outfit misc;
    public outfit bot;
    public GameObject canvas;
    // Reference to the health bar
    public Slider healthbar;

    private string attackType;
    // Create single element input queue
    private string[] inputQueue;
    // Queue check 
    private bool isBlocking;
    private float shield;
    private int currentHitNumber;

    // Create an animator variable and animation overrider for outfit switching
    Animator anim;
    AnimatorOverrideController animatorOverrideController;
    
    // Initialize animator, current health and healthbar value
    void Start()
    {
        controller = GetComponent<CharacterController>();
        checkpoint.updateCheckpoint(transform.position);
        //transform.localScale = new Vector3(0.35F, 0.35f, 0.35f);
        attackType = "";
        shield = 100;
        isBlocking = false;
        inputQueue = new string[1] { "" };
        currentHitNumber = 0;
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthbar.value = currentHealth / maxHealth;
    }
    // Get user inputs
    void Update()
    {
        Debug.Log(transform.right);
        if (Input.GetButtonDown("XButton"))
        {
            inputQueue[0] = "punch";
        }
        // Activate kick when user presses Y
        if (Input.GetButtonDown("YButton"))
        {
            /*
            if (inputQueue.Count < 3)
            {
                inputQueue.Add("kick");
            }*/
            killPlayer();
        }
        // Add misc attack to input queue
        if (Input.GetButtonDown("AButton"))
        {
            inputQueue[0] = "misc";
        }
        // Don't call attacks if the player is mid-attack already.
        if (!isAttacking) 
        {

         
            // Add punch to input queue
            
            // Block initiation
            if (Input.GetButton("BButton"))
            {
                if (!isBlocking)
                {
                    anim.SetTrigger("block");
                    isBlocking = true;
                    // Disable player motion when isBlocking
                    gameObject.GetComponent<PlayerMove>().changeBlock();
                }
            }
            // Enable player motion after isBlocking is complete  
            else if (isBlocking)
            {
                isBlocking = false;
                gameObject.GetComponent<PlayerMove>().changeBlock();
                anim.SetTrigger("block");
            }

            // Resets the hit number when the plaer has reached a max of 4 hits or when 6 seconds has past without input
            if (currentHitNumber == 4)
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
        outfit currentOutfitItem = null;
        // Set the collider beig used based on current attack type
        Collider attack = null;
        int timeListIncrement = 0;
        if(attackType == "punch")
        {
            currentOutfitItem = top;
        }
        else if (attackType == "kick")
        {
            currentOutfitItem = bot;
        }
        else if (attackType == "misc")
        {
            currentOutfitItem = misc;
        }
        attack = currentOutfitItem.attackColliders[currentHitNumber];
        isAttacking = true;
        gameObject.GetComponent<PlayerMove>().changeAttacking(isAttacking);
        // Do hitbox calcuation after 0.2 seconds. ADJUST THIS TO MATCH ANIMATION TIME LATER?
        yield return new WaitForSeconds(currentOutfitItem.getTimeInterval(currentHitNumber, timeListIncrement));
        timeListIncrement++;
        //overlapSphere is best if applicable
        for (float i = 0f; i < currentOutfitItem.getTimeInterval(currentHitNumber, timeListIncrement); i += Time.deltaTime)
        {
            // Create a list of all objects that have collided with the attack hitbox
            Collider[] cols = Physics.OverlapBox(attack.bounds.center, attack.bounds.extents, attack.transform.rotation, LayerMask.GetMask("Hitbox"));
            // Iterate through each collision eventc
            foreach (Collider c in cols)
            {
                // If you collided with an enemy  them
                if (c.tag == "Enemy")
                {
                    // Decrease the hit target's health by 10 CHANGE TO ATTACK DAMAGE
                    c.SendMessageUpwards("DecreaseHealth", currentOutfitItem.attackDamage[currentHitNumber]);
                }
            }
            yield return null;
        }
        timeListIncrement++;
        // "Cooldown" time
        Debug.Log(currentOutfitItem.getTimeInterval(currentHitNumber, timeListIncrement));
        yield return new WaitForSeconds(currentOutfitItem.getTimeInterval(currentHitNumber, timeListIncrement)); 
        Debug.Log("checkQueue");
        checkQueue();
        isAttacking = false;
        gameObject.GetComponent<PlayerMove>().changeAttacking(isAttacking);
    }
    // Activate punch
    public void pressX()
    {
        Debug.Log("pressed x");
        anim.SetTrigger("punch");
        attackType = "punch";
        StartCoroutine("launchAttack");
        currentHitNumber += 1;
    }
    // Activate kick
    public void pressY()
    {
        anim.SetTrigger("kick");
        attackType = "kick";
        StartCoroutine("launchAttack");
        currentHitNumber += 1;
    }
    // Activate misc attack
    public void pressA()
    {
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
        }else
        {
            Debug.Log("here");
            anim.SetTrigger("idle");
            //trigger idle
        }
    }
    // Change outfit function takes in the new outfit 
    public void changeOutfit(outfit newOutfit)
    {
        if(newOutfit.outfitType == "Top")
        {
            top = newOutfit;
        }else if (newOutfit.outfitType == "Misc")
        {
            misc = newOutfit;
        }
        else if (newOutfit.outfitType == "Bot")
        {
            bot = newOutfit;
        }
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
    }
}
