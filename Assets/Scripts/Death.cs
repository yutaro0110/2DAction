using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Death : MonoBehaviour
{

    public Text scoreText;

    GameDirector DirectorFunc;

    void Start()
    {
        GameDirector.score = GameDirector.score - 100 < 0 ? 0 : GameDirector.score - 100;

        scoreText.text = "score:" + GameDirector.score.ToString("D6");

        DirectorFunc = GameObject.Find("GameDirector (1)").GetComponent<GameDirector>();

        
    }

    
    void Update()
    {
        DirectorFunc.SceneChange((int)GameDirector.cond.Return);
    }
}
