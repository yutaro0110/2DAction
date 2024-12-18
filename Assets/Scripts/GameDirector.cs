using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    static GameDirector instance;

    public static string[] sceneName =
    {
        "TitleScene",
        "GameScene",
        "GameScene2",
        "GameScene3",
        "DeathScene",
    };

    public enum cond
    {
        None,
        Death,
        Return,
        Goal,
    }

    public static GameDirector Instance => instance;

    //UI
    [SerializeField] Text scoreText;
    [SerializeField] Text timeText;
    public static int score;
    public static float time;

    //���Ԑ���(���̂Ƃ��g���Ă��Ȃ�)
    float maxTime = 100;

    //�X�e�[�W�p
    public static int nowStage;
    int nowStageTemp;

    [SerializeField] GameObject UI;

    [SerializeField] AudioSource[] aud;
    [SerializeField] AudioSource button;
    [SerializeField] AudioSource death;


    float timer;
    const float circleSpeed = 22.0f;

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
        nowStage = 0;
        aud[nowStage].Play();

        //���[�h���̏�����ǉ�
        SceneManager.sceneLoaded += audioManager;

        Application.targetFrameRate = 60;



    }


    void Update()
    {

        if (FadeManager.Instance.IsFading() != false) return;

        scoreText.text = "score:" + score.ToString("D6");

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

    public void SceneChange(int condition)
    {

        // �t�F�[�h������Ȃ����`�F�b�N
        if (FadeManager.Instance.IsFading() == false)
        {
            string nextScene;

            UI.SetActive(false);

            if(condition == (int)cond.Death)
            {
                death.Play();
                nextScene = sceneName[4];
            }
            else
            {
                button.Play();
                if(nowStage >= sceneName.Length - 1)
                {
                    nowStage = 0;
                }
                nextScene = sceneName[nowStage];
            }

            // �t�F�[�h�A�E�g���āA�V�[���ύX
            // �����ɃV�[�����A�t�F�[�h�b����ݒ�
            FadeManager.Instance.LoadScene(nextScene, 3.0f);
            
        }

    }

    

    

}
