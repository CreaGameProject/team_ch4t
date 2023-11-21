using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 勝利時に呼び出してください
    public void nextStage()
    {
        int StageUnlock = PlayerPrefs.GetInt("StageUnlock");
        int NowScene = SceneManager.GetActiveScene().buildIndex;   // 現在のシーン番号を取得
        if(StageUnlock < NowScene) //  今のステージ番号が解放済みステージ番号よりも大きい場合（初めてクリアした場合）
        {
            PlayerPrefs.SetInt("StageUnlock", NowScene);   // 解放済みステージ番号を更新
        }
        
        /*
        if (NowScene < SceneManager.sceneCountInBuildSettings)
        {
            if(StageUnlock < NowScene)
            {
                PlayerPrefs.SetInt("StageUnlock", NowScene);
            }
            SceneManager.LoadScene(NowScene);
        }
        else
        {
            SceneManager.LoadScene(0);  // すべてクリア済みのためタイトル画面に遷移
        }
        */
    }
}
