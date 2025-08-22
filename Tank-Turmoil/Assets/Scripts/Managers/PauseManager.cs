using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject PauseGamePanel;
    public static PauseManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.P))
            PauseGame();
    }

    public void PauseGame()
    {
        PauseGamePanel.SetActive(true);
    }

    public void Continue()
    {
        PauseGamePanel.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
