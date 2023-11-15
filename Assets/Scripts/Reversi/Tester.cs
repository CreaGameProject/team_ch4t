using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    private void Start()
    {
        // 手数を取得する
        // ※ 0 になったらゲームオーバー
        int restTurn = Board.instance.getPresetRestTurn;
        Debug.Log(string.Format("【Tester】restTurn | {0}", restTurn));

        // 手版（現在誰のターンか）を取得する
        // ※ turn => neutral : どちらでもない
        // ※ turn => player : プレイヤーのターン
        // ※ turn => computer : コンピュータのターン
        Turn.Type turn = Board.instance.getTurn;
        Debug.Log(string.Format("【Tester】restTurn | {0}", turn));

        // セリフ表示スクリプトにこれを追加する
        // デリゲートにイベントハンドラを追加
        Board.instance.OnMethodAExecuted += OnMethodAExecutedHandler;
    }

    // セリフ表示スクリプトにこれを追加する
    private void OnMethodAExecutedHandler()
    {
        // キャラクターが喋る処理を記述する
        Debug.Log("【Tester】OnMethodAExecutedHandler() | SpeakComputer()が実行されました！");
    }
}
