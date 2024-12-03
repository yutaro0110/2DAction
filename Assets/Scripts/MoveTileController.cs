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
        lean,
        circle,
    }

    [SerializeField] MoveType mType;

    Vector3 ver;
    Vector3 startPos;

    float offset;
    [SerializeField] float length;
    [SerializeField] float time;
    float t;
    float shift;

    void Start()
    {
        shift = 0.25f;
        offset = length / 2.0f;
        

        switch ((int)mType)
        {
            case 1:
                startPos = transform.position;
                break;
            case 0:
                startPos = transform.position;
                break;
            case 3:
                startPos = transform.position;
                break;
            case 4:
                
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
                leanControl();
                break;
            default:
                break;

        }

    }

    void horizonControl()
    {
        t = 4.0f * length * (Time.time / time + shift);
        ver.x = Mathf.PingPong(t, 2.0f * length) - offset;
        transform.position = ver + startPos;
    }

    void verticalControl()
    {
        t = 4.0f * length * (Time.time / time + shift);
        ver.y = Mathf.PingPong(t, 2.0f * length) - offset + transform.position.x;
        transform.position = ver + startPos;
    }

    void leanControl() //éŒÇﬂ(Ç»ÇÒÇ∆Ç»Ç≠Ç≈Ç≈Ç´ÇΩÇæÇØ)
    {
        t = 4.0f * length * (Time.time / time + shift);
        ver.x = Mathf.PingPong(t, 2.0f * length) - offset;

        t = 4.0f * length * (Time.time / time + shift);
        ver.y = Mathf.PingPong(t, 2.0f * length) - offset;

        transform.position = ver + startPos;
    }

    void circleControl()
    {

    }

}
