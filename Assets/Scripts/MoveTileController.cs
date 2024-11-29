using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTileController : MonoBehaviour
{
    
    public enum MoveType
    {
        None = 0,
        horizon,
        vertical,
        circle,
    }

    public MoveType mType;

    Vector3 ver;

    float offset;
    public float length;

    void Start()
    {

        offset = length / 2.0f;
        

        switch ((int)mType)
        {
            case 1:
                ver = transform.position;
                break;
            case 0:
                ver = transform.position;
                break;
            case 3:

                break;
            default:
                Debug.Log("ê›íËÇµÇƒÇ»Ç¢");
                break;
        }

    }

    
    void Update()
    {

        switch ((int)mType)
        {
            case 1:
                horizonControl();
                break;
            case 2:
                verticalControl();
                break;
            case 3:
                circleControl();
                break;
            default:
                break;
        }

    }

    void horizonControl()
    {
        ver.x = Mathf.PingPong(Time.time, length) - offset;
        transform.position = ver;
    }

    void verticalControl()
    {

    }

    void circleControl()
    {

    }

}
