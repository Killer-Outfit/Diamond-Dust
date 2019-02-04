using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	// Internal variables
	public float maxHealth = 100;
	public float currentHealth;
	
	// External variables
	public Slider healthbar;
	
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
		updateHealthbar();
    }

    // Update is called once per frame
    void Update()
    {
		currentHealth += Random.Range(-1f, 1f);
        updateHealthbar();
    }
	
	void updateHealthbar()
	{
		healthbar.value = currentHealth / maxHealth;
	}
}
