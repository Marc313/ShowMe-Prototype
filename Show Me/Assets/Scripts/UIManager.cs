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
    public GameObject EndScreen;

    public Text RespawnText;
    public Text timerText;

    private Timer timer = new Timer();

    private bool isPaused;

    private void Awake()
    {
        pauseButton.performed += context => PauseSwitch();
    }

    private void OnEnable()
    {
        pauseButton.Enable();
        EventSystem.Subscribe(EventName.BOAT_READY, (name, value) => timer.Start());
        EventSystem.Subscribe(EventName.LEVEL_END, (name, value) => ShowEndScreen(name, value));
    }

    private void OnDisable()
    {
        pauseButton.Disable();
        EventSystem.Unsubscribe(EventName.BOAT_READY, (name, value) => timer.Start());
        EventSystem.Unsubscribe(EventName.LEVEL_END, (name, value) => ShowEndScreen(name, value));
    }

    private void Start()
    {
        PauseMenu.SetActive(false);
        EndScreen.SetActive(false);
    }

    private void Update()
    {
        timer.Tick();
        timerText.text = timer.TimeToString();
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

    private void ShowEndScreen(EventName _name, object _value = null)
    {
        EndScreen.SetActive(true);
        RespawnText.text = $"Total Time: {timer.TimeToString()}";
        timerText.gameObject.SetActive(false);
    }
}
