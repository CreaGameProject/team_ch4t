using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    //第一次元⇒x座標、右に行けば増える
    //第二次元⇒y座標、下に行けば増える
    //[SerializeField] private Cell.Type[,] board = new Cell.Type[8, 8];
    [SerializeField] private (Cell.Type, Cell.Color)[,] board = new (Cell.Type, Cell.Color)[8, 8];
    public (Cell.Type, Cell.Color)[,] GetBoard() { return this.board; }

    [SerializeField] private Transform boardAnchor = null;

    [SerializeField] private Computer computer = null;

    public Turn.Type turn = new Turn.Type();



    // Start is called before the first frame update
    void Start()
    {
        UpdateCell((3, 3), Cell.Type.white);
        UpdateCell((4, 3), Cell.Type.black);
        UpdateCell((3, 4), Cell.Type.black);
        UpdateCell((4, 4), Cell.Type.white);

        /*List<(int, int)> proposedCells = GetProposedCell(Cell.Type.black);
        (int, int) cellIndex = proposedCells[Random.Range(0, proposedCells.Count)];
        UpdateCell(cellIndex, Cell.Type.secret);*/

        //石を置く

        GameStart();
    }

    public void GameStart()
    {
        int number = UnityEngine.Random.Range(1, 100 + 1);
        if (number % 2 == 0)
        {
            StartPlayerTurn();
        }
        else
        {
            StartComputerTurn();
        }
    }

    public void StartPlayerTurn()
    {
        this.turn = Turn.Type.player;
        Debug.Log("<b><color=#00B9CB>【 Board - ChangeTurn 】Player のターンです。</color></b>");

        ViewBoard(this.turn, true);

        if (GetProposedCell(ConvertTypeToColor(Cell.Type.white)).Count == 0)
        {
            Debug.Log("<b><color=#ED1454>【 Board - ChangeTurn 】石を置ける場所がないのでパスしました。</color></b>");
            TurnEnd();
        }
    }

    public void StartComputerTurn()
    {
        this.turn = Turn.Type.computer;
        Debug.Log("<b><color=#00B9CB>【 Board - ChangeTurn 】Computer のターンです。</color></b>");

        ViewBoard(this.turn, true);

        if (GetProposedCell(ConvertTypeToColor(Cell.Type.black)).Count == 0)
        {
            Debug.Log("<b><color=#ED1454>【 Board - ChangeTurn 】石を置ける場所がないのでパスしました。</color></b>");
            TurnEnd();
        }
        else
        {
            this.computer.Action();
        }
    }

    public void TurnEnd()
    {
        bool willTheMatchContinue = ContinuationJudgment();
        if (willTheMatchContinue)
        {
            ChangeTurn();
        }
        else
        {
            string result = GameResultJudgment();
            Debug.Log("<b><color=#F26E3E>【 Board 】GAME SET! => " + result + "</color></b>");
        }
    }

    //ゲームの勝敗を返す
    private string GameResultJudgment()
    {
        int numberOfPlayerCell = 0;
        int numberOfComputerCell = 0;

        for (int y = 0; y < this.board.GetLength(1); y++)
        {
            for (int x = 0; x < this.board.GetLength(0); x++)
            {
                if (this.board[x, y].Item2 == Cell.Color.white)
                {
                    numberOfPlayerCell++;
                }
                else if (this.board[x, y].Item2 == Cell.Color.black)
                {
                    numberOfComputerCell++;
                }
            }
        }

        string result = "";

        if (numberOfPlayerCell > numberOfComputerCell)
        {
            result = "Player WIN | player => " + numberOfPlayerCell + " : computer => " + numberOfComputerCell;
        }
        else if (numberOfPlayerCell < numberOfComputerCell)
        {
            result = "Computer WIN | player => " + numberOfPlayerCell + " : computer => " + numberOfComputerCell;
        }
        else if (numberOfPlayerCell == numberOfComputerCell)
        {
            result = "DROW | player => " + numberOfPlayerCell + " : computer => " + numberOfComputerCell;
        }

        return result;
    }

    //ゲームが継続するか調べる
    private bool ContinuationJudgment()
    {
        bool willTheMatchContinue = true;

        int numberOfPlayerProposedCell = GetProposedCell(ConvertTypeToColor(Cell.Type.white)).Count;
        int numerOfComputerProposedCell = GetProposedCell(ConvertTypeToColor(Cell.Type.black)).Count;

        if (numberOfPlayerProposedCell == 0 && numerOfComputerProposedCell == 0)
        {
            willTheMatchContinue = false;
        }

        return willTheMatchContinue;

    }

    //ターンを変更する
    public void ChangeTurn()
    {
        if (this.turn == Turn.Type.computer)
        {
            StartPlayerTurn();
        }
        else if (this.turn == Turn.Type.player)
        {
            StartComputerTurn();
        }
    }

    // 盤面をコンソールに表示する
    //第１引数 ⇒ 
    //第２引数 ⇒ 石を設置可能な場所を表示するかどうか。｜true ⇒ 表示する｜false ⇒ 表示しない
    public void ViewBoard(Turn.Type myType, bool isViewProposedCell)
    {
        //参照コピー（元の値も変わる）を防ぐため複写している
        (Cell.Type, Cell.Color)[,] board = new (Cell.Type, Cell.Color)[8, 8];
        Array.Copy(this.board, board, this.board.Length);

        if (isViewProposedCell)
        {
            Cell.Type type = Cell.Type.empty;
            if (myType == Turn.Type.computer)
            {
                type = Cell.Type.black;
            }
            else if (myType == Turn.Type.player)
            {
                type = Cell.Type.white;
            }

            List<((int, int), int) > proposedCells = GetProposedCell(ConvertTypeToColor(type));

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
                    case Cell.Type.empty: line += "＃"; break;
                    case Cell.Type.white: line += "●"; break;
                    case Cell.Type.black: line += "〇"; break;
                    case Cell.Type.proposed: line += "☆"; break;
                    case Cell.Type.secret: line += "◇"; break;
                    default: line += "？"; break;
                }
            }

            content += line + "\n";
        }

        Debug.Log(content);
    }

    //相手の色を取得する
    public Cell.Color GetOppositeColor(Cell.Color myColor)
    {

        Cell.Color oppositeType = Cell.Color.empty;

        if (myColor == Cell.Color.white)
        {
            oppositeType = Cell.Color.black;
        }
        else if (myColor == Cell.Color.black)
        {
            oppositeType = Cell.Color.white;
        }

        return oppositeType;
    }

    // 石の種類を色に変換する
    private Cell.Color ConvertTypeToColor(Cell.Type type)
    {
        Cell.Color color = Cell.Color.empty;
        switch (type)
        {
            case Cell.Type.white: color = Cell.Color.white; break;
            case Cell.Type.black: color = Cell.Color.black; break;
            case Cell.Type.secret: color = Cell.Color.black; break;
            default: break;
        }
        return color;
    }

    // 盤面に石を置く（プレイヤー用）
    // 戻り値 bool型 result ⇒ 正しく石が置かれたかどうか
    public bool PutStone((float, float) mousePosition, Cell.Type type)
    {
        //受け取った座標を盤面上のインデックスに変換する
        float x = Mathf.Floor(mousePosition.Item1 - boardAnchor.position.x);
        float z = -1 * Mathf.Floor((mousePosition.Item2 - boardAnchor.position.z));
        (int, int) indexOnBoard = ((int)x, (int)(z - 1));

        return PutStone(indexOnBoard, type);
    }

    // 盤面に石を置く
    // 戻り値 bool型 result ⇒ 正しく石が置かれたかどうか
    public bool PutStone((int, int) indexOnBoard, Cell.Type myType)
    {
        // 場外判定
        bool isXIndexSafe = indexOnBoard.Item1 > -1 && 8 > indexOnBoard.Item1;
        bool isYIndexSafe = indexOnBoard.Item2 > -1 && 8 > indexOnBoard.Item2;
        if (!isXIndexSafe || !isYIndexSafe)
        {

            Debug.Log("<b><color=#FDC110>【 Board - PutStone 】" + string.Format("（列：{0}, 行：{1}）", indexOnBoard.Item1 + 1, indexOnBoard.Item2 + 1) + " は盤面の外です。</color></b>");
            return false;
        }

        // 既に石が置かれていないか判定
        var checkCell = this.board[indexOnBoard.Item1, indexOnBoard.Item2];
        if (checkCell.Item1 == Cell.Type.black || checkCell.Item1 == Cell.Type.white || checkCell.Item1 == Cell.Type.secret)
        {
            Debug.Log("<b><color=#FDC110>【 Board - PutStone 】" + string.Format("（列：{0}, 行：{1}）", indexOnBoard.Item1 + 1, indexOnBoard.Item2 + 1) + " には既に石が置かれています。</color></b>");
            return false;
        }

        // 石を置ける場所か判定する
        List<((int, int), int) > proposedCells = GetProposedCell(ConvertTypeToColor(myType));
        bool didIndexMatchAnyItem = false;
        foreach (((int, int), int) cell in proposedCells)
        {
            if (indexOnBoard == cell.Item1)
            {
                didIndexMatchAnyItem = true;
                break;
            }
        }

        if (!didIndexMatchAnyItem)
        {
            Debug.Log("<b><color=#FDC110>【 Board - PutStone 】" + string.Format("（列：{0}, 行：{1}）には石 {2} を置けません。", indexOnBoard.Item1 + 1, indexOnBoard.Item2 + 1, myType.ToString()) + "</color></b>");
            return false;
        }

        UpdateCell(indexOnBoard, myType);

        FlipCell(indexOnBoard, (myType == Cell.Type.secret) ? Cell.Type.black : myType);
        //ViewBoard(myType, true);

        return true;
    }

    //盤面（１マス）を更新する
    public void UpdateCell((int, int) indexOnBoard, Cell.Type type)
    {
        Cell.Color color = Cell.Color.empty;
        switch (type)
        {
            case Cell.Type.white : color = Cell.Color.white; break;
            case Cell.Type.black : color = Cell.Color.black; break;
            case Cell.Type.secret : color = Cell.Color.black; break;
            default: break;
        }

        this.board[indexOnBoard.Item1, indexOnBoard.Item2] = (type, color);
        Debug.Log("<b><color=#01AC56>【 Board - UpdateCell 】" + string.Format("（{0}列, {1}行）を type = {2}, color = {3} に更新しました。", indexOnBoard.Item1 + 1, indexOnBoard.Item2 + 1, type.ToString(), color.ToString()) + "</color></b>");
    }

    //石を裏返す
    public void FlipCell((int, int) indexOnBoard, Cell.Type type)
    {

        int x = indexOnBoard.Item1;
        int y = indexOnBoard.Item2;

        Cell.Color myColor = ConvertTypeToColor(type);
        Cell.Color oppositeColor = GetOppositeColor(myColor);

        //裏返すセルを探す

        int numberOfCellsToFlip = 0; //裏返す石の数
        (int, int) originIndex = (0, 0);//探索開始位置（index表記）
        int X, Y;

        //上方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x, y - 1);
        for (Y = y - 1; Y > -1; Y--)
        {
            if (this.board[originIndex.Item1, Y].Item1 == Cell.Type.empty) { break; }

            if (this.board[originIndex.Item1, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (this.board[originIndex.Item1, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    UpdateCell((originIndex.Item1, originIndex.Item2 - j), type);
                }

                break;
            }
        }

        //右上方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x + 1, y - 1);
        for (X = x + 1, Y = y - 1; X < 8 && Y > -1; X++, Y--)
        {
            if (this.board[X, Y].Item1 == Cell.Type.empty) { break; }

            if (this.board[X, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (this.board[X, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    UpdateCell((originIndex.Item1 + j, originIndex.Item2 - j), type);
                }

                break;
            }
        }

        //右方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x + 1, y);
        for (X = x + 1; X < 8; X++)
        {
            if (this.board[X, originIndex.Item2].Item1 == Cell.Type.empty) { break; }

            if (this.board[X, originIndex.Item2].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (this.board[X, originIndex.Item2].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    UpdateCell((originIndex.Item1 + j, originIndex.Item2), type);
                }

                break;
            }
        }

        //右下方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x + 1, y + 1);
        for (X = x + 1, Y = y + 1; X < 8 && Y < 8; X++, Y++)
        {
            if (this.board[X, Y].Item1 == Cell.Type.empty) { break; }

            if (this.board[X, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (this.board[X, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    UpdateCell((originIndex.Item1 + j, originIndex.Item2 + j), type);
                }

                break;
            }
        }

        //下方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x, y + 1);
        for (Y = y + 1; Y < 8; Y++)
        {
            if (this.board[originIndex.Item1, Y].Item1 == Cell.Type.empty) { break; }

            if (this.board[originIndex.Item1, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (this.board[originIndex.Item1, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    //this.board[originIndex.Item1, originIndex.Item2 + j] = type;
                    UpdateCell((originIndex.Item1, originIndex.Item2 + j), type);
                }

                break;
            }
        }

        //左下方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x - 1, y + 1);
        for (X = x - 1, Y = y + 1; X > -1 && Y < 8; X--, Y++)
        {
            if (this.board[X, Y].Item1 == Cell.Type.empty) { break; }

            if (this.board[X, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (this.board[X, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    //this.board[originIndex.Item1 - j, originIndex.Item2 + j] = type;
                    UpdateCell((originIndex.Item1 - j, originIndex.Item2 + j), type);
                }

                break;
            }
        }

        //左方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x - 1, y);
        for (X = x - 1; X > -1; X--)
        {
            if (this.board[X, originIndex.Item2].Item1 == Cell.Type.empty) { break; }

            if (this.board[X, originIndex.Item2].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (this.board[X, originIndex.Item2].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    //this.board[originIndex.Item1 - j, originIndex.Item2] = type;
                    UpdateCell((originIndex.Item1 - j, originIndex.Item2), type);
                }

                break;
            }
        }

        //左上方向への検索
        numberOfCellsToFlip = 0;
        originIndex = (x - 1, y - 1);
        for (X = x - 1, Y = y - 1; X > -1 && Y > -1; X--, Y--)
        {
            if (this.board[X, Y].Item1 == Cell.Type.empty) { break; }

            if (this.board[X, Y].Item2 == oppositeColor) { numberOfCellsToFlip++; }

            if (this.board[X, Y].Item2 == myColor)
            {
                for (int j = 0; j < numberOfCellsToFlip; j++)
                {
                    //this.board[originIndex.Item1 - j, originIndex.Item2 - j] = type;
                    UpdateCell((originIndex.Item1 - j, originIndex.Item2 - j), type);
                }

                break;
            }
        }

    }

    //設置可能な座標（index）を調べて、配列で返す
    //戻り値：
    public List<((int, int), int)> GetProposedCell(Cell.Color myColor)
    {
        Cell.Color oppositeColor = GetOppositeColor(myColor);

        // 候補地
        List<((int, int), int)> proposedCells = new List<((int, int), int)>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                //自分の色を起点に検索する
                if (this.board[x, y].Item2 != myColor)
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
                    if (this.board[originIndex.Item1, Y].Item1 == Cell.Type.empty || this.board[originIndex.Item1, Y].Item2 == myColor) { break; }

                    if (this.board[originIndex.Item1, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (Y - 1 < 0) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (this.board[originIndex.Item1, Y - 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (this.board[originIndex.Item1, Y - 1].Item1 == Cell.Type.empty)
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
                    if (this.board[originIndex.Item1, Y].Item1 == Cell.Type.empty || this.board[originIndex.Item1, Y].Item2 == myColor) { break; }

                    if (this.board[originIndex.Item1, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (Y + 1 > 7) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (this.board[originIndex.Item1, Y + 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (this.board[originIndex.Item1, Y + 1].Item1 == Cell.Type.empty)
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
                    if (this.board[X, originIndex.Item2].Item1 == Cell.Type.empty || this.board[X, originIndex.Item2].Item2 == myColor) { break; }

                    if (this.board[X, originIndex.Item2].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X - 1 < 0) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (this.board[X - 1, originIndex.Item2].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (this.board[X - 1, originIndex.Item2].Item1 == Cell.Type.empty)
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
                    if (this.board[X, originIndex.Item2].Item1 == Cell.Type.empty || this.board[X, originIndex.Item2].Item2 == myColor) { break; }

                    if (this.board[X, originIndex.Item2].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X + 1 > 7) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (this.board[X + 1, originIndex.Item2].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (this.board[X + 1, originIndex.Item2].Item1 == Cell.Type.empty)
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
                    if (this.board[X, Y].Item1 == Cell.Type.empty || this.board[X, Y].Item2 == myColor) { break; }

                    if (this.board[X, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X + 1 > 7 || Y - 1 < 0) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (this.board[X + 1, Y - 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (this.board[X + 1, Y - 1].Item1 == Cell.Type.empty)
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
                    if (this.board[X, Y].Item1 == Cell.Type.empty || this.board[X, Y].Item2 == myColor) { break; }

                    if (this.board[X, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X + 1 > 7 || Y + 1 > 7) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (this.board[X + 1, Y + 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (this.board[X + 1, Y + 1].Item1 == Cell.Type.empty)
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
                    if (this.board[X, Y].Item1 == Cell.Type.empty || this.board[X, Y].Item2 == myColor) { break; }

                    if (this.board[X, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X - 1 < 0 || Y - 1 < 0) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (this.board[X - 1, Y - 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (this.board[X - 1, Y - 1].Item1 == Cell.Type.empty)
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
                    if (this.board[X, Y].Item1 == Cell.Type.empty || this.board[X, Y].Item2 == myColor) { break; }

                    if (this.board[X, Y].Item2 == oppositeColor)
                    {
                        //場外判定
                        if (X - 1 < 0 || Y + 1 > 7) { break; }

                        numberOfCellsToFlip++;

                        //相手⇒検索続行
                        if (this.board[X - 1, Y + 1].Item2 == oppositeColor)
                        {
                            continue;
                        }

                        //空白⇒候補地決定
                        if (this.board[X - 1, Y + 1].Item1 == Cell.Type.empty)
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

        /*foreach(var cell in proposedCells)
        {
            Debug.Log("<b>【 Board - PutStone 】候補地 " + string.Format("({0}, {1})", cell.Item1, cell.Item2) + "</b>");
        }*/

        return proposedCells;
    }

    //設置可能な座標（index）を調べて、配列で返す
    public List<(int, int)> GetSecretCell()
    {
        // 候補地
        List<(int, int)> secretCells = new List<(int, int)>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                //自分の色を起点に検索する
                if (this.board[x, y].Item1 == Cell.Type.secret) 
                {
                    secretCells.Add((x, y));
                }
            }
        }

        return secretCells;
    }
}
