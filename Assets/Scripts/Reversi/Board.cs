using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static Board;
using Random = UnityEngine.Random;

[System.Serializable]
public class Preset
{
    //[Header("手数（０になるとゲームオーバー）")] public int turn;

    [Header("プリセット\n・＃ ⇒ 何も置かれていない\n・● ⇒ プレイヤーの石\n・○ ⇒ コンピュータの石\n・◆ ⇒ ヒミツマス")]
    [TextArea(9, 9)] public string board;

    //[Header("メモ")] public string memo;
}

public class Board : MonoBehaviour
{
    public static Board instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        ChangeRestTurn(this.restTurn);
    }

    //第一次元⇒x座標、右に行けば増える
    //第二次元⇒y座標、下に行けば増える
    //[SerializeField] private Cell.Type[,] board = new Cell.Type[8, 8];
    [SerializeField] private (Cell.Type, Cell.Color)[,] board = new (Cell.Type, Cell.Color)[8, 8];
    public (Cell.Type, Cell.Color)[,] GetBoard() { return this.board; }

    [Header("アンカー（盤面の左上に設置）")]
    [SerializeField] private Transform boardAnchor = null;

    [Header("プレイヤー")]
    [SerializeField] private Player player = null;

    [Header("コンピュータ")]
    [SerializeField] private Computer computer = null;
    public Computer.Opponent getOpponent { get { return this.computer.getOpponent; } }

    [Header("石のプレハブ")]
    [SerializeField] private GameObject normalStone = null;

    [Header("ヒミツのプレハブ")]
    [SerializeField] private GameObject secretStone = null;

    [Header("ボード上のオブジェクト")]
    [SerializeField] private GameObject[] boardObjects = new GameObject[64];

    [Header("経過ターン数（現在のプリセットで何ターン目か）")]
    [SerializeField] private int turnCounter = 0;

    [Header("現在、誰のターンか")]
    [SerializeField] private Turn.Type turn = Turn.Type.neutral;
    public Turn.Type getTurn { get { return this.turn; } }

    [Header("手数（０になるとゲームオーバー）")] public int restTurn;
    public int getPresetRestTurn { get { return this.restTurn; } } // todo : getRestTurn に変更する

    [Header("プリセットインデックス")]
    [SerializeField] private int presetIndex = 0;
    public int getHowManyHimituDidGet { get { return this.presetIndex; } }


    [Header("盤面と手数のプリセット")]
    [SerializeField] private List<Preset> presets = new List<Preset>();


    // ヒミツマスを裏返したときに実行される
    public delegate UniTask SecretCellPerformanceExecutedDelegate();
    public event SecretCellPerformanceExecutedDelegate OnSecretCellPerformanceExecuted;
    async public UniTask SecretCellPerformance()
    {
        Debug.Log("<b><color=#ef476f>【Board - FlipSecretCell】ヒミツマスを裏返したときの演出</color></b>");
        if (OnSecretCellPerformanceExecuted != null) { await OnSecretCellPerformanceExecuted(); }
    }

    // コンピュータが喋るときに実行される
    public delegate UniTask SpeakComputerExecutedDelegate();
    public event SpeakComputerExecutedDelegate OnSpeakComputerExecuted;
    async public UniTask SpeakComputer()
    {
        Debug.Log("<b><color=#ef476f>【Board - SpeakComputer】コンピュータが喋るよ！</color></b>");
        if (OnSpeakComputerExecuted != null) { await OnSpeakComputerExecuted(); }
    }

    // 手番（現在のターン）が変更されたときに実行される
    public delegate void ChangeTurnExecutedDelegate(Turn.Type type);
    public event ChangeTurnExecutedDelegate OnChangeTurnExecuted;

    public void ChangeTurn(Turn.Type type)
    {
        Debug.Log("<b><color=#ef476f>【Board - ChangeTurn】手番（現在のターン）が変更されたときの演出</color></b>");
        if (OnChangeTurnExecuted != null) { OnChangeTurnExecuted(type); }
    }

    // 残り手数が変更されたときに実行される
    public delegate void ChangeRestTurnExecutedDelegate(int restTurn);
    public event ChangeRestTurnExecutedDelegate OnChangeRestTurnExecuted;

    public void ChangeRestTurn(int restTurn)
    {
        Debug.Log("<b><color=#ef476f>【Board - ChangeRestTurn】残り手数が変更されたときの演出</color></b>");
        if (OnChangeRestTurnExecuted != null) { OnChangeRestTurnExecuted(restTurn); }
    }

    // 現在取得しているヒミツの数が変わったら実行される
    public delegate void ChangeHimituNumberExecutedDelegate(int howManyHimituDidGet);
    public event ChangeHimituNumberExecutedDelegate OnChangeHimituNumberExecuted;
    public void OnChangeHimituNumber(int howManyHimituDidGet)
    {
        Debug.Log("<b><color=#ef476f>【Board - OnChangeHimituNumber】現在取得しているヒミツの数が変わったら実行される</color></b>");
        if (OnChangeHimituNumberExecuted != null) { OnChangeHimituNumberExecuted(howManyHimituDidGet); }
    }


    // ゲームオーバーになった時に実行される
    // 勝敗・引き分け => プレイヤーの勝ち、プレイヤーの負け、引き分け
    // 勝因・敗因 => ヒミツを全て暴いた、プレイヤーの持ち手が０になった、（引き分けは理由なし）
    public enum GameResult { None = 0, Player_WIN = 1, Player_LOSE = 2, Drow = 3 }
    public delegate void GameOverExecutedDelegate(GameResult gameResult);
    public event GameOverExecutedDelegate OnGameOverExecuted;
    public void OnGameOver(GameResult gameResult)
    {
        Debug.Log("<b><color=#ef476f>【Board - OnGameOver】ゲームオーバーになった時に実行される</color></b>");
        if (OnGameOverExecuted != null) { OnGameOverExecuted(gameResult); }
    }

    // Start is called before the first frame update
    async void Start()
    {
        ChangeRestTurn(this.restTurn);

        await SetPresetOnBoard();

        await Game();
    }

    async private UniTask Game()
    {
        // ゲームスタート
        Debug.Log("<b><color=#F26E3E>【 Board 】GAME START! </color></b>");

        int code = 1; // ゲーム終了

        // ゲームが終わるまでターンを繰り返す
        while (true)
        {
            await UniTask.Yield();

            bool didFlipSecretCell = false;

            // ターン数インクリメント
            //Debug.Log("【Board】TurnUnit() | ターン数インクリメント");
            this.turnCounter++;

            Debug.Log("<b><color=#00B9CB>【 Board - ChangeTurn 】Player のターンです。</color></b>");
            this.turn = Turn.Type.player;

            // ターンが変わった時の演出
            ChangeTurn(this.turn);

            // 盤面をコンソールに表示する
            ViewBoard(Turn.Type.player, true);

            // プレイヤーが石を置けるか確認する、置けないならパスする
            //Debug.Log("【Board】TurnUnit() | プレイヤーが石を置けるか確認する");
            List<((int, int), int)> proposedCells = GetProposedCell(Cell.Color.black);
            if (proposedCells.Count > 0)
            {
                // プレイヤーが石を置く
                (bool, bool) returnBool = await this.player.Action();
                didFlipSecretCell = returnBool.Item2;
                Debug.Log(string.Format("didFlipSecretCell : {0}", didFlipSecretCell));
            }
            else
            {
                Debug.Log("<b><color=#ED1454>【 Board - ChangeTurn 】石を置ける場所がないのでパスしました。</color></b>");
            }

            // プレイヤーの手数を減らす
            // ヒミツマスを裏返したのが、最後のプリセットのときは減らさない
            Debug.Log(string.Format("presetIndex : {0} | presets.Count : {1}", this.presetIndex, this.presets.Count));
            if (!(didFlipSecretCell && this.presetIndex + 1 == this.presets.Count && this.restTurn == 1))
            {
                this.restTurn--;

                // 残り手数が減った時の演出
                ChangeRestTurn(this.restTurn);
            }

            // ヒミツマスを裏返したのなら...
            if (didFlipSecretCell)
            {
                // ヒミツマスを裏返したときの演出
                await SecretCellPerformance();

                // プリセットインデックスをインクリメントする
                this.presetIndex++;

                // 現在取得しているヒミツの数が変わったら実行される
                OnChangeHimituNumber(this.presetIndex);

                // プリセットを展開する
                if (this.presetIndex != this.presets.Count) { await SetPresetOnBoard(); }
            }

            // ゲームが継続するか調べる
            //Debug.Log("【Board】TurnUnit() | ゲームが継続するか調べる");
            code = ContinuationJudgment(this.presetIndex);
            if (210 % code == 0 && code != 2) { break; }

            // ヒミツマスを裏返したらプレイヤーから始まる
            if (didFlipSecretCell) { continue; }

            Debug.Log("<b><color=#00B9CB>【 Board - ChangeTurn 】Computer のターンです。</color></b>");
            this.turn = Turn.Type.computer;

            // ターンが変わった時の演出
            ChangeTurn(this.turn);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

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

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            // ゲームが継続するか調べる
            // Debug.Log("【Board】TurnUnit() | ゲームが継続するか調べる");
            code = ContinuationJudgment(this.presetIndex);
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
                await UpdateCell(cell, Cell.Type.secret);
            }

            // コンピュータが喋る
            //if (this.turnCounter % 2 == 0) { SpeakComputer(); }
            await SpeakComputer();
        }

        /*
        if (code % 3 == 0) { Debug.Log("【 Board 】Game()｜両者とも石の設置が不可能"); } 
        if (code % 5 == 0) { Debug.Log("【 Board 】Game()｜ターン数が０になった"); }
        if (code % 7 == 0) { Debug.Log("【 Board 】Game()｜ヒミツが０になった"); }
        */

        (GameResult, string) result = GameResultJudgment(code);
        
        Debug.Log("<b><color=#F26E3E>【 Board 】GAME SET! => " + result.Item2 + "</color></b>");

        OnGameOver(result.Item1);
    }

    // ゲームの勝敗を返す
    private (GameResult, string) GameResultJudgment(int code)
    {
        (GameResult, string) result = (GameResult.None, "");

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
                result.Item1 = GameResult.Player_WIN;
                result.Item2 = "Player WIN | player => " + numberOfPlayerCell + " : computer => " + numberOfComputerCell;
            }
            else if (numberOfPlayerCell < numberOfComputerCell)
            {
                result.Item1 = GameResult.Player_LOSE;
                result.Item2 = "Computer WIN | player => " + numberOfPlayerCell + " : computer => " + numberOfComputerCell;
            }
            else if (numberOfPlayerCell == numberOfComputerCell)
            {
                result.Item1 = GameResult.Drow;
                result.Item2 = "DROW | player => " + numberOfPlayerCell + " : computer => " + numberOfComputerCell;
            }
        }
        else if (code % 5 == 0)
        {
            Debug.Log("【 Board 】Game()｜ターン数が０になった");
            result.Item1 = GameResult.Player_LOSE;
            result.Item2 = "Computer WIN";
        }
        else if (code % 7 == 0)
        {
            Debug.Log("【 Board 】Game()｜ヒミツが０になった");
            result.Item1 = GameResult.Player_WIN;
            result.Item2 = "Player WIN";
        }

        return result;
    }

    // ゲームが継続するか調べる
    // 戻り値：code：どの
    private int ContinuationJudgment(int presetIndex)
    {
        Debug.Log(string.Format("ContinuationJudgment(int presetIndex) | presetIndex : {0}", presetIndex));

        int code = 2;

        if (presetIndex == this.presets.Count) // ヒミツが０になった
        { 
            code *= 7;
        }
        else
        {
            int proposedCellPlayer = GetProposedCell(ConvertTypeToColor(Cell.Type.black)).Count;
            int proposedCellComputer = GetProposedCell(ConvertTypeToColor(Cell.Type.white)).Count;
            if (proposedCellPlayer == 0 && proposedCellComputer == 0) { code *= 3; } // 両者とも石の設置が不可能

            int turn = this.restTurn;
            //Debug.Log(string.Format("turn : {0}", turn));
            if (turn == 0) { code *= 5; } // ターン数が０になった 
        }
        
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

    // 盤面をプリセットに上書きする
    async private UniTask SetPresetOnBoard()
    {
        // プリセットを読み込み
        (Cell.Type, Cell.Color)[,] newBoard = new (Cell.Type, Cell.Color)[8, 8];
        string boardString = this.presets[presetIndex].board;
        char[] removeChars = new char[] { '\r', '\n' };
        foreach (char c in removeChars) { boardString = boardString.Replace(c.ToString(), ""); }

        for (int i = 0; i < boardString.Length; i++)
        {
            (Cell.Type, Cell.Color) cell = (Cell.Type.empty, Cell.Color.empty);
            switch (boardString[i])
            {
                case '＃': cell = (Cell.Type.empty, Cell.Color.empty); break;
                case '○': cell = (Cell.Type.black, Cell.Color.black); break; // 記号
                case '〇': cell = (Cell.Type.black, Cell.Color.black); break; // 漢数字
                case '●': cell = (Cell.Type.white, Cell.Color.white); break;
                case '◆': cell = (Cell.Type.secret, Cell.Color.white); break;
                default: Debug.Log(string.Format("プリセット読み込みエラー：意図しない文字 {0} が含まれています。", boardString[i])); break;
            }

            newBoard[i % 8, (int)(i / 8)] = cell;
        }

        // 盤面を初期化する
        for (int index = 0; index < 64; index++) { UpdateCell((index % 8, (int)(index / 8)), Cell.Type.empty); }
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));


        // 盤面にある石の数を計算する
        //int numberOfCell = 0;
        //for (int index = 0; index < 64; index++) { if (newBoard[index % 8, (int)(index / 8)].Item1 != Cell.Type.empty) { numberOfCell++; } }

        // 盤面をアップデート
        for (int y = 0; y < newBoard.GetLength(1); y++)
        {
            for (int x = 0; x < newBoard.GetLength(0); x++)
            {
                await UpdateCell((x, y), newBoard[x, y].Item1);
                //await UpdateCell((x, y), newBoard[x, y].Item1, 4.0f / numberOfCell);
            }
        }

        // 盤面をコンソールに表示する
        ViewBoard(Turn.Type.computer, false);
    }

    // 盤面に石を置く（プレイヤー用）
    // 戻り値 bool型 result ⇒ 正しく石が置かれたかどうか
    async public UniTask<(bool, bool)> PutStone((float, float) mousePosition, Cell.Type type)
    {
        //受け取った座標を盤面上のインデックスに変換する
        float x = Mathf.Floor(mousePosition.Item1 - boardAnchor.position.x);
        float z = -1 * Mathf.Floor((mousePosition.Item2 - boardAnchor.position.z));
        (int, int) indexOnBoard = ((int)x, (int)(z - 1));

        return await PutStone(indexOnBoard, type);
    }

    // 盤面に石を置く
    // 戻り値 bool型 result ⇒ 正しく石が置かれたかどうか
    async public UniTask<(bool, bool)> PutStone((int, int) indexOnBoard, Cell.Type myType)
    {
        // 場外判定
        bool isXIndexSafe = indexOnBoard.Item1 > -1 && 8 > indexOnBoard.Item1;
        bool isYIndexSafe = indexOnBoard.Item2 > -1 && 8 > indexOnBoard.Item2;
        if (!isXIndexSafe || !isYIndexSafe)
        {

            Debug.Log("<b><color=#FDC110>【 Board - PutStone 】" + string.Format("（列：{0}, 行：{1}）", indexOnBoard.Item1 + 1, indexOnBoard.Item2 + 1) + " は盤面の外です。</color></b>");
            return (false, false);
        }

        // 既に石が置かれていないか判定
        var checkCell = this.board[indexOnBoard.Item1, indexOnBoard.Item2];
        if (checkCell.Item1 == Cell.Type.black || checkCell.Item1 == Cell.Type.white || checkCell.Item1 == Cell.Type.secret)
        {
            Debug.Log("<b><color=#FDC110>【 Board - PutStone 】" + string.Format("（列：{0}, 行：{1}）", indexOnBoard.Item1 + 1, indexOnBoard.Item2 + 1) + " には既に石が置かれています。</color></b>");
            return (false, false);
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
            return (false, false);
        }

        await UpdateCell(indexOnBoard, myType);

        bool didFlipSecretCell = await FlipCell(indexOnBoard, (myType == Cell.Type.secret) ? Cell.Type.black : myType);
         

        if (didFlipSecretCell)
        {
            Debug.Log("ヒミツマスを裏返した。");

            //this.presetIndex++;

            this.turnCounter = 0;
            //this.didFlipSecretCell = true;

            // todo : ヒミツマスを裏返したときの演出を実行する
            // await SecretCellPerformance();

            // if (this.presetIndex != this.presets.Count) { await SetPresetOnBoard(); }
        }

        return (true, didFlipSecretCell);
    }

    //盤面（１マス）を更新する
    async UniTask UpdateCell((int, int) indexOnBoard, Cell.Type type) { await UpdateCell(indexOnBoard, type, 0.2f); }

    //盤面（１マス）を更新する : 石のアニメーション時間を指定する場合
    async UniTask UpdateCell((int, int) indexOnBoard, Cell.Type type, float time)
    {
        Cell.Color color = Cell.Color.empty;
        switch (type)
        {
            case Cell.Type.white : color = Cell.Color.white; break;
            case Cell.Type.black : color = Cell.Color.black; break;
            case Cell.Type.secret : color = Cell.Color.white; break;
            default: break;
        }

        // empty => anything without empty : 石を生成する
        if (this.board[indexOnBoard.Item1, indexOnBoard.Item2].Item1 == Cell.Type.empty && type != Cell.Type.empty)
        {
            Vector3 pos = new Vector3(-3.5f + indexOnBoard.Item1, 0, 3.5f - indexOnBoard.Item2);

            GameObject g = null;

            if (type != Cell.Type.secret)
            {
                g = Instantiate(this.normalStone, pos, Quaternion.identity);
            }
            else 
            {
                g = Instantiate(this.secretStone, pos, Quaternion.identity);
            } 
            
            Stone stone = g.GetComponent<Stone>();
            stone.color = color;
            await stone.Generate(time);
            this.boardObjects[indexOnBoard.Item1 + indexOnBoard.Item2 * 8] = g;
        }

        // empty => secret : ヒミツマスを生成する
        /*if (this.board[indexOnBoard.Item1, indexOnBoard.Item2].Item1 == Cell.Type.empty && type == Cell.Type.secret)
        {
            Vector3 pos = new Vector3(-3.5f + indexOnBoard.Item1, 0, 3.5f - indexOnBoard.Item2);
            GameObject g = Instantiate(this.secretStone, pos, Quaternion.identity);
            Stone stone = g.GetComponent<Stone>();
            stone.color = color;
            await stone.Generate(0.3f);
            this.boardObjects[indexOnBoard.Item1 + indexOnBoard.Item2 * 8] = g;
        }*/

        // white => secret : ヒミツマスを生成する
        if (this.board[indexOnBoard.Item1, indexOnBoard.Item2].Item1 == Cell.Type.white && type == Cell.Type.secret)
        {
            // 既存の白石を破棄する
            GameObject white = this.boardObjects[indexOnBoard.Item1 + indexOnBoard.Item2 * 8];
            await white.GetComponent<Stone>().Destroy(0.2f);
            Destroy(white);

            // ヒミツマスを生成
            Vector3 pos = new Vector3(-3.5f + indexOnBoard.Item1, 0, 3.5f - indexOnBoard.Item2);
            GameObject g = Instantiate(this.secretStone, pos, Quaternion.identity);
            Stone stone = g.GetComponent<Stone>();
            stone.color = color;
            await stone.Generate(0.3f);
            this.boardObjects[indexOnBoard.Item1 + indexOnBoard.Item2 * 8] = g;
        }


        // anything without empty => empty : 石を破棄する
        if (this.board[indexOnBoard.Item1, indexOnBoard.Item2].Item1 != Cell.Type.empty && type == Cell.Type.empty)
        {
            // 既存の石を破棄する
            GameObject stone = this.boardObjects[indexOnBoard.Item1 + indexOnBoard.Item2 * 8];
            await stone.GetComponent<Stone>().Destroy(0.2f);
            Destroy(stone);
        }


        // white => black : 石を裏返す
        if (this.board[indexOnBoard.Item1, indexOnBoard.Item2].Item2 == Cell.Color.white && type == Cell.Type.black)
        {
            GameObject g = this.boardObjects[indexOnBoard.Item1 + indexOnBoard.Item2 * 8];
            Stone stone = g.GetComponent<Stone>();
            stone.color = Cell.Color.black;
            await stone.Flip();
        }

        // black => white : 石を裏返す
        if (this.board[indexOnBoard.Item1, indexOnBoard.Item2].Item2 == Cell.Color.black && type == Cell.Type.white)
        {
            GameObject g = this.boardObjects[indexOnBoard.Item1 + indexOnBoard.Item2 * 8];
            Stone stone = g.GetComponent<Stone>();
            stone.color = Cell.Color.white;
            await stone.Flip();
        }

        this.board[indexOnBoard.Item1, indexOnBoard.Item2] = (type, color);
        Debug.Log("<b><color=#01AC56>【 Board - UpdateCell 】" + string.Format("（{0}列, {1}行）を type = {2}, color = {3} に更新しました。", indexOnBoard.Item1 + 1, indexOnBoard.Item2 + 1, type.ToString(), color.ToString()) + "</color></b>");

    }

    // 石を裏返す
    // 戻り値：裏返したセルにヒミツマスが含まれているか
    async public UniTask<bool> FlipCell((int, int) indexOnBoard, Cell.Type type)
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
                    await UpdateCell((originIndex.Item1, originIndex.Item2 - j), type);
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
                    await UpdateCell((originIndex.Item1 + j, originIndex.Item2 - j), type);
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
                    await UpdateCell((originIndex.Item1 + j, originIndex.Item2), type);
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
                    await UpdateCell((originIndex.Item1 + j, originIndex.Item2 + j), type);
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
                    await UpdateCell((originIndex.Item1, originIndex.Item2 + j), type);
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
                    await UpdateCell((originIndex.Item1 - j, originIndex.Item2 + j), type);
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
                    await UpdateCell((originIndex.Item1 - j, originIndex.Item2), type);
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
                    await UpdateCell((originIndex.Item1 - j, originIndex.Item2 - j), type);
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
        int[,] frequency = new int[8, 8] /*{ { -1, -1, -1, -1, -1, -1, -1, -1},
                                           { -1, -1, -1, -1, -1, -1, -1, -1},
                                           { -1, -1, -1, -1, -1, -1, -1, -1},
                                           { -1, -1, -1, -1, -1, -1, -1, -1},
                                           { -1, -1, -1, -1, -1, -1, -1, -1},
                                           { -1, -1, -1, -1, -1, -1, -1, -1},
                                           { -1, -1, -1, -1, -1, -1, -1, -1},
                                           { -1, -1, -1, -1, -1, -1, -1, -1} }*/;

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
                //if (frequency[x, y] <= 0) { continue; }

                if (this.board[x, y].Item2 != Cell.Color.white) { continue; }

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
