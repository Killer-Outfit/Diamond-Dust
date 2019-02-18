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

    private string attackType;
    private string[] punchAnims;
    private string[] kickAnims;
    private string[] miscAnims;

    private List<string> inputQueue;
    private List<string> animQueueStateNames;

    private GameObject enemyHit;
    // Create an animator variable
    Animator anim;
    AnimatorOverrideController animatorOverrideController;
    
    // Reference to the health bar
    public Slider healthbar;

    public outfit outfit1;
    public outfit outfit2;


    private float currentInputTimer;
    private float inputStartTime;
    private float shield;
    private int currentHitNumber;
    private bool blocking;

    // Initialize animator, current health and healthbar value
    void Start()
    {
        //transform.localScale = new Vector3(0.35F, 0.35f, 0.35f);
        attackType = "";
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


        //animatorOverrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        //anim.runtimeAnimatorController = animatorOverrideController;
        //var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

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
            /*if (Input.GetButtonDown("YButton"))
            {
                if (inputQueue.Count < 3)
                {
                    inputQueue.Add("kick");
                }
            }*/

            if (Input.GetButtonDown("AButton"))
            {
                if (inputQueue.Count < 3)
                {
                    inputQueue.Add("misc");
                }
            }


            if (Input.GetButton("BButton"))
            {
                //Debug.Log("pressing B");
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
    IEnumerator launchAttack()
    {
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
        yield return new WaitForSeconds(0.2f); // Do hitbox calcuation after 0.2 seconds. Adjust this to match the animation later?
        //overlapSphere is best if applicable
        // Create a list of all objects that have collided with the attack hitbox
        Collider[] cols = Physics.OverlapBox(attack.bounds.center, attack.bounds.extents, attack.transform.rotation, LayerMask.GetMask("Hitbox"));
        // Iterate through each collision event
        foreach(Collider c in cols)
        {
            Debug.Log(c.name);
            if (c.tag == "Enemy")
            {
                Debug.Log("hit the " + c.name);
                // Decrease the hit target's health by 10
                c.SendMessageUpwards("DecreaseHealth", 10);
            }
        }
        yield return new WaitForSeconds(0.2f); //"Cooldown" time
        this.isAttacking = false;
        gameObject.GetComponent<PlayerMove>().changeAttacking(isAttacking);
    }
    public void pressX()
    {
        inputStartTime = currentInputTimer;
        anim.SetTrigger("punch");
        //launchAttack(punchHitboxes[currentHitNumber]);
        attackType = "punch";
        StartCoroutine("launchAttack");
        currentHitNumber += 1;
    }

    public void pressY()
    {
        //Debug.Log("I am pressing Y");
        inputStartTime = currentInputTimer;
        anim.SetTrigger("kick");
        //launchAttack(kickHitBoxes[currentHitNumber]);
        attackType = "kick";
        StartCoroutine("launchAttack");
        currentHitNumber += 1;
    }
    public void pressA()
    {
        //Debug.Log("I am pressing A");
        inputStartTime = currentInputTimer;
        anim.SetTrigger("miscAttack");
        //launchAttack(miscHitBoxes[currentHitNumber]);
        attackType = "misc";
        StartCoroutine("launchAttack");
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

    public void changeOutfit(outfit newOutfit)
    {
        newOutfit.outfitSkinRenderer.sharedMesh = newOutfit.outfitMesh;
        newOutfit.outfitSkinRenderer.material = newOutfit.outfitMaterial;

        AnimatorOverrideController aoc = new AnimatorOverrideController(anim.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        int index = 0;
        foreach (var a in aoc.animationClips)
            if (a.name.Contains(newOutfit.outfitType))
            {
                Debug.Log(a);
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, newOutfit.attacks[index]));
                index += 1;
            }
        aoc.ApplyOverrides(anims);
        anim.runtimeAnimatorController = aoc;
        punchHitboxes = newOutfit.attackColliders;
    }
}
