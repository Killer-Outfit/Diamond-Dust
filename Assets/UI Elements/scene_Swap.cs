using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class scene_Swap : MonoBehaviour
{

    public void load_nextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    public void load_Main() => SceneManager.LoadScene("Main");
    public void load_Start() => SceneManager.LoadScene("Evan-Dev2");

}
