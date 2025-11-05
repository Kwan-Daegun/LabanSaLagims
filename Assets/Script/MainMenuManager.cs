using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    
    [Header("scene names")]
    public string firstLevelScene = "Scene1";  // set in inspector

    public void PlayGame()
    {
        SceneManager.LoadScene(firstLevelScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game pressed!");
        Application.Quit();
    }
    
}
