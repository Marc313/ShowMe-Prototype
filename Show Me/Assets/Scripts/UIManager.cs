using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject RespawnScreen;

    public Text RespawnText;

    private bool isPaused;

    private void OnEnable()
    {
        EventSystem.Subscribe(EventName.PLAYER_KILLED, ShowRespawnScreen);
    }

    private void OnDisable()
    {
        EventSystem.Unsubscribe(EventName.PLAYER_KILLED, ShowRespawnScreen);
    }

    private void Start()
    {
        PauseMenu.SetActive(false);
        RespawnScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) ShowPauseScreen();
            else HidePauseScreen();
        }
    }

    // For build
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // For build
    public void Quit()
    {
        Application.Quit();
    }

    private void ShowPauseScreen()
    {
        PauseMenu.SetActive(true);
    }

    private void HidePauseScreen()
    {
        PauseMenu.SetActive(false);
    }

    private void ShowRespawnScreen(EventName _name, object _value = null)
    {
        RespawnScreen.SetActive(true);
        RespawnText.text = "You got caught by a Siren! Respawning...";
        Invoke(nameof(Restart), 3f);
    }
}
