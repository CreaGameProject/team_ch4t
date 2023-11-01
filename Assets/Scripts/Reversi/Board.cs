using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Preset
{
    [Header("手数（０になるとゲームオーバー）")] public int turn;

    [Header("プリセット\n・＃ ⇒ 何も置かれていない\n・● ⇒ プレイヤーの石\n・○ ⇒ コンピュータの石\n・◇ ⇒ ヒミツマス")]
    [TextArea(9, 9)] public string board;
}

public class Board : MonoBehaviour
{
    //第一次元⇒x座標、右に行けば増える
    //第二次元⇒y座標、下に行けば増える
    //[SerializeField] private Cell.Type[,] board = new Cell.Type[8, 8];
    [SerializeField] private (Cell.Type, Cell.Color)[,] board = new (Cell.Type, Cell.Color)[8, 8];
    public (Cell.Type, Cell.Color)[,] GetBoard() { return this.board; }

    [SerializeField] private Transform boardAnchor = null;

    [SerializeField] private Player player = null;
    [SerializeField] private Computer computer = null;

    [SerializeField] private int turnCounter = 0;


    [SerializeField] private List<Preset> presets =new List<Preset>();

    //public Turn.Type turn = new Turn.Type();



    // Start is called before the first frame update
    async void Start()
    {
        UpdateCell((3, 3), Cell.Type.white);
        UpdateCell((4, 3), Cell.Type.black);
        UpdateCell((3, 4), Cell.Type.black);
        UpdateCell((4, 4), Cell.Type.white);

        

        await Game();
    }

    bool didFlipSecretCell = false;

    async private UniTask Game()
    {
        // ゲームスタート
        Debug.Log("<b><color=#F26E3E>【 Board 】GAME START! </color></b>");

        int code = 1; // ゲーム終了

        // ゲームが終わるまでターンを繰り返す
        while (true)
        {
            await UniTask.Yield();

            this.didFlipSecretCell = false;

            // ターン数インクリメント
            //Debug.Log("【Board】TurnUnit() | ターン数インクリメント");
            this.turnCounter++;

            Debug.Log("<b><color=#00B9CB>【 Board - ChangeTurn 】Player のターンです。</color></b>");

            // 盤面をコンソールに表示する
            ViewBoard(Turn.Type.player, true);

            // プレイヤーが石を置けるか確認する、置けないならパスする
            //Debug.Log("【Board】TurnUnit() | プレイヤーが石を置けるか確認する");
            List<((int, int), int)> proposedCells = GetProposedCell(Cell.Color.black);
            if (proposedCells.Count > 0)
            {
                // プレイヤーが石を置く
                await this.player.Action();
            }
            else
            {
                Debug.Log("<b><color=#ED1454>【 Board - ChangeTurn 】石を置ける場所がないのでパスしました。</color></b>");
            }

            // ヒミツマスを裏返したターンは手数が減らないようにする必要アリ
            if (!this.didFlipSecretCell) { this.presets[0].turn--; } // プレイヤーの手数を減らす


            // ゲームが継続するか調べる
            //Debug.Log("【Board】TurnUnit() | ゲームが継続するか調べる");
            code = ContinuationJudgment(this.presets[0].turn, this.presets.Count);
            if (210 % code == 0 && code != 2) { break; }

            Debug.Log("<b><color=#00B9CB>【 Board - ChangeTurn 】Computer のターンです。</color></b>");

            // 盤面をコンソールに表示する
            ViewBoard(Turn.Type.computer, true);

            // コンピュータが石を置けるか確認する、置けないならパスする
            //Debug.Log("【Board】TurnUnit() | コンピュータが石を置けるか確認する");
            proposedCells.Clear();
            proposedCells = GetProposedCell(Cell.Color.white);
            if (proposedCells.Count > 0)
            {
                // コンピュータが石を置く
                await this.computer.Action();
            }
            else
            {
                Debug.Log("<b><color=#ED1454>【 Board - ChangeTurn 】石を置ける場所がないのでパスしました。</color></b>");
            }

            // 偶数ターン時にコンピュータが喋る
            //Debug.Log("【Board】TurnUnit() | 偶数ターン時にコンピュータが喋る");
            if (this.turnCounter % 2 == 0)
            {
                Debug.Log("【Board】TurnUnit() | キャラクターが喋るよ。");
            }

            // ゲームが継続するか調べる
            //Debug.Log("【Board】TurnUnit() | ゲームが継続するか調べる");
            code = ContinuationJudgment(this.presets[0].turn, this.presets.Count);
            if (210 % code == 0 && code != 2) { break; }

            // ヒミツマスを設置する
            //Debug.Log("【Board】TurnUnit() | ヒミツマスを設置する");
            int secretCellsCount = GetSecretCell().Count;
            if (this.turnCounter == 3 && secretCellsCount == 0)
            {
                // ヒミツマスを設置する場所の候補地を取得する
                List<(int, int)> cells = GetMostDifficultToTurnOverCells(this.board);

                // 複数ある場合はランダムで選ぶ
                (int, int) cell = cells[Random.Range(0, cells.Count)];

                // ヒミツマスを設置する
                UpdateCell(cell, Cell.Type.secret);
            }
        }

        /*
        if (code % 3 == 0) { Debug.Log("【 Board 】Game()｜両者とも石の設置が不可能"); } 
        if (code % 5 == 0) { Debug.Log("【 Board 】Game()｜ターン数が０になった"); }
        if (code % 7 == 0) { Debug.Log("【 Board 】Game()｜ヒミツが０になった"); }
        */

        string result = GameResultJudgment(code);
        Debug.Log("<b><color=#F26E3E>【 Board 】GAME SET! => " + result + "</color></b>");
    }

