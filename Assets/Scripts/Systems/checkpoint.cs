using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public CheckpointManager manager;

    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.tag);
        if (collision.tag == "Player")
        {
            Debug.Log("check!!!!!");
            active = true;
            activateCheckpoint();
        }
    }

    public void activateCheckpoint()
    {
        manager.updateCheckpoint(transform.position);
    }
}
