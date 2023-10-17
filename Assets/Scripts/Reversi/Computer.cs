using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        List<((int, int), int) > proposedCells = this.board.GetProposedCell(Cell.Color.black);


        //ヒミツマスを探す
        /*List<(int, int)> SecretCells = this.board.GetSecretCell();
        if (SecretCells.Count != 0)
        {
            Cell.Type[,] board = this.board.GetBoard();

            foreach ((int,int) secretCell in SecretCells)
            {
                //周囲８マスにプレイヤーの石がないか調べる
                
                for (int y = secretCell.Item2 - 1; y < secretCell.Item2 + 2; y++)
                {
                    for (int x = secretCell.Item1 - 1; x < secretCell.Item1 + 2; x++)
                    {
                        // 場外判定
                        if (!(x > -1 && 8 > x) || !(y > -1 && 8 > y)) { continue; }

                        // プレイヤーの石があるなら、
                        if (board[x, y] != Cell.Type.white) { continue; }

                        // 周囲８マスを探索し、
                        for (int j = -1; j < 2; j++)
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                // 場外判定
                                if (!(x + i > -1 && 8 > x + i) || !(y + j > -1 && 8 > y + j)) { continue; }

                                // 石があるなら、その石を裏返せる候補地がないか調べる
                                if (board[x + i, y + j] != Cell.Type.white) { continue; }

                                Debug.Log("到達A (x + i, y + j) => " + (x + i, y + j));

                                //候補地とマッチするか調べる
                                bool wasMatch = false;
                                foreach((int, int) proposedCell in proposedCells)
                                {
                                    if (proposedCell == (x + i, y + j))
                                    {
                                        wasMatch = true;
                                        break;
                                    }
                                }

                                if (wasMatch)
                                {
                                    Debug.Log("到達B");
                                    cellIndex = (x + i, y + j);
                                }

                            }
                        }
                    }
                }
            }
        }*/

        //石を置く

        (int, int) cellIndex = (-1, -1); 

        List<(int, int)> secretCells = this.board.GetSecretCell();

        Cell.Type cellType = Cell.Type.black;

        if (secretCells.Count == 0)
        {
            // ランダムに石を置く
            cellIndex = proposedCells[Random.Range(0, proposedCells.Count)].Item1;

            // 【一時的】１０％でヒミツを設置する
            int number = Random.Range(1, 100 + 1);
            if (number <= 30)
            {
                cellType = Cell.Type.secret;
            }
        }
        else
        {
            // 最も石を裏返せる位置に置く

            int maxLength = proposedCells[0].Item2;
            List<(int, int)> maxCell = new List<(int, int)> { proposedCells[0].Item1 };
            for (int i = 1; i < proposedCells.Count; i++)
            {
                if (maxLength < proposedCells[i].Item2)
                {
                    maxLength = proposedCells[i].Item2;
                    maxCell.Clear();
                    maxCell.Add(proposedCells[i].Item1);
                }
            }

            Debug.Log(string.Format("maxLength => {0}", maxLength));

            // 候補が複数ある場合はランダムで選ぶ
            cellIndex = maxCell[Random.Range(0, maxCell.Count)];

            /*foreach (((int, int), int) proposedCell in proposedCells)
            {
                Debug.Log(string.Format("proposedCell => {0}", proposedCell));
            }*/
        }

        

        bool wasTheStonePlacedCorrectly = this.board.PutStone(cellIndex, cellType);
        if (wasTheStonePlacedCorrectly)
        {
            this.board.ChangeTurn();
        }

        //ターンエンド
    }
}
