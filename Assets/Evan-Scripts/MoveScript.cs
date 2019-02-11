using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour {
    public float movementSpeed = 10;
    public float turningSpeed = 10;
    
    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("LStick X") * movementSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("LStick Y") * movementSpeed * Time.deltaTime;
        transform.Translate(horizontal, 0, vertical);
        horizontal = 0;
        vertical = 0;
    }
}