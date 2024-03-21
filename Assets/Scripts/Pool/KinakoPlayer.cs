using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KinakoPlayer : MonoBehaviour
{
    public int speed;
    public KinakoLibrary library;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        // 衝突判定 もしぶつかった相手がアイテムなら
        int itemId = other.gameObject.GetComponent<KinakoItemObject>().id;
        library.OnGetItem(itemId);
    }
}
