using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalCrowd : MonoBehaviour
{
    public GameObject crowdPrefab;
    public static int arenaSize = 5;

    static int numCrowd = 10;
    public static GameObject[] allCrowd = new GameObject[numCrowd];

    public static Vector3 goalPos = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < numCrowd; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-arenaSize, arenaSize),
                                      0,
                                      Random.Range(-arenaSize, arenaSize));
            allCrowd[i] = (GameObject) Instantiate(crowdPrefab, pos, Quaternion.identity);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Random.Range(0,10000) < 50)
        {
            goalPos = new Vector3(Random.Range(-arenaSize, arenaSize),
                                  0,
                                  Random.Range(-arenaSize, arenaSize));
        }
    }
}
