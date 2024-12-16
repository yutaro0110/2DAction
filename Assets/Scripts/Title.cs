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
    bool stick;
    float stickHz;
    bool pushButton;

    Vector3[] arrowPos = 
    { 
        new Vector3(-2,-3,0),
        new Vector3(2,-3,0)
    };


    void Start()
    {
        posNo = 0;
        stick = true;
        pushButton = false;
    }

    
    void Update()
    {

        SelectArrow();


        SceneController();
    }

    void SelectArrow()
    {


        if (Input.GetAxisRaw("Horizontal") != 0 && stick)
        {
            posNo = posNo == 0 ? 1 : 0;

            if (posNo == 0 && Input.GetAxisRaw("Horizontal") < 0)
            {
                return;
            }
            else if(posNo == 1 && Input.GetAxisRaw("Horizontal") > 0)
            {
                return;
            }

            posNo = posNo == 0 ? 1 : 0;

            arrow.transform.position = arrowPos[posNo];
            stick = false;
        }
        else if(Input.GetAxisRaw("Horizontal") == 0 || stickHz * Input.GetAxisRaw("Horizontal") < 0)
        {
            stick = true;
        }

        stickHz = Input.GetAxisRaw("Horizontal");

    }

    void SceneController()
    {
        //Œˆ’è&ƒV[ƒ“‘JˆÚ
        if((posNo == 0 && Input.GetKeyDown("joystick button 1")) && !pushButton)
        {
            pushButton = true;
            GameDirector.nowStage++;
            SceneManager.LoadScene(GameDirector.nowStage);
        }
    }

}
