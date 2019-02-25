using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartScript : MonoBehaviour
{
    GameObject deathScreen;

    // Start is called before the first frame update
    void Start()
    {
        deathScreen = GameObject.Find("GameOverElements");
        deathScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //PlayerDead(false);
    }

    private void PlayerDead(bool isDead)
    {
        if(isDead)
            deathScreen.SetActive(true);
    }
}
