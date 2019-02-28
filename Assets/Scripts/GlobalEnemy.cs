using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnemy : MonoBehaviour
{
    private float attackTimer;
    private float attackSpacing;
    public List<GameObject> battleRing;
    public List<GameObject> availableFighters;
    
    // Start is called before the first frame update
    void Start()
    {
        attackTimer = Random.Range(4f, 8f);
        attackSpacing = Random.Range(0.1f, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            StartCoroutine("CallAttack");
            attackTimer = Random.Range(4f, 8f);
        }
    }

    // Adds an enemy to the battle ring. Returns the index.
    public void AddEnemy(GameObject enemy)
    {
        battleRing.Add(enemy);
        enemy.GetComponent<EnemyScript>().managerIndex = battleRing.Count - 1;
    }

    // Removes an enemy from the battle ring. Called on death.
    public void RemoveEnemy(int index)
    {
        battleRing.RemoveAt(index);
        for (int i = 0; i < battleRing.Count; i++)
        {
            battleRing[i].GetComponent<EnemyScript>().managerIndex = i;
        }
    }

    public void EnemyReady(int i)
    {
        availableFighters.Add(battleRing[i]);
    }

    private IEnumerator CallAttack()
    {
        if (availableFighters.Count > 0)
        {
            int randIndex;
            int numAttackers = Mathf.RoundToInt(availableFighters.Count / 3) + 1;
            for (int i = numAttackers; i > 0; i--)
            {
                randIndex = Random.Range(0, availableFighters.Count);
                if (availableFighters[randIndex].GetComponent<EnemyScript>().attackReady)
                {
                    availableFighters[randIndex].GetComponent<EnemyScript>().state = "approaching";
                    availableFighters.RemoveAt(randIndex);
                }
                yield return new WaitForSeconds(attackSpacing);
            }
            attackSpacing = Random.Range(0.1f, 0.4f);
        }
        else
        {
            attackTimer = 1f;
        }
    }
}

// Old variables

//public GameObject minorEnemy;
//public GameObject majorEnemy;
//public GameObject player;
//public static int numMinor = 0;
//public static int numMajor;
//public static GameObject[] allMinor = new GameObject[numMinor];
//public static GameObject[] allMajor = new GameObject[numMajor];
//Vector3 pos1;
//Vector3 pos2;
//Vector3 pos3;
//Vector3 pos4;
//int truePos;

// Old Start code. Spawning?

//void Start()
//{
//    for(int i = 0; i < numMinor; i++)
//    {
//        truePos = RandTruePos();
//        pos1 = RandPos1();
//        pos2 = RandPos2();
//        pos3 = RandPos3();
//        pos4 = RandPos4();

//        if (truePos == 1)
//        {
//            allMinor[i] = (GameObject)Instantiate(minorEnemy, pos1, Quaternion.identity);
//        }

//        else if (truePos == 2)
//        {
//            allMinor[i] = (GameObject)Instantiate(minorEnemy, pos2, Quaternion.identity);
//        }

//        else if (truePos == 3)
//        {
//            allMinor[i] = (GameObject)Instantiate(minorEnemy, pos3, Quaternion.identity);
//        }

//        else if (truePos == 4)
//        {
//            allMinor[i] = (GameObject)Instantiate(minorEnemy, pos4, Quaternion.identity);
//        }
//    }
//}


// Old functions. Random positoning?

//private Vector3 RandPos1()
//{
//    return new Vector3(Random.Range(player.transform.position.x + 25, player.transform.position.x + 50),
//                   0,
//                   Random.Range(player.transform.position.z - 50, player.transform.position.z + 50));
//}

//private Vector3 RandPos2()
//{
//    return new Vector3(Random.Range(player.transform.position.x - 50, player.transform.position.x - 25),
//                   0,
//                   Random.Range(player.transform.position.z - 50, player.transform.position.z + 50));
//}

//private Vector3 RandPos3()
//{
//    return new Vector3(Random.Range(player.transform.position.x - 50, player.transform.position.x + 50),
//                   0,
//                   Random.Range(player.transform.position.z + 25, player.transform.position.z + 50));
//}

//private Vector3 RandPos4()
//{
//    return new Vector3(Random.Range(player.transform.position.x - 50, player.transform.position.x + 50),
//                   0,
//                   Random.Range(player.transform.position.z - 50, player.transform.position.z - 25));
//}

//private int RandTruePos()
//{
//    return Random.Range(1, 4);
//}
//private int RandEnemy()
//{
//    return Random.Range(0, numMinor - 1);
//}
