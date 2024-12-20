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
    [SerializeField] int dir;
    float t;
    [SerializeField] float shift;

    void Start()
    {
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
                Debug.Log("設定してない");
                break;
        }

        startPos.x -= length / 2.0f;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Foot")
        {
            collision.transform.parent.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.transform.parent.SetParent(null);
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
        t = 4.0f * length * (Time.time / time + shift + 0.25f);
        ver.x = (Mathf.PingPong(t, 2.0f * length) - offset);
        transform.position = ver + startPos;
    }

    void verticalControl()
    {
        t = 4.0f * length * (Time.time / time + shift);
        ver.y = Mathf.PingPong(t, 2.0f * length) - offset + transform.position.x;
        transform.position = ver + startPos;
    }

    void leanControl() //斜め(なんとなくでできただけ)
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
