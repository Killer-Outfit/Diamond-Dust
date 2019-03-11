using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    GameObject menu;
    GameObject outfitMenu;
    GameObject playerHealth;
	GameObject changeTop;
	GameObject close;
    GameObject mainCanvas;
    GameObject mainCamera;
    GameObject outfitCamera;
    //GameObject curButton;
    GameObject controls;
    Camera cam1;
    Camera cam2;
    Canvas outfitCanvas;
	
    bool isPaused;
    bool isControlsOpen;
    bool isOutfitMenuOpen;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        cam = mainCamera.GetComponent<Camera>();
        menu = GameObject.Find("PauseMenuElements");
        playerHealth = GameObject.Find("Player Health");
        outfitMenu = GameObject.Find("OutfitMenuElements");
        mainCanvas = GameObject.Find("Canvas");
        outfitCanvas = mainCanvas.GetComponent<Canvas>();
        menu.SetActive(false);
        outfitMenu.SetActive(false);
        isPaused = false;
        isControlsOpen = false;
        cam1.enabled = true;
        cam2.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //curButton = EventSystem.current.currentSelectedGameObject;
        //curButton.GetComponent<Button>().GetComponent<Image>().color = Color.red;

        if (Input.GetButtonDown("StartButton") && !isPaused && !isControlsOpen)
        {
            // Enable Pause Menu
            menu.SetActive(true);
            //playerHealth.

            // Disable Gameplay
            Time.timeScale = 0.0f;
            isPaused = true;
            Time.timeScale = 0.0f;
        }
        else if ((Input.GetButtonDown("StartButton") || Input.GetButtonDown("Cancel")) && isPaused && !isOutfitMenuOpen && !isControlsOpen)
        {
            // Disable Pause Menu
            firstSelected.Select();
            menu.SetActive(false);

            // Enable Gameplay
            Time.timeScale = 1f;
            isPaused = false;
            Time.timeScale = 1f;
        }

        if (Input.GetButtonDown("ViewControls") && !isPaused)
        {
            ViewControls();
        }
        else if ((Input.GetButtonDown("ViewControls") || Input.GetButtonDown("Cancel") || Input.GetButtonDown("Submit")) && isPaused)
        {
            ResumeGame();
        }
    }
	public bool checkPause()
    {
        return isPaused;
    }
	
    public void ResumeGame()
    {

        // Disable Pause Menu
        firstSelected.Select();
        menu.SetActive(false);
        controls.SetActive(false);
        

        // Enable Gameplay
        Time.timeScale = 1f;
        isPaused = false;
        isControlsOpen = false;
        Time.timeScale = 1f;

    }

    public void ViewControls()
    {
        menu.SetActive(false);
        // Disable Gameplay
        Time.timeScale = 0.0f;
        isPaused = true;
        isControlsOpen = true;
        Time.timeScale = 0.0f;

        controls.SetActive(true);
    }

    public void OpenOutfitMenu()
    {
        menu.SetActive(false);
        outfitMenu.SetActive(true);
        outfitCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        outfitCanvas.worldCamera = cam;
    }

    public void CloseOutfitMenu()
    {
        outfitMenu.SetActive(false);
        menu.SetActive(true);
        outfitCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
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