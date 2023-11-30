using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Board;

public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Board.instance != null)
        {
            Debug.Log("【SceneChanger - Start】Board.instance != null");
            Board.instance.OnGameOverExecuted += OnGameOverExecutedHandler;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
        Application.Quit();//ゲームプレイ終了
#endif
    }

    // 引数：gameResult：対局の結果
    // ※ gameResult => None : 初期状態
    // ※ gameResult => Player_WIN : プレイヤーの勝ち
    // ※ gameResult => Player_LOSE : プレイヤーの負け
    // ※ gameResult => Drow : 引き分け
    private void OnGameOverExecutedHandler(GameResult gameResult)
    {
        // 対局が決着したら実行される
        //Debug.Log("【Tester】OnChangeRestTurnExecutedHandler | 対局が決着したら実行される");
        //Debug.Log(string.Format("【Tester】OnChangeRestTurnExecutedHandler | gameResult : {0}", gameResult));
        if (gameResult == GameResult.Player_WIN)
        {
            LoadScene("Clear");
        }
        else
        {
            LoadScene("Gameover");
        }
    }
}
