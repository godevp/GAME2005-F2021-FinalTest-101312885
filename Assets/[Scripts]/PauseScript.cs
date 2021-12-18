using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseScript : MonoBehaviour
{

    public bool isPaused = true;
    public GameObject pauseUI;

    void Start()
    {
        isPaused = true;
        Time.timeScale = 0.0f;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(isPaused)
            {
                Debug.Log("Resume");
                _Resume();
            }
            else
            {
                Debug.Log("Pause");
                _Pause();
            }
        }
        if(Input.GetKeyDown(KeyCode.M) && isPaused)
        {
            _MainMenu();
        }
    }

    public void _Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
    }
    public void _Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void _MainMenu()
    {
         SceneManager.LoadScene(1);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
