using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public Collider laser;
    public Collider laserSpawn;
    public Collider NONONO;
    public GameObject gameManager;
    CharacterController controller;
    public CheckpointManager checkpoint;
    // Set health variables
    public float maxHealth;
    public float currentHealth;
    // State bools
    public string state;
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
    private bool outfit2;

    // Create an animator variable and animation overrider for outfit switching
    Animator anim;
    AnimatorOverrideController animatorOverrideController;
    
    // Initialize animator, current health and healthbar value
    void Start()
    {
        outfit2 = false;
        state = "idle";
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
        if (!gameManager.GetComponent<PauseScript>().checkPause())
        {
            // Buffer inputs if the player is not blocking, continue block otherwise.
            if (state != "blocking")
            {
                if (Input.GetButtonDown("XButton") || Input.GetMouseButtonDown(0))
                {
                    //Instantiate(laser, transform.position, transform.rotation);
                    inputQueue[0] = "punch";
                }
                else if (Input.GetButtonDown("YButton") || Input.GetMouseButtonDown(1))
                {
                    inputQueue[0] = "kick";
                }
                else if (Input.GetButtonDown("AButton") || Input.GetKeyDown(KeyCode.Space))
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

            // When no actions are being performed, tell the movement script and check the queue.
            if (state == "idle" || state == "run")
            {
                gameObject.GetComponent<PlayerMove>().changeAttacking(false);
                CheckQueue();
            }
            if(state == "attacking")
            {

                GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
                cam.GetComponent<FollowCamera>().isAttacking = true;
            }else
            {
                GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
                cam.GetComponent<FollowCamera>().isAttacking = false;
            }
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
            currentHitNumber = 0;
            if (state == "attacking")
            {
                gameObject.GetComponent<PlayerMove>().changeAttacking(false);
                state = "idle";
                anim.SetTrigger("backtoIdle");
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
        //Destroy(this.gameObject);
        currentHealth = maxHealth;
		healthbar.value = currentHealth / maxHealth;
        //transform.position = checkpoint.getCheckpoint();
        //canvas.SendMessage("PlayerDead", true);
    }

    // Make the attack activate
    IEnumerator launchAttack()
    {
        int happened = 0;
        float startTime = 0f;
        bool hit;
        outfit currentOutfitItem = null;
        // Set the collider being used based on current attack type
        Collider attack = null;
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

        // Go through each phase of the attack based on the outfit attack stats
        for (int i = 0; i < currentOutfitItem.GetPhases(currentHitNumber); i++)
        {
            startTime += Time.deltaTime;
            // Reset hit counter and set speed
            hit = false;
            GetComponent<PlayerMove>().movementSpeed = currentOutfitItem.GetPhaseMove(currentHitNumber, i);
            GetComponent<PlayerMove>().collideMaxSpeed = currentOutfitItem.GetPhaseMove(currentHitNumber, i);
            GetComponent<PlayerMove>().turningSpeed = currentOutfitItem.GetPhaseTurnSpeed(currentHitNumber, i);

            // Go through this phase's timer
            for (float j = 0; j < currentOutfitItem.GetPhaseTime(currentHitNumber, i); j += Time.deltaTime)
            {
                // Apply acceleration
                GetComponent<PlayerMove>().movementSpeed += currentOutfitItem.GetPhaseAcc(currentHitNumber, i);

                // if this phase is an active hitbox and hasn't hit an enemy yet, try to hit an enemy
                if (currentOutfitItem.GetPhaseActive(currentHitNumber, i) && hit == false)
                {
                    Collider[] cols = Physics.OverlapBox(attack.bounds.center, attack.bounds.extents, attack.transform.rotation, LayerMask.GetMask("Hitbox"));
                    foreach (Collider c in cols)
                    {
                        if (c.tag == "Enemy")
                        {
                            // Decrease the hit target's health based on the attack's damage
                            c.SendMessageUpwards("DecreaseHealth", currentOutfitItem.attackDamage[currentHitNumber]);
                            hit = true;
                        }
                    }
                }
                yield return null;
                if (attackType == "misc" && outfit2)
                {
                    if (currentHitNumber == 0)
                    {
                        Vector3 spawnPos = transform.position;
                        spawnPos.y += 2;
                        Instantiate(laser, spawnPos, transform.rotation);
                        //StartCoroutine("launchStraightLaser");
                    }else if (currentHitNumber == 2)
                    {
                        Vector3 spawnPos = transform.position;
                        spawnPos = spawnPos + transform.forward * -5;
                        if ((int)startTime % 10 == 0)
                        {

                            Instantiate(NONONO, spawnPos, transform.rotation);
                        }
                    }
                    else if (currentHitNumber == 3)
                    {
                        happened++;
                        if(happened == 1)
                        {
                            
                            Vector3 spawnPos = transform.position;
                            spawnPos.y += 20;
                            spawnPos = spawnPos + transform.forward * -5;
                            Instantiate(laserSpawn, spawnPos, transform.rotation);
                        }
                    }
                }
            }
        }
        
        GetComponent<PlayerMove>().DefaultTurn();
        GetComponent<PlayerMove>().DefaultSpeed();
        currentHitNumber++;
        if (currentHitNumber == 4)
        {
            currentHitNumber = 0;
        }
        CheckQueue();
    }

    // Activate punch
    public void pressX()
    {
        Debug.Log("pressed x");
        anim.SetTrigger("punch");
        attackType = "punch";
        state = "attacking";
        gameObject.GetComponent<PlayerMove>().changeAttacking(true);
        StartCoroutine("launchAttack");
    }

    // Activate kick
    public void pressY()
    {
        anim.SetTrigger("kick");
        attackType = "kick";
        state = "attacking";
        gameObject.GetComponent<PlayerMove>().changeAttacking(true);
        StartCoroutine("launchAttack");  
    }

    // Activate misc attack
    public void pressA()
    {
        anim.SetTrigger("miscAttack");
        attackType = "misc";
        state = "attacking";
        gameObject.GetComponent<PlayerMove>().changeAttacking(true);
        StartCoroutine("launchAttack");
   
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
            if (outfit2)
            {
                outfit2 = false;
            }else
            {
                outfit2 = true;
            }
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
            if (a.name.Contains(newOutfit.attackType))
            {
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, newOutfit.attacks[index]));
                index += 1;
            }
        // Override all animations in the anims list
        aoc.ApplyOverrides(anims);
        anim.runtimeAnimatorController = aoc;
    }

    IEnumerable launchStraightLaser()
    {
        for(float i = 0; i < 10; i += Time.deltaTime)
        {
            Instantiate(laser, transform.position, transform.rotation);
            yield return null;
        }
        
    }
}
