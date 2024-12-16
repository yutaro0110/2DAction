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

    //時間制限(今のとこ使っていない)
    float maxTime = 100;

    //ステージ用
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
        //もしセーブを実装するなら変更
        score = 0;
        life = 5;
        nowStage = 0;
        aud[nowStage].Play();

        //ロード時の処理を追加
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

    //ロードするときの処理
    void audioManager(Scene scene, LoadSceneMode mode)
    {
        AudioSource audHz = null;
        for (int i = 0; i < aud.Length; i++)
        {
            //シーンによって曲を変える
            if(nowStage == i)
            {
                aud[i].Play();

                //保存して止められないようにする
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
