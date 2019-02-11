using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseScript : MonoBehaviour
{
    GameObject menu;
    bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        menu = GameObject.Find("Main Menu (Button)");
        menu.SetActive(false);
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("StartButton") && !isPaused)
        {
            menu.SetActive(true);
            isPaused = true;
        }else if (Input.GetButtonDown("StartButton") && isPaused)
        {
            menu.SetActive(false);
            isPaused = false;
        }
    }
}
