using System.Collections;
using System.Collections.Generic;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {

        GameObject rivalObj = collision.gameObject;
        HitCheck rivalHit = rivalObj.GetComponent<HitCheck>();

        //地面に当たったりしても反応する
        //実際は確認用(設定してなかった時とか)
        if(rivalHit == null)
        {
            Debug.Log("設定してないものに当たりました");
            return;
        }

        //敵と敵や自分が発射した弾に当たったとき
        if(type == rivalHit.type) { return; }

        //HPがHitBaseに入ってるから取得しておく
        GameObject master = transform.root.gameObject;
        HitBase hBase = master.GetComponent<HitBase>();

        //攻撃側の処理
        if(type == HitType.Attack)
        {
            //HitBase(親)にアタックしたことを伝えている
            hBase.result = HitBase.HitResult.AtkDone;
            return;
        }

        //防御側の処理
        if(type == HitType.Defence)
        {

            hBase.result = HitBase.HitResult.DefDone;
            int atkPow = rivalHit.AtkPow;

            hBase.nowHp -= atkPow;

            //このあたりの処理は自分で考える

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
