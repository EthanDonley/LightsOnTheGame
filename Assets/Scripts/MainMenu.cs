using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Loads "LightOn!" scene
        Debug.Log("The game scene has been loaded");
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
