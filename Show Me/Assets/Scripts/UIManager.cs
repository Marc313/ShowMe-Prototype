using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InputAction pauseButton;
    public GameObject PauseMenu;
    public GameObject RespawnScreen;

    public Text RespawnText;

    private bool isPaused;

    private void Awake()
    {
        pauseButton.performed += context => PauseSwitch();
    }

    private void OnEnable()
    {
        pauseButton.Enable();
        EventSystem.Subscribe(EventName.PLAYER_KILLED, ShowRespawnScreen);
    }

    private void OnDisable()
    {
        pauseButton.Disable();
        EventSystem.Unsubscribe(EventName.PLAYER_KILLED, ShowRespawnScreen);
    }

    private void Start()
    {
        PauseMenu.SetActive(false);
        RespawnScreen.SetActive(false);
    }

    private void PauseSwitch()
    {
        if (!isPaused) ShowPauseScreen();
        else HidePauseScreen();

        isPaused = !isPaused;
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