    // ゲームの勝敗を返す
    private string GameResultJudgment(int code)
    {
        string result = "";

        if (code % 3 == 0)
        {
            Debug.Log("【 Board 】Game()｜両者とも石の設置が不可能");

            int numberOfPlayerCell = 0;
            int numberOfComputerCell = 0;

            for (int y = 0; y < this.board.GetLength(1); y++)
            {
                for (int x = 0; x < this.board.GetLength(0); x++)
                {
                    if (this.board[x, y].Item2 == Cell.Color.black)
                    {
                        numberOfPlayerCell++;
                    }
                    else if (this.board[x, y].Item2 == Cell.Color.white)
                    {
                        numberOfComputerCell++;
                    }
                }
            } 

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
        }
        else if (code % 5 == 0)
        {
            Debug.Log("【 Board 】Game()｜ターン数が０になった");
            result = "Computer WIN";
        }
        else if (code % 7 == 0)
        {
            Debug.Log("【 Board 】Game()｜ヒミツが０になった");
            result = "Player WIN";
        }

        return result;
    }

    // ゲームが継続するか調べる
    // 戻り値：code：どの
    private int ContinuationJudgment(int turn, int presetCount)
    {
        int code = 2;

        int proposedCellPlayer = GetProposedCell(ConvertTypeToColor(Cell.Type.black)).Count;
        int proposedCellComputer = GetProposedCell(ConvertTypeToColor(Cell.Type.white)).Count;
        if (proposedCellPlayer == 0 && proposedCellComputer == 0) { code *= 3; } // 両者とも石の設置が不可能
        if (turn == 0) { code *= 5; } // ターン数が０になった
        if (presetCount == 0) { code *= 7; } // ヒミツが０になった

        return code;
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
                type = Cell.Type.white;
            }
            else if (myType == Turn.Type.player)
            {
                type = Cell.Type.black;
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
                    case Cell.Type.black: line += "○"; break;
                    case Cell.Type.proposed: line += "☆"; break;
                    case Cell.Type.secret: line += "◆"; break;
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

        bool didFlipSecretCell = FlipCell(indexOnBoard, (myType == Cell.Type.secret) ? Cell.Type.black : myType);
         

        if (didFlipSecretCell)
        {
            Debug.Log("ヒミツマスを裏返した。");

            this.turnCounter = 0;
            this.didFlipSecretCell = true;

            // プリセットを読み込み
            (Cell.Type, Cell.Color)[,] newBoard = new (Cell.Type, Cell.Color)[8, 8];
            string boardString = this.presets[0].board;
            char[] removeChars = new char[] { '\r', '\n' };
            foreach (char c in removeChars) { boardString = boardString.Replace(c.ToString(), ""); }

            for (int i = 0; i < boardString.Length; i++)
            {
                (Cell.Type, Cell.Color) cell = (Cell.Type.empty, Cell.Color.empty);
                switch (boardString[i])
                {
                    case '＃': cell = (Cell.Type.empty, Cell.Color.empty);  break;
                    case '○': cell = (Cell.Type.black, Cell.Color.black); break; // 記号
                    case '〇': cell = (Cell.Type.black, Cell.Color.black); break; // 漢数字
                    case '●': cell = (Cell.Type.white, Cell.Color.white); break;
                    case '◆': cell = (Cell.Type.secret, Cell.Color.white); break;
                    default : Debug.Log(string.Format("プリセット読み込みエラー：意図しない文字 {0} が含まれています。", boardString[i])); break;
                }

                newBoard[i % 8, (int)(i / 8)] = cell;
            }

            // 盤面をアップデート
            for (int y = 0; y < newBoard.GetLength(1); y++)
            {
                for (int x = 0; x < newBoard.GetLength(0); x++)
                {
                    UpdateCell((x, y), newBoard[x, y].Item1);
                }
            }

            // 盤面をコンソールに表示する
            ViewBoard(Turn.Type.computer, false);


            this.presets.RemoveAt(0);
        }

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
            case Cell.Type.secret : color = Cell.Color.white; break;
            default: break;
        }

