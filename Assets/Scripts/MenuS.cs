using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuS : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(OnStartClick);
        exitButton.onClick.AddListener(OnExitClick);
    }

    void OnStartClick()
    {
        World.DisposeAllWorlds();
        SceneManager.LoadScene("MainScene");
        DefaultWorldInitialization.Initialize("World");
    }

    void OnExitClick()
    {
        Application.Quit();
    }
}
