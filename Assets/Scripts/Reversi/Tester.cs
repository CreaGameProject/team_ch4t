using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Board;

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

        // 対戦相手を取得する（対戦相手の名前）
        // ※ opponent => None : 誰でもない（初期状態）
        // ※ opponent => Yukihira_ui : 雪平 優依（ゆきひら うい）
        // ※ opponent => Takahashi_shota : 高橋 翔太（たかはし しょうた）
        //Computer.Opponent opponent = Board.instance.getOpponent;
        Computer.Opponent opponent = Computer.opponent;
        Debug.Log(string.Format("【Tester】opponent | {0}", opponent));

        // 現在取得しているヒミツの数を取得する
        int howManyHimituDidGet = Board.instance.getHowManyHimituDidGet;
        Debug.Log(string.Format("【Tester】howManyHimituDidGet | {0}", howManyHimituDidGet));

        Board.instance.OnChangeRestTurnExecuted += OnChangeRestTurnExecutedHandler;
        Board.instance.OnChangeTurnExecuted += OnChangeTurnExecutedHandler;
        Board.instance.OnChangeHimituNumberExecuted += OnChangeHimituNumberExecutedHandler;
        Board.instance.OnGameOverExecuted += OnGameOverExecutedHandler;

        // セリフ表示スクリプトにこれを追加する
        Board.instance.OnSpeakComputerExecuted += OnSpeakComputerExecutedHandler;
        Board.instance.OnSecretCellPerformanceExecuted += OnSecretCellPerformanceExecutedHandler;

        // 
        Board.instance.ImpossiblePlaceStonesExecuted += ImpossiblePlaceStones;
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

    // 現在取得しているヒミツの数が変わったら実行される
    // 引数：howManyHimituDidGet：変更された後の現在取得しているヒミツの数
    private void OnChangeHimituNumberExecutedHandler(int howManyHimituDidGet)
    {
        // 現在取得しているヒミツの数が変更されたときの処理を記述する
        Debug.Log("【Tester】OnChangeRestTurnExecutedHandler | 現在取得しているヒミツの数が変更されたときの処理を記述する");
    }

    // 対局が決着したら実行される
    // 引数：gameResult：対局の結果
    // ※ gameResult => None : 初期状態
    // ※ gameResult => Player_WIN : プレイヤーの勝ち
    // ※ gameResult => Player_LOSE : プレイヤーの負け
    // ※ gameResult => Drow : 引き分け
    private void OnGameOverExecutedHandler(GameResult gameResult)
    {
        // 対局が決着したら実行される
        Debug.Log("【Tester】OnChangeRestTurnExecutedHandler | 対局が決着したら実行される");

        Debug.Log(string.Format("【Tester】OnChangeRestTurnExecutedHandler | gameResult : {0}", gameResult));
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


    public bool popUpWindowLock = false; // 「はい」ボタンを押したときに true に
    async public UniTask ImpossiblePlaceStones()
    {
        await UniTask.Yield();

        // ウィンドウをポップアップさせる処理

        while (!popUpWindowLock)
        {
            // 100ミリ秒待機して再試行
            await UniTask.Delay(10);
        }

        popUpWindowLock = false;

        // 
    }
}
