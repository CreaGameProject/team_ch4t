using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Board board = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("左ボタンが押されています。");

            if (this.board.turn != Turn.Type.player) { return; }

            // カーソル位置を取得
            Vector3 mousePosition = Input.mousePosition;
            // カーソル位置のz座標を10に
            mousePosition.z = 10;
            // カーソル位置をワールド座標に変換
            Vector3 target = Camera.main.ScreenToWorldPoint(mousePosition);

            bool wasTheStonePlacedCorrectly = this.board.PutStone((target.x, target.z), Cell.Type.white);
            if (wasTheStonePlacedCorrectly)
            {
                this.board.TurnEnd();
            }
        }
    }
}
