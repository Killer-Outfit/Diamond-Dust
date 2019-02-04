using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalCrowd : MonoBehaviour
{
    public GameObject crowdPrefab;
    public static int arenaSize = 50;
    public GameObject playerPrefab;
    static int numCrowd = 150;
    public static GameObject[] allCrowd = new GameObject[numCrowd];
    // private static Vector3 pPos = Vector3.zero;

    public static Vector3 goalPos = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numCrowd; i++)
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
            // Vector3 pRange = new Vector3(pPos.x + 10, 0, pPos.z + 10);
            // while (goalPos.x <= pRange.x || goalPos.z <= pRange.z)
            // {
                goalPos = new Vector3(Random.Range(-arenaSize, arenaSize),
                                      0,
                                      Random.Range(-arenaSize, arenaSize));
            // }
        }
    }
}
