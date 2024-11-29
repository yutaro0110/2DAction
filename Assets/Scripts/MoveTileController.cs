using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTileController : MonoBehaviour
{
    
    public enum MoveType
    {
        None,
        horizon,
        vertical,
        circle,
    }

    public MoveType mType;

    void Start()
    {
        
    }

    
    void Update()
    {

        switch ((int)mType)
        {
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
            default:
                break;
        }

    }

    void horizonControl()
    {

    }

    void verticalControl()
    {

    }

    void circleControl()
    {

    }

}
