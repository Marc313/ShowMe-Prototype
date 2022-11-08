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
    public GameObject Vignette;

    public Text RespawnText;
    public Text timerText;

    private Timer timer;

    private bool isPaused;

    private void Awake()
    {
        timer = new Timer();
        pauseButton.performed += context => PauseSwitch();
    }

    private void OnEnable()
    {
        pauseButton.Enable();
        EventSystem.Subscribe(EventName.BOAT_READY, (name, value) => timer.Start());
        EventSystem.Subscribe(EventName.LEVEL_END, (name, value) => ShowEndScreen());
    }

    private void OnDisable()
    {
        pauseButton.Disable();
        EventSystem.Unsubscribe(EventName.BOAT_READY, (name, value) => timer.Start());
        EventSystem.Unsubscribe(EventName.LEVEL_END, (name, value) => ShowEndScreen());
    }

    private void Start()
    {
        PauseMenu.SetActive(false);
        EndScreen.SetActive(false);
        //Debug.Log(EndScreen);
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
        Invoke(nameof(ReloadScene), .1f);
    }

    private void ReloadScene()
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

    private void ShowEndScreen()
    {
        if (EndScreen == null) EndScreen = GameObject.Find("EndMenu");

        EndScreen.SetActive(true);
        RespawnText.text = $"Total Time: {timer.TimeToString()}";
        timerText.gameObject.SetActive(false);
    }

    public void ShowVignette()
    {
       // Debug.Log("SHOW");
        Vignette.gameObject.SetActive(true);
    }

    public void ScaleVignette(Vector3 _scale)
    {
        Vignette.gameObject.transform.localScale = _scale;
    }

    public void HideVignette()
    {
        //Debug.Log("HIDE");

        Vignette.gameObject.SetActive(false);
    }
}
