using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public LayerMask GroundCheck;

    Animator anim;

    float speed = 2.0f;

    int dir;

    Vector3 offset = new Vector3(0.7f, 0, 0);

    void Start()
    {

        dir = -1;
        anim = GetComponent<Animator>();
        anim.Play("Ene1Run");

    }

    
    void Update()
    {

        front();

        Ene1Controller();

    }

    void front()
    {

        Vector3 center = transform.position;
        Vector3 end = center + offset * dir;

        RaycastHit2D hitResult = Physics2D.Linecast(center, end, GroundCheck);

        Debug.DrawLine(center, end, Color.red);

        if(hitResult.collider != null)
        {
            dir *= -1;
            Vector3 scale = transform.localScale;
            scale.x = dir;
            transform.localScale = scale;
        }

        

    }

    void Ene1Controller()
    {
        transform.position += new Vector3(speed * Time.deltaTime * dir, 0, 0);
    }

}
