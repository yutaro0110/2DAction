using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum slowDown
    {
        Normal = 15,
        Ice = 1,
    }

    public enum slowDownFlip
    {
        Normal = 20,
        IceWalk = 3,
        IceRun = 4,
        IceDash = 5

    }

    public enum moveCond
    {
        walk,
        run,
        dash,
    }
    int moveCondTemp;
    float moveMax;

    Animator anim;

    Rigidbody2D rb2d;

    BoxCollider2D bcol;

    HitBase hBase;

    GameObject underGroundObj;

    public LayerMask GroundLayer;

    //走る系統の変数
    const float walk = 3.0f;
    const float run = 5.0f;
    const float dash = 8.0f;
    const float dashChangeTime = 1.0f;
    bool flip;
    bool dashMidst;
    float runTime;

    //ジャンプ系
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
                //Debug.Log(hitResult.collider);
                if(i == 1)
                {
                    underGroundObj = hitResult.collider.gameObject;
                }
                //Debug.Log("地面");
                isGround = true;
                
            }
            center.x += width;
            end.x += width;

        }

    }


    //動くか動かないか
    void moveCheck()
    {

        horizon = Input.GetAxisRaw("Horizontal"); //コントローラーにするときに確認

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
        if (Input.GetButtonDown("Jump") && isGround == true) //コントローラーにするときに確認
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
        

        //移動処理

        Vector2 ver = Vector2.zero;

        if(horizon != 0)
        {
            //移動状態(走りなど)を確認
            ver = rb2d.velocity;
            moveCondTemp = (int)moveCond.walk;
            if (Input.GetKey(KeyCode.Z))  //コントローラーにするときに変える
            {
                if(horizon == rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) || rb2d.velocity.x == 0)
                {
                    if (runTime < dashChangeTime)
                    {
                        if (isGround)
                        {
                            runTime += Time.deltaTime;
                        }
                        moveCondTemp = (int)moveCond.run;
                    }
                    else
                    {
                        moveCondTemp = (int)moveCond.dash;
                        runTime = dashChangeTime;
                    }
                }

            }

            //移動
            if(horizon == rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) || rb2d.velocity.x == 0) //velocityと反対に向かったら
            {
                //移動
                switch (moveCondTemp)
                {
                    case 0:
                        runTime = 0;
                        ver.x = horizon * walk;
                        break;
                    case 1:
                        ver.x = horizon * run;
                        break;
                    case 2:
                        ver.x = horizon * dash;
                        break;
                }
                rb2d.velocity = ver;
            }
            else
            {

                //反転処理
                switch (underGroundObj.tag)//床の種類によって滑りを変える
                {
                    //床の確認
                    case "Normal":
                        ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDownFlip.Normal;
                        break;
                    case "Ice":
                        //氷の時はvelocityの値で減速の値を決める
                        if(Mathf.Abs(rb2d.velocity.x) > run)
                        {
                            ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDownFlip.IceDash;
                        }
                        else if(Mathf.Abs(rb2d.velocity.x) > walk)
                        {
                            ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDownFlip.IceRun;
                        }
                        else
                        {
                            ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDownFlip.IceWalk;
                        }
                        
                        break;
                }

                rb2d.AddForce(ver);
            }
            
            
        }
        else
        {
            //減速
            runTime = 0;
            if(Mathf.Abs(rb2d.velocity.x) > 0.5f)
            {
                switch (underGroundObj.tag)//床の種類によって滑りを変える
                {
                    case "Normal":
                        ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDown.Normal;
                        break;
                    case "Ice":
                        ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDown.Ice;
                        break;
                }

                rb2d.AddForce(ver);
            }
            else
            {
                //移動量が小さくなったら0にする
                ver = rb2d.velocity;
                ver.x = 0;
                rb2d.velocity = ver;
            }
        }

        //急ブレーキ
        //空中にいるときどうするか
        //「velocityの反対方向へ入力した時」という処理を入れる?
        //猶予フレームの追加(コントローラーで反転時のフレームを計測する)

        if (isGround)
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
