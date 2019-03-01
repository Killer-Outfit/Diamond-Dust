using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDodgeBasic : MonoBehaviour
{
    // Store the attached EnemyScript
    private EnemyScript thisEnemyScript;
    private float dodgetime = 0;
    private Animator anim;
    private float speed = 0;
    private float accel = 0;

    // Allows the attached enemy to dodge.
    void Start()
    {
        thisEnemyScript = this.gameObject.GetComponent<EnemyScript>();
        anim = GetComponent<Animator>();
        thisEnemyScript.canDodge = true;
    }

    // Updates the dodge timer if blocking.
    void Update()
    {
        dodgetime -= Time.deltaTime;
        speed += accel;
        if (speed > 0f)
        {
            speed = 0f;
        }

        // Dodge startlag (0.2s)
        if (thisEnemyScript.state == "dodgestart")
        {
            thisEnemyScript.agent.Move((thisEnemyScript.player.transform.position - transform.position).normalized * speed);
            if (dodgetime <= 0.7f)
            {
                speed = -0.7f;
                accel = 0.03f;
                thisEnemyScript.state = "dodging";
            }
        }
        // Dodge active time (0.5s)
        else if (thisEnemyScript.state == "dodging")
        {
            thisEnemyScript.agent.Move((thisEnemyScript.player.transform.position - transform.position).normalized * speed);
            if (dodgetime <= 0.2f)
            {
                accel = 0.05f;
                thisEnemyScript.state = "dodgeend";
            }
        }
        // Dodge endlag (0.2s)
        else if (thisEnemyScript.state == "dodgeend")
        {
            thisEnemyScript.agent.Move((thisEnemyScript.player.transform.position - transform.position).normalized * speed);
            if (dodgetime <= 0)
            {
                speed = 0f;
                accel = 0f;
                thisEnemyScript.state = "neutral";
                //anim.SetBool("Dodge", false);
            }
        }
    }

    // Provides the Block() function for the enemy. Call this from EnemyScript using SendMessage("Block")
    public void Dodge()
    {
        dodgetime = 0.9f;
        thisEnemyScript.attackReady = false;
        thisEnemyScript.attackTimer = 1f;
        thisEnemyScript.state = "dodgestart";
        speed = 0f;
        accel = -0.01f;
        //anim.SetBool("Dodge", true);
    }
}
