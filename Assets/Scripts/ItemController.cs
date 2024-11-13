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
            UIController.score = (int)itemName;
            Instantiate(itemScore,transform.position,Quaternion.identity);
            //PlayerÇ…ìñÇΩÇ¡ÇΩÇÁçÌèú
            Destroy(gameObject);
        }

    }
}
