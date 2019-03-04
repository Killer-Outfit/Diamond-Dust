using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlockBasic : MonoBehaviour
{

    // Store the attached EnemyScript
    private EnemyScript thisEnemyScript;
    private float blocktime;
    private Animator anim;

    // Allows the attached enemy to block.
    void Start()
    {
        blocktime = 0;
        thisEnemyScript = this.gameObject.GetComponent<EnemyScript>();
        anim = GetComponent<Animator>();
        thisEnemyScript.canBlock = true;
    }

    // Updates the block timer if blocking.
    void Update()
    {
        if (thisEnemyScript.state == "blocking")
        {
            blocktime -= Time.deltaTime;
            if (blocktime <= 0)
            {
                thisEnemyScript.state = "neutral";
                anim.SetBool("Block", false);
            }
        }
    }

    // Provides the Block() function for the enemy. Call this from EnemyScript using SendMessage("Block")
    public void Block()
    {
        blocktime = 1;
        thisEnemyScript.isAttackReady = false;
        thisEnemyScript.attackTimer = 1f;
        thisEnemyScript.state = "blocking";
        anim.SetBool("Block", true);
    }
}
