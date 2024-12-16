using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public enum slowDown
    {
        Normal = 15,
        Ice = 4,
    }

    public enum slowDownFlip
    {
        Normal = 20,
        IceWalk = 6,
        IceRun = 8,
        IceDash = 10

    }

    public enum moveCond
    {
        walk,
        run,
        dash,
    }
    int moveCondTemp;

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
    float runTime;
    bool frontGround;
    const int dashFrame = 5;  //反転時の猶予フレーム
    int dashFrameCnt;          //フレームカウント
    bool dashHanten;           //ダッシュ後の反転時にスピードを残す用
    bool controlStop;          //反転時の操作止める
    float flipDir;                     //反転時どっちに進んでいたか保存



    //ジャンプ系
    [SerializeField] float JumpPow;
    float dieJump;
    float EnemyStep;
    [SerializeField] AudioSource JumpAud;


    //その他
    bool isGround;              //地面に触れているか
    bool Goal;                  //ゴール判定
    bool move;                  //動くか動かないか
    bool die;                   //死んだかどうか
    Vector3 diestop;            //死んだあとの硬直(演出用)
    GameDirector DirectorFunc;  //関数使用用



    public float horizon;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        bcol = GetComponent<BoxCollider2D>();
        hBase = GetComponent<HitBase>();
        Goal = false;
        die = false;
        dieJump = JumpPow + 100.0f;
        dashFrameCnt = 0;
    }

    
    void Update()
    {

        if (Goal) return;

        if (die)
        {
            DirectorFunc.deathRestart();
            diestop.y = transform.position.y;
            transform.position = diestop;

            return;
        }

        //地面についてるか
        GroundCheck();

        //左右キーを押してるか
        moveCheck();

        //ジャンプ
        JumpControl();

        //敵を踏んだ時の処理
        step();

        DieControl();

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
        horizon = Mathf.Abs(horizon) < 0.3f ? 0 : horizon;
        if(horizon != 0)
        {
            horizon = horizon < 0 ? -1 : 1;
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
        if (Input.GetKeyDown("joystick button 1") && isGround == true) //コントローラーにするときに確認
        {
            Vector2 vel = rb2d.velocity;
            vel.y = 0;
            rb2d.velocity = vel;
            rb2d.AddForce(Vector2.up * JumpPow);
            JumpAud.Play();
            controlStop = false;
        }

    }


    //アニメーションと移動
    void AnimControl()
    {


        if (isGround)
        {

            if (horizon != 0 && !frontGround)
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

        if (horizon != 0)
        {
            //playerの絵を反転
            Vector3 scale = transform.localScale;
            scale.x = horizon < 0 ? -1 : 1;
            transform.localScale = scale;
        }

        //移動処理

        Vector2 ver = Vector2.zero;

        if(horizon != 0)
        {
            ////操作できるか(急ブレーキ中か)
            //if(rb2d.velocity.x == 0)
            //{
            //    controlStop = false;
            //}
            //else if (controlStop == true && flipDir / Mathf.Abs(flipDir) != rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x))
            //{
                
            //    controlStop = false;
                
            //}


            Vector3 offset = new Vector3(transform.localScale.x * 0.7f, -0.3f, 0);
            Vector3 center = transform.position;
            Vector3 end = center + offset;
            center.y += offset.y;

            RaycastHit2D RunResult = Physics2D.Linecast(center, end, GroundLayer);

            Debug.DrawLine(center, end, Color.red);

            if (RunResult.collider == null)
            {
                frontGround = false;
                //移動状態(走りなど)を確認(これが大切!!)
                moveCondTemp = (int)moveCond.walk;
                ver = rb2d.velocity;
                if (Input.GetKey("joystick button 0"))  //ダッシュ確認
                {
                    if (horizon == rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) || rb2d.velocity.x == 0) //velocityと反対に向かったら
                    {

                        if (dashHanten)
                        {
                            runTime = dashChangeTime;
                        }

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
                            dashHanten = true;
                            dashFrameCnt = 0;
                            moveCondTemp = (int)moveCond.dash;
                            runTime = dashChangeTime;
                        }
                    }

                }
                else
                {
                    dashHanten = false;
                }

                //移動処理(velocityやAddForceをする場所)(反転処理と移動処理の判断)
                if ((horizon == rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x)|| rb2d.velocity.x == 0) 
                                                         ) //velocityと反対に向かったらelse
                {
                    //普通の移動処理
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
                    //if(rb2d.velocity.x == 0)//velocityのXが0の時に処理を終了
                    //{
                    //    return;
                    //}

                    //if(controlStop == false)
                    //{
                    //    flipDir = rb2d.velocity.x;
                    //}

                    controlStop = true;

                    if (underGroundObj == null) return;

                    switch (underGroundObj.tag)//床の種類によって滑りを変える
                    {
                        //床の確認
                        case "Normal":
                            ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDownFlip.Normal;
                            break;
                        case "Ice":
                            //氷の時はvelocityの値で減速の値を決める
                            if (Mathf.Abs(rb2d.velocity.x) > run)
                            {
                                ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDownFlip.IceDash;
                            }
                            else if (Mathf.Abs(rb2d.velocity.x) > walk)
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
                //壁にぶつかったとき
                runTime = 0;
                frontGround = true;
                dashHanten = false;
            }
            
        }
        else
        {
            if (dashHanten)
            {
                dashFrameCnt++;
            }

            if(dashFrameCnt > dashFrame)
            {
                dashHanten = false;
                dashFrameCnt = 0;
            }
            //減速
            runTime = 0;
            if(Mathf.Abs(rb2d.velocity.x) > 0.5f)
            {
                if (underGroundObj == null) return;
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
        //猶予フレームの追加(コントローラーで反転時のフレームを計測する)
        //歩きの時の反転を小さくしてもいい
        //反転をどうやるか
        //デッドゾーンは必要か
        //氷の減速処理変える
        //ダッシュ中に反転がたくさんできちゃう
        //反転したときにvelocityがどっちの方向に力を持っているか保存する

    }

    
    //ゴールの時(変えるのもあり)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Goal) return;

        if (collision.tag == "Goal")
        {
            //ゴールした時
            Debug.Log("ゴール");
            Goal = true;
            GameDirector.nowStage++;
            SceneManager.LoadScene(GameDirector.nowStage);
        }
    }

    //敵を踏んだ時の処理
    void step()
    {

        if(hBase.result == HitBase.HitResult.AtkDone && hBase.opponent == HitCheck.HitLayer.Enemy)
        {

            Vector3 vel = rb2d.velocity;
            EnemyStep = Input.GetKey("joystick button 1") ? 10 : 0;
            vel.y = JumpPow / 100.0f + EnemyStep;
            rb2d.velocity = vel;
            hBase.result = HitBase.HitResult.None;
            
        }

    }

    void DieControl()
    {

        Vector2 min = Camera.main.ViewportToWorldPoint(Vector2.zero);

        if(transform.position.y < min.y || hBase.result == HitBase.HitResult.Die)
        {
            die = true;
            StartCoroutine(DeathCoroutine());
        }

    }

    IEnumerator DeathCoroutine()
    {
        
        //死んだ
        diestop = transform.position;
        rb2d.velocity = Vector2.zero;
        bcol.enabled = false;
        horizon = 0;
        anim.Play("PLIdle");

        foreach(Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }


        //重力なくす(重力保存)
        float g = rb2d.gravityScale;
        rb2d.gravityScale = 0;

        //待機
        yield return new WaitForSeconds(0.5f);

        //重力戻す
        rb2d.gravityScale = g;
        //ジャンプ
        rb2d.AddForce(Vector2.up * dieJump);
        hBase.nowHp = 0;

        diestop.x = transform.position.x;

        StopCoroutine(DeathCoroutine());
        yield break;
        
    }

    

}
