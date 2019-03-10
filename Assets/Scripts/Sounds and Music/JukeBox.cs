using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeBox : MonoBehaviour
{
	[FMODUnity.EventRef]
	public string musicEventName;//Set this in the editor to the event that corrosponds to the music you want to play
		
	//A float between zero and one. Set to 1 during combat, and to 0 when outside combat
	public float InBattle
	{
		get 
		{
			float val;
			inBattle.getValue(out val);
			return val;
		}
		set {inBattle.setValue(value);}
	}
	
	
	private FMOD.Studio.EventInstance musicEventInstance;//this is the runtime object that will play the music
	private FMOD.Studio.ParameterInstance inBattle;//this is the instance of the music event's parameter "In Battle"
	
    // Start is called before the first frame update
    void Start()
    {
        musicEventInstance = FMODUnity.RuntimeManager.CreateInstance(musicEventName);//create the runtime object that plays the music
		musicEventInstance.getParameter("In Battle", out inBattle);//get its parameter "In Battle"
        musicEventInstance.start();//play the music!
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
		{
			InBattle = 1.0f;
		}
    }
}
