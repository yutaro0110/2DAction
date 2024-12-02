using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public LayerMask GroundCheck;

    Rigidbody2D rb2d;

    BoxCollider2D bcol;

    HitBase hBase;

    Animator anim;

    float speed = 2.0f;

    int dir;

    Vector3 offset = new Vector3(0.7f, 0, 0);

    void Start()
    {

        dir = -1;
        anim = GetComponent<Animator>();
        anim.Play("Ene1Run");
        hBase = GetComponent<HitBase>();
        rb2d = GetComponent<Rigidbody2D>();
        bcol = GetComponent<BoxCollider2D>();

    }

    
    void Update()
    {
        if (bcol.enabled == false) return;
        //€‚ñ‚¾‚Æ‚«‚Ìˆ—
        if (hBase.result == HitBase.HitResult.Die && bcol.enabled == true)
        {
            float deathJump = 10.0f;
            rb2d.velocity = Vector2.up * deathJump;
            bcol.enabled = false;
            //€‚ñ‚¾‚Æ‚«‚Éã‚Éã‚ª‚é‚Ì‚ğˆê“x‚¾‚¯‚É‚·‚é

            foreach(Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }

            Destroy(gameObject,2.0f);
            
            
        }

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

    void Hit()
    {
        if (hBase.opponent == HitCheck.HitLayer.Item)
        {
            //‚à‚µHP‚ª2ˆÈã‚Ì“G‚ğì‚é‚ÆHP‚ª‘S‰ñ•œ‚·‚é
            hBase.nowHp = hBase.maxHp;
        }
    }


}
