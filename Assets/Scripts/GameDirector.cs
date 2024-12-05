using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    static GameDirector instance;

    public static GameDirector Instance => instance;

    public Text scoreText;
    public Text lifeText;
    public Text timeText;

    public static int score;
    public static int life;
    public static float time;

    float maxTime = 100;

    private void Awake()
    {
        time = maxTime;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        score = 0;
        life = 5;
        Application.targetFrameRate = 60;
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("GameScene");
        }

        scoreText.text = "score:" + score.ToString("D6");
        lifeText.text = "Life:" + life.ToString("D2");

        time -= Time.deltaTime;
        timeText.text = "Time:" + time.ToString("000");

        if (SceneManager.GetActiveScene().name == "DeathScene" || SceneManager.GetActiveScene().name == "TitleScene")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
        

    }
}
