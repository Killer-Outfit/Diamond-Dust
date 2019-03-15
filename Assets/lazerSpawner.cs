using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lazerSpawner : MonoBehaviour
{
    public Collider laser;
    private Vector3 pos;
    private float rotate;
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(180, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Instantiate(laser, transform.position, transform.rotation);
    }
}