        this.board[indexOnBoard.Item1, indexOnBoard.Item2] = (type, color);
        Debug.Log("<b><color=#01AC56>【 Board - UpdateCell 】" + string.Format("（{0}列, {1}行）を type = {2}, color = {3} に更新しました。", indexOnBoard.Item1 + 1, indexOnBoard.Item2 + 1, type.ToString(), color.ToString()) + "</color></b>");
    }

    // 石を裏返す
    // 戻り値：裏返したセルにヒミツマスが含まれているか
    public bool FlipCell((int, int) indexOnBoard, Cell.Type type)
    {
        bool didFlipSecretCell = false;

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
                    if (this.board[originIndex.Item1, originIndex.Item2 - j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
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
                    if(this.board[originIndex.Item1 + j, originIndex.Item2 - j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
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
                    if (this.board[originIndex.Item1 + j, originIndex.Item2].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
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
                    if (this.board[originIndex.Item1 + j, originIndex.Item2 + j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
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
                    if (this.board[originIndex.Item1, originIndex.Item2 + j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
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
                    if (this.board[originIndex.Item1 - j, originIndex.Item2 + j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
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
                    if (this.board[originIndex.Item1 - j, originIndex.Item2].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
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
                    if (this.board[originIndex.Item1 - j, originIndex.Item2 - j].Item1 == Cell.Type.secret) { didFlipSecretCell = true; }
                    UpdateCell((originIndex.Item1 - j, originIndex.Item2 - j), type);
                }

                break;
            }
        }

        return didFlipSecretCell;

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

    private List<(int, int)> GetMostDifficultToTurnOverCells((Cell.Type, Cell.Color)[,] board)
    {
        // 裏返される回数をカウントする
        int[,] frequency = new int[8, 8];
        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                if (board[x, y].Item2 != Cell.Color.black) { continue; }

                Cell.Color myColor = Cell.Color.black; // プレイヤーの色
                Cell.Color oppositeColor = Cell.Color.white; // コンピュータの色

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

        // 最も裏返される回数が少ない場所を探す。
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
}
