using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinakoLibrary : MonoBehaviour
{
    public KinakoItemData[] itemDataArray = new[]
    {
        new KinakoItemData(0, "回復薬", "体力が10回復する"),
        new KinakoItemData(1, "瞬足", "コーナーで差をつけろ！"),
        new KinakoItemData(2, "酒", "飲み過ぎには注意"),
        new KinakoItemData(3, "宝石", "高い")
    };

    public int[] scoreArray = new int[4];

    private void Start()
    {
        for (int i = 0; i < scoreArray.Length; i++)
        {
            scoreArray[i] = 0;
        }
        
        OnGetItem(1);
    }

    public void OnGetItem(int id)
    {
        scoreArray[id] += 1;
    }
}
