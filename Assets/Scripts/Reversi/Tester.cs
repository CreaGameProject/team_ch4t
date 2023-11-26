using Cysharp.Threading.Tasks;
using System;
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

        Board.instance.OnChangeRestTurnExecuted += OnChangeRestTurnExecutedHandler;
        Board.instance.OnChangeTurnExecuted += OnChangeTurnExecutedHandler;

        // セリフ表示スクリプトにこれを追加する
        Board.instance.OnSpeakComputerExecuted += OnSpeakComputerExecutedHandler;
        Board.instance.OnSecretCellPerformanceExecuted += OnSecretCellPerformanceExecutedHandler;
    }

    // 残り手数が変更されたときに実行される
    // 引数：restTurn：変更された後の残りの手数
    private void OnChangeRestTurnExecutedHandler(int restTurn)
    {
        // 残り手数が変更されたときの処理を記述する
        Debug.Log("【Tester】OnChangeRestTurnExecutedHandler | 残り手数が変更されたときの処理を記述する");
    }

    // 手番（現在のターン）が変更されたときに実行される
    // 引数：type：変更された後の手番（現在のターン）：
    private void OnChangeTurnExecutedHandler(Turn.Type type)
    {
        // 手番（現在のターン）が変更されたときの処理を記述する
        Debug.Log("【Tester】OnChangeRestTurnExecutedHandler | 手番（現在のターン）が変更されたときの処理を記述する");
    }

    async public UniTask OnSecretCellPerformanceExecutedHandler()
    {
        // ヒミツマスが裏返されたときの演出を記述する
        Debug.Log("【Tester】OnSecretCellPerformanceExecutedHandler | ヒミツマスが裏返されたときの演出を記述する");

        await UniTask.Yield();
    }

    async public UniTask OnSpeakComputerExecutedHandler()
    {
        // キャラクターが喋る処理を記述する
        Debug.Log("【Tester】OnSpeakComputerExecutedHandler() | キャラクターが喋る処理を記述する");

        await UniTask.Yield();
    }
}
