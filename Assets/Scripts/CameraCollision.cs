using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    private Renderer objRend;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        objRend = collision.gameObject.GetComponent<Renderer>();

        Debug.Log("Camera collided with object");

        objRend.enabled = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        objRend = collision.gameObject.GetComponent<Renderer>();

        Debug.Log("Camera no longer colliding with object");

        objRend.enabled = true;
    }

}
