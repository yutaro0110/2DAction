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
                if(i == 1)
                {
                    underGroundObj = hitResult.collider.gameObject;
                }
                //Debug.Log("�n��");
                isGround = true;
                
            }
            center.x += width;
            end.x += width;

        }

    }


    //�����������Ȃ���
    void moveCheck()
    {

        horizon = Input.GetAxisRaw("Horizontal"); //�R���g���[���[�ɂ���Ƃ��Ɋm�F

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
        if (Input.GetButtonDown("Jump") && isGround == true) //�R���g���[���[�ɂ���Ƃ��Ɋm�F
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
        

        //�ړ�����

        Vector2 ver = Vector2.zero;

        if(horizon != 0)
        {
            //�ړ����(����Ȃ�)���m�F
            ver = rb2d.velocity;
            moveCondTemp = (int)moveCond.walk;
            if (Input.GetKey(KeyCode.Z))  //�R���g���[���[�ɂ���Ƃ��ɕς���
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
                switch (underGroundObj.tag)//���̎�ނɂ���Ċ����ς���
                {
                    //���̊m�F
                    case "Normal":
                        ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDownFlip.Normal;
                        break;
                    case "Ice":
                        //�X�̎���velocity�̒l�Ō����̒l�����߂�
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
            //����
            runTime = 0;
            if(Mathf.Abs(rb2d.velocity.x) > 0.5f)
            {
                switch (underGroundObj.tag)//���̎�ނɂ���Ċ����ς���
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
                //�ړ��ʂ��������Ȃ�����0�ɂ���
                ver = rb2d.velocity;
                ver.x = 0;
                rb2d.velocity = ver;
            }
        }

        //�}�u���[�L
        //�󒆂ɂ���Ƃ��ǂ����邩
        //�uvelocity�̔��Ε����֓��͂������v�Ƃ�������������?
        //�P�\�t���[���̒ǉ�(�R���g���[���[�Ŕ��]���̃t���[�����v������)

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
