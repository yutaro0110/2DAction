using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{

    [SerializeField] GameObject arrow;

    int posNo;
    float select;
    Vector3[] arrowPos = 
    { 
        new Vector3(-2,-3,0),
        new Vector3(2,-3,0)
    };


    void Start()
    {
        posNo = 0;
    }

    
    void Update()
    {

        SelectArrow();


        SceneController();
    }

    void SelectArrow()
    {

        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            posNo = posNo == 0 ? 1 : 0;
            arrow.transform.position = arrowPos[posNo];
        }

    }

    void SceneController()
    {
        if(posNo == 0 && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(GameDirector.nowStage);
        }
    }

}
