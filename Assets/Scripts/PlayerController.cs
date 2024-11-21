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
        Normal = -5,
        Ice = -1,
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
                Debug.Log(hitResult.collider);
                underGroundObj = hitResult.collider.gameObject;
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
        //段階をつけるifから

        //↓あんまよくないけどしょうがないvelocity保存
        Vector2 v = rb2d.velocity;

        //左右が押されていたら
        if (horizon != 0)
        {

            //Zでダッシュ
            if (Input.GetKey(KeyCode.Z))
            {
                //一段階目の速度を入れる
                v.x = run * horizon;

                //ダッシュを一定時間押しながら進んでいたら速度を上書きして二段階目の速度を入れる
                runTime = (runTime + Time.deltaTime) > dashChangeTime ? dashChangeTime : runTime + Time.deltaTime;
                if (runTime >= dashChangeTime)
                {
                    moveCondTemp = (int)moveCond.dash;
                    v.x = dash * horizon;
                    dashMidst = true;
                }
                else
                {
                    moveCondTemp = (int)moveCond.run;
                }
                
            }
            else
            {
                //普通の歩き
                v.x = walk * horizon;
                runTime = 0;
                moveCondTemp = (int)moveCond.walk;
            }

        }
        else
        {
            //減速処理
            runTime = 0;

            //足場ごとに減速を調整(マジックナンバーenumが小数を扱ってないから)
            switch (underGroundObj.tag)
            {
                case "Normal":
                    v.x = v.x + ((float)slowDown.Normal / 10.0f) * transform.localScale.x;
                    break;
                case "Ice":
                    v.x = v.x + ((float)slowDown.Ice / 10.0f) * transform.localScale.x;
                    break;
            }

            //方向によりけりで最後の減速の部分を変える
            if(transform.localScale.x == 1)
            {
                v.x = v.x < 0 ? 0 : v.x;
            }
            else
            {
                v.x = v.x > 0 ? 0 : v.x;
            }
        }

        
        //xのvelocityを初期化するのがダメ
        rb2d.velocity = v;

        switch (moveCondTemp)
        {
            case (int)moveCond.walk:
                moveMax = walk;
                break;
            case (int)moveCond.run:
                moveMax = run;
                break;
            case (int)moveCond.dash:
                moveMax = dash;
                break;
        }

        //急ブレーキ
        //移動方向を反転させるときに猶予を与えて猶予の間に反転の入力が行われたらスピードMAXのまま動く
        //床(地面)ごとに減速を変える(すべるようにしたり)
        //反転時はboolで判断したい
        //反転したら速度がなくなってから動き出す
        //反転しているタイミングは操作を聞かなくする?
        //空中にいるときどうするか
        //ダッシュから歩きに移行したときどうするか
        //すべることはできているが反転したときに普通に動けるのを治す

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
