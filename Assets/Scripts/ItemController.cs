using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    HitBase hBase;

    [SerializeField] item itemName;

    [SerializeField] GameObject itemScore;

    enum item
    {
        Cherry = 100,
    }

    void Start()
    {
        hBase = GetComponent<HitBase>();
    }

    
    
    void Update()
    {
        
        if(hBase.result == HitBase.HitResult.AtkDone)
        {
            if(hBase.opponent == HitCheck.HitLayer.Enemy)
            {
                //Debug.Log("敵に当たった");
                hBase.result = HitBase.HitResult.None;
                hBase.nowHp = hBase.maxHp;
                return;
            }
            GameDirector.score += (int)itemName;
            Instantiate(itemScore,transform.position,Quaternion.identity);
            //Playerに当たったら削除
            Destroy(gameObject);
        }

    }
}
