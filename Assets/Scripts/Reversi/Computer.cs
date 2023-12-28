using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Microsoft.VisualBasic;
using UnityEditor;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using System.Linq;
//using System;

public class Computer : MonoBehaviour
{
    [SerializeField] private Board board = null;

    private Cell.Color myColor = Cell.Color.white;

    [System.Serializable]
    public enum Opponent
    {
        Yukihira_ui = 0,
        Takahashi_shota = 1,
    }

    [SerializeField] private Opponent com = Opponent.Takahashi_shota;

    public static Opponent opponent = Opponent.Yukihira_ui;
    //public static Opponent opponent = Opponent.Takahashi_shota;

    private void Awake()
    {
        opponent = this.com;
    }

    [Header("N 手先まで思考する（部長AIのみ有効）")] // default => 1
    [SerializeField] private int foresight = 1;
    
    // 再帰的に計算する（実行コスト高い）
    async private UniTask<List<int>> Recursive((Cell.Type, Cell.Color)[,] virtualBoard, int p, int n, int f, int min)
    {
        await UniTask.Yield();

        List<int> return_predict = new List<int>();

        List<((int, int), int)> proposedCells_com = GetProposedCell(Cell.Color.white, virtualBoard);

        //Debug.Log("proposedCells[j] : " + proposedCells[i]);
        Debug.Log(string.Format("<color=red><b>{0} 手先を試行中...</b></color>", n));
        //Debug.Log(string.Format("<color=red><b>{0} 回目の呼び出し</b></color>", n));

        // => コンピュータが置ける場所に石を置いた場合の盤面を計算する
        for (int i = 0; i < proposedCells_com.Count; i++)
        {

            int P = p;

            //Debug.Log(string.Format("P : {0} | min : {1}", P, min));
            Debug.Log(string.Format("computerパターン : {0}", i));

            if ((return_predict.Count != 0) ? return_predict.Contains(0) : false)
            {
                Debug.Log(string.Format("探索打ち切り - A | 最善手を発見済み"));
                P += 1;
                return_predict.Add(P);
                break;
            }
            else if (P >= min)
            {
                Debug.Log(string.Format("探索打ち切り - A | 見込みなし"));
                P += 1;
                return_predict.Add(P);
                break;
            }            

            //Debug.Log(string.Format("proposedCells_com[{0}] : {1}", i, proposedCells_com[i]));

            ((Cell.Type, Cell.Color)[,], bool) predict_com = Predict(virtualBoard, proposedCells_com[i].Item1, Cell.Color.white, Cell.Color.black, Cell.Type.white);
            //((Cell.Type, Cell.Color)[,], bool) predict_computer = Predict(virtualBoard, proposedCells[i].Item1, Cell.Color.white, Cell.Color.black, Cell.Type.white);

            (Cell.Type, Cell.Color)[,] virtualBoard_com = predict_com.Item1;

            virtualBoard_com[proposedCells_com[i].Item1.Item1, proposedCells_com[i].Item1.Item2] = UpdateCell(Cell.Type.white);

            ViewBoard(Turn.Type.player, true, virtualBoard_com);

            // => => プレイヤーが置ける場所を探す

            List<((int, int), int)> proposedCells_player = GetProposedCell(Cell.Color.black, virtualBoard_com);

            for (int j = 0; j < proposedCells_player.Count; j++)
            {
                Debug.Log(string.Format("computerパターン : {0} | playerパターン : {1}", i, j));

                ((Cell.Type, Cell.Color)[,], bool) predict_player = Predict(virtualBoard_com, proposedCells_player[j].Item1, Cell.Color.black, Cell.Color.white, Cell.Type.black);

                (Cell.Type, Cell.Color)[,] virtualBoard_player = predict_player.Item1;

                ViewBoard(Turn.Type.computer, true, virtualBoard_player);

                bool didFlipSecret = predict_player.Item2;

                //if (didFlipSecret) { predict[i] += 1; }
                if (didFlipSecret) { P += 1; }

                if (n != f)
                {
                    if ((return_predict.Count != 0) ? return_predict.Contains(0) : false)
                    {
                        Debug.Log(string.Format("探索打ち切り - B | 最善手を発見済み"));
                        P += 1;
                        break;
                    }
                    else if (P >= min)
                    {
                        Debug.Log(string.Format("探索打ち切り - B | 見込みなし"));
                        P += 1;
                        break;
                    }
                    else
                    {
                        int MIN = (return_predict.Count != 0) ? (return_predict.Min() >= P) ? P : return_predict.Min() : min;
                        //int MIN = (return_predict.Count != 0) ? (return_predict.Min() >= P) ? P : return_predict.Min() : P;
                        //int MIN = 0;
                        List<int> next_p = await Recursive(virtualBoard_player, P, n + 1, f, MIN);
                        //Debug.Log(string.Format("next_p.Sum() : {0}", next_p.Sum()));
                        P += next_p.Sum();
                    }
                }
            }

            //Debug.Log("P => " + P);

            return_predict.Add(P);
        }

        return return_predict;
    }

