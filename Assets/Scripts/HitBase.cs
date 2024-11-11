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
        Atk,
        Def,
        Heal,
        Die,
    }

    public HitResult result;

    private void Start()
    {
        nowHp = maxHp;
    }
}
