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

        if(type == HitType.Attack)
        {
            
        }

    }


}
