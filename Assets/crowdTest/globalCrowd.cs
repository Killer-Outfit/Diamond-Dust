using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalCrowd : MonoBehaviour
{
    public GameObject crowdPrefab;
    public static int arenaSize = 1000;
    // public GameObject playerPrefab;
    public static int numCrowd = 50;
    public static GameObject[] allCrowd = new GameObject[numCrowd];
    // private static Vector3 pPos = Vector3.zero;

    // public static int spaceHeight = 1960;
    public static Vector3 goalPos = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numCrowd; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-arenaSize, arenaSize),
                                      0,
                                      Random.Range(-arenaSize, arenaSize));
            // Debug.Log("SpaceHeight = " + pos.y);
            allCrowd[i] = (GameObject) Instantiate(crowdPrefab, pos, Quaternion.identity);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Random.Range(0,10000) < 50)
        {
            // Vector3 pRange = new Vector3(pPos.x + 10, 0, pPos.z + 10);
            // while (goalPos.x <= pRange.x || goalPos.z <= pRange.z)
            // {
                goalPos = new Vector3(Random.Range(-arenaSize, arenaSize),
                                      0,
                                      Random.Range(-arenaSize, arenaSize));
            //Debug.Log("new SpaceHeight = " + goalPos.y);
            // }
        }
    }
}
