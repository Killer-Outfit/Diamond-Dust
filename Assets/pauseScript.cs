using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseScript : MonoBehaviour
{
    GameObject menu;
	GameObject changeTop;
	GameObject close;
	
    bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        menu = GameObject.Find("PauseMenuElements");
        menu.SetActive(false);
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("StartButton") && !isPaused)
        {
            // Enable Pause Menu
			menu.SetActive(true);
            
			// Disable Gameplay
			Time.timeScale = 0.0f;
            isPaused = true;
            Time.timeScale = 0.0f;
        }else if (Input.GetButtonDown("StartButton") && isPaused)
        {
            // Disable Pause Menu
			menu.SetActive(false);
			
			// Enable Gameplay
            Time.timeScale = 1f;
            isPaused = false;
            Time.timeScale = 1f;
        }
    }
	
	// Loop Through Children
	/*
	void enableChildren(GameObject target)
	{
		for(int i = 0; i < target.transform.childCount; i++)
		{
		   GameObject temp = target.transform.GetChild(i).gameObject;
		   temp.SetActive(true);
		}
	}
	
	void disableChildren(GameObject target)
	{
		for(int i = 0; i < target.transform.childCount; i++)
		{
		   GameObject temp = target.transform.GetChild(i).gameObject;
		   temp.SetActive(false);
		}
	}
	*/
}