using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("ui panels")]
    public GameObject youWinPanel;
    public GameObject youLosePanel;
    public GameObject pausePanel;  
    public GameObject pause;  // 

    [Header("timer")]
    public float timeLimit = 60f; // seconds
    public TextMeshProUGUI timerText; // assign a ui text in inspector

    [Header("tags")]
    public string enemyTag = "Enemy";
    public string playerTag = "Player";

    [Header("scenes")]
    public string mainMenuScene = "MainMenu";   // set in inspector
    public string nextSceneName = "";           // set next level name in inspector
   

    private bool gameEnded = false;
    private bool timerRunning = false; // only counts after cutscene
    private float currentTime;
    private bool isPaused = false;     // pause state tracker

    void Start()
    {
        currentTime = timeLimit;

        if (timerText != null)
            timerText.text = "Timer: " + Mathf.Ceil(currentTime).ToString();
    }
    

    void Update()
    {
        if (gameEnded || !timerRunning || isPaused) return; // stops when paused or ended

        // countdown
        currentTime -= Time.deltaTime;
        if (currentTime < 0) currentTime = 0;

        // update ui
        if (timerText != null)
            timerText.text = "timer: " + Mathf.Ceil(currentTime).ToString();

        // win condition
        if (GameObject.FindGameObjectsWithTag(enemyTag).Length == 0)
        {
            YouWin();
        }

        // lose condition - player destroyed
        if (GameObject.FindGameObjectsWithTag(playerTag).Length == 0)
        {
            YouLose();
        }

        // lose condition - time ran out
        if (currentTime <= 0)
        {
            YouLose();
        }
    }

    public void StartTimer()
    {
        timerRunning = true;
        Debug.Log("Timer started!");
    }

    void YouWin()
    {
        gameEnded = true;
        youWinPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void YouLose()
    {   
        
        gameEnded = true;
        youLosePanel.SetActive(true);
        Time.timeScale = 0f;
    }


    public void Pause()
    {
        if (gameEnded) return; // don't allow pause after win/lose

        isPaused = true;
        pausePanel.SetActive(true);
        pause.SetActive(false);
        Time.timeScale = 0f;

}

public void Resume()
{
    isPaused = false;
    pausePanel.SetActive(false);
    pause.SetActive(true);
    Time.timeScale = 1f;
}

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("right the right name u fucking bastard");
        }
    }
}
