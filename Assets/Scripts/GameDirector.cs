using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    static GameDirector instance;

    public static GameDirector Instance => instance;

    //UI
    [SerializeField] Text scoreText;
    [SerializeField] Text lifeText;
    [SerializeField] Text timeText;
    public static int score;
    public static int life;
    public static float time;

    //���Ԑ���(���̂Ƃ��g���Ă��Ȃ�)
    float maxTime = 100;

    //�X�e�[�W�p
    public static int nowStage;

    [SerializeField] GameObject UI;

    [SerializeField] AudioSource[] aud;

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
        //�����Z�[�u����������Ȃ�ύX
        score = 0;
        life = 5;
        nowStage = 0;
        aud[nowStage].Play();

        //���[�h���̏�����ǉ�
        SceneManager.sceneLoaded += audioManager;

        Application.targetFrameRate = 60;
    }


    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    SceneManager.LoadScene("GameScene");
        //}

        scoreText.text = "score:" + score.ToString("D6");
        lifeText.text = "Life:" + life.ToString("D2");

        time -= Time.deltaTime;
        timeText.text = "Time:" + time.ToString("000");

        if (SceneManager.GetActiveScene().name == "DeathScene" || SceneManager.GetActiveScene().name == "TitleScene")
        {
            UI.SetActive(false);
        }
        else
        {
            UI.SetActive(true);
        }
        

    }

    //���[�h����Ƃ��̏���
    void audioManager(Scene scene, LoadSceneMode mode)
    {
        AudioSource audHz = null;
        for (int i = 0; i < aud.Length; i++)
        {
            //�V�[���ɂ���ċȂ�ς���
            if(nowStage == i)
            {
                aud[i].Play();

                //�ۑ����Ď~�߂��Ȃ��悤�ɂ���
                audHz = aud[i];
            }
            else
            {
                if (audHz != aud[i])
                {
                    aud[i].Stop();
                    Debug.Log(i);
                }
            }
        }
    }

    public void deathRestart()
    {

        if (Input.GetKeyDown("joystick button 1"))
        {
            SceneManager.LoadScene(nowStage);
        }

    }

}
