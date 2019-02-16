using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnemy : MonoBehaviour
{
    public GameObject minorEnemy;
    public GameObject majorEnemy;
    public GameObject player;
    public static int numMinor = 0;
    public static int numMajor;
    public static GameObject[] allMinor = new GameObject[numMinor];
    public static GameObject[] allMajor = new GameObject[numMajor];
    Vector3 pos1;
    Vector3 pos2;
    Vector3 pos3;
    Vector3 pos4;
    int truePos;

    //bool isAttacking = false;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < numMinor; i++)
        {
            truePos = RandTruePos();
            pos1 = RandPos1();
            pos2 = RandPos2();
            pos3 = RandPos3();
            pos4 = RandPos4();

            if (truePos == 1)
            {
                allMinor[i] = (GameObject)Instantiate(minorEnemy, pos1, Quaternion.identity);
            }

            else if (truePos == 2)
            {
                allMinor[i] = (GameObject)Instantiate(minorEnemy, pos2, Quaternion.identity);
            }

            else if (truePos == 3)
            {
                allMinor[i] = (GameObject)Instantiate(minorEnemy, pos3, Quaternion.identity);
            }

            else if (truePos == 4)
            {
                allMinor[i] = (GameObject)Instantiate(minorEnemy, pos4, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Vector3 RandPos1()
    {
        return new Vector3(Random.Range(player.transform.position.x + 25, player.transform.position.x + 50),
                       0,
                       Random.Range(player.transform.position.z - 50, player.transform.position.z + 50));
    }

    private Vector3 RandPos2()
    {
        return new Vector3(Random.Range(player.transform.position.x - 50, player.transform.position.x - 25),
                       0,
                       Random.Range(player.transform.position.z - 50, player.transform.position.z + 50));
    }

    private Vector3 RandPos3()
    {
        return new Vector3(Random.Range(player.transform.position.x - 50, player.transform.position.x + 50),
                       0,
                       Random.Range(player.transform.position.z + 25, player.transform.position.z + 50));
    }

    private Vector3 RandPos4()
    {
        return new Vector3(Random.Range(player.transform.position.x - 50, player.transform.position.x + 50),
                       0,
                       Random.Range(player.transform.position.z - 50, player.transform.position.z - 25));
    }

    private int RandTruePos()
    {
        return Random.Range(1, 4);
    }
    private int RandEnemy()
    {
        return Random.Range(0, numMinor - 1);
    }
}
