using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private Button[] stageButtons;

    void Start()
    {
        // "StageUnlock"にプレイヤーが解放したステージ番号を保存
        int stageUnlock = PlayerPrefs.GetInt("StageUnlock", 1);
        Debug.Log("現在のステージ番号" + stageUnlock);
        

        // ステージボタンの有効化
        for (int i = 0; i < stageButtons.Length; i++)
        {
            if (i < stageUnlock)
            {
                stageButtons[i].interactable = true;
            }
            else
            {
                stageButtons[i].interactable = false;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // stage番号を引数にしてステージシーンに遷移する
    public void ToStageScene(int stage)
    {
        SceneManager.LoadScene("Stage" + stage);
    }
}
