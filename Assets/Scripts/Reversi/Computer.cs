using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Computer : MonoBehaviour
{
    [SerializeField] private Board board = null;

    [SerializeField] private Cell.Type myType = Cell.Type.black;

    private void Start()
    {
        
    }

    public void Action()
    {
        //石を置ける場所を探す

        List<(int, int)> proposedCells = this.board.GetProposedCell(this.myType);

        //石を置く場所を探す

        //ランダムで選ぶ

        (int, int) cellIndex = proposedCells[Random.Range(0, proposedCells.Count)];

        //石を置く

        // 【一時的】１０％でヒミツを設置する
        Cell.Type cellType = this.myType;
        int number = Random.Range(1, 100 + 1);
        if (number <= 10)
        {
            cellType = Cell.Type.secret;
        }
        
        bool wasTheStonePlacedCorrectly = this.board.PutStone(cellIndex, cellType);
        if (wasTheStonePlacedCorrectly)
        {
            this.board.ChangeTurn();
        }

        //ターンエンド
    }
}