    int GetRandomMinIndex(List<int> list)
    {
        if (list == null || list.Count == 0)
        {
            // リストが空の場合やnullの場合に適切なエラー処理を行うか、例外をスローするなどの対応が必要です。
            //throw new ArgumentException("リストが空またはnullです。");
            Debug.Log("リストが空またはnullです。");
        }

        int minValue = list[0];
        List<int> minIndices = new List<int> { 0 };

        // 最小値を見つける
        for (int i = 1; i < list.Count; i++)
        {
            int currentValue = list[i];
            if (currentValue < minValue)
            {
                minValue = currentValue;
                minIndices.Clear();
                minIndices.Add(i);
            }
            else if (currentValue == minValue)
            {
                minIndices.Add(i);
            }
        }

        for (int i = 0; i < minIndices.Count; i++)
        {
            Debug.Log(string.Format("minIndices[i] => {0}", minIndices[i]));
        }

        // 最小値のインデックスをランダムに選ぶ
        System.Random random = new System.Random();
        int randomIndex = minIndices[random.Next(minIndices.Count)];
        return randomIndex;
    }

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

                //List<(int, int)> cells = GetMostDifficultToTurnOverCells(board.GetBoard());

            }
            else
            {
                if (opponent == Opponent.Yukihira_ui)
                {
                    Debug.Log("===== ういの挙動です =====");

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
                        else if (maxLength == proposedCells[i].Item2)
                        {
                            maxCell.Add(proposedCells[i].Item1);
                        }
                    }

                    // 候補が複数ある場合はランダムで選ぶ
                    cellIndex = maxCell[Random.Range(0, maxCell.Count)];
                }
                else if (opponent == Opponent.Takahashi_shota)
                {
                    Debug.Log("===== 部長の挙動です =====");

                    (Cell.Type, Cell.Color)[,] realBoard = this.board.GetBoard();
                    (Cell.Type, Cell.Color)[,] virtualBoard = new (Cell.Type, Cell.Color)[8, 8];

                    for (int y = 0; y < virtualBoard.GetLength(1); y++)
                    {
                        for (int x = 0; x < virtualBoard.GetLength(0); x++)
                        {
                            virtualBoard[x, y] = realBoard[x, y];
                        }
                    }

                    Debug.Log(string.Format("AIが思考を開始します。"));

                    ViewBoard(Turn.Type.computer, true, virtualBoard);

                    List<int> predict = new List<int>();

                    //List<((int, int), int)> proposedCells = GetProposedCell(Cell.Color.white, virtualBoard);
                    //List<((int, int), int)> proposedCells_com = GetProposedCell(Cell.Color.white, virtualBoard);

                    List<int> next_p = await Recursive(virtualBoard, 0, 1, this.foresight, 9999);
                    predict = next_p;

                    /*
                    for (int n = 0; n < this.foresight; n++)
                    {
                        //Debug.Log(string.Format("{0} 手先を読んでいます...", n + 1));

                        (Cell.Type, Cell.Color)[,] v_board = virtualBoard;

                        predict = new List<int> { proposedCells.Count };

                        // => コンピュータが置ける場所に石を置いた場合の盤面を計算する
                        for (int i = 0; i < proposedCells.Count; i++)
                        {
                            predict.Add(0);

                            //Debug.Log("proposedCells[j] : " + proposedCells[i]);
                            Debug.Log(string.Format("{0} 手先 パターン{1} を読んでいます...", n + 1, i + 1));

                            ((Cell.Type, Cell.Color)[,], bool) predict_computer = Predict(v_board, proposedCells[i].Item1, Cell.Color.white, Cell.Color.black, Cell.Type.white);
                            //((Cell.Type, Cell.Color)[,], bool) predict_computer = Predict(virtualBoard, proposedCells[i].Item1, Cell.Color.white, Cell.Color.black, Cell.Type.white);

                            (Cell.Type, Cell.Color)[,] virtualBoard_predict_1 = predict_computer.Item1;

                            virtualBoard_predict_1[proposedCells[i].Item1.Item1, proposedCells[i].Item1.Item2] = UpdateCell(Cell.Type.white);

                            ViewBoard(Turn.Type.player, true, virtualBoard_predict_1);

                            // => => プレイヤーが置ける場所を探す

                            List<((int, int), int)> proposedCells_player = GetProposedCell(Cell.Color.black, virtualBoard_predict_1);

                            for (int j = 0; j < proposedCells_player.Count; j++)
                            {
                                //Debug.Log("proposedCells_player[j] : " + proposedCells_player[j]);

                                // => => => プレイヤーが置ける場所に石を置いた場合の盤面を計算する

                                ((Cell.Type, Cell.Color)[,], bool) predict_player = Predict(virtualBoard_predict_1, proposedCells_player[j].Item1, Cell.Color.black, Cell.Color.white, Cell.Type.black);

                                (Cell.Type, Cell.Color)[,] virtualBoard_predict_2 = predict_player.Item1;

                                Debug.Log(string.Format("{0} 手先 パターン{1} の パターン{2} を読んでいます...", n + 1, i + 1, j + 1));

                                ViewBoard(Turn.Type.player, true, virtualBoard_predict_2);

                                bool didFlipSecret = predict_player.Item2;

                                if (didFlipSecret) { predict[i] += 1; }

                                v_board = virtualBoard_predict_2;
                            }


                        }
                    }*/

                    
                    for (int i = 0; i < predict.Count; i++)
                    {
                        Debug.Log(string.Format("predict[i] => {0}", predict[i]));
                    }


                    //Debug.Log("predict.Count => " + predict.Count);
                    //Debug.Log("proposedCells.Count => " + proposedCells.Count);

                    int minIndex = GetRandomMinIndex(predict);

                    cellIndex = proposedCells[minIndex].Item1;

                    Debug.Log(string.Format("predict.Min() => {0} | minIndex => {1} | cellIndex => {2}", predict.Min(), minIndex, cellIndex));

                    //cellIndex = proposedCells[predict.IndexOf(predict.Min())].Item1;
                }
            }

            (bool, bool) returnBool = await board.PutStone(cellIndex, cellType);
            wasTheStonePlacedCorrectly = returnBool.Item1;

        } while (!wasTheStonePlacedCorrectly);

    }

    private (Cell.Type, Cell.Color) UpdateCell(Cell.Type type)
    {
        Cell.Color color = Cell.Color.empty;
        switch (type)
        {
            case Cell.Type.white: color = Cell.Color.white; break;
            case Cell.Type.black: color = Cell.Color.black; break;
            case Cell.Type.secret: color = Cell.Color.white; break;
            default: break;
        }

        return (type, color);
    }

    //設置可能な座標（index）を調べて、配列で返す
    //戻り値：
    public List<((int, int), int)> GetProposedCell(Cell.Color myColor, (Cell.Type, Cell.Color)[,] board)
    {
        Cell.Color oppositeColor = Cell.Color.empty;
        switch (myColor)
        {
            case Cell.Color.white: oppositeColor = Cell.Color.black; break;
            case Cell.Color.black: oppositeColor = Cell.Color.white; break;
            default: break;
        }
        

        // 候補地
        List<((int, int), int)> proposedCells = new List<((int, int), int)>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                //自分の色を起点に検索する
                if (board[x, y].Item2 != myColor)
                {
                    continue;
                }

                int numberOfCellsToFlip = 0; //裏返す石の数
                (int, int) originIndex = (0, 0); //探索開始位置（index表記）
                int X, Y;

                //上方向への検索
                originIndex = (x, y - 1);
                numberOfCellsToFlip = 0;
                for (Y = y - 1; Y > -1; Y--)
                {
                    if (board[originIndex.Item1, Y].Item1 == Cell.Type.empty || board[originIndex.Item1, Y].Item2 == myColor || board[originIndex.Item1, Y].Item1 == Cell.Type.hole) { break; }

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
                            if (numberOfCellsToFlip > 0)
                            {
                                proposedCells.Add(((originIndex.Item1, Y - 1), numberOfCellsToFlip));
                                break;
                            }
                        }
                    }
                }

                //下方向への検索
                originIndex = (x, y + 1);
                numberOfCellsToFlip = 0;
                for (Y = y + 1; Y < 8; Y++)
                {
                    if (board[originIndex.Item1, Y].Item1 == Cell.Type.empty || board[originIndex.Item1, Y].Item2 == myColor || board[originIndex.Item1, Y].Item1 == Cell.Type.hole) { break; }

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
                            if (numberOfCellsToFlip > 0)
                            {
                                proposedCells.Add(((originIndex.Item1, Y + 1), numberOfCellsToFlip));
                                break;
                            }
                        }
                    }
                }

                //左方向への検索
                originIndex = (x - 1, y);
                numberOfCellsToFlip = 0;
                for (X = x - 1; X > -1; X--)
                {
                    if (board[X, originIndex.Item2].Item1 == Cell.Type.empty || board[X, originIndex.Item2].Item2 == myColor || board[X, originIndex.Item2].Item1 == Cell.Type.hole) { break; }

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
                            if (numberOfCellsToFlip > 0)
                            {
                                proposedCells.Add(((X - 1, originIndex.Item2), numberOfCellsToFlip));
                                break;
                            }
                        }
                    }
                }

                //右方向への検索
                originIndex = (x + 1, y);
                numberOfCellsToFlip = 0;
                for (X = x + 1; X < 8; X++)
                {
                    if (board[X, originIndex.Item2].Item1 == Cell.Type.empty || board[X, originIndex.Item2].Item2 == myColor || board[X, originIndex.Item2].Item1 == Cell.Type.hole) { break; }

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
                            if (numberOfCellsToFlip > 0)
                            {
                                proposedCells.Add(((X + 1, originIndex.Item2), numberOfCellsToFlip));
                                break;
                            }
                        }
                    }
                }

                //右上方向への検索
                originIndex = (x + 1, y - 1);
                numberOfCellsToFlip = 0;
                for (X = x + 1, Y = y - 1; X < 8 && Y > -1; X++, Y--)
                {
                    if (board[X, Y].Item1 == Cell.Type.empty || board[X, Y].Item2 == myColor || board[X, Y].Item1 == Cell.Type.hole) { break; }

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
                            if (numberOfCellsToFlip > 0)
                            {
                                proposedCells.Add(((X + 1, Y - 1), numberOfCellsToFlip));
                                break;
                            }
                        }
                    }
                }

                //右下方向への検索
                originIndex = (x + 1, y + 1);
                numberOfCellsToFlip = 0;
                for (X = x + 1, Y = y + 1; X < 8 && Y < 8; X++, Y++)
                {
                    if (board[X, Y].Item1 == Cell.Type.empty || board[X, Y].Item2 == myColor || board[X, Y].Item1 == Cell.Type.hole) { break; }

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
                            if (numberOfCellsToFlip > 0)
                            {
                                proposedCells.Add(((X + 1, Y + 1), numberOfCellsToFlip));
                                break;
                            }
                        }
                    }
                }

                //左上方向への検索
                originIndex = (x - 1, y - 1);
                numberOfCellsToFlip = 0;
                for (X = x - 1, Y = y - 1; X > -1 && Y > -1; X--, Y--)
                {
                    if (board[X, Y].Item1 == Cell.Type.empty || board[X, Y].Item2 == myColor || board[X, Y].Item1 == Cell.Type.hole) { break; }

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
                            if (numberOfCellsToFlip > 0)
                            {
                                proposedCells.Add(((X - 1, Y - 1), numberOfCellsToFlip));
                                break;
                            }
                        }
                    }
                }

                //左下方向への検索
                originIndex = (x - 1, y + 1);
                numberOfCellsToFlip = 0;
                for (X = x - 1, Y = y + 1; X > -1 && Y < 8; X--, Y++)
                {
                    if (board[X, Y].Item1 == Cell.Type.empty || board[X, Y].Item2 == myColor || board[X, Y].Item1 == Cell.Type.hole) { break; }

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
                            if (numberOfCellsToFlip > 0)
                            {
                                proposedCells.Add(((X - 1, Y + 1), numberOfCellsToFlip));
                                break;
                            }
                        }
                    }
                }

            }
        }

        return proposedCells;
    }

    private ((Cell.Type, Cell.Color)[,], bool) Predict((Cell.Type, Cell.Color)[,] board, (int, int) cellIndex, Cell.Color myColor, Cell.Color oppositeColor, Cell.Type type)
    {
        (Cell.Type, Cell.Color)[,] b = new (Cell.Type, Cell.Color)[8, 8];
        System.Array.Copy(board, b, b.Length);

        bool didFlipSecretCell = false;

        int x = cellIndex.Item1;
        int y = cellIndex.Item2;

        //裏返すセルを探す

        int numberOfCellsToFlip = 0; //裏返す石の数
        (int, int) originIndex = (0, 0);//探索開始位置（index表記）
        int X, Y;

        //上方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x, y - 1);
        for (Y = y - 1; Y > -1; Y--)
        {
            if (b[originIndex.Item1, Y].Item1 == Cell.Type.empty) { break; }

            if (b[originIndex.Item1, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (b[originIndex.Item1, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    if (b[originIndex.Item1, originIndex.Item2 - j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }

                    b[originIndex.Item1, originIndex.Item2 - j] = UpdateCell(type);
                }

                break;
            }
        }

        //右上方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x + 1, y - 1);
        for (X = x + 1, Y = y - 1; X < 8 && Y > -1; X++, Y--)
        {
            if (b[X, Y].Item1 == Cell.Type.empty) { break; }

            if (b[X, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (b[X, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    if (b[originIndex.Item1 + j, originIndex.Item2 - j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
                    b[originIndex.Item1 + j, originIndex.Item2 - j] = UpdateCell(type);
                }

                break;
            }
        }

        //右方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x + 1, y);
        for (X = x + 1; X < 8; X++)
        {
            if (b[X, originIndex.Item2].Item1 == Cell.Type.empty) { break; }

            if (b[X, originIndex.Item2].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (b[X, originIndex.Item2].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    if (b[originIndex.Item1 + j, originIndex.Item2].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
                    //await UpdateCell((originIndex.Item1 + j, originIndex.Item2), type);
                    b[originIndex.Item1 + j, originIndex.Item2] = UpdateCell(type);
                }

                break;
            }
        }

        //右下方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x + 1, y + 1);
        for (X = x + 1, Y = y + 1; X < 8 && Y < 8; X++, Y++)
        {
            if (b[X, Y].Item1 == Cell.Type.empty) { break; }

            if (b[X, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (b[X, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    if (b[originIndex.Item1 + j, originIndex.Item2 + j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
                    //await UpdateCell((originIndex.Item1 + j, originIndex.Item2 + j), type);
                    b[originIndex.Item1 + j, originIndex.Item2 + j] = UpdateCell(type);
                }

                break;
            }
        }

        //下方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x, y + 1);
        for (Y = y + 1; Y < 8; Y++)
        {
            if (b[originIndex.Item1, Y].Item1 == Cell.Type.empty) { break; }

            if (b[originIndex.Item1, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (b[originIndex.Item1, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    if (b[originIndex.Item1, originIndex.Item2 + j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
                    //await UpdateCell((originIndex.Item1, originIndex.Item2 + j), type);
                    b[originIndex.Item1, originIndex.Item2 + j] = UpdateCell(type);
                }

                break;
            }
        }

        //左下方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x - 1, y + 1);
        for (X = x - 1, Y = y + 1; X > -1 && Y < 8; X--, Y++)
        {
            if (b[X, Y].Item1 == Cell.Type.empty) { break; }

            if (b[X, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (b[X, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    if (b[originIndex.Item1 - j, originIndex.Item2 + j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
                    //await UpdateCell((originIndex.Item1 - j, originIndex.Item2 + j), type);
                    b[originIndex.Item1 - j, originIndex.Item2 + j] = UpdateCell(type);
                }

                break;
            }
        }

        //左方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x - 1, y);
        for (X = x - 1; X > -1; X--)
        {
            if (b[X, originIndex.Item2].Item1 == Cell.Type.empty) { break; }

            if (b[X, originIndex.Item2].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (b[X, originIndex.Item2].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    if (b[originIndex.Item1 - j, originIndex.Item2].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
                    //await UpdateCell((originIndex.Item1 - j, originIndex.Item2), type);
                    b[originIndex.Item1 - j, originIndex.Item2] = UpdateCell(type);
                    
                }

                break;
            }
        }

        //左上方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x - 1, y - 1);
        for (X = x - 1, Y = y - 1; X > -1 && Y > -1; X--, Y--)
        {
            if (b[X, Y].Item1 == Cell.Type.empty) { break; }

            if (b[X, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (b[X, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    if (b[originIndex.Item1 - j, originIndex.Item2 - j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
                    //await UpdateCell((originIndex.Item1 - j, originIndex.Item2 - j), type);
                    b[originIndex.Item1 - j, originIndex.Item2 - j] = UpdateCell(type);
                }

                break;
            }
        }

        return (b, didFlipSecretCell);
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
        int minFrequency = 999;
        for (int y = 0; y < frequency.GetLength(1); y++)
        {
            for (int x = 0; x < frequency.GetLength(0); x++)
            {
                if (frequency[x, y] <= 0) { continue; }

                if (frequency[x, y] < minFrequency)
                {
                    minFrequency = frequency[x, y];

                    mostDifficultToTurnOverCells.Clear();
                    mostDifficultToTurnOverCells.Add((x, y));
                }
                else if (frequency[x, y] == minFrequency)
                {
                    mostDifficultToTurnOverCells.Add((x, y));
                }

            }
        }

        // 盤面の頻度を表示する
        /*
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
        */

        return mostDifficultToTurnOverCells;
    }

    // 盤面をコンソールに表示する
    //第１引数 ⇒ 
    //第２引数 ⇒ 石を設置可能な場所を表示するかどうか。｜true ⇒ 表示する｜false ⇒ 表示しない
    public void ViewBoard(Turn.Type myType, bool isViewProposedCell, (Cell.Type, Cell.Color)[,] b)
    {
        //参照コピー（元の値も変わる）を防ぐため複写している
        (Cell.Type, Cell.Color)[,] board = new (Cell.Type, Cell.Color)[8, 8];
        System.Array.Copy(b, board, b.Length);

        if (isViewProposedCell)
        {
            Cell.Type type = Cell.Type.empty;
            if (myType == Turn.Type.computer)
            {
                type = Cell.Type.white;
            }
            else if (myType == Turn.Type.player)
            {
                type = Cell.Type.black;
            }

            Cell.Color oppositeColor = Cell.Color.empty;
            switch (myColor)
            {
                case Cell.Color.white: oppositeColor = Cell.Color.black; break;
                case Cell.Color.black: oppositeColor = Cell.Color.white; break;
                default: break;
            }

            List<((int, int), int)> proposedCells = GetProposedCell(oppositeColor, board);

            foreach (((int, int), int) cell in proposedCells)
            {
                board[cell.Item1.Item1, cell.Item1.Item2] = (Cell.Type.proposed, Cell.Color.empty);
            }
        }

        string content = "";

        for (int y = 0; y < board.GetLength(1); y++)
        {
            string line = "";

            for (int x = 0; x < board.GetLength(0); x++)
            {
                switch (board[x, y].Item1)
                {
                    case Cell.Type.empty: line += '＃'; break;
                    case Cell.Type.white: line += "●"; break;
                    case Cell.Type.black: line += "○"; break;
                    case Cell.Type.proposed: line += "☆"; break;
                    case Cell.Type.secret: line += "◆"; break;
                    case Cell.Type.hole: line += "□"; break;
                    default: line += "？"; break;
                }
            }

            content += line + "\n";
        }

        Debug.Log(content);
    }

    public void test((Cell.Type, Cell.Color)[,] board)
    {
        // コンピュータが置ける場所を探す

        //List<((int, int), int)> proposedCells = GetProposedCell(Cell.Color.black);

        // => => プレイヤーが置ける場所を探す

        // => => => プレイヤーが置ける場所に石を置いた場合の盤面を計算する

        // => => => ヒミツマスを裏返せるなら値Aをインクリメント

        // 値Aが最も小さい場所を選択する
    }


}
