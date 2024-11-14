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
                Debug.Log("ìGÇ…ìñÇΩÇ¡ÇΩ");
                hBase.result = HitBase.HitResult.None;
                hBase.nowHp = hBase.maxHp;
                return;
            }
            GameDirector.score = (int)itemName;
            Instantiate(itemScore,transform.position,Quaternion.identity);
            //PlayerÇ…ìñÇΩÇ¡ÇΩÇÁçÌèú
            Destroy(gameObject);
        }

    }
}
