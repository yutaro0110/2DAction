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

    //����n���̕ϐ�
    const float walk = 3.0f;
    const float run = 5.0f;
    const float dash = 8.0f;
    const float dashChangeTime = 1.0f;
    bool flip;
    bool dashMidst;
    float runTime;

    //�W�����v�n
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

        //�n�ʂɂ��Ă邩
        GroundCheck();

        //���E�L�[�������Ă邩
        moveCheck();

        //�W�����v
        JumpControl();

        //�G�𓥂񂾎��̏���
        step();

    }

    private void FixedUpdate()
    {

        //�A�j���[�V�����ƈړ�
        AnimControl();

    }


    //�n�ʂɂ��Ă��邩�ǂ���
    void GroundCheck()
    {
        //false�ɂ����ق����悳����(�悭�킩���)
        isGround = false;

        Vector3 center = transform.position;
        Vector3 end = center + Vector3.down * 1.1f;
        float width = bcol.size.x / 2.0f;
        center.x -= width;
        end.x -= width;


        //�n�ʂɐG��Ă��锻��
        for(int i = 0;i < 3; i++)
        {
            RaycastHit2D hitResult = Physics2D.Linecast(center, end, GroundLayer);
            Debug.DrawLine(center, end, Color.red);

            if(hitResult.collider != null)
            {
                //Debug.Log(hitResult.collider);
                underGroundObj = hitResult.collider.gameObject;
                //Debug.Log("�n��");
                isGround = true;
                return;
            }
            center.x += width;
            end.x += width;

        }

    }


    //�����������Ȃ���
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


    //�W�����v
    void JumpControl()
    {
        
        //�W�����v���ʂ�update��Fixed�����߂�܂���Fixed�̕��Ŕ������������̂����
        if (Input.GetButtonDown("Jump") && isGround == true)
        {
            Vector2 vel = rb2d.velocity;
            vel.y = 0;
            rb2d.velocity = vel;
            rb2d.AddForce(Vector2.up * JumpPow);
        }

    }


    //�A�j���[�V�����ƈړ�
    void AnimControl()
    {
        //�ړ�(�ς��邩��)
        //�i�K������if����

        ////������܂悭�Ȃ����ǂ��傤���Ȃ�velocity�ۑ�
        //Vector2 v = rb2d.velocity;


        ////���E��������Ă�����
        //if (horizon != 0)
        //{

        //    //Z�Ń_�b�V��
        //    if (Input.GetKey(KeyCode.Z))
        //    {
        //        //��i�K�ڂ̑��x������
        //        v.x = run * horizon;

        //        //�_�b�V������莞�ԉ����Ȃ���i��ł����瑬�x���㏑�����ē�i�K�ڂ̑��x������
        //        runTime = (runTime + Time.deltaTime) > dashChangeTime ? dashChangeTime : runTime + Time.deltaTime;//�����Ă��鎞�Ԍv��
        //        if (runTime >= dashChangeTime)
        //        {
        //            moveCondTemp = (int)moveCond.dash;
        //            v.x = dash * horizon;
        //            dashMidst = true;
        //        }
        //        else
        //        {
        //            moveCondTemp = (int)moveCond.run;
        //        }

        //    }
        //    else
        //    {
        //        //���ʂ̕���
        //        v.x = walk * horizon;
        //        runTime = 0;
        //        moveCondTemp = (int)moveCond.walk;
        //    }

        //}
        //else
        //{
        //    //��������
        //    runTime = 0;


        //    if(rb2d.velocity.x != 0)
        //    {
        //        //���ꂲ�ƂɌ����𒲐�(�}�W�b�N�i���o�[enum�������������ĂȂ�����)
        //        switch (underGroundObj.tag)
        //        {
        //            case "Normal":
        //                v.x = v.x + (((float)slowDown.Normal / 10.0f) * (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * 1.0f));
        //                break;
        //            case "Ice":
        //                v.x = v.x + (((float)slowDown.Ice / 10.0f) * (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * 1.0f));
        //                break;
        //        }
        //    }

        //    ////�����ɂ�肯��ōŌ�̌����̕�����ς���(��΂Ƀ_��)
        //    //if(transform.localScale.x == 1)
        //    //{
        //    //    v.x = v.x < 0 ? 0 : v.x;
        //    //}
        //    //else
        //    //{
        //    //    v.x = v.x > 0 ? 0 : v.x;
        //    //}
        //}



        ////x��velocity������������̂��_��+=�ɂ�����
        //rb2d.velocity = v;
        //rb2d.AddForce(new Vector2(slow, 0));

        //if(rb2d.velocity.x < 0.01f)
        //{
        //    rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        //}

        //switch (moveCondTemp)
        //{
        //    case (int)moveCond.walk:
        //        moveMax = walk;
        //        break;
        //    case (int)moveCond.run:
        //        moveMax = run;
        //        break;
        //    case (int)moveCond.dash:
        //        moveMax = dash;
        //        break;
        //}

        //�ړ�������蒼��

        Vector2 ver = Vector2.zero;

        if(horizon != 0)
        {
            //�ړ����(����Ȃ�)���m�F
            ver = rb2d.velocity;
            moveCondTemp = (int)moveCond.walk;
            if (Input.GetKey(KeyCode.Z))
            {
                if(runTime < dashChangeTime)
                {
                    runTime += Time.deltaTime;
                    moveCondTemp = (int)moveCond.run;
                }
                else
                {
                    moveCondTemp = (int)moveCond.dash;
                    runTime = dashChangeTime;
                }
            }

            //�ړ�
            if(horizon == rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) || rb2d.velocity.x == 0) //velocity�Ɣ��΂Ɍ���������
            {
                //�ړ�
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
                
                //���]����


            }
            
            
        }
        else
        {
            //����
            runTime = 0;
            if(Mathf.Abs(rb2d.velocity.x) > 0.5f)
            {
                switch (underGroundObj.tag)//���̎�ނɂ���Ċ����ς���
                {
                    case "Normal":
                        ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDown.Normal;
                        Debug.Log(ver.x);
                        break;
                    case "Ice":
                        ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDown.Ice;
                        Debug.Log(ver.x);
                        break;
                }

                rb2d.AddForce(ver);
            }
            else
            {
                //�ړ��ʂ��������Ȃ�����0�ɂ���
                ver = rb2d.velocity;
                ver.x = 0;
                rb2d.velocity = ver;
            }
        }

        //�}�u���[�L
        //�ړ������𔽓]������Ƃ��ɗP�\��^���ėP�\�̊Ԃɔ��]�̓��͂��s��ꂽ��X�s�[�hMAX�̂܂ܓ���
        //��(�n��)���ƂɌ�����ς���(���ׂ�悤�ɂ�����)
        //���]����bool�Ŕ��f������
        //���]�����瑬�x���Ȃ��Ȃ��Ă��瓮���o��
        //���]���Ă���^�C�~���O�͑���������Ȃ�����?
        //�󒆂ɂ���Ƃ��ǂ����邩
        //�_�b�V����������Ɉڍs�����Ƃ��ǂ����邩
        //���ׂ邱�Ƃ͂ł��Ă��邪���]�����Ƃ��ɕ��ʂɓ�����̂�����
        //�uvelocity�̔��Ε����֓��͂������v�Ƃ�������������?

        if (isGround)
        {
            
            if(horizon != 0)
            {
                anim.Play("PLRun");
            }
            else
            {
                //�~�܂��Ă���Ƃ�
                anim.Play("PLIdle");
            }
            
        }
        else
        {
            anim.Play("PLJump");
        }

        if(horizon != 0)
        {
            //player�̊G�𔽓]
            Vector3 scale = transform.localScale;
            scale.x = horizon;
            transform.localScale = scale;
        }

        

    }

    
    //�S�[���̎�(�ς���̂�����)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Goal")
        {
            //�S�[��������
            Debug.Log("�S�[��");
        }
    }

    //�G�𓥂񂾎��̏���
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
