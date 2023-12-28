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

    public static Opponent opponent = Opponent.Yukihira_ui;

    [Header("N 手先まで思考する（部長AIのみ有効）")] // default => 1
    [SerializeField] private int foresight = 1;
    
    // 再帰的に計算する（実行コスト高い）
    async private UniTask<List<int>> Recursive((Cell.Type, Cell.Color)[,] virtualBoard, int n, int f)
    {
        await UniTask.Yield();

        List<((int, int), int)> proposedCells_com = GetProposedCell(Cell.Color.white, virtualBoard);

        List<int> predict = new List<int>();

        //Debug.Log("proposedCells[j] : " + proposedCells[i]);
        Debug.Log(string.Format("{0} 手先を試行中...", n));

        // => コンピュータが置ける場所に石を置いた場合の盤面を計算する
        for (int i = 0; i < proposedCells_com.Count; i++)
        {
            //int p = 0;
            predict.Add(0);

            Debug.Log(string.Format("computerパターン : {0}", i));

            // 探索打ち切り
            //if (predict.Contains(0)) { Debug.Log(string.Format("探索打ち切り")); continue; }

            ((Cell.Type, Cell.Color)[,], bool) predict_com = Predict(virtualBoard, proposedCells_com[i].Item1, Cell.Color.white, Cell.Color.black, Cell.Type.white);
            //((Cell.Type, Cell.Color)[,], bool) predict_computer = Predict(virtualBoard, proposedCells[i].Item1, Cell.Color.white, Cell.Color.black, Cell.Type.white);

            (Cell.Type, Cell.Color)[,] virtualBoard_com = predict_com.Item1;

            virtualBoard_com[proposedCells_com[i].Item1.Item1, proposedCells_com[i].Item1.Item2] = UpdateCell(Cell.Type.white);

            ViewBoard(Turn.Type.player, true, virtualBoard_com);

            // => => プレイヤーが置ける場所を探す

            List<((int, int), int)> proposedCells_player = GetProposedCell(Cell.Color.black, virtualBoard_com);

            for (int j = 0; j < proposedCells_player.Count; j++)
            {
                // 探索打ち切り
                //if (predict.Contains(0)) { Debug.Log(string.Format("探索打ち切り")); continue; }

                Debug.Log(string.Format("computerパターン : {0} | playerパターン : {1}", i, j));

                ((Cell.Type, Cell.Color)[,], bool) predict_player = Predict(virtualBoard_com, proposedCells_player[j].Item1, Cell.Color.black, Cell.Color.white, Cell.Type.black);

                (Cell.Type, Cell.Color)[,] virtualBoard_player = predict_player.Item1;

                ViewBoard(Turn.Type.computer, true, virtualBoard_player);

                bool didFlipSecret = predict_player.Item2;

                if (didFlipSecret) { predict[i] += 1; }

                // 探索打ち切り
                //if (predict[i] > predict.Min()) { Debug.Log(string.Format("探索打ち切り")); predict[i] = 999999; continue; }

                if (n != f) { await Recursive(virtualBoard_player, n + 1, f); }
            }

            
        }

        return predict;
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
                    Debug.Log("！部長用AIの挙動です！");

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
                    Debug.Log("！部長用AIの挙動です！");

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

                    ViewBoard(Turn.Type.player, true, virtualBoard);

                    List<int> predict = new List<int>();

                    predict = await Recursive(virtualBoard, 1, this.foresight);

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

                    Debug.Log("predict => " + predict);


                    int min = 999;
                    List<int> index = new List<int>();
                    for (int p = 0; p < predict.Count; p++)
                    {
                        Debug.Log(string.Format("predict[{0}] => {1}", p, predict[p]));

                        if (min == predict[p])
                        {
                            index.Add(p);
                        }

                        if (min > predict[p])
                        {
                            index.Clear();
                            min = predict[p];
                            index.Add(p);
                        }
                    }

                    cellIndex = proposedCells[index[Random.Range(0, index.Count)]].Item1;
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
