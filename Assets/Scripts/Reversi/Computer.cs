using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Microsoft.VisualBasic;
using UnityEditor;
using System.ComponentModel;
using Cysharp.Threading.Tasks;

public class Computer : MonoBehaviour
{
    [SerializeField] private Board board = null;

    [SerializeField] private Cell.Color myColor = Cell.Color.white;

    async public UniTask Action()
    {
        
        bool wasTheStonePlacedCorrectly = false;

        do
        {
            await UniTask.Yield();

            //石を置ける場所を探す

            List<((int, int), int)> proposedCells = board.GetProposedCell(this.myColor);

            //石を置く

            (int, int) cellIndex = (-1, -1); // 石を置く場所

            List<(int, int)> secretCells = board.GetSecretCell();

            Cell.Type cellType = Cell.Type.white;

            if (secretCells.Count == 0)
            {

                // ランダムに石を置く
                cellIndex = proposedCells[Random.Range(0, proposedCells.Count)].Item1;

                List<(int, int)> cells = GetMostDifficultToTurnOverCells(board.GetBoard());
                /*for (int i = 0; i < cells.Count; i++)
                {
                    Debug.Log("cells => " + cells[i]);
                }*/
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

                // 候補が複数ある場合はランダムで選ぶ
                cellIndex = maxCell[Random.Range(0, maxCell.Count)];

            }

            wasTheStonePlacedCorrectly = board.PutStone(cellIndex, cellType);

        } while (!wasTheStonePlacedCorrectly);

    }

