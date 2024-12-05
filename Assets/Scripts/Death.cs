using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Death : MonoBehaviour
{

    public Text lifeText;
    public Text scoreText;
    void Start()
    {

        GameDirector.life--;

        scoreText.text = "score:" + GameDirector.score.ToString("D6");
        lifeText.text = "Life:" + GameDirector.life.ToString("D2");

    }

    
    void Update()
    {
        
    }
}
