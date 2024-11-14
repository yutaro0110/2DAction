using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBase : MonoBehaviour
{
    public int maxHp;
    public int nowHp;

    public enum HitResult
    {
        None,
        AtkDone,
        DefDone,
        Die,
    }

    

    public HitResult result;
    public HitCheck.HitLayer opponent;

    private void Start()
    {
        nowHp = maxHp;
    }
}
