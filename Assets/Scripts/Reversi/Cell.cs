using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [System.Serializable]
    public enum Type
    {
        empty = 0, //何も置かれていない
        white = 1, //白石が置かれている
        black = 2, //黒石が置かれている
        proposed = 3, //石を置ける場所
        secret = 4, //ヒミツマス
        hole = 5, //穴
    }

    [System.Serializable]
    public enum Color
    {
        empty = 0, //何も置かれていない
        white = 1, //白石が置かれている
        black = 2, //黒石が置かれている
    }
}
