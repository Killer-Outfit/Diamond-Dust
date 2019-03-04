using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeBox : MonoBehaviour
{
	public FMOD.Studio.EventInstance areaMusic;
	public FMOD.Studio.ParameterInstance battleParam;
	[FMODUnity.EventRef]
	    public string areaMusicEvent;
    // Start is called before the first frame update
    void Start()
    {
        areaMusic = FMODUnity.RuntimeManager.CreateInstance(areaMusicEvent);
		areaMusic.getParameter("InBattle", out battleParam);
        areaMusic.start();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
		{
			battleMusic(1.0f);
		}
    }
	
	public void battleMusic(float inBattle)
	{
		battleParam.setValue(1.0f);
		//Debug.Log(battleParam.);
	}
}
