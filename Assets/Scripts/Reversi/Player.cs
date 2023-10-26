using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Board board = null;
    [SerializeField] private Cell.Color myColor = Cell.Color.black;

    async public UniTask Action()
    {
        bool wasTheStonePlacedCorrectly = false;

        do
        {
            await UniTask.Yield();

            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));

            // カーソル位置を取得
            Vector3 mousePosition = Input.mousePosition;
            // カーソル位置のz座標を10に
            mousePosition.z = 10;
            // カーソル位置をワールド座標に変換
            Vector3 target = Camera.main.ScreenToWorldPoint(mousePosition);

            wasTheStonePlacedCorrectly = this.board.PutStone((target.x, target.z), Cell.Type.black);

        } while (!wasTheStonePlacedCorrectly);

    }
}
