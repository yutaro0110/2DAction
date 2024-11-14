using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator anim;

    Rigidbody2D rb2d;

    BoxCollider2D bcol;

    public LayerMask GroundLayer;

    const float speed = 4f;
    [SerializeField] float JumpPow;

    bool isGround;
    bool Goal;
    bool move;

    public float horizon;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        bcol = GetComponent<BoxCollider2D>();
        Goal = false;
    }

    
    void Update()
    {

        //地面についてるか
        GroundCheck();

        //左右キーを押してるか
        moveCheck();

        //ジャンプ
        JumpControl();


    }

    private void FixedUpdate()
    {

        //アニメーションと移動
        AnimControl();

    }

    void GroundCheck()
    {
        //falseにしたほうがよさそう(よくわからん)
        isGround = false;

        Vector3 center = transform.position;
        Vector3 end = center + Vector3.down * 1.1f;
        float width = bcol.size.x / 2.0f;
        center.x -= width;
        end.x -= width;


        //地面に触れている判定
        for(int i = 0;i < 3; i++)
        {
            RaycastHit2D hitResult = Physics2D.Linecast(center, end, GroundLayer);
            Debug.DrawLine(center, end, Color.red);

            if(hitResult.collider != null)
            {
                //Debug.Log("地面");
                isGround = true;
                return;
            }
            center.x += width;
            end.x += width;

        }

    }

    void moveCheck()
    {

        horizon = Input.GetAxisRaw("Horizontal");

        if(horizon != 0)
        {
            move = true;
        }
        else
        {
            move = false;
        }

    }

    void JumpControl()
    {
        

        if (Input.GetButtonDown("Jump") && isGround == true)
        {
            rb2d.AddForce(Vector2.up * JumpPow);
        }

    }


    void AnimControl()
    {   
        //移動(変えるかも)
        transform.position += new Vector3(speed * horizon * Time.deltaTime, 0, 0);

        if(isGround)
        {
            
            if(horizon != 0)
            {
                anim.Play("PLRun");
            }
            else
            {
                //止まっているとき
                anim.Play("PLIdle");
            }
            
        }
        else
        {
            anim.Play("PLJump");
        }

        if(horizon != 0)
        {
            //playerの絵を反転
            Vector3 scale = transform.localScale;
            scale.x = horizon;
            transform.localScale = scale;
        }

        

    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Goal")
        {
            //ゴールした時
            Debug.Log("ゴール");
        }
    }

}
