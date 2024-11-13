using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    static UIController instance;

    public static UIController Instance => instance;

    public Text scoreText;

    public static int score;


    private void Awake()
    {
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

        Application.targetFrameRate = 60;

    }

    
    void Update()
    {

        scoreText.text = "score:" + score.ToString("D6");

    }
}
