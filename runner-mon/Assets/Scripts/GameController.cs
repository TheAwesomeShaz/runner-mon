using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool isPlaying;
    int currSceneIndex;

    // GameObject joystick;
    [SerializeField] GameObject startText;

    public static GameController instance;

    private void Awake()
    {
        instance = this;
        currSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Start is called before the first frame update
    void Start()
    {
        //TODO: later set this behaviour on tapping once the game starts
        isPlaying = true;
        startText.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayerController.instance.isEvolving = true;
        }
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            startText.SetActive(false);
            PlayerController.instance.tapToAttackText.gameObject.SetActive(false);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currSceneIndex);
    }

    public void NextLevel()
    {
        if (currSceneIndex < 2)
        {
            SceneManager.LoadScene(currSceneIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

}
