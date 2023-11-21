using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSet : MonoBehaviour
{   
    // スタート画面等最初にロードされるシーンで実行してください
    // Start is called before the first frame update
    void Start()
    {
        // "StageUnlock"にプレイヤーが解放したステージ番号を保存(ゲームスタート時に1にする)
        
        PlayerPrefs.SetInt("StageUnlock", 1);
        Debug.Log("現在のステージ番号" + PlayerPrefs.GetInt("StageUnlock"));

        PlayerPrefs.SetInt("Reports", 0);
        Debug.Log("現在のレポート数" + PlayerPrefs.GetInt("Reports"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
