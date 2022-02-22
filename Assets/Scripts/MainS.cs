using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Unity.Entities;

public class MainS : MonoBehaviour
{
    public TMP_Text heartHealth;
    public Image unitSelected;
    public TMP_Text unitSelectedNumber;
    public GameObject player;

    public GameObject youLoseScreen;
    public TMP_Text timeSurvivedCurrent;
    public TMP_Text timeSurvivedLose;
    public Button retryButton;
    public Button mainMenuButton;

    public GameObject pauseScreen;
    public Button continueButton;
    public Button mainMenuButton2;


    void Start()
    {
        retryButton.onClick.AddListener(OnRetryClick);
        mainMenuButton.onClick.AddListener(OnMainMenuClick);
        mainMenuButton2.onClick.AddListener(OnMainMenuClick);
        continueButton.onClick.AddListener(OnContinueClick);
    }

    void Update()
    {
        timeSurvivedCurrent.text = TimeSpan.FromSeconds(Time.timeSinceLevelLoadAsDouble).ToString("mm':'ss");
        heartHealth.text = Utils.heartHealth.ToString();
        if(Utils.selectedUnits > 0)
        {
            unitSelected.enabled = true;
            unitSelectedNumber.enabled = true;
            unitSelectedNumber.text = Utils.selectedUnits.ToString();
        }
        else
        {
            unitSelected.enabled = false;
            unitSelectedNumber.enabled = false;
        }
        if (Utils.heartHealth <= 0)
        {
            if (!Utils.lost) Utils.timeSurvived = Time.timeSinceLevelLoadAsDouble;
            player.GetComponent<PlayerS>().enabled = false;
            youLoseScreen.SetActive(true);
            timeSurvivedLose.text = TimeSpan.FromSeconds(Utils.timeSurvived).ToString("mm':'ss");
            Utils.lost = true;
        }
        else
        {
            player.GetComponent<PlayerS>().enabled = true;
            youLoseScreen.SetActive(false);
            Utils.lost = false;
        }

        if (Utils.paused)
        {
            player.GetComponent<PlayerS>().enabled = false;
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            player.GetComponent<PlayerS>().enabled = true;
            pauseScreen.SetActive(false);
            Time.timeScale = 1;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Utils.paused = !Utils.paused;
        }
    }



    void OnRetryClick()
    {
        World.DisposeAllWorlds();
        SceneManager.LoadScene("MainScene");
        DefaultWorldInitialization.Initialize("World");
    }

    void OnMainMenuClick()
    {
        Utils.paused = false;
        Utils.isSelecting = false;
        Utils.lost = false;
        World.DisposeAllWorlds();
        SceneManager.LoadScene("MainMenu");
    }

    void OnContinueClick()
    {
        Utils.paused = false;
    }
}
