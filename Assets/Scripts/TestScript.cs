using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestScript : MonoBehaviour
{

    float length = 2;

    float max;
    float min;

    float value;

    void Start()
    {
        max = -999;
        min = 999;
    }

    void Update()
    {
        value = Mathf.PingPong(Time.time, length);

        

        // yÀ•W‚ð‰•œ‚³‚¹‚Äã‰º‰^“®‚³‚¹‚é
        transform.localPosition = new Vector3(0, value, 0);

    }




    void maxmin()
    {
        if(value > max)
        {
            max = value;
        }
        if(value < min)
        {
            min = value;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("MIN =" + min + " , MAX = " + max);
        }
    }

}
