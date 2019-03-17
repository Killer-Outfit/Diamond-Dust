using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yesnt : MonoBehaviour
{
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = .15f;
        transform.Rotate(0, 180, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed);
    }
}
