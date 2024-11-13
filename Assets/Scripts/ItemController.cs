using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    HitBase hBase;

    [SerializeField] item itemName;

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
            //PlayerÇ…ìñÇΩÇ¡ÇΩÇÁçÌèú
            Destroy(gameObject);
        }

    }
}
