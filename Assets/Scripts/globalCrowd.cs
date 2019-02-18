using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalCrowd : MonoBehaviour
{
    public GameObject crowdPrefab;
    public static int arenaSize = 300;
    public GameObject playerPrefab;
    public static int numCrowd = 200;
    public static GameObject[] allCrowd = new GameObject[numCrowd];
    //public static int spaceHeight = 1960;
    public static Vector3 goalPos = Vector3.zero;

    //private static Vector3 pPos = Vector3.zero

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numCrowd; i++)
        {
            Vector3 pos = new Vector3(Random.Range(playerPrefab.transform.position.x - arenaSize, playerPrefab.transform.position.x + arenaSize),
                                      0,
                                      Random.Range(playerPrefab.transform.position.z-arenaSize, playerPrefab.transform.position.z + arenaSize));
            //Debug.Log("SpaceHeight = " + pos.y);
            allCrowd[i] = (GameObject) Instantiate(crowdPrefab, pos, Quaternion.identity);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < numCrowd; i++)
        {
            if (Vector3.Distance(playerPrefab.transform.position, allCrowd[i].transform.position) > arenaSize)
            {
                Vector3 pos = RandSpawn();
                //bool cantSpawn = Physics.CheckSphere(pos, 1);

                //if (!cantSpawn)
                //{
                Destroy(allCrowd[i]);
                allCrowd[i] = (GameObject)Instantiate(crowdPrefab, pos, Quaternion.identity);
                //}
            }
        }

        if(Random.Range(0,10000) < 50)
        {
            //Vector3 pRange = new Vector3(pPos.x + 10, 0, pPos.z + 10);
            //while (goalPos.x <= pRange.x || goalPos.z <= pRange.z)
            //{
                goalPos = new Vector3(Random.Range(playerPrefab.transform.position.x - arenaSize, playerPrefab.transform.position.x + arenaSize),
                                      0,
                                      Random.Range(playerPrefab.transform.position.z - arenaSize, playerPrefab.transform.position.z + arenaSize));
            //Debug.Log("new SpaceHeight = " + goalPos.y);
            //}
        }
    }

    private Vector3 RandSpawn()
    {
        Vector3 placement = new Vector3();
        int rand = Random.Range(1, 4);
        if (rand == 1)
        {
            placement =  new Vector3(Random.Range(playerPrefab.transform.position.x + arenaSize*2/3, playerPrefab.transform.position.x + arenaSize),
                               0,
                               Random.Range(playerPrefab.transform.position.z - arenaSize, playerPrefab.transform.position.z + arenaSize));
        }
        else if (rand == 2)
        {
            placement = new Vector3(Random.Range(playerPrefab.transform.position.x - arenaSize, playerPrefab.transform.position.x - arenaSize * 2 / 3),
                               0,
                               Random.Range(playerPrefab.transform.position.z - arenaSize, playerPrefab.transform.position.z + arenaSize));
        }
        else if (rand == 3)
        {
            placement = new Vector3(Random.Range(playerPrefab.transform.position.x - arenaSize, playerPrefab.transform.position.x + arenaSize),
                               0,
                               Random.Range(playerPrefab.transform.position.z + arenaSize * 2 / 3, playerPrefab.transform.position.z + arenaSize));
        }
        else if (rand == 4)
        {
            placement = new Vector3(Random.Range(playerPrefab.transform.position.x - arenaSize, playerPrefab.transform.position.x + arenaSize),
                               0,
                               Random.Range(playerPrefab.transform.position.z - arenaSize, playerPrefab.transform.position.z - arenaSize * 2 / 3));
        }
        return placement;
    }
}
