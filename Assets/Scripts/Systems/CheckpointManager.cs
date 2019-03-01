using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Vector3 currentCheckpoint;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void updateCheckpoint(Vector3 newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }

    public Vector3 getCheckpoint()
    {
        return currentCheckpoint;
    }
}
