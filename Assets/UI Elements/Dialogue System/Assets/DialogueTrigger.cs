using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	public Dialogue dialogue;
    public GameObject dialoguePrompt;
    public Animator animator;

    GameObject dialogueClone;
    Vector3 promptTransform;

    private bool isTalking = false;

    public void TriggerDialogue ()
	{
		FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
          // Setting the position above NPC head
          promptTransform = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z); 
          dialogueClone = Instantiate(dialoguePrompt, promptTransform, Quaternion.identity) as GameObject;
          
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && Input.GetButton("Submit") && !isTalking)
        {
            //Debug.Log("im talkin");
            TriggerDialogue();
            isTalking = true;
           
        }

        /*
        if(talking && Input.GetButton("Submit"))
        {
            Debug.Log("Next line pls");
            FindObjectOfType<DialogueManager>().DisplayNextSentence();

        }
        */
        
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "Player") 
        {
            isTalking = false;
            Destroy(dialogueClone, .01f);
            animator.SetBool("IsOpen", false);
        }

    }

}