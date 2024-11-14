using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitCheck : MonoBehaviour
{
    
    public enum HitType
    {
        Attack,Defence,
    }

    public enum HitLayer
    {
        Player,Enemy,Item,
    }

    public int AtkPow;
    public HitType type;
    public HitLayer layer;

    private void Start()
    {
        if(type == HitType.Defence)
        {
            AtkPow = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        GameObject rivalObj = collision.gameObject;
        HitCheck rivalHit = rivalObj.GetComponent<HitCheck>();

        //�n�ʂɓ��������肵�Ă���������
        //���ۂ͊m�F�p(�ݒ肵�ĂȂ��������Ƃ�)
        if(rivalHit == null)
        {
            Debug.Log("�ݒ肵�ĂȂ����̂ɓ�����܂���");
            return;
        }

        //�G�ƓG�⎩�������˂����e�ɓ��������Ƃ�
        if(type == rivalHit.type) { return; }

        

        //HP��HitBase�ɓ����Ă邩��擾���Ă���
        GameObject master = transform.root.gameObject;
        HitBase hBase = master.GetComponent<HitBase>();

        //�U�����̏���
        if(type == HitType.Attack)
        {
            //HitBase(�e)�ɃA�^�b�N�������Ƃ�`���Ă���
            hBase.result = HitBase.HitResult.AtkDone;
            //�N�ɓ���������
            hBase.opponent = rivalHit.layer;
            return;
        }

        //�h�䑤�̏���
        if(type == HitType.Defence)
        {
            //�e�ɃA�^�b�N�������Ƃ�`����
            hBase.result = HitBase.HitResult.DefDone;
            //�N�ɓ���������
            hBase.opponent = rivalHit.layer; 
            int atkPow = rivalHit.AtkPow;

            hBase.nowHp -= atkPow;

            //���̂�����̏����͎����ōl����

            if(atkPow > 0)
            {
                if(hBase.nowHp < 0)
                {
                    hBase.nowHp = 0;
                    hBase.result = HitBase.HitResult.Die;
                }
            }


        }

        if(hBase.nowHp <= 0)
        {
            hBase.result = HitBase.HitResult.Die;
        }

    }


}
