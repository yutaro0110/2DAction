using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGetScoreController : MonoBehaviour
{

    Vector3 speed = new Vector3(0, 1, 0);

    void Start()
    {
        Destroy(gameObject, 1.3f);
    }

    
    void Update()
    {

        transform.position += speed * Time.deltaTime;

    }
}