    private List<(int, int)> GetMostDifficultToTurnOverCells((Cell.Type, Cell.Color)[,] board)
    {
        int[,] frequency = new int[8, 8];

        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                if (board[x, y].Item2 != Cell.Color.black) { continue; }

                //Debug.Log("到達");

                Cell.Color myColor = Cell.Color.black;
                Cell.Color oppositeColor = Cell.Color.white;

                int numberOfCellsToFlip = 0; //裏返す石の数
                (int, int) originIndex = (0, 0); //探索開始位置（index表記）
                int X, Y;

                //上方向への検索
                originIndex = (x, y - 1);
                numberOfCellsToFlip = 0;
                for (Y = y - 1; Y > -1; Y--)
                {
                    if (board[originIndex.Item1, Y].Item1 == Cell.Type.empty || board[originIndex.Item1, Y].Item2 == myColor) { break; }

                    if (board[originIndex.Item1, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (Y - 1 < 0) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (board[originIndex.Item1, Y - 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (board[originIndex.Item1, Y - 1].Item1 == Cell.Type.empty)
                        {
                            for (int n = 0; n < numberOfCellsToFlip; n++)
                            {
                                frequency[originIndex.Item1, originIndex.Item2 - n] += 1;
                            }

                            break;
                        }
                    }
                }

                //下方向への検索
                originIndex = (x, y + 1);
                numberOfCellsToFlip = 0;
                for (Y = y + 1; Y < 8; Y++)
                {
                    if (board[originIndex.Item1, Y].Item1 == Cell.Type.empty || board[originIndex.Item1, Y].Item2 == myColor) { break; }

                    if (board[originIndex.Item1, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (Y + 1 > 7) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (board[originIndex.Item1, Y + 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (board[originIndex.Item1, Y + 1].Item1 == Cell.Type.empty)
                        {
                            for (int n = 0; n < numberOfCellsToFlip; n++)
                            {
                                frequency[originIndex.Item1, originIndex.Item2 + n] += 1;
                            }

                            break;
                        }
                    }
                }

                //左方向への検索
                originIndex = (x - 1, y);
                numberOfCellsToFlip = 0;
                for (X = x - 1; X > -1; X--)
                {
                    if (board[X, originIndex.Item2].Item1 == Cell.Type.empty || board[X, originIndex.Item2].Item2 == myColor) { break; }

                    if (board[X, originIndex.Item2].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X - 1 < 0) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (board[X - 1, originIndex.Item2].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (board[X - 1, originIndex.Item2].Item1 == Cell.Type.empty)
                        {
                            for (int n = 0; n < numberOfCellsToFlip; n++)
                            {
                                frequency[originIndex.Item1 - n, originIndex.Item2] += 1;
                            }

                            break;
                        }
                    }
                }

                //右方向への検索
                originIndex = (x + 1, y);
                numberOfCellsToFlip = 0;
                for (X = x + 1; X < 8; X++)
                {
                    if (board[X, originIndex.Item2].Item1 == Cell.Type.empty || board[X, originIndex.Item2].Item2 == myColor) { break; }

                    if (board[X, originIndex.Item2].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X + 1 > 7) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (board[X + 1, originIndex.Item2].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (board[X + 1, originIndex.Item2].Item1 == Cell.Type.empty)
                        {
                            for (int n = 0; n < numberOfCellsToFlip; n++)
                            {
                                frequency[originIndex.Item1 + n, originIndex.Item2] += 1;
                            }

                            break;
                        }
                    }
                }

                //右上方向への検索
                originIndex = (x + 1, y - 1);
                numberOfCellsToFlip = 0;
                for (X = x + 1, Y = y - 1; X < 8 && Y > -1; X++, Y--)
                {
                    if (board[X, Y].Item1 == Cell.Type.empty || board[X, Y].Item2 == myColor) { break; }

                    if (board[X, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X + 1 > 7 || Y - 1 < 0) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (board[X + 1, Y - 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (board[X + 1, Y - 1].Item1 == Cell.Type.empty)
                        {
                            for (int n = 0; n < numberOfCellsToFlip; n++)
                            {
                                frequency[originIndex.Item1 + n, originIndex.Item2 - n] += 1;
                            }

                            break;
                        }
                    }
                }

                //右下方向への検索
                originIndex = (x + 1, y + 1);
                numberOfCellsToFlip = 0;
                for (X = x + 1, Y = y + 1; X < 8 && Y < 8; X++, Y++)
                {
                    if (board[X, Y].Item1 == Cell.Type.empty || board[X, Y].Item2 == myColor) { break; }

                    if (board[X, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X + 1 > 7 || Y + 1 > 7) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (board[X + 1, Y + 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (board[X + 1, Y + 1].Item1 == Cell.Type.empty)
                        {
                            for (int n = 0; n < numberOfCellsToFlip; n++)
                            {
                                frequency[originIndex.Item1 + n, originIndex.Item2 + n] += 1;
                            }

                            break;
                        }
                    }
                }

                //左上方向への検索
                originIndex = (x - 1, y - 1);
                numberOfCellsToFlip = 0;
                for (X = x - 1, Y = y - 1; X > -1 && Y > -1; X--, Y--)
                {
                    if (board[X, Y].Item1 == Cell.Type.empty || board[X, Y].Item2 == myColor) { break; }

                    if (board[X, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X - 1 < 0 || Y - 1 < 0) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (board[X - 1, Y - 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (board[X - 1, Y - 1].Item1 == Cell.Type.empty)
                        {
                            for (int n = 0; n < numberOfCellsToFlip; n++)
                            {
                                frequency[originIndex.Item1 - n, originIndex.Item2 - n] += 1;
                            }

                            break;
                        }
                    }
                }

                //左下方向への検索
                originIndex = (x - 1, y + 1);
                numberOfCellsToFlip = 0;
                for (X = x - 1, Y = y + 1; X > -1 && Y < 8; X--, Y++)
                {
                    if (board[X, Y].Item1 == Cell.Type.empty || board[X, Y].Item2 == myColor) { break; }

                    if (board[X, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X - 1 < 0 || Y + 1 > 7) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (board[X - 1, Y + 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (board[X - 1, Y + 1].Item1 == Cell.Type.empty)
                        {
                            for (int n = 0; n < numberOfCellsToFlip; n++)
                            {
                                frequency[originIndex.Item1 - n, originIndex.Item2 + n] += 1;
                            }

                            break;
                        }
                    }
                }

            }
        }

        List<(int, int)> mostDifficultToTurnOverCells = new List<(int, int)>();
        int maxFrequency = frequency[0, 0];
        for (int y = 0; y < frequency.GetLength(1); y++)
        {
            for (int x = 0; x < frequency.GetLength(0); x++)
            {
                if (frequency[x, y] <= 0) { continue; }

                if (frequency[x, y] > maxFrequency)
                {
                    maxFrequency = frequency[x, y];

                    mostDifficultToTurnOverCells.Clear();
                    mostDifficultToTurnOverCells.Add((x, y));
                }
                else if (frequency[x, y] == maxFrequency)
                {
                    mostDifficultToTurnOverCells.Add((x, y));
                }

            }
        }


        string content = "";

        for (int y = 0; y < frequency.GetLength(1); y++)
        {
            string line = "";

            for (int x = 0; x < frequency.GetLength(0); x++)
            {
                switch (frequency[x, y])
                {
                    case 0 : line += "０"; break;
                    case 1 : line += "１"; break;
                    case 2 : line += "２"; break;
                    case 3 : line += "３"; break;
                    case 4 : line += "４"; break;
                    case 5 : line += "５"; break;
                    case 6 : line += "６"; break;
                    case 7 : line += "７"; break;
                    case 8 : line += "８"; break;
                    case 9 : line += "９"; break;
                    default: line += "Ｍ"; break;
                }
            }

            content += line + "\n";
        }

        Debug.Log(content);

        return mostDifficultToTurnOverCells;
    }


}
