using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "BoardPreset", menuName = "BoardPreset")]
public class BoardPreset : ScriptableObject
{
    public Character character = new Character();
}

[System.Serializable]
public class Character
{
    [Header("キャラクターID")] public int id;
    [Header("キャラクター名")] public string name;
    [Header("プリセット")]
    [Header("＃ ⇒ 何も置かれていない\n● ⇒ プレイヤーの石\n○ ⇒ コンピュータの石\n◇ ⇒ ヒミツマス")]
    [TextArea(9, 9)] public List<string> boardPresets = new List<string> { "＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃",
                                                                           "＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃\r\n＃＃＃＃＃＃＃＃" };
}
