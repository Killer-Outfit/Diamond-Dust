using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : MonoBehaviour
{
    private GameObject player;
    private bool hasHit;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        hasHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        hasHit = GetComponent<Projectile>().hasHit;
    }

    IEnumerable moveStraight(float distance, bool isPiercing)
    {

        while(getDistance(player) < distance)
        {
            if(hasHit && !isPiercing)
            {
                break;
            }
            transform.Translate(Vector3.forward * Time.deltaTime);
            yield return null;
        }
        StartCoroutine("returnStraight");
    }

    IEnumerable returnStraight()
    {
        while(getDistance(player) > 3)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime);
            yield return null;
        }
    }

    public float getDistance(GameObject player)
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        return distance;
    }
}
