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

    //����n���̕ϐ�
    const float walk = 3.0f;
    const float run = 5.0f;
    const float dash = 8.0f;
    const float dashChangeTime = 1.0f;
    float runTime;
    bool frontGround;
    const int dashFrame = 5;  //���]���̗P�\�t���[��
    int dashFrameCnt;          //�t���[���J�E���g
    bool dashHanten;           //�_�b�V����̔��]���ɃX�s�[�h���c���p
    bool controlStop;          //���]���̑���~�߂�
    float flipDir;                     //���]���ǂ����ɐi��ł������ۑ�



    //�W�����v�n
    [SerializeField] float JumpPow;
    float dieJump;
    float EnemyStep;
    [SerializeField] AudioSource JumpAud;


    //���̑�
    bool isGround;              //�n�ʂɐG��Ă��邩
    bool Goal;                  //�S�[������
    bool move;                  //�����������Ȃ���
    bool die;                   //���񂾂��ǂ���
    Vector3 diestop;            //���񂾂��Ƃ̍d��(���o�p)
    GameDirector DirectorFunc;  //�֐��g�p�p



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

        //�n�ʂɂ��Ă邩
        GroundCheck();

        //���E�L�[�������Ă邩
        moveCheck();

        //�W�����v
        JumpControl();

        //�G�𓥂񂾎��̏���
        step();

        DieControl();

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


    //�W�����v
    void JumpControl()
    {
        
        //�W�����v���ʂ�update��Fixed�����߂�܂���Fixed�̕��Ŕ������������̂����
        if (Input.GetKeyDown("joystick button 1") && isGround == true) //�R���g���[���[�ɂ���Ƃ��Ɋm�F
        {
            Vector2 vel = rb2d.velocity;
            vel.y = 0;
            rb2d.velocity = vel;
            rb2d.AddForce(Vector2.up * JumpPow);
            JumpAud.Play();
            controlStop = false;
        }

    }


    //�A�j���[�V�����ƈړ�
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
                //�~�܂��Ă���Ƃ�
                anim.Play("PLIdle");
            }

        }
        else
        {
            anim.Play("PLJump");
        }

        if (horizon != 0)
        {
            //player�̊G�𔽓]
            Vector3 scale = transform.localScale;
            scale.x = horizon < 0 ? -1 : 1;
            transform.localScale = scale;
        }

        //�ړ�����

        Vector2 ver = Vector2.zero;

        if(horizon != 0)
        {
            ////����ł��邩(�}�u���[�L����)
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
                //�ړ����(����Ȃ�)���m�F(���ꂪ���!!)
                moveCondTemp = (int)moveCond.walk;
                ver = rb2d.velocity;
                if (Input.GetKey("joystick button 0"))  //�_�b�V���m�F
                {
                    if (horizon == rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) || rb2d.velocity.x == 0) //velocity�Ɣ��΂Ɍ���������
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

                //�ړ�����(velocity��AddForce������ꏊ)(���]�����ƈړ������̔��f)
                if ((horizon == rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x)|| rb2d.velocity.x == 0) 
                                                         ) //velocity�Ɣ��΂Ɍ���������else
                {
                    //���ʂ̈ړ�����
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
                    //if(rb2d.velocity.x == 0)//velocity��X��0�̎��ɏ������I��
                    //{
                    //    return;
                    //}

                    //if(controlStop == false)
                    //{
                    //    flipDir = rb2d.velocity.x;
                    //}

                    controlStop = true;

                    if (underGroundObj == null) return;

                    switch (underGroundObj.tag)//���̎�ނɂ���Ċ����ς���
                    {
                        //���̊m�F
                        case "Normal":
                            ver.x = (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x) * -1.0f) * (int)slowDownFlip.Normal;
                            break;
                        case "Ice":
                            //�X�̎���velocity�̒l�Ō����̒l�����߂�
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
                //�ǂɂԂ������Ƃ�
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
            //����
            runTime = 0;
            if(Mathf.Abs(rb2d.velocity.x) > 0.5f)
            {
                if (underGroundObj == null) return;
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
        //�P�\�t���[���̒ǉ�(�R���g���[���[�Ŕ��]���̃t���[�����v������)
        //�����̎��̔��]�����������Ă�����
        //���]���ǂ���邩
        //�f�b�h�]�[���͕K�v��
        //�X�̌��������ς���
        //�_�b�V�����ɔ��]����������ł����Ⴄ
        //���]�����Ƃ���velocity���ǂ����̕����ɗ͂������Ă��邩�ۑ�����

    }

    
    //�S�[���̎�(�ς���̂�����)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Goal) return;

        if (collision.tag == "Goal")
        {
            //�S�[��������
            Debug.Log("�S�[��");
            Goal = true;
            GameDirector.nowStage++;
            SceneManager.LoadScene(GameDirector.nowStage);
        }
    }

    //�G�𓥂񂾎��̏���
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
        
        //����
        diestop = transform.position;
        rb2d.velocity = Vector2.zero;
        bcol.enabled = false;
        horizon = 0;
        anim.Play("PLIdle");

        foreach(Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }


        //�d�͂Ȃ���(�d�͕ۑ�)
        float g = rb2d.gravityScale;
        rb2d.gravityScale = 0;

        //�ҋ@
        yield return new WaitForSeconds(0.5f);

        //�d�͖߂�
        rb2d.gravityScale = g;
        //�W�����v
        rb2d.AddForce(Vector2.up * dieJump);
        hBase.nowHp = 0;

        diestop.x = transform.position.x;

        StopCoroutine(DeathCoroutine());
        yield break;
        
    }

    

}
