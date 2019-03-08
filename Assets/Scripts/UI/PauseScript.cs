using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseScript : MonoBehaviour
{
    public Button firstSelected;
    public Button outfitSelected;

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
    bool isOutfitMenuOpen;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        outfitCamera = GameObject.Find("OutfitCamera");
        cam1 = mainCamera.GetComponent<Camera>();
        cam2 = outfitCamera.GetComponent<Camera>();
        menu = GameObject.Find("PauseMenuElements");
        controls = GameObject.Find("Controls");
        playerHealth = GameObject.Find("Player Health");
        outfitMenu = GameObject.Find("OutfitMenuElements");
        mainCanvas = GameObject.Find("Canvas");
        outfitCanvas = mainCanvas.GetComponent<Canvas>();
        menu.SetActive(false);
        outfitMenu.SetActive(false);
        controls.SetActive(false);
        isPaused = false;
        cam1.enabled = true;
        cam2.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //curButton = EventSystem.current.currentSelectedGameObject;
        //curButton.GetComponent<Button>().GetComponent<Image>().color = Color.red;

        if (Input.GetButtonDown("StartButton") || Input.GetKeyDown(KeyCode.Escape)) && !isPaused)
        {
            // Enable Pause Menu
            menu.SetActive(true);
            //playerHealth.
            firstSelected.Select();
            // Disable Gameplay
            Time.timeScale = 0.0f;
            isPaused = true;
            Time.timeScale = 0.0f;
        }
        else if (Input.GetButtonDown("StartButton") || Input.GetKeyDown(KeyCode.Escape)) && isPaused && !isOutfitMenuOpen)
        {
            // Disable Pause Menu
            menu.SetActive(false);

            // Enable Gameplay
            Time.timeScale = 1f;
            isPaused = false;
            Time.timeScale = 1f;
        }
        else if (Input.GetButtonDown("Cancel") && isPaused && !isOutfitMenuOpen)
        {
            // Disable Pause Menu
            menu.SetActive(false);

            // Enable Gameplay
            Time.timeScale = 1f;
            isPaused = false;
            Time.timeScale = 1f;
        }
    }
	public bool checkPause()
    {
        return isPaused;
    }
	
    public void ResumeGame()
    {
        
        // Disable Pause Menu
        menu.SetActive(false);
        controls.SetActive(false);
        

        // Enable Gameplay
        Time.timeScale = 1f;
        isPaused = false;
        Time.timeScale = 1f;

    }

    public void ViewControls()
    {
        menu.SetActive(false);
        // Disable Gameplay
        Time.timeScale = 0.0f;
        isPaused = true;
        Time.timeScale = 0.0f;

        controls.SetActive(true);
    }

    public void OpenOutfitMenu()
    {
        menu.SetActive(false);
        outfitMenu.SetActive(true);
        outfitCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        cam1.enabled = false;
        cam2.enabled = true;
        outfitCanvas.worldCamera = cam2;
        isOutfitMenuOpen = true;
        outfitSelected.Select();
    }

    public void CloseOutfitMenu()
    {
        outfitMenu.SetActive(false);
        menu.SetActive(true);
        outfitCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        cam1.enabled = true;
        cam2.enabled = false;
        isOutfitMenuOpen = false;
        firstSelected.Select();
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