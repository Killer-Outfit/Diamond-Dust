using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlockBasic : MonoBehaviour
{

    // Store the attached EnemyScript
    private EnemyScript thisEnemyScript;
    private float blocktime = 0;
    private Animator anim;

    // Allows the attached enemy to block.
    void Start()
    {
        thisEnemyScript = this.gameObject.GetComponent<EnemyScript>();
        anim = GetComponent<Animator>();
        thisEnemyScript.canBlock = true;
    }

    // Updates the block timer if blocking.
    void Update()
    {
        if (thisEnemyScript.isBlocking = true)
        {
            blocktime -= Time.deltaTime;
            if (blocktime <= 0)
            {
                thisEnemyScript.isBlocking = false;
                anim.SetBool("Block", false);
            }
        }
    }

    // Provides the Block() function for the enemy. Call this from EnemyScript using SendMessage("Block")
    public void Block()
    {
        blocktime = 1;
        thisEnemyScript.isBlocking = true;
        anim.SetBool("Block", true);
    }
}
