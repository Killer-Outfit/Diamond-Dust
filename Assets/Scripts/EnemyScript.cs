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
    public string state = "inactive";
    private bool attackReady = false;
    private bool isAttacking = false;
    private bool isStaggered = false;
    private bool isApproaching = false;
    private bool isFar = false;
    private bool isMiddle = false;
    private bool isClose = false;
    private bool isSpreading = false;
    public bool canBlock = false;
    public bool isBlocking = false;
    // Other
    private Vector3 curPos = Vector3.zero;
    public Collider[] attackHitboxes;
    private float attack1Timer;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        maxhealth = health;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        attack1Timer = Random.Range(5f, 10f);
    }
    
    void Update()
    {
        // While 'inactive' (out of range), just keep checking distance to the player.
        if (state == "inactive")
        {
            if (player && GetDistance() < 80)
            {
                state = "neutral";
                //enemyManager.GetComponent<GlobalEnemy>().AddEnemy();
            }
        }
        // Enemy is active, do the following.
        else
        {
            GetMoveState(); // Tells the enemy how far it is from the player.
            DoMovement(); // Does movement based on distance and state.

            if (state == "neutral" || state == "approaching") // Enemy is in neutral stance or simply running at the player. Not attacking or blocking.
            {
                TrackPlayer();
                if (GetDistance() < 5 && canBlock && player.gameObject.GetComponent<Player>().isAttacking)
                {
                    SendMessage("Block");
                }
            }
            //if (!isAttacking)
            //{
            //    if (!isBlocking) // Don't attack while blocking.
            //    {
            //        attack1Timer -= Time.deltaTime;

            //        if (attack1Timer <= 0 && !isApproaching)
            //        {
            //            isApproaching = true;
            //        }

            //        if (isApproaching) // Running in for an attack
            //        {
            //            if (Vector3.Distance(agent.transform.position, player.transform.position) < 5)
            //            {
            //                isApproaching = false;
            //                StartCoroutine("Attack1");
            //            }
            //        }
            //    }

            //    // Try to block if the player is close enough and attacking, and if the enemy can block. Sets the block script's timer to 1, so it refreshes if the player attacks repeatedly.
            //    if (Vector3.Distance(agent.transform.position, player.transform.position) < 5 && canBlock && player.gameObject.GetComponent<Player>().isAttacking)
            //    {
            //        SendMessage("Block");
            //    }
            //}

            //if (!InterruptingMovement()) // Movement and tracking scripts. Overridden by enemy actions.
            //{
            //    TrackPlayer();
            //    GetMoveState();
            //    DoMovement();
            //}
        }
        // Attack readiness counter for the enemy manager.
        if (!attackReady)
        {
            attack1Timer -= Time.deltaTime;
            if (attack1Timer <= 0)
            {
                attackReady = true;
            }
        }
        // HP bar billboarding
        HPBar.LookAt(Camera.main.transform.position, -Vector3.up);
    }

    private void GetMoveState() // Gets distance state in relation to the player
    {
        float distance = GetDistance();
        isFar = (distance > followDistanceUpper);
        isMiddle = (!isFar && distance > followDistanceLower);
        isClose = (!isMiddle && distance < followDistanceLower);
    }

    private void DoMovement()
    {
        agent.destination = player.transform.position;
        if (state == "neutral")
        {
            if (isFar)
            {
                anim.SetBool("isIdle", false);
                agent.speed = speed;
            }
            else if (isMiddle)
            {
                float mod = .01f;
                anim.SetBool("isIdle", true);
                agent.speed = speed * 0.0f;
                if (GetDistance() < 15) { mod = mod * -1; }
                agent.Move(((player.transform.position - transform.position).normalized * mod));
            }
            else if (isClose)
            {
                anim.SetBool("isIdle", false);
                agent.speed = 0.0f;
                agent.Move((player.transform.position - transform.position).normalized * -.07f);
            }
        }
        else if (state == "approaching")
        {
            agent.speed = speed;
            anim.SetBool("isIdle", false);
        }
        else if (state == "stagger")
        {
            agent.speed = 0.0f;
            agent.Move((player.transform.position - transform.position).normalized * -.03f);
        }
        else if (state == "blockstagger")
        {
            agent.speed = 0.0f;
            agent.Move((player.transform.position - transform.position).normalized * -.02f);
        }
    }

    private void TrackPlayer() // Smooth turn towards the player using turnSpeed, and set destination.
    {
        if (GetFlatAngle() > 5f)
        {
            Vector3 targetDir = -1 * (player.transform.position - transform.position);
            float step = turnSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    IEnumerator Attack1()
    {
        anim.SetTrigger("Punch");
        state = "attacking";
        // overlapSphere is best if applicable
        Collider attack = attackHitboxes[0];
        Collider[] cols = Physics.OverlapBox(attack.bounds.center, attack.bounds.extents, attack.transform.rotation, LayerMask.GetMask("Hitbox"));
        bool hitPlayer = false;

        while(state == "attacking")
        {
            agent.speed = speed * 1.5f;
            yield return new WaitForSeconds(0.3f);
            // Reset attack readiness as soon as the hitbox appears rather than the end of the attack sequence.
            attackReady = false;
            attack1Timer = Random.Range(5f, 10f);
            agent.speed = speed * 0.5f;
            for (float i = 0f; i < 0.2f; i += Time.deltaTime)
            {
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
                yield return null;
            }
            agent.speed = speed * 0.25f;
            yield return new WaitForSeconds(0.3f);
            state = "neutral";
            yield return null;
        }
    }

    // Returns distance between the enemy and the player
    public float GetDistance()
    {
        return Vector3.Distance(agent.transform.position, player.transform.position);
    }

    // Returns the angle between the enemy and the player, without the Y axis
    public float GetFlatAngle()
    {
        Vector3 fvec = transform.forward;
        Vector3 pvec = transform.position - player.transform.position;
        return Vector3.Angle(new Vector3(fvec.x, 0, fvec.z), new Vector3(pvec.x, 0 ,pvec.z));
    }

    public void DecreaseHealth(int damage)
    {
        if (state == "blocking" || state == "blockstagger") //Take no damage if blocking.
        {
            Debug.Log("get blocked idiot");
            StartCoroutine("BlockStagger");
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
        state = "stagger";
        anim.SetTrigger("Stagger");
        yield return new WaitForSeconds(0.4f);
        state = "neutral";
    }

    IEnumerator BlockStagger()
    {
        state = "blockstagger";
        transform.LookAt(2 * transform.position - player.transform.position);
        yield return new WaitForSeconds(0.1f);
        state = "blocking";
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
