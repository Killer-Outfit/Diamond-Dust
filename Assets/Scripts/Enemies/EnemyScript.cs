using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    // References
    private Animator anim;
    private Transform closestEnemy;
    private Transform frontTargeter;
    private Transform sideTargeter;
    public GameObject player;
    public NavMeshAgent agent;
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
    public string state;
    public bool isAttackReady;
    private bool isFar;
    private bool isMiddle;
    private bool isClose;
    public bool canBlock;
    public bool canDodge;
    // Other
    public float attackTimer;
    private float randomWander;
    private float wanderTimer;
    private float spacingTimer;
    private Vector3 curPos;
    public Collider[] attackHitboxes;
    public int managerIndex;

    void Start()
    {
        isFar = false;
        isMiddle = false;
        isClose = false;
        canBlock = false;
        canDodge = false;
        isAttackReady = false;
        state = "inactive";
        spacingTimer = 0f;
        curPos = Vector3.zero;
        player = GameObject.FindWithTag("Player");
        maxhealth = health;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        frontTargeter = transform.Find("FrontTargeter");
        sideTargeter = transform.Find("SideTargeter");
        attackTimer = 1f;
        wanderTimer = Random.Range(2f, 4f);
        randomWander = Random.Range(-20f, 20f);
    }
    
    void Update()
    {
        // While 'inactive' (out of range), just keep checking distance to the player.
        if (state == "inactive")
        {
            if (player && GetDistance() < 80)
            {
                enemyManager.GetComponent<GlobalEnemy>().AddEnemy(this.gameObject);
                state = "neutral";
            }
        }
        // Enemy is active, do the following.
        else
        {
            if (state == "neutral" || state == "approaching" || state == "dodgestart" || state == "dodging" || state == "dodgeend") // Enemy is in neutral stance or simply running at the player. Not attacking or blocking.
            {
                agent.destination = player.transform.position;
                TrackPlayer();
                if (GetDistance() < 10 && canBlock && player.gameObject.GetComponent<Player>().isAttacking)
                {
                    SendMessage("Block");
                }
                if (GetDistance() < 10 && canDodge && player.gameObject.GetComponent<Player>().isAttacking)
                {
                    SendMessage("Dodge");
                }
            }

            if (state == "approaching" && GetDistance() < 5)
            {
                StartCoroutine("Attack1");
            }
            GetMoveState(); // Tells the enemy how far it is from the player.
            DoMovement(); // Does movement based on distance and state.

            // Attack readiness counter for the enemy manager.
            if (!isAttackReady)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    isAttackReady = true;
                    enemyManager.GetComponent<GlobalEnemy>().EnemyReady(managerIndex);
                }
            }
        }

        // Random 'strafing' that gets added to certain movements
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            wanderTimer = Random.Range(2f, 4f);
            randomWander = Random.Range(-20f, 20f);
        }

        // Spacing if another enemy gets too close
        if (spacingTimer > 0)
        {
            spacingTimer -= Time.deltaTime;
        }
        else
        {
            closestEnemy = GetClosestEnemy();
            if (Vector3.Distance(agent.transform.position, closestEnemy.position) < 8)
            {
                if (GetFlatAngle(closestEnemy.position) > 0)
                {
                    randomWander = Mathf.Abs(randomWander);
                }
                else
                {
                    randomWander = -Mathf.Abs(randomWander);
                }
            }
            spacingTimer = Random.Range(1f,2f);
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

    private Transform GetClosestEnemy()
    {
        Transform enemy;
        float closestDist = 100;
        List<GameObject> relevantEnemies = enemyManager.GetComponent<GlobalEnemy>().battleRing;
        if (relevantEnemies.Count > 1)
        {
            enemy = this.transform;
            for (int i = 0; i < relevantEnemies.Count; i++)
            {
                float distToThisEnemy = Vector3.Distance(agent.transform.position, relevantEnemies[i].transform.position);
                if (distToThisEnemy < closestDist && distToThisEnemy >= 0.2f)
                {
                    closestDist = Vector3.Distance(agent.transform.position, relevantEnemies[i].transform.position);
                    enemy = relevantEnemies[i].transform;
                }
            }
        }
        else
        {
            enemy = this.transform;
        }
        return enemy;
    }

    private void DoMovement()
    {
        if (state == "neutral")
        {
            if (isFar)
            {
                anim.SetBool("isIdle", false);
                agent.speed = speed;
            }
            else if (isMiddle)
            {
                // Slowly move towards the middle of the center ring
                float mod = 2f;
                if (GetDistance() < 15)
                {
                    mod = mod * -1;
                }

                anim.SetBool("isIdle", true);
                agent.speed = speed * 0.0f;
                agent.Move(((player.transform.position - transform.position).normalized * mod * Time.deltaTime));
                agent.Move((sideTargeter.position - transform.position).normalized * randomWander * Time.deltaTime);
            }
            else if (isClose)
            {
                anim.SetBool("isIdle", false);
                agent.speed = 0.0f;
                agent.Move((player.transform.position - transform.position).normalized * -5f * Time.deltaTime);
                agent.Move(((sideTargeter.position - transform.position).normalized * randomWander) * Time.deltaTime / 2);
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
            agent.Move((player.transform.position - transform.position).normalized * -.02f);
        }
        else if (state == "blockstagger")
        {
            agent.speed = 0.0f;
            agent.Move((player.transform.position - transform.position).normalized * -.02f);
        }
    }

    private void TrackPlayer() // Smooth turn towards the player using turnSpeed, and set destination.
    {
        if (Mathf.Abs(GetFlatAngle(player.transform.position)) > 5f)
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
            for (float i = 0f; i < 0.3f; i += Time.deltaTime)
            {
                agent.Move((frontTargeter.position - transform.position).normalized * -15f * Time.deltaTime);
                yield return null;
            }
            // Reset attack readiness as soon as the hitbox appears rather than the end of the attack sequence.
            isAttackReady = false;
            attackTimer = Random.Range(5f, 10f);
            for (float i = 0f; i < 0.2f; i += Time.deltaTime)
            {
                agent.Move((frontTargeter.position - transform.position).normalized * -20f * Time.deltaTime);
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
            for (float i = 0f; i < 0.3f; i += Time.deltaTime)
            {
                agent.Move((frontTargeter.position - transform.position).normalized * -15f * Time.deltaTime);
                yield return null;
            }
            state = "neutral";
            yield return null;
        }
    }

    // Returns distance between the enemy and the player
    public float GetDistance()
    {
        return Vector3.Distance(agent.transform.position, player.transform.position);
    }

    // Returns the angle between the enemy and a coordinate, without the Y axis
    public float GetFlatAngle(Vector3 coord)
    {
        Vector3 fvec = transform.forward;
        Vector3 pvec = transform.position - coord;
        return Vector3.SignedAngle(new Vector3(fvec.x, 0, fvec.z), new Vector3(pvec.x, 0 ,pvec.z), Vector3.up);
    }

    public void DecreaseHealth(int damage)
    {
        if (state == "blocking" || state == "blockstagger") // If blocking, take no damage but get pushed back a little.
        {
            Debug.Log("blocked idiot");
            StartCoroutine("BlockStagger");
        }
        else if (state == "dodging") // If dodge frames are active, take no damage.
        {
            Debug.Log("dodged idiot");
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
        yield return new WaitForSeconds(0.5f);
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
        enemyManager.GetComponent<GlobalEnemy>().RemoveEnemy(managerIndex);
        Destroy(this.gameObject);
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
