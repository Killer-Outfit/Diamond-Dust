using System.Collections;
using System.Collections.Generic;
using UnityEngine;
struct State
{
    public State[] accessibleStates;
    public string stateName;
    public bool isMovable;
};
public class StateMachine : MonoBehaviour
{
    private State currentState;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
