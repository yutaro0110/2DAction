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

    HitBase hBase;

    public LayerMask GroundLayer;

    const float speed = 5.0f;
    [SerializeField] float JumpPow;
    float EnemyStep;

    bool isGround;
    bool Goal;
    bool move;

    public float horizon;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        bcol = GetComponent<BoxCollider2D>();
        hBase = GetComponent<HitBase>();
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

        //敵を踏んだ時の処理
        step();

    }

    private void FixedUpdate()
    {

        //アニメーションと移動
        AnimControl();

    }


    //地面についているかどうか
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
                Debug.Log(hitResult.collider);
                //Debug.Log("地面");
                isGround = true;
                return;
            }
            center.x += width;
            end.x += width;

        }

    }


    //動くか動かないか
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


    //ジャンプ
    void JumpControl()
    {
        
        //ジャンプ普通のupdateかFixedか決めるまたはFixedの方で反応がいいものを作る
        if (Input.GetButtonDown("Jump") && isGround == true)
        {
            Vector2 vel = rb2d.velocity;
            vel.y = 0;
            rb2d.velocity = vel;
            rb2d.AddForce(Vector2.up * JumpPow);
        }

    }


    //アニメーションと移動
    void AnimControl()
    {
        //移動(変えるかも)
        rb2d.AddForce(Vector3.right * 20 * horizon);

        //↑変更する急ブレーキや左右に動くことを少し考える

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

    
    //ゴールの時(変えるのもあり)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Goal")
        {
            //ゴールした時
            Debug.Log("ゴール");
        }
    }

    //敵を踏んだ時の処理
    void step()
    {

        if(hBase.result == HitBase.HitResult.AtkDone && hBase.opponent == HitCheck.HitLayer.Enemy)
        {
            Vector3 vel = rb2d.velocity;
            EnemyStep = Input.GetButton("Jump") ? 10 : 0;
            vel.y = JumpPow / 100.0f + EnemyStep;
            rb2d.velocity = vel;
            hBase.result = HitBase.HitResult.None;
            
        }

    }

}
