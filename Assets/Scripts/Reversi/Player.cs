using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Board board = null;
    [SerializeField] private Cell.Color myColor = Cell.Color.black;

    [SerializeField] private bool isInputLocked = false;
    public void SetIsInputLocked(bool b)
    {
        this.isInputLocked = b;
    }

    async public UniTask<(bool, bool)> Action()
    {
        (bool, bool) returnBool = (false, false);

        do
        {
            await UniTask.Yield();

            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));

            if (!this.isInputLocked)
            {
                // カーソル位置を取得
                Vector3 mousePosition = Input.mousePosition;
                // カーソル位置のz座標を10に
                mousePosition.z = 10;
                // カーソル位置をワールド座標に変換
                Vector3 target = Camera.main.ScreenToWorldPoint(mousePosition);

                returnBool = await this.board.PutStone((target.x, target.z), Cell.Type.black);
            }

        } while (!returnBool.Item1);

        return returnBool;

    }
}
