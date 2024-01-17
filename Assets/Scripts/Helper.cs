using System.Collections.Generic;
using UnityEngine;

public class Helper
{
    internal static T GetRandom<T> (IList<T> Params)
    {
        return Params [Random.Range (0, Params.Count)];
    }

    public const string BattleDialogueJsonPath = "JSON/text_event";
    public const string OutGameDialogueJsonPath = "JSON/outgame_event";
    public const string CharacterFilePath = "Sprites/Characters/";
    public const string ItemFilePath = "Sprites/UI/";
    public const string BackgroundPath = "Sprites/UI/";
    public const string SoundFilePath = "Sound/";

    public const string ClearSceneMessage = "新しいキャラが追加されました。";
}