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
}
