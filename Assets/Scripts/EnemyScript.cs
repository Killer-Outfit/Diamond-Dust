using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    // References
    private GameObject player;
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject closestEnemy;
    public Image HPBarSprite;
    public Transform HPBar;
    public GameObject enemyManager;
    // Stats
    private int maxhealth;
    public float speed;
    public float turnSpeed;
    public float followDistanceUpper;
    public float followDistanceLower;
    public int health;
    // Flags
    private bool canLaunchAttack = false;
    private bool isAttacking = false;
    private bool isStaggered = false;
    private bool isApproaching = false;
    private bool isFar = false;
    private bool isMiddle = false;
    private bool isClose = false;
    private bool isDanger = false;
    private bool isSpreading = false;
    public bool active = false;
    public bool canBlock = false;
    public bool isBlocking = false;
    // Other
    private Vector3 curPos = Vector3.zero;
    public Collider[] attackHitboxes;
    public float attack1Timer;
    private float attack1TimeReset;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        maxhealth = health;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        attack1TimeReset = attack1Timer;
    }
    
    void Update()
    {
        // While 'inactive' (out of range), just keep checking distance to the player.
        if (!active)
        {
            if (player && GetDistance() < 80)
            {
                active = true;
                //enemyManager.GetComponent<GlobalEnemy>().AddEnemy();
            }
        }
        // Enemy is active, do the following.
        else
        {
            if (!isAttacking)
            {
                if (!isBlocking) // Don't attack while blocking.
                {
                    attack1Timer -= Time.deltaTime;

                    if (attack1Timer <= 0 && !isApproaching)
                    {
                        isApproaching = true;
                    }

                    if (isApproaching) // Running in for an attack
                    {
                        if (Vector3.Distance(agent.transform.position, player.transform.position) < 5)
                        {
                            isApproaching = false;
                            StartCoroutine("Attack1");
                        }
                    }
                }

                // Try to block if the player is close enough and attacking, and if the enemy can block. Sets the block script's timer to 1, so it refreshes if the player attacks repeatedly.
                if (Vector3.Distance(agent.transform.position, player.transform.position) < 5 && canBlock && player.gameObject.GetComponent<Player>().isAttacking)
                {
                    SendMessage("Block");
                }
            }

            if (!InterruptingMovement()) // Movement and tracking scripts. Overridden by enemy actions.
            {
                TrackPlayer();
                GetState();
                DoMovement();
            }
        }
        // HP bar billboarding
        HPBar.LookAt(Camera.main.transform.position, -Vector3.up);
    }

    private void GetState() // Gets distance state in relation to the player
    {
        float distance = GetDistance();
        isFar = (GetDistance() > followDistanceUpper);
        isMiddle = (!isFar && GetDistance() > followDistanceLower);
        isClose = (!isMiddle && GetDistance() < followDistanceLower);
        isDanger = (isMiddle || isClose);
    }

    private void DoMovement()
    {
        agent.destination = player.transform.position;
        if (isFar)
        {
            anim.SetBool("isIdle", false);
            agent.speed = speed;
        }
        else if (isMiddle)
        {
            anim.SetBool("isIdle", true);
            agent.speed = 0.0f;
        }
        else if (isClose)
        {
            agent.speed = 0.0f;
            transform.Translate(Vector3.forward / 10);
            anim.SetBool("isIdle", false);
        }

        if (isApproaching) // Attack approach overrides other movements
        {
            agent.speed = speed;
            anim.SetBool("isIdle", false);
        }
    }

    private void TrackPlayer() // Smooth turn towards the player using turnSpeed, and set destination.
    {
        agent.destination = player.transform.position;
        Vector3 targetDir = -1 * (player.transform.position - transform.position);
        float step = turnSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    IEnumerator Attack1()
    {
        anim.SetTrigger("Punch");
        isAttacking = true;
        // overlapSphere is best if applicable
        Collider attack = attackHitboxes[0];
        Collider[] cols = Physics.OverlapBox(attack.bounds.center, attack.bounds.extents, attack.transform.rotation, LayerMask.GetMask("Hitbox"));
        bool hitPlayer = false;
        float timer = 0f;

        while(isAttacking)
        {
            timer += Time.deltaTime;
            if (timer > 0.0f && timer < 0.3f)
            {
                agent.speed = speed * 1.5f;
            }
            if(timer > 0.3f && timer < 0.5f)
            {
                agent.speed = speed * 0.5f;
                if (!hitPlayer)
                {
                    foreach (Collider c in cols)
                    {
                        if (c.tag == "Player")
                        {
                            hitPlayer = true;
                            c.SendMessageUpwards("DecreaseHealth", 10);
                        }
                    }
                }
            }
            if (timer > 0.5f && timer < 0.8f)
            {
                agent.speed = speed * 0.25f;
            }
            if (timer >= 0.8f)
            {
                isAttacking = false;
                attack1Timer = attack1TimeReset;
            }
            yield return null;
        }
    }

    public float GetDistance()
    {
        return Vector3.Distance(agent.transform.position, player.transform.position);
    }

    public void DecreaseHealth(int damage)
    {
        if (isBlocking) //Take no damage if blocking.
        {
            Debug.Log("get blocked idiot");
        }
        else
        {
            health -= damage;
            HPBarSprite.fillAmount = (float)health / (float)maxhealth;
            if (health <= 0)
            {
                Die();
            }
            StartCoroutine("Stagger");
        }
    }

    IEnumerator Stagger()
    {
        isStaggered = true;
        anim.SetTrigger("Stagger");
        float timer = 0;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector3.forward / 5);
            yield return null;
        }
        isStaggered = false;
    }

    private void Die()
    {
        //enemyManager.GetComponent<GlobalEnemy>().RemoveEnemy();
        Destroy(this.gameObject);
    }

    private bool InterruptingMovement()
    {
        return (isAttacking || isBlocking || isStaggered);
    }

    protected void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
         if (collision.gameObject.tag == "Player")
         {
             GetComponent<Rigidbody>().isKinematic = true;
             GetComponent<Rigidbody>().velocity = Vector3.zero;
         }
            
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            GetComponent<Rigidbody>().isKinematic = false;
    }
}
