﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneSwap : MonoBehaviour
{

    public void loadNextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    public void loadMain() => SceneManager.LoadScene("Main");
    public void loadStart() => SceneManager.LoadScene("Evan-Dev2");   
    public void restartLevel() => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

}
