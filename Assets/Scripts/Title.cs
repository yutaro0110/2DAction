using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{

    [SerializeField] GameObject arrow;

    int posNo;
    float select;
    Vector3[] arrowPos = new Vector3[2];


    void Start()
    {
        
    }

    
    void Update()
    {

        SelectArrow();

    }

    void SelectArrow()
    {

        

        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            
        }

    }
}
