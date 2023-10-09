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

        
        bool wasTheStonePlacedCorrectly = this.board.PutStone(cellIndex, this.myType);
        if (wasTheStonePlacedCorrectly)
        {
            this.board.ChangeTurn();
        }

        //石を裏返す

        //ターンエンド
    }
}
